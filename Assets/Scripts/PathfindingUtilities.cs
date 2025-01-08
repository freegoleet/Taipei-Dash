using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathfindingUtilities
{
    public enum PathfindType
    {
        Pedestrian,
        Car
    }

    public static  void ToggleStepThrough(bool active, bool stepThrough = false) {
        stepThrough = active;
        Pathfinder.ToggleStepThrough(active);
    }

    public static List<TileGameplay> GetPathTiles(TileGameplay startTile, TileGameplay endTile, PathfindType pathfindType, GridManager gridManager) {
        return Pathfinder.Automatic(startTile, endTile, pathfindType, gridManager);
    }

    public static Vector2Int[] GetPathVector2Int(TileGameplay startTile, TileGameplay endTile, PathfindType pathfindType, GridManager gridManager) {
        List<TileGameplay> path = GetPathTiles(startTile, endTile, pathfindType, gridManager);
        Vector2Int[] vector2Path = null;
        vector2Path = new Vector2Int[path.Count];
        for (int i = 0; i < vector2Path.Length; i++) {
            vector2Path[i] = new Vector2Int(path.ElementAt(i).Col, path.ElementAt(i).Row);
        }
        return vector2Path;
    }
}
