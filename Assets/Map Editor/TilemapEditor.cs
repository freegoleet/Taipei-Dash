using System;
using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    public enum TileType
    {
        Gameplay,
        Road,
        Autofit,
        Deco,
        None
    }

    public enum TileModType
    {
        Placement,
        Connections,
        Crosswalk,
        Stopline,
        Pointer
    }

    [ExecuteInEditMode]
    public class TilemapEditor : MonoBehaviour
    {
        [Header("Generic")]
        [SerializeField]
        private GridManager m_GridManager = null;
        [SerializeField]
        private TilemapCursor m_TilemapCursor = null;
        [SerializeField]
        private UI_Neighbors m_Neighbors = null;

        public Direction[] Directions = { Direction.Left, Direction.Right, Direction.Up, Direction.Down };

        public int TileSize { get; private set; } = 10;

        // Scriptable Objects
        public SOTile_List TileList { get; private set; }
        public SO_Tile SelectedTile { get; private set; } = null;
        public List<SO_TileGameplay> SOGameplayTiles { get; private set; } = null;
        public List<SO_TileDecorative> SODecoTiles { get; private set; } = null;
        public List<SO_RoadPointer> SORoadPointers { get; private set; } = null;
        public List<SO_RoadLine> SORoadLines { get; private set; } = null;

        // Other
        public TileModType CurrentTileModType { get; private set; }
        public bool ShowConnections { get; private set; } = false;
        public Tile HoveredTile { get; private set; } = null;
        public List<TileGUI> GUITileList { get; private set; } = new();
        public Dictionary<HashSet<Direction>, SO_RoadPointer> Pointers { get; private set; } = null;
        public Dictionary<SO_Tile, UI_Item_Button_Tile> Buttons { get; private set; } = new();
        public GridManager GridManager { get => m_GridManager; }
        public GameObject HoverOverlay { get; private set; } = null;
        public UI_Neighbors UI_Neighbors { get { return m_Neighbors; } }
        public Texture2D[] Textures { get; private set; } = null;
        public int LayerToEdit { get; private set; } = 0;
        public bool ShowPointer { get; private set; } = false;
        public TilemapCursor TilemapCursor { get => m_TilemapCursor; private set => m_TilemapCursor = value; }

        public Action<int> OnNewCursorTile = null;
        public Action<int, bool> OnShowLayer = null;

        public void ToggleShowConnections(bool show) {
            ShowConnections = show;
            var tiles = GridManager.TileManager.RoadTileManager.Tiles;
            foreach (var tile in tiles) {
                tile.DebugDirections.ToggleShowDebugDirections(show);
            }
        }

        public void SelectNewTileType(int index) {
            SelectedTile = SOGameplayTiles[index];
            TilemapCursor.SelectNewTileType(SOGameplayTiles[index]);

            if (SOGameplayTiles[index].Rotatable == false) {
                Tile cursorTile = TilemapCursor.GetCurrentCursorTile();
                if(cursorTile != null) { 
                    TilemapCursor.GetCurrentCursorTile().SetFacing(Direction.Up);
                }
            }
        }

        public void SelectLayerToEdit(int layer) {
            LayerToEdit = layer;
            if (layer == 0) {

                return;
            }
        }

        public void ShowLayer(int i, bool show) {
            m_GridManager.ToggleLayerVisibility(i, show);
        }

        public void PlaceTile(Tile tile) {
            HandleGameplayTile(tile);
            //if (LayerToEdit == 0) {
            //    return;
            //}

            //TileDeco decoTile = m_GridManager.GetDecorativeTileAtLocation(LayerToEdit - 1, tile.transform.position);
            //if (decoTile == null) {
            //    TileDeco newTile = m_GridManager.TileManager.DecoTileManager.AddNewDecorativeTile(
            //        LayerToEdit - 1,
            //        (SO_TileDecorative)TilemapCursor.GetCursorTile().Data,
            //        tile.transform.position,
            //        tile.GridPosition);
            //    newTile.Shadow.gameObject.SetActive(true);
            //    newTile.ImgBackground.sortingLayerName = "Deco";
            //    newTile.ImgBackground.sortingOrder = LayerToEdit + 1;
            //    newTile.Shadow.sortingLayerName = "Deco";
            //    newTile.Shadow.sortingOrder = LayerToEdit;
            //}
        }

        private void HandleGameplayTile(Tile tile) {
            if (tile.TileType != TilemapCursor.CurrentTile.TileType) {
                tile = GridManager.TileManager.ReplaceTile(tile, TilemapCursor.CurrentTile.TileType);
            }

            switch (tile.TileType) {
                case TileType.Gameplay:
                    break;
                case TileType.Road:
                    break;
                case TileType.Autofit:
                    break;
                case TileType.Deco:
                    break;
                case TileType.None:
                    break;
            }

            tile.Initialize(TilemapCursor.CurrentTile.Data, TilemapCursor.HoveredTileGridIndex);
            GridManager.SetNeighbors(tile, true);
            if (tile.Data.Rotatable == true) {
                tile.SetFacing(TilemapCursor.CurrentTile.Facing);
            }
            GridManager.TileManager.SetupTileNeighbors((TileGameplay)tile, true);


            if (tile is TileRoad roadTile) {
                roadTile.ToggleShowConnections(ShowConnections);
            }
        }

        public void RemoveTile(Tile tile) {
            if (LayerToEdit == 0) {
                return;
            }
            m_GridManager.TileManager.ReturnTile(tile);
        }

        public void Refresh() {
            SOGameplayTiles = GridManager.TileList.GameplayTiles;
            SORoadLines = GridManager.TileList.Lines;
            SORoadPointers = GridManager.TileList.Pointers;
            SODecoTiles = GridManager.TileList.DecoTiles;

            TileSize = GridManager.TileSize;
            Pointers = GridManager.TileList.GetPointers();
            Textures = new Texture2D[SOGameplayTiles.Count];
            OnNewCursorTile = SelectNewTileType;
            UI_Neighbors.Initialize();

            for (int i = 0; i < SOGameplayTiles.Count; i++) {
                TileGUI newTile = new TileGUI();
                newTile.Texture2D = SOGameplayTiles[i].Background.texture;
                GUITileList.Add(newTile);
                if (SOGameplayTiles[i].Background != null) {
                    Textures[i] = SOGameplayTiles[i].Background.texture;
                    if (SOGameplayTiles[i] is SO_TileAutoFit af) {
                        Textures[i] = af.TileMiddle.texture;
                    }
                    continue;
                }
            }
        }

        public void SwitchTileModType(TileModType tileModType) {
            CurrentTileModType = tileModType;

            switch (tileModType) {
                case TileModType.Placement:
                    TilemapCursor.ToggleShowCursorTile(true);
                    break;
                case TileModType.Connections:
                    TilemapCursor.ToggleShowCursorTile(false);
                    break;
                case TileModType.Crosswalk:
                    TilemapCursor.ToggleShowCursorTile(false);
                    break;
                case TileModType.Stopline:
                    TilemapCursor.ToggleShowCursorTile(false);
                    break;
                case TileModType.Pointer:
                    TilemapCursor.ToggleShowCursorTile(false);
                    break;
            }
        }

        public void EditTile(Tile tile, bool clockwise) {
            switch (CurrentTileModType) {
                case TileModType.Placement:
                    PlaceTile(tile);
                    break;
            }
            if (tile is TileRoad tileRoad == true) {
                switch (CurrentTileModType) {
                    case TileModType.Connections:
                        GridManager.TileManager.RoadUtils.ToggleDrivableDirections(tileRoad, clockwise);
                        break;
                    case TileModType.Crosswalk:
                        GridManager.TileManager.RoadUtils.ToggleCrosswalk(tileRoad);
                        break;
                    case TileModType.Stopline:
                        GridManager.TileManager.RoadUtils.ToggleStopline(tileRoad);
                        break;
                    case TileModType.Pointer:
                        GridManager.TileManager.RoadUtils.TogglePointer(tileRoad);
                        break;

                }
            }
        }
    }
}