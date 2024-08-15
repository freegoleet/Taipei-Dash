using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Traffic
{
    public class DecorativeTileManager
    {
        public List<TileDeco> AllTiles { get; private set; } = new();
        public List<TileDeco> ActiveTiles { get; private set; } = new();
        public List<TileDeco> InactiveTiles { get; private set; } = new();
        public Dictionary<int, DecorativeTileLayer> DecoLayers { get; private set; } = new();

        public Transform TileContainer { get; set; } = null;
        public TileDeco TilePrefab { get; set; } = null;

        public int TileCount { get; private set; } = 0;

        public DecorativeTileManager(int layerCount) {
            for (int i = 0; i < layerCount; i++) {
                DecoLayers.Add(i, new DecorativeTileLayer(i));
            }
        }

        public void SetupDecorativeLayers(int rows) {
            for (int i = 0; i < rows; i++) {
                if (DecoLayers.ContainsKey(i) == true) {
                    continue;
                }
                DecoLayers.Add(i, new(i));
            }
        }

        public TileDeco AddNewDecorativeTile(int layer, SO_Tile tileType, Vector2 position, int col, int row) {
            TileDeco tile = GetNewTile();
            tile.SetLayer(layer);
            tile.Initialize(tileType, col, row);
            tile.transform.position = position;
            DecoLayers[layer].AddTile(tile);

            return tile;
        }

        public TileDeco GetNewTile() {
            TileDeco tile = null;

            if (InactiveTiles.Count > 0) {
                if (InactiveTiles[0] == null) {
                    InactiveTiles.RemoveAt(0);
                    return GetNewTile();
                }

                tile = InactiveTiles[0];

                tile.gameObject.SetActive(true);
                InactiveTiles.Remove(tile);
            }
            else {
                tile = Object.Instantiate(TilePrefab, TileContainer);
                AllTiles.Add(tile);
                TileCount++;
            }

            SceneVisibilityManager.instance.DisablePicking(tile.gameObject, true);
            ActiveTiles.Add(tile);

            return tile;
        }

        public void ReturnDecoTile(Vector2 pos, int layer) {
            TileDeco tile = DecoLayers[layer].GetTile(pos);
            if (tile == null) {
                return;
            }

            DecoLayers[layer].RemoveTile(pos);
            InactiveTiles.Add(tile);
            tile.gameObject.SetActive(false);
        }

    }
}