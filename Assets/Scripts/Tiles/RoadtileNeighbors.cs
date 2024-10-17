using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class RoadtileNeighbors : NeighborSystem
{
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
}
