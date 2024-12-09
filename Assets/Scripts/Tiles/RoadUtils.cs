using System;
using System.Collections.Generic;
using System.Linq;
using Traffic;
using UnityEngine;

public class RoadUtils
{
    public GridManager GridManager { get; private set; } = null;

    // Lines
    public SO_RoadLine DirectionSeparatorDotted { get; private set; } = null;
    public SO_RoadLine DirectionSeparatorWhole { get; private set; } = null;
    public SO_RoadLine LaneSeparatorDotted { get; private set; } = null;
    public SO_RoadLine LaneSeparatorWhole { get; private set; } = null;
    public SO_RoadLine StopLine { get; private set; } = null;


    public RoadUtils(GridManager gridManager) {
        GridManager = gridManager;
        DirectionSeparatorDotted = GridManager.TileList.GetRoadlines()[LineType.DirectionSeparatorDotted];
        DirectionSeparatorWhole = GridManager.TileList.GetRoadlines()[LineType.DirectionSeparatorWhole];
        LaneSeparatorDotted = GridManager.TileList.GetRoadlines()[LineType.LaneSeparatorDotted];
        LaneSeparatorWhole = GridManager.TileList.GetRoadlines()[LineType.LaneSeparatorWhole];
        StopLine = GridManager.TileList.GetRoadlines()[LineType.Stop];
    }

    public void FitTile(TileRoad tile) {
        List<Direction> adjacents = new();
        adjacents.AddRange(TrafficUtilities.GetFlankDirections(tile.Facing).ToList());
        adjacents.Add(tile.Facing);
        adjacents.Add(TrafficUtilities.ReverseDirections(tile.Facing));

        for (int i = 0; i < adjacents.Count; i++) {
            if (tile.NeighborSystem.GetNeighborTile((adjacents[i], Direction.None)) is TileRoad neighbor == false) {
                continue;
            }
            if (neighbor.Facing == TrafficUtilities.ReverseDirections(tile.Facing)) { // Facing opposite
                continue;
            }
            if (tile.RoadtileNeighbors.IsNeighborFacingThis(adjacents[i])) { // Facing this
                tile.AddInConnection(adjacents[i]);
                neighbor.AddOutConnection(TrafficUtilities.ReverseDirections(adjacents[i]));
                continue;
            }
            if (neighbor.Facing == adjacents[i]) { // Facing away
                if (neighbor.Connections.InConnections[(int)tile.Facing] == true) {
                    continue;
                }
                tile.AddOutConnection(adjacents[i]);
                neighbor.AddInConnection(TrafficUtilities.ReverseDirections(adjacents[i]));
                continue;
            }
            if (neighbor.Facing == tile.Facing) { // Facing Same
                tile.AddInConnection(adjacents[i]);
                neighbor.AddInConnection(TrafficUtilities.ReverseDirections(adjacents[i]));
                tile.AddOutConnection(adjacents[i]);
                neighbor.AddOutConnection(TrafficUtilities.ReverseDirections(adjacents[i]));
                continue;
            }
            if (TrafficUtilities.ReverseDirections(tile.Facing) == adjacents[i]) { // Neighbor facing this
                tile.AddInConnection(adjacents[i]);
                neighbor.AddOutConnection(TrafficUtilities.ReverseDirections(adjacents[i]));
                continue;
            }
        }

        SetLines(tile);
    }

