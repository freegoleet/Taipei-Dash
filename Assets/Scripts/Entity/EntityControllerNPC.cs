using System;
using System.Collections.Generic;
using Traffic;
using UnityEngine;

public struct EntityPathPoint
{
    public Vector2 Position { get; set; }
    public float DistanceToNext { get; set; }
    public TileGameplay Tile { get; set; }
}

public struct TurnData
{
    public int EulerAngles { get; set; }
    public bool ReverseOrder { get; set; }
}

public class EntityControllerNPC
{
    public float MovementSpeed { get; private set; } = 30f;

    private float Timer { get; set; } = 0.0f;
    private float TimeToRotate { get; set; } = 0.2f;
    private Entity Entity { get; set; } = null;
    private int CurrentPathIndex { get; set; } = -1;
    private bool IsMoving { get; set; } = false;
    private int TurnPoints { get; set; } = 10;
    private EntityPathPoint[] PathPoints { get; set; } = null;
    private Direction CurrentDrivingDirection { get; set; } = Direction.None;
    private Direction TurnDirection { get; set; } = Direction.None;
    private Vector2 DrivingDirection { get; set; } = Vector2.zero;
    public Action StepCompleted = null;
    public Action PathFinished = null;
    private float Distance { get; set; } = 0f;
    private EntityPathPoint CurrentPathPoint { get; set; } = default;
    private EntityPathPoint NextPathPoint { get; set; } = default;
    private float EqualityTolerance { get; set; } = 2f;
    private Vector3 StartRotation { get; set; } = Vector3.zero;
    private Vector3 TargetRotation { get; set; } = Vector3.zero;
    private float StartAngle { get; set; } = 0f;
    private float TargetAngle { get; set; } = 0f;
    private float Angle { get; set; } = 0f;
    private float NewAngle { get; set; } = 0f;
    private float LastAngle { get; set; } = 0f;
    private Vector3 Offset {  get; set; } = Vector3.zero;

    public EntityControllerNPC(Entity entity) {
        Entity = entity;
        StepCompleted += MoveNext;
        PathFinished += FinishedMoving;
        Timer = MovementSpeed;
    }

    public void Tick(float dt) {
        Step(dt);
    }

    public void MoveEntity(List<TileGameplay> path) {
        if (IsMoving == true) {
            return;
        }

        SetupPath(path);

        CurrentPathPoint = PathPoints[0];

        IsMoving = true;
        CurrentPathIndex = 0;
        MoveNext();
    }

