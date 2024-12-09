using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class RoadtileNeighbors
{
    private TileRoad Tile { get; set; } = null;

    public List<TileRoad> ParallelRoads { get; set; } = new();

    public RoadtileNeighbors(TileRoad tile) {
        Tile = tile;
    }

    private Dictionary<Vector2Int, Direction[]> DrivableDirectionsOfNeighbors = new() {
        { new Vector2Int(0, 1), new Direction[4] },
        { new Vector2Int(0, -1), new Direction[4] },
        { new Vector2Int(-1, 0), new Direction[4] },
        { new Vector2Int(1, 0), new Direction[4] }
    };

    public void SetDrivableDirectionsOfNeighbor(Vector2Int relativeNeighborGridpos, Direction[] directions) {
        DrivableDirectionsOfNeighbors[relativeNeighborGridpos] = directions;
    }

    public Dictionary<Vector2Int, Direction[]> GetDrivableDirectionsOfNeighbors() {
        return DrivableDirectionsOfNeighbors;
    }

    public bool IsNeighborFacingThis(Direction directionToNeighbor) {
        Tile tile = Tile.NeighborSystem.GetNeighborTile((directionToNeighbor, Direction.None));
        if (Tile == tile.NeighborSystem.GetNeighborTile((tile.Facing, Direction.None))) {
            return true;
        }
        return false;
    }

    public bool IsNeighborFacingOpposite(Direction directionToNeighbor) {
        Tile tile = Tile.NeighborSystem.GetNeighborTile((directionToNeighbor, Direction.None));
        if(TrafficUtilities.ReverseDirections((Tile.Facing, Direction.None)).Item1 == tile.Facing) {
            return true;
        }
        return false;
    }

    public bool IsNeighborFacingAway(Direction directionToNeighbor) {
        if (Tile.NeighborSystem.GetNeighborTile((directionToNeighbor, Direction.None)).Facing == directionToNeighbor) {
            return true;
        }
        return false;
    }

    public Direction GetDirOfNeighborInRelationToFacing(Direction direction) {
        int index = Mathf.Abs(Tile.Facing - direction);
        switch (index) {
            case 0:
                return Direction.Down;
            case 1: 
                return Direction.Right;
            case 2:
                return Direction.Down;
            case 3:
                return Direction.Left;
        }
        return Direction.None;
    }
}
