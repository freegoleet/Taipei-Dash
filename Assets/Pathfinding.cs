using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    public Entity Entity { get; private set; } = null;
    public HashSet<Vector2Int> CurrentPath { get; private set; } = null;
    public HashSet<TileGameplay> CurrentPathTiles { get; private set; } = null;
    private GridManager GridManager { get; set; } = null;

    public Pathfinding(Entity entity) {
        Entity = entity;
        GridManager = GameService.Instance.GridManager;
    }

    public void SetPath(HashSet<TileGameplay> path) {
        ResetPath();
        CurrentPathTiles = path;
        HighlightPath();
    }

    public void TogglePathingValues(bool show) {
        int value = 1;
        var path = CurrentPathTiles;

        foreach (Tile tile in path.Reverse()) {
            tile.SetValue(value);
            value++;
            if (tile != path.First()) {
                tile.ToggleValue(show);
            }
        }
    }

    public void ToggleAStarValues(bool active) {
        foreach (Tile tile in GridManager.TileManager.AllTiles) {
            tile.ToggleValue(active);
        }
    }

    public void HighlightPath() {
        int currentSteps = 0;

        foreach (var tile in CurrentPathTiles) {
            tile.ToggleHighlight(true, GridManager.TileSettings.GetColor(SOTileSettings.eHighlightType.Path));
            currentSteps++;
        }
    }

    public void ResetPath(bool fullReset = false, bool alwaysShowAStarValues = false) {
        if (CurrentPathTiles == null) {
            return;
        }

        if (CurrentPathTiles.Count <= 0) {
            return;
        }

        if (alwaysShowAStarValues == false) {
            foreach (Tile tile in Pathfinder.GetAStarTiles()) {
                tile.ToggleValue(false);
            }
        }

        foreach (Tile tile in GameService.Instance.GridManager.TileManager.AllTiles) {
            tile.SetG(0);
            tile.SetH(0);
        }

        foreach (Tile tile in CurrentPathTiles) {
            tile.ToggleHighlight(false);
        }

        TogglePathingValues(false);
    }

}