    private void MoveNext() {
        if (CurrentPathPoint.Tile != null) {
            Entity.LeaveTile(CurrentPathPoint.Tile);
        }

        if (CurrentPathIndex >= PathPoints.Length) {
            PathFinished?.Invoke();
            return;
        }

        CurrentPathPoint = PathPoints[CurrentPathIndex];
        if (CurrentPathIndex + 1 < PathPoints.Length) {
            NextPathPoint = PathPoints[CurrentPathIndex + 1];
            Distance = Vector2.Distance(PathPoints[CurrentPathIndex].Position, PathPoints[CurrentPathIndex + 1].Position);

            Vector3 pivotPoint = Entity.PivotPoint.position;
            Vector3 relativePos = (Vector3)NextPathPoint.Position - pivotPoint;
            //Angle = Vector3.Angle(relativePos, Vector3.up);
            //StartAngle = Entity.transform.rotation.eulerAngles.z;
            //NewAngle = Angle - StartAngle;
            //LastAngle = Angle;
            //TargetAngle = StartAngle + NewAngle > 360 ? StartAngle + NewAngle - 360 : StartAngle + NewAngle;
            //NewAngle = StartAngle + 180 < TargetAngle ? NewAngle : -NewAngle;
            //TimeToRotate = Distance / MovementSpeed;
            Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, relativePos);
            Quaternion newRot = Quaternion.RotateTowards(Entity.PivotPoint.rotation, targetRot, 90);
            Entity.transform.rotation = newRot;

            Vector3 ppOffset = pivotPoint - Entity.PivotPoint.position;
            Entity.transform.position += ppOffset;

            Offset = Entity.PivotPoint.position - Entity.transform.position;
            //Debug.Log("start " + StartAngle);
            //Debug.Log("target " + TargetAngle);
        }
        if (NextPathPoint.Tile != null) {
            Entity.OccupyTile(NextPathPoint.Tile);
        }
        Timer = 0f;
        CurrentPathIndex++;
        return;
    }

    private void Step(float dt) {
        if (IsMoving == false) {
            return;
        }

        if (Equals(Entity.PivotPoint.position, NextPathPoint.Position) == false) {
            //Timer += dt;
            //if (Equals(Entity.transform.eulerAngles.z, TargetAngle) == false) {
            //    if (Timer < TimeToRotate) {
            //        float angle = dt * (NewAngle / TimeToRotate);
            //        Entity.transform.RotateAround(Entity.PivotPoint.position, Vector3.forward, angle);
            //    }
            //}

            var increment = ((Vector3)NextPathPoint.Position - Entity.transform.position - Offset).normalized * dt * MovementSpeed;
            Entity.IncrementPosition(new Vector3(increment.x, increment.y));
            return;
        }
        Timer = 0;

        StepCompleted?.Invoke();
    }

    private void StopMoving() {
        PathPoints = null;
        CurrentPathIndex = int.MaxValue;
    }

    public bool CancelAction() {
        if (IsMoving == false) {
            return false;
        }

        StopMoving();

        return true;
    }

    private void FinishedMoving() {
        IsMoving = false;
        Entity.LeaveTile(CurrentPathPoint.Tile);
        CurrentPathPoint = default;
    }

    private void SetupPath(List<TileGameplay> path) {
        List<EntityPathPoint> pathPoints = new List<EntityPathPoint>();
        path.Reverse();
        CurrentDrivingDirection = path[0].Facing;

        for (int i = 0; i < path.Count; i++) {
            if (i - 1 >= 0 && i + 1 < path.Count) {
                if (TryGetTurnDirection(path[i], path[i + 1]) is Direction turnDir) {
                    if (turnDir != Direction.None) {
                        pathPoints.AddRange(
                            GetTurnCurve(
                                path[i - 1].transform.position,
                                SwapPosition(path[i - 1].GridPosition, path[i].GridPosition, path[i + 1].GridPosition),
                                TrafficUtilities.GetDirectionFromPosToPos(path[i - 1].GridPosition, path[i].GridPosition),
                                turnDir));
                        continue;
                    }
                }
            }
            pathPoints.Add(new EntityPathPoint() { Position = path[i].transform.position, Tile = path[i] });
        }

        PathPoints = pathPoints.ToArray();
    }

    private Direction TryGetTurnDirection(TileGameplay currentTile, TileGameplay nextTile) {
        Direction dir = TrafficUtilities.GetDirectionFromPosToPos(currentTile.GridPosition, nextTile.GridPosition);
        if (dir != CurrentDrivingDirection) {
            CurrentDrivingDirection = dir;
            return dir;
        }
        return Direction.None;
    }

    public List<EntityPathPoint> GetTurnCurve(Vector2 startpoint, Vector2 Center, Direction inDir, Direction outDir) {
        TurnData turnData = GetCurveRotation(TrafficUtilities.ReverseDirections(inDir), outDir);

        float radius = (Center - startpoint).magnitude;
        float anglePerPoint = 90f / (TurnPoints + 2); // A turn is always 90 degrees in this game
        float currentAngle = anglePerPoint;

        Vector2 pos = Vector2.zero;
        List<EntityPathPoint> turnPoints = new List<EntityPathPoint>();
        turnPoints.Clear();

        for (int i = 0; i < TurnPoints; i++) {
            pos.x = Center.x + (radius * Mathf.Cos(Mathf.Deg2Rad * (currentAngle + turnData.EulerAngles)));
            pos.y = Center.y + (radius * Mathf.Sin(Mathf.Deg2Rad * (currentAngle + turnData.EulerAngles)));
            currentAngle = currentAngle + anglePerPoint;
            turnPoints.Add(new EntityPathPoint() { Position = pos });
        }
        if (turnData.ReverseOrder == true) {
            turnPoints.Reverse();
        }

        return turnPoints;
    }

    private TurnData GetCurveRotation(Direction indir, Direction outdir) {
        int rot = 0;
        bool reverse = false;
        switch (indir) {
            case Direction.Up:
                if (outdir == Direction.Left) {
                    rot = 270;
                    reverse = true;
                    break;
                }
                rot = 180;
                reverse = false;
                break;
            case Direction.Down:
                if (outdir == Direction.Right) {
                    rot = 90;
                    reverse = true;
                    break;
                }
                rot = 0;
                reverse = false;
                break;
            case Direction.Left:
                if (outdir == Direction.Down) {
                    rot = 0;
                    reverse = true;
                    break;
                }
                rot = 270;
                reverse = false;
                break;
            case Direction.Right:
                if (outdir == Direction.Up) {
                    rot = 180;
                    reverse = true;
                    break;
                }
                rot = 90;
                reverse = false;
                break;
        }

        return new TurnData() { EulerAngles = rot, ReverseOrder = reverse };
    }

    private Vector2 SwapPosition(Vector2Int startTile, Vector2Int turnTile, Vector2Int endTile) {
        int x = 0;
        int y = 0;
        if (startTile.x != turnTile.x) {
            x = startTile.x;
        }
        else {
            x = endTile.x;
        }
        if (startTile.y != turnTile.y) {
            y = startTile.y;
        }
        else {
            y = endTile.y;
        }

        return GameService.Instance.GetTileManager().GetTileByPos(new Vector2Int(x, y)).transform.position;
    }


    private Color GetColor(int index) {
        float value = (float)index / PathPoints.Length;
        return Color.Lerp(Color.green, Color.red, value);
    }

    public void DrawPath() {
        if (PathPoints == null) {
            return;
        }
        for (int i = 0; i < PathPoints.Length; i++) {
            if (i > 0) {
                if (PathPoints[i - 1].Position != null) {
                    Gizmos.color = GetColor(i);
                    Gizmos.DrawLine(PathPoints[i - 1].Position, PathPoints[i].Position);
                }
            }
        }
    }

    public bool Equals(Vector3 vector1, Vector3 vector2) {
        return Math.Abs(vector1.x - vector2.x) < EqualityTolerance &&
               Math.Abs(vector1.y - vector2.y) < EqualityTolerance;
    }

    public bool Equals(float value1, float value2) {
        return Math.Abs(value1 - value2) < 2f;
    }
}
