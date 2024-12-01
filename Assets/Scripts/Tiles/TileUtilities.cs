using UnityEngine;

public static class TileUtilities
{
    static TileUtilities()
    {
    }

    public static bool HoverTile(TileGameplay hoveredTile)
    {
        if (hoveredTile == null)
        {
            return false;
        }

        if (Input.GetMouseButtonDown(0))
        {
        }

        if (hoveredTile == TilemapUtilities.PreviousHoverTile)
        {
            return false;
        }

        TilemapUtilities.PreviousHoverTile = hoveredTile;

        return true;
    }

    

}
