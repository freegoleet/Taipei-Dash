using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class MathTesting : MonoBehaviour
{
    [Header("Sides")]
    [SerializeField]
    private Transform Top = null;
    [SerializeField]
    private Transform Bot = null;
    [SerializeField]
    private Transform Left = null;
    [SerializeField]
    private Transform Right = null;

    [Header("Corners")]
    [SerializeField]
    private Transform UpLeft = null;
    [SerializeField]
    private Transform UpRight = null;
    [SerializeField]
    private Transform DownLeft = null;
    [SerializeField]
    private Transform DownRight = null;

    [Header("Settings")]
    [SerializeField]
    private int Points = 10;
    [SerializeField]
    private int Radius = 1;
    [SerializeField]
    private Direction m_InDir = Direction.None;
    [SerializeField]
    private Direction m_OutDir = Direction.None;

    private Transform Center { get; set; } = null;
    private List<Vector2> AllPoints = new List<Vector2>();
    private bool Reverse { get; set; } = false;

    public void DrawCurve() {
        int rotation = GetCurveRotation(m_InDir, m_OutDir);
        float angle = (90 + Points) / Points;
        float currentAngle = 0;
        Vector2 pos = Vector2.zero;
        AllPoints.Clear();

        for (int i = 0; i < Points; i++) {
            pos.x = Center.position.x + (Radius * Mathf.Cos(Mathf.Deg2Rad * (currentAngle + rotation)));
            pos.y = Center.position.y + (Radius * Mathf.Sin(Mathf.Deg2Rad * (currentAngle + rotation)));

            currentAngle = (angle * i) + angle;
            AllPoints.Add(pos);
        }
        if(Reverse == true) {
            AllPoints.Reverse();
        }
    }

    private int GetCurveRotation(Direction indir, Direction outdir) {
        int rot = 0;
        switch (indir) {
            case Direction.Up:
                if(outdir == Direction.Left) {
                    Center = UpLeft;
                    rot = 270;
                    Reverse = true;
                    break;
                }
                Center = UpRight;
                rot = 180;
                Reverse = false;
                break;
            case Direction.Down:
                if (outdir == Direction.Right) {
                    Center = DownRight;
                    rot = 90;
                    Reverse = true;
                    break;
                }
                Center = DownLeft;
                rot = 0;
                Reverse = false;
                break;
            case Direction.Left:
                if (outdir == Direction.Down) {
                    Center = DownLeft;
                    rot = 0;
                    Reverse = true;
                    break;
                }
                Center = UpLeft;
                rot = 270;
                Reverse = false;
                break;
            case Direction.Right:
                if (outdir == Direction.Up) {
                    Center = UpRight;
                    rot = 180;
                    Reverse = true;
                    break;
                }
                rot = 90;
                Center = DownRight;
                Reverse = false;
                break;
        }

        return rot;
    }

    private Color GetColor(int index) {
        float value = (float)index / Points;
        return Color.Lerp(Color.green, Color.red, value);
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < AllPoints.Count; i++) {
            if (i > 0) {
                if (AllPoints[i - 1] != null) {
                    Gizmos.color = GetColor(i);
                    Gizmos.DrawLine(AllPoints[i - 1], AllPoints[i]);
                }
            }
        }
    }
}
