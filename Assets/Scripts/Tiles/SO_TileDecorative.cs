using Traffic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDecorative", menuName = "ScriptableObjects/Tiles/TileDecorative", order = 1)]
public class SO_TileDecorative : SO_Tile
{
    public override TileType GetTileType() {
        return TileType.Deco;
    }
}
