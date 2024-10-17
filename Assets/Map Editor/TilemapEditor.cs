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
        public List<SO_TileGameplay> SOGameplayTiles { get; private set; } = null;
        public List<SO_TileDecorative> SODecoTiles { get; private set; } = null;
        public List<SO_RoadPointer> SORoadPointers { get; private set; } = null;
        public List<SO_RoadLine> SORoadLines { get; private set; } = null;

        // Other
        public Tile HoveredTile { get; private set; } = null;
        public List<TileGUI> GUITileList { get; private set; } = new();
        public Dictionary<List<Direction>, SO_RoadPointer> Pointers { get; private set; } = null;
        public Dictionary<SO_Tile, UI_Item_Button_Tile> Buttons { get; private set; } = new();
        public GridManager GridManager { get => m_GridManager; }
        public GameObject HoverOverlay { get; private set; } = null;
        public UI_Neighbors UI_Neighbors { get { return m_Neighbors; } }
        public Texture2D[] Textures { get; private set; } = null;
        public int LayerToEdit { get; private set; } = 0;
        public bool ShowPointers { get; private set; } = false;
        public TilemapCursor TilemapCursor { get => m_TilemapCursor; private set => m_TilemapCursor = value; }

        public Action<int> OnNewCursorTile = null;
        public Action<int, bool> OnShowLayer = null;

        public void ToggleShowPointers(bool show) {
            var tiles = GridManager.TileManager.GetAllActiveGameplayTiles();
            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i] is TileRoad tile) {
                    if (tile.Data == null) {
                        tile.SetData(SOGameplayTiles[0]);
                    }

                    if (tile.Data.Rotatable == true) {
                        tile.TogglePointer(show);
                    }
                }
            }

            if (TilemapCursor.GetCursorTile() as NodeBase is TileRoad roadTile) {
                roadTile.TogglePointer(show);
            }

            ShowPointers = show;
        }

        public void SelectNewTileType(int index) {
            TilemapCursor.SelectNewTileType(SOGameplayTiles[index]);

            if (SOGameplayTiles[index].Rotatable == false) {
                TilemapCursor.GetCursorTile().SetFacing(Traffic.Direction.Up);
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
            if (LayerToEdit == 0) {
                HandleGameplayTile(tile);
                return;
            }

            TileDeco decoTile = m_GridManager.GetDecorativeTileAtLocation(LayerToEdit - 1, tile.transform.position);
            if (decoTile == null) {
                TileDeco newTile = m_GridManager.TileManager.DecoTileManager.AddNewDecorativeTile(
                    LayerToEdit - 1,
                    (SO_TileDecorative)TilemapCursor.GetCursorTile().Data,
                    tile.transform.position,
                    tile.GridPosition);
                newTile.Shadow.gameObject.SetActive(true);
                newTile.ImgBackground.sortingLayerName = "Deco";
                newTile.ImgBackground.sortingOrder = LayerToEdit + 1;
                newTile.Shadow.sortingLayerName = "Deco";
                newTile.Shadow.sortingOrder = LayerToEdit;

            }
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
                    //TileAutofit autofit = (TileAutofit)tile;
                    //SetupAutofit(autofit, true);
                    break;
                case TileType.Deco:
                    break;
                case TileType.None:
                    break;
            }

            tile.Initialize(TilemapCursor.CurrentTile.Data, TilemapCursor.HoveredTileGridIndex);
            GridManager.TileManager.SetupAutofit((TileGameplay)tile, true);
            if(tile.Data.Rotatable == true) {
                tile.SetFacing(TilemapCursor.CurrentTile.Facing);
            }

            if (tile is TileRoad roadTile) {
                roadTile.TogglePointer(ShowPointers);
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
    }
}