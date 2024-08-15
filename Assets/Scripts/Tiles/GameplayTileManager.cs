using System.Collections.Generic;
using System.Linq;
using Traffic;
using UnityEditor;
using UnityEngine;

public class GameplayTileManager
{
    public Dictionary<TileType, Tile> TilePrefabs { get; private set; } = new();

    public List<Tile> AllTiles { get; private set; } = new();
    public List<Tile> AllActiveTiles { get; private set; } = new();
    public Dictionary<Vector2, Tile> ActiveTilesByLocation { get; private set; } = new();


    public Dictionary<TileType, List<Tile>> InactiveTiles { get; private set; } = new();
    public Dictionary<TileType, List<Tile>> AllTilesByType { get; private set; } = new();
    public Dictionary<TileType, List<Tile>> ActiveTiles { get; private set; } = new();
    public Transform TileContainer { get; set; } = null;

    public int TileCount { get; private set; } = 0;
    public int ActiveTileCount { get; private set; } = 0;

    public void AddTiles(List<Tile> tiles) {
        AllTiles.AddRange(tiles);
    }

    public void ReturnTile(Tile tile) {
        tile.Entity = null;
        ActiveTiles[tile.TileType].Remove(tile);
        InactiveTiles[tile.TileType].Add(tile);
        tile.gameObject.SetActive(false);
    }

    public void ReturnTiles(List<Tile> tiles) {
        foreach (Tile tile in tiles) {
            ReturnTile(tile);
        }
    }

    public Tile GetNewTile(TileType tileType) {
        Tile tile = null;
        List<Tile> list = InactiveTiles[tileType].ToList();

        if (list.Count > 0) {
            if (list[0] == null) {
                list.RemoveAt(0);
                return GetNewTile(tileType);
            }

            tile = list[0];

            tile.gameObject.SetActive(true);
            list.Remove(tile);
        }
        else {
            tile = Object.Instantiate(TilePrefabs[tileType], TileContainer);
            AllTilesByType[tileType].Add(tile);
            AllTiles.Add(tile);
            TileCount++;
        }

        SceneVisibilityManager.instance.DisablePicking(tile.gameObject, true);
        ActiveTiles[tileType].Add(tile);

        return tile;
    }

    public void DestroyAllTiles() {
        foreach (Tile tile in AllTiles) {
            if (tile == null) {
                continue;
            }
            Object.DestroyImmediate(tile.gameObject);
        }

        AllActiveTiles.Clear();
        AllTiles.Clear();
        ActiveTiles.Clear();
        InactiveTiles.Clear();
        AllTilesByType.Clear();
    }
}
