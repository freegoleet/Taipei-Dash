using System;
using System.Collections.Generic;
using Traffic;
using UnityEngine;

public enum eTilemapMode
{
    Pathfinding,
    LineOfSight
}

public static class TilemapUtilities
{
    public static List<Entity> EveryEntity { get; private set; } = new();

    // Mouse
    public static GameObject TargetGO { get; set; } = null;
    public static GameObject HoverGO { get; set; } = null;

    // Tiles
    public static TileGameplay CurrentHoverTile { get; set; } = null;
    public static TileGameplay PreviousHoverTile { get; set; } = null;
    public static TileGameplay StartTile { get; set; } = null;
    public static TileGameplay TargetTile { get; set; } = null;

    // Managers
    public static GridManager GridManager { get; private set; } = null;

    public static Action OnSetStartTile = null;

    private static eTilemapMode m_CurrentTilemapMode = eTilemapMode.Pathfinding;


    static TilemapUtilities()
    {
    }

    public static bool HoverTile(TileGameplay hoveredTile)
    {
        if (hoveredTile == null)
        {
            return false;
        }

        if (CurrentHoverTile != hoveredTile)
        {
            CurrentHoverTile = hoveredTile;
            HoverGO.transform.position = CurrentHoverTile.transform.position;
        }

        if (hoveredTile == PreviousHoverTile)
        {
            return true;
        }

        PreviousHoverTile = hoveredTile;

        HoverNewTile();

        return true;
    }

    public static void HoverNewTile()
    {
        switch (m_CurrentTilemapMode)
        {
            case eTilemapMode.Pathfinding:
                break;
            case eTilemapMode.LineOfSight:
                break;
        }
    }

    public static void InitializePrefabs() {
        TargetGO.SetActive(false);

        HoverGO.SetActive(true);
    }
}
