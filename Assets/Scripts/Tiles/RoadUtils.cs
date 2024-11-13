using System;
using System.Collections.Generic;
using System.Linq;
using Traffic;

public class RoadUtils
{
    public GridManager GridManager { get; private set; } = null;

    // Lines
    public SO_RoadLine DirectionSeparatorDotted { get; private set; } = null;
    public SO_RoadLine DirectionSeparatorWhole{ get; private set; } = null;
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
        adjacents.AddRange(TrafficLib.GetFlankDirections(tile.Facing).ToList());
        adjacents.Add(tile.Facing);
        adjacents.Add(TrafficLib.ReverseDirections(tile.Facing));

        for (int i = 0; i < adjacents.Count; i++) {
            if (tile.NeighborSystem.Neighbors[(adjacents[i], Direction.None)].Tile is TileRoad neighbor == false) {
                continue;
            }
            if (neighbor.Facing == TrafficLib.ReverseDirections(tile.Facing)) { // Facing opposite
                continue;
            }
            if (tile.RoadtileNeighbors.IsNeighborFacingThis(adjacents[i])) { // Facing this
                tile.AddInConnection(adjacents[i]);
                neighbor.AddOutConnection(TrafficLib.ReverseDirections(adjacents[i]));
                continue;
            }
            if (neighbor.Facing == adjacents[i]) { // Facing away
                if (neighbor.InConnections[tile.Facing] == true) {
                    continue;
                }
                tile.AddOutConnection(adjacents[i]);
                neighbor.AddInConnection(TrafficLib.ReverseDirections(adjacents[i]));
                continue;
            }
            if (neighbor.Facing == tile.Facing) { // Facing Same
                tile.AddInConnection(adjacents[i]);
                neighbor.AddInConnection(TrafficLib.ReverseDirections(adjacents[i]));
                tile.AddOutConnection(adjacents[i]);
                neighbor.AddOutConnection(TrafficLib.ReverseDirections(adjacents[i]));
                continue;
            }
        }

        SetLines(tile);
    }

    public void SetLines(TileRoad tile) {
        HashSet<Direction> connections = new();
        foreach (var con in tile.InConnections) {
            if (con.Value == false) {
                continue;
            }
            connections.Add(con.Key);
        }
        foreach (var con in tile.OutConnections) {
            if (con.Value == false) {
                continue;
            }
            connections.Add(con.Key);
        }

        foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
            if(dir == Direction.None) {
                continue;
            }

            if (tile.NeighborSystem.Neighbors[(dir, Direction.None)].Tile is TileRoad neighbor == false) { 
                tile.ToggleLine(dir, true); // Roadside
                tile.SetLineType(dir, DirectionSeparatorWhole);
                continue;
            }

            if (tile.OutConnections[dir] == true && tile.InConnections[dir] == true) {
                tile.SetLineType(dir, LaneSeparatorDotted);
                tile.ToggleLine(dir, true);
                continue;
            }

            if (tile.OutConnections[dir] == false && tile.InConnections[dir] == false) {
                tile.SetLineType(dir, DirectionSeparatorWhole);
                tile.ToggleLine(dir, true);
                continue;
            }

            tile.ToggleLine(dir, false);
        }

    }

    public void ToggleCrosswalk(TileRoad tile) {
        Direction[] flankDirections = TrafficLib.GetFlankDirections(tile.Facing);
        TileRoad currentRoad = tile;
        HashSet<TileRoad> roadToSetCrosswalk = new HashSet<TileRoad>() { tile };
        bool addCrosswalk = !tile.HasCrosswalk;

        for (int i = 0; i < flankDirections.Length; i++) {
            bool roadEnd = false;
            while (roadEnd == false) {
                roadToSetCrosswalk.Add(currentRoad);
                if (currentRoad.NeighborSystem.Neighbors[(flankDirections[i], Direction.None)].Tile is TileRoad nextRoad) {
                    currentRoad = nextRoad;
                }
                else {
                    roadEnd = true;
                }
            }
        }

        foreach (TileRoad tileToAddCw in roadToSetCrosswalk) {
            tileToAddCw.ToggleCrosswalk(addCrosswalk);
            if (tileToAddCw.NeighborSystem.Neighbors[(TrafficLib.ReverseDirections(tileToAddCw.Facing), Direction.None)].Tile is TileRoad stopTile) {
                stopTile.SetLineType(stopTile.Facing, StopLine);
                stopTile.ToggleLine(stopTile.Facing, addCrosswalk);
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

        Direction[] flanks = TrafficLib.GetFlankDirections(tile.Facing);
        Direction up = tile.Facing;
        Direction left = flanks[0];
        Direction right = flanks[1];

        Dictionary<Direction, bool> modifiedSides = new() {
            { up, false },
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
                outDirs[up] = true;
                break;
            case 1:
                outDirs[up] = true;
                outDirs[left] = true;
                break;
            case 2:
                outDirs[up] = true;
                outDirs[right] = true;
                break;
            case 3:
                outDirs[up] = true;
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
            Direction dir = TrafficLib.ReverseDirections(kvp.Key);
            if (tile.NeighborSystem.Neighbors[(kvp.Key, Direction.None)].Tile is TileRoad tileRoad == false) {
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
        Direction[] flanks = TrafficLib.GetFlankDirections(tile.Facing);

        for (int i = 0; i < tilesToCheck; i++) {
            foreach (Direction dir in flanks) {
                if (currentTile.NeighborSystem.Neighbors[(dir, Direction.None)].Tile is TileRoad flankTile) {
                    if (currentTile.OutConnections[dir] == false) {
                        continue;
                    }
                    if (flankTile.OutConnections[TrafficLib.ReverseDirections(dir)] == true) {
                        continue;
                    }
                    arrowDirs.Add(dir);

                    if (currentTile.OutConnections[tile.Facing] == false) {
                        continue;
                    }
                    arrowDirs.Add(tile.Facing);
                    Finish(tile, arrowDirs);
                    break;
                }
            }
            if (currentTile.NeighborSystem.Neighbors[(tile.Facing, Direction.None)].Tile is TileRoad nextTile) {
                currentTile = nextTile;
                continue;
            }
            break;
        }

        Finish(tile, arrowDirs);

        void Finish(TileRoad tile, HashSet<Direction> arrowDirs) {
            GridManager.TileList.GetRoadPointer(arrowDirs);
            tile.Pointer.SetDirections(GridManager.TileList.GetRoadPointer(arrowDirs));
            tile.TogglePointer(!tile.Pointer.gameObject.activeSelf);
            return;
        }
    }

}

