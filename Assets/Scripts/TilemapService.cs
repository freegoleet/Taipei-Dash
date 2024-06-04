using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class TilemapService
{

    // Generic
    public List<Entity> EveryEntity { get; private set; } = new();

    // Mouse
    private GameObject m_TargetGO = null;
    private GameObject m_HoverGO = null;

    // Tiles
    private Tile m_HoverTile = null;
    private Tile m_PreviousHoverTile = null;
    private Tile m_StartTile = null;
    private Tile m_TargetTile = null;

    // Managers
    private GridManager m_GridManager = null;

    public Action<TilemapService> OnSetStartTile = null;

    public GameObject TargetGO { get => m_TargetGO; set => m_TargetGO = value; }
    public GameObject HoverGO { get => m_HoverGO; set => m_HoverGO = value; }
    public Tile HoverTile { get => m_HoverTile; set => m_HoverTile = value; }
    public Tile PreviousHoverTile { get => m_PreviousHoverTile; set => m_PreviousHoverTile = value; }
    public Tile StartTile { get => m_StartTile; set => m_StartTile = value; }
    public Tile TargetTile { get => m_TargetTile; set => m_TargetTile = value; }
    public GridManager GridManager { get => m_GridManager; set => m_GridManager = value; }

    public TilemapService(GridManager gridManager)
    {
        m_GridManager = gridManager;
    }

    public void InitializePrefabs()
    {
        m_TargetGO.SetActive(false);

        m_HoverGO.SetActive(true);
    }
}