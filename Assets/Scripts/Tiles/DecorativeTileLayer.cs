using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class DecorativeTileLayer
{
    public Dictionary<Vector2, TileDeco> TilesByLocation { get; private set; }

    public int Layer { get; private set; }

    public DecorativeTileLayer(int layer) {
        Layer = layer;
    }

    public TileDeco GetTile(Vector2 pos) {
        return TilesByLocation[pos];
    }

    public void AddTile(TileDeco tile) {
        TilesByLocation.Add(tile.Position, tile);
    }

    public void RemoveTile(Vector2 pos) {
        TilesByLocation.Remove(pos);
    }

    public void ToggleVisibility(bool visible) {
        foreach (var tile in TilesByLocation) {
            tile.Value.gameObject.SetActive(visible);
        }
    }
}
