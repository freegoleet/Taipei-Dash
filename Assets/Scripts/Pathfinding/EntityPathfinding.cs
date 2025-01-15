using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityPathfinding
{
    public Entity Entity { get; private set; } = null;
    public List<Vector2Int> CurrentPath { get; private set; } = null;
    public List<TileGameplay> CurrentPathTiles { get; private set; } = null;
    private GridManager GridManager { get; set; } = null;

    public EntityPathfinding(Entity entity) {
        Entity = entity;
        GridManager = GameService.Instance.GridManager;
    }

    public void SetPath(List<TileGameplay> path) {
        ResetPath();
        CurrentPathTiles = path;
        if(CurrentPathTiles != null) {
            HighlightPath();
        }
    }

    public void TogglePathingValues(bool show) {
        int value = 1;
        var path = CurrentPathTiles;
        path.Reverse();
        foreach (Tile tile in path) {
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
