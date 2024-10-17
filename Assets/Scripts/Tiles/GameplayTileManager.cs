using System.Collections.Generic;
using System.Linq;
using Traffic;
using UnityEditor;
using UnityEngine;

public class GameplayTileManager<T> where T : Tile
{
    public TileType TileType { get; private set; } = TileType.None;
    public T TilePrefab { get; private set; } = null;
    public Transform TileContainer { get; set; } = null;
    public TileManager TileManager { get; private set; } = null;

    public List<T> AllTiles { get; private set; } = new();
    public Dictionary<Vector2Int, T> ActiveTilesByGridpos { get; private set; } = new();
    public int TileCount { get; private set; } = 0;

    public GameplayTileManager(TileType tileType, T tile, Transform activeTileContainer, TileManager tileManager) {
        TileType = tileType;
        TilePrefab = tile;
        TileContainer = activeTileContainer;
        TileManager = tileManager;
    }

    public List<T> AddTiles() {
        void AddNewTile(T tile) {
            Debug.Log("added tile " + typeof(T));
            ActiveTilesByGridpos.Add(tile.GridPosition, tile);
            TileManager.AllTilesByGridpos.Add(tile.GridPosition, tile);
            TileManager.GameplayTilecount++;
        }

        T[] list = TileContainer.GetComponentsInChildren<T>(true).ToArray();
        for (int i = 0; i < list.Length; i++) {
            if (list[i].GetType() != typeof(T)) {
                continue;
            }
            AllTiles.Add(list[i]);
        }

        foreach (var tile in AllTiles) {
            if(tile.GetType() != typeof(T)) {
                continue;
            }
            if (tile.gameObject.activeSelf == true) {
                try {
                    AddNewTile(tile);
                }
                catch {
                    if (tile.Data == null) {
                        tile.SetData(TileManager.GridManager.TileList.GetSOTileListByType(TileType)[0]);
                    }

                    int col = (int)tile.transform.localPosition.x / TileManager.GridManager.TileSize;
                    int row = (int)tile.transform.localPosition.y / TileManager.GridManager.TileSize;
                    tile.Initialize(tile.Data, new Vector2Int(col, row));
                    AddNewTile(tile);
                }
            }
        }

        return AllTiles;
    }

    public void RemoveTile(T tile) {
        AllTiles.Remove(tile);
        ActiveTilesByGridpos.Remove(tile.GridPosition);
        GameObject.DestroyImmediate(tile.gameObject);
        TileCount--;
        TileManager.GameplayTilecount--;
    }

    public void ReturnTile(Vector2Int pos) {
        T tile = ActiveTilesByGridpos[pos];
        RemoveTile(tile);
    }

    public void ReturnTiles(List<T> tiles) {
        foreach (T tile in tiles) {
            RemoveTile(tile);
        }
    }

    /// <summary>
    /// Get a new tile.
    /// </summary>
    /// <param name="gridPosition"> A position in the grid, not the transform's position. </param>
    /// <returns></returns>
    public T GetNewTile(Vector2Int gridPosition, int index) {
        T tile = default;
        
        tile = Object.Instantiate(TilePrefab.gameObject, TileContainer).GetComponent<T>();

        TileCount++;
        TileManager.GameplayTilecount++;

        AllTiles.Add(tile);
        TileManager.AllTiles[index] = tile;
        TileManager.AllTilesByGridpos[gridPosition] = tile;

        if (ActiveTilesByGridpos.ContainsKey(gridPosition) == true) {
            ActiveTilesByGridpos[gridPosition] = tile;
        }
        else {
            ActiveTilesByGridpos.Add(gridPosition, tile);
        }

        if(index >= 0) {
            tile.transform.SetSiblingIndex(index);
        }

        tile.transform.localPosition = (Vector3Int)(gridPosition * TileManager.GridManager.TileSize);
        SceneVisibilityManager.instance.DisablePicking(tile.gameObject, true); 

        return tile;
    }

    public List<T> GetAllTiles() {
        return AllTiles;
    }

    public void DestroyAllTiles() {
        foreach (T tile in AllTiles) {
            if (tile == null) {
                continue;
            }
            Object.DestroyImmediate(tile.gameObject);
        }

        AllTiles.Clear();
        ActiveTilesByGridpos.Clear();
        TileCount = 0;
    }
}