    public void SetLines(TileRoad tile) {
        HashSet<Direction> connections = new();
        for (int i = 0; i < tile.Connections.InConnections.Length; i++) {
            if (tile.Connections.InConnections[i] == false) {
                continue;
            }
            connections.Add((Direction)i);
        }
        for (int i = 0; i < tile.Connections.OutConnections.Length; i++) {
            if (tile.Connections.OutConnections[i] == false) {
                continue;
            }
            connections.Add((Direction)i);
        }

        foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
            if (dir == Direction.None) {
                continue;
            }

            if (tile.NeighborSystem.GetNeighborTile((dir, Direction.None)) is TileRoad neighbor == false) {
                tile.ToggleLine(dir, true); // Roadside
                tile.SetLineType(dir, DirectionSeparatorWhole);
                continue;
            }

            if (tile.Connections.OutConnections[(int)dir] == true && tile.Connections.InConnections[(int)dir] == true) {
                tile.SetLineType(dir, LaneSeparatorDotted);
                tile.ToggleLine(dir, true);
                continue;
            }

            if (tile.Connections.OutConnections[(int)dir] == false && tile.Connections.InConnections[(int)dir] == true) {
                tile.ToggleLine(dir, false);
                continue;
            }

            if (tile.Connections.OutConnections[(int)dir] == true && tile.Connections.InConnections[(int)dir] == false) {
                tile.ToggleLine(dir, false);
                continue;
            }

            if (tile.Connections.OutConnections[(int)dir] == false && tile.Connections.InConnections[(int)dir] == false) {
                tile.SetLineType(dir, DirectionSeparatorWhole);
                tile.ToggleLine(dir, true);
                continue;
            }

            tile.ToggleLine(dir, false);
        }
    }

    public void ToggleCrosswalk(TileRoad tile) {
        Direction[] flankDirections = TrafficUtilities.GetFlankDirections(tile.Facing);
        TileRoad currentRoad = tile;
        HashSet<TileRoad> roadToSetCrosswalk = new HashSet<TileRoad>() { tile };
        bool addCrosswalk = !tile.HasCrosswalk;

        for (int i = 0; i < flankDirections.Length; i++) {
            bool roadEnd = false;
            while (roadEnd == false) {
                roadToSetCrosswalk.Add(currentRoad);
                if (currentRoad.NeighborSystem.GetNeighborTile((flankDirections[i], Direction.None)) is TileRoad nextRoad) {
                    currentRoad = nextRoad;
                }
                else {
                    roadEnd = true;
                }
            }
        }

        foreach (TileRoad tileToAddCw in roadToSetCrosswalk) {
            tileToAddCw.ToggleCrosswalk(addCrosswalk);
            if (tileToAddCw.NeighborSystem.GetNeighborTile((TrafficUtilities.ReverseDirections(tileToAddCw.Facing), Direction.None)) is TileRoad stopTile) {
                if (stopTile.Connections.OutConnections.Sum(x => x == true ? 1 : 0) < 2) {
                    stopTile.SetLineType(tileToAddCw.Facing, StopLine);
                    stopTile.ToggleLine(tileToAddCw.Facing, addCrosswalk);
                }
            }
            SetLines(tileToAddCw);
        }

        return;
    }

    public bool CheckForCrossing(TileRoad tile) {
        NodeBase[] aNeighbors = GridManager.GetAdjacentNeighbors(tile);
        TileRoad nextTile = null;
        Direction perpendicularDirIndex = tile.Facing + 1;
        switch (tile.Facing) {
            case Direction.Up:
                nextTile = (TileRoad)aNeighbors[0];
                break;
            case Direction.Down:
                nextTile = (TileRoad)aNeighbors[1];
                break;
            case Direction.Left:
                nextTile = (TileRoad)aNeighbors[2];
                perpendicularDirIndex = 0;
                break;
            case Direction.Right:
                nextTile = (TileRoad)aNeighbors[3];
                break;
        }
        return false;
    }

    public void ToggleDrivableDirections(TileRoad tile, bool clockwise) {
        Dictionary<Direction, bool> outDirs = new() {
            { Direction.Up, false },
            { Direction.Down, false },
            { Direction.Left, false },
            { Direction.Right, false },
        };

        Direction[] flanks = TrafficUtilities.GetFlankDirections(tile.Facing);
        Direction forward = tile.Facing;
        Direction left = flanks[0];
        Direction right = flanks[1];

        Dictionary<Direction, bool> modifiedSides = new() {
            { forward, false },
            { left, false },
            { right, false },
        };

        if (clockwise == false) {
            if (tile.ConnectionIndex > 0) {
                tile.ConnectionIndex--;
            }
            else if (tile.ConnectionIndex == 0) {
                tile.ConnectionIndex = 6;
            }
        }
        else {
            if (tile.ConnectionIndex < 6) {
                tile.ConnectionIndex++;
            }
            else {
                tile.ConnectionIndex = 0;
            }
        }

        switch (tile.ConnectionIndex) {
            case 0:
                outDirs[forward] = true;
                break;
            case 1:
                outDirs[forward] = true;
                outDirs[left] = true;
                break;
            case 2:
                outDirs[forward] = true;
                outDirs[right] = true;
                break;
            case 3:
                outDirs[forward] = true;
                outDirs[left] = true;
                outDirs[right] = true;
                break;
            case 4:
                outDirs[left] = true;
                outDirs[right] = true;
                break;
            case 5:
                outDirs[left] = true;
                break;
            case 6:
                outDirs[right] = true;
                break;
        }

        tile.SetOutConnections(outDirs);
        SetLines(tile);

        foreach (var kvp in outDirs) {
            Direction dir = TrafficUtilities.ReverseDirections(kvp.Key);
            if (tile.NeighborSystem.GetNeighborTile((kvp.Key, Direction.None)) is TileRoad tileRoad == false) {
                continue;
            }

            if (kvp.Value == true) {
                tileRoad.AddInConnection(dir);
            }
            else {
                tileRoad.RemoveInConnection(dir);
            }
            SetLines(tileRoad);
        }
    }

    public void ToggleStopline(TileRoad tile) {
        tile.SetLineType(tile.Facing, StopLine);
        tile.HasStopLine = !tile.HasStopLine;
    }

    public void TogglePointer(TileRoad tile) {
        int tilesToCheck = 5;
        TileRoad currentTile = tile;
        HashSet<Direction> arrowDirs = new();
        Direction[] flanks = TrafficUtilities.GetFlankDirections(tile.Facing);

        for (int i = 0; i < tilesToCheck; i++) {
            foreach (Direction dir in flanks) {
                if (currentTile.NeighborSystem.GetNeighborTile((dir, Direction.None)) is TileRoad flankTile) {
                    if (currentTile.Connections.OutConnections[(int)dir] == false) {
                        continue;
                    }
                    if (flankTile.Connections.OutConnections[(int)TrafficUtilities.ReverseDirections(dir)] == true) {
                        continue;
                    }
                    arrowDirs.Add(dir);
                    if (currentTile.Connections.OutConnections[(int)tile.Facing] == true) {
                        arrowDirs.Add(tile.Facing);
                    }
                }
            }
            if (currentTile.NeighborSystem.GetNeighborTile((tile.Facing, Direction.None)) is TileRoad nextTile) {
                currentTile = nextTile;
                continue;
            }
            break;
        }

        if (arrowDirs.Count == 0) {
            arrowDirs.Add(tile.Facing);
        }

        bool up = false;
        if (arrowDirs.Contains(tile.Facing)) {
            up = true;
        }
        tile.Pointer.ToggleShow(false);
        Traffic.RoadPointer sidePointer = up == true ? Traffic.RoadPointer.SmallRight : Traffic.RoadPointer.BigRight;

        foreach (Direction direction in arrowDirs) {
            Direction relativeDir = TrafficUtilities.RelativeRotation(tile.Facing, direction);
            tile.Pointer.ToggleShow(relativeDir, true);
            if (relativeDir == Direction.Up) {
                tile.Pointer.SetPointer(relativeDir, GridManager.TileList.GetPointers()[Traffic.RoadPointer.Forward]);
                continue;
            }
            tile.Pointer.SetPointer(relativeDir, GridManager.TileList.GetPointers()[sidePointer]);
        }
        return;
    }

    public void ToggleTrafficLight(TileRoad tile) {
        Direction[] flankDirections = TrafficUtilities.GetFlankDirections(tile.Facing);
        TileRoad currentRoad = tile;
        HashSet<TileRoad> roadToModify = new HashSet<TileRoad>() { tile };
        bool addTrafficLight = tile.TrafficLight == null;
        TrafficLight trafficLight = null;

        for (int i = 0; i < flankDirections.Length; i++) {
            bool roadEnd = false;
            while (roadEnd == false) {
                roadToModify.Add(currentRoad);
                if (currentRoad.NeighborSystem.GetNeighborTile((flankDirections[i], Direction.None)) is TileRoad nextRoad) {
                    currentRoad = nextRoad;
                }
                else {
                    roadEnd = true;
                }
            }
        }

        foreach (TileRoad tileToMod in roadToModify) {
            if (addTrafficLight) {
                if (trafficLight == null) {
                    trafficLight = GameObject.Instantiate<TrafficLight>(GridManager.TileList.TrafficLight, tileToMod.transform);
                    trafficLight.Initialize(tileToMod);
                }
                tileToMod.AddToTrafficLight(trafficLight);
                trafficLight.AddRoad(tileToMod);
                continue;
            }
            if (tileToMod.gameObject.TryGetComponent<TrafficLight>(out var tl) == true) {
                GameService.Instance.RemoveTrafficLight(tl);
                GameObject.DestroyImmediate(tl);
            }
            trafficLight.RemoveRoad(tileToMod);
            tileToMod.RemoveFromTrafficLight();
        }
    }

}

