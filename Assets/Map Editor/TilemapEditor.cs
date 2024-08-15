using System;
using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    public enum TileType
    {
        Road,
        Gameplay,
        Deco,
        None
    }

    [ExecuteInEditMode]
    public class TilemapEditor : MonoBehaviour
    {
        [Header("Generic")]
        [SerializeField]
        private GridManager m_GridManager = null;

        [Header("Cursor")]
        [SerializeField]
        private GameObject m_HoverOverlay = null;
        [SerializeField]
        private TileGameplay m_CursorGameplay = null;
        [SerializeField]
        private TileRoad m_CursorRoad = null;
        [SerializeField]
        private TileDeco m_CursorDeco = null;

        private TileType CurrentCursorTileType { get; set; } = TileType.None;

        public Directions[] Directions = { Traffic.Directions.Left, Traffic.Directions.Right, Traffic.Directions.Up, Traffic.Directions.Down };

        public int TileSize { get; private set; } = 10;

        // Scriptable Objects
        public List<SO_TileGameplay> SOGameplayTiles { get; private set; } = null;
        public List<SO_TileRoad> SORoadTiles { get; private set; } = null;
        public List<SO_TileDecorative> SODecoTiles { get; private set; } = null;
        public List<SO_RoadPointer> SORoadPointers { get; private set; } = null;
        public List<SO_RoadLine> SORoadLines { get; private set; } = null;

        // Cursor
        public Tile CursorTile { get; private set; }
        public TileGameplay CursorGameplay { get => m_CursorGameplay; set => m_CursorGameplay = value; }
        public TileRoad CursorRoad { get => m_CursorRoad; set => m_CursorRoad = value; }
        public TileDeco CursorDeco { get => m_CursorDeco; set => m_CursorDeco = value; }

        // Other
        public List<TileRoadGUI> TileList { get; private set; } = new();
        public Dictionary<List<Directions>, SO_RoadPointer> Pointers { get; private set; } = null;
        public Dictionary<SO_Tile, UI_Item_Button_Tile> Buttons { get; private set; } = new();
        public GridManager GridManager { get => m_GridManager; }
        public GameObject HoverOverlay { get; private set; }
        public Texture2D[] Textures { get; private set; }
        public int LayerToEdit { get; private set; } = 0;
        public bool ShowPointers { get; private set; } = false;

        public Action<int> OnNewCursorTile = null;
        public Action<int, bool> OnShowLayer = null;

        private void Start() {
            CursorTile = GetComponentInChildren<Tile>(true);

            if (HoverOverlay == null) {
                HoverOverlay = Instantiate(m_HoverOverlay, transform);
            }
        }

        public void ToggleShowCursorTile(bool show, TileType tileType = TileType.None) {
            HoverOverlay.SetActive(show);

            switch (CurrentCursorTileType) {
                case TileType.Road:
                    CursorRoad.gameObject.SetActive(show);
                    break;
                case TileType.Gameplay:
                    CursorGameplay.gameObject.SetActive(show);
                    break;
                case TileType.Deco:
                    CursorDeco.gameObject.SetActive(show);
                    break;
                case TileType.None:
                    break;
                default:
                    break;
            }

            if (show == false) {
                return;
            }

            if (tileType == CurrentCursorTileType) {
                return;
            }

            switch (tileType) {
                case TileType.Road:
                    CursorRoad.gameObject.SetActive(true);
                    break;
                case TileType.Gameplay:
                    CursorGameplay.gameObject.SetActive(true);
                    break;
                case TileType.Deco:
                    CursorDeco.gameObject.SetActive(true);
                    break;
                case TileType.None:
                    return;
            }

            CurrentCursorTileType = tileType;

            //if (HoverOverlay == null) {
            //    HoverOverlay = Instantiate(m_HoverOverlay, transform);
            //}

            //CursorTile = GetComponentInChildren<Tile>(true);

            //if (CursorTile == null) {
            //    CursorTile = Instantiate(Tile, transform);
            //    CursorTile.Initialize(SOTileList[0]);
            //    CursorTile.ImgTile.sortingLayerName = "Controls";
            //    if(CursorTile is TileRoad tile) {
            //        tile.Pointer.sortingLayerName = "Controls";
            //        tile.TogglePointer(show);
            //    }
            //}

            //CursorTile.gameObject.SetActive(show);
        }

        public void ToggleShowPointers(bool show) {
            var tiles = GridManager.GetActiveTiles();
            for (int i = 0; i < tiles.Count; i++) {
                if (tiles[i] is TileRoad tile) {
                    if (tile.Data == null) {
                        tile.SetData(SOGameplayTiles[0]);
                    }

                    if (tile.Data.Rotatable == true) {
                        tile.TogglePointer(show);
                    }
                }
            }

            if (CursorTile is TileRoad roadTile) {
                roadTile.TogglePointer(show);
            }

            ShowPointers = show;
        }

        public void UpdateCursorPos(Vector2 pos) {
            CursorTile.transform.position = pos;
            HoverOverlay.transform.position = pos;
        }

        public void SelectNewTileType(SO_Tile tile) {
            CursorTile.Initialize(tile);
        }

        public void SelectNewTileType(int index) {
            CursorTile.Initialize(SOGameplayTiles[index]);
            if (SOGameplayTiles[index].Rotatable == false) {
                CursorTile.SetDirection(Traffic.Directions.Up);
            }
            Debug.Log("select new tile");
        }

        public void SelectLayerToEdit(int layer) {
            Debug.Log("Layer " + layer + " selected.");
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
                TileDeco newTile = m_GridManager.DecoTileManager.AddNewDecorativeTile(LayerToEdit - 1, CursorTile.Data, tile.transform.position, tile.Col, tile.Row);
                newTile.Shadow.gameObject.SetActive(true);
                newTile.ImgTile.sortingLayerName = "Deco";
                newTile.ImgTile.sortingOrder = LayerToEdit + 1;
                newTile.Shadow.sortingLayerName = "Deco";
                newTile.Shadow.sortingOrder = LayerToEdit;

            }
        }

        private void HandleGameplayTile(Tile tile) {

            if (CursorTile is TileRoad road) {
                if(road.Data)
                m_GridManager.GetNeighbors(tile);
            
            }





            if (tile is TileGameplay gameplay) {
                //CursorTile.Data
            }


            tile.Initialize(CursorTile.Data);
            tile.SetDirection(CursorTile.TileRotation);
            if (tile is TileRoad roadTile) {
                roadTile.TogglePointer(ShowPointers);
            }
        }

        public void RemoveTile(Tile tile) {
            if (LayerToEdit == 0) {
                return;
            }
            m_GridManager.DecoTileManager.ReturnDecoTile(tile.transform.position, LayerToEdit - 1);
        }

        public void Refresh() {
            SOGameplayTiles = GridManager.TileList.Tiles;
            SORoadTiles = GridManager.TileList.RoadTiles;
            SORoadLines = GridManager.TileList.Lines;
            SORoadPointers = GridManager.TileList.Pointers;
            SODecoTiles = GridManager.TileList.DecoTiles;

            TileSize = GridManager.TileSize;
            Pointers = GridManager.TileList.GetPointers();
            Textures = new Texture2D[SOGameplayTiles.Count];
            OnNewCursorTile = SelectNewTileType;

            for (int i = 0; i < SOGameplayTiles.Count; i++) {
                TileRoadGUI newTile = new TileRoadGUI();
                newTile.Texture2D = SOGameplayTiles[i].Sprite.texture;
                TileList.Add(newTile);
                Textures[i] = SOGameplayTiles[i].Sprite.texture;
            }
        }
    }
}