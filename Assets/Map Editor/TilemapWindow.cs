using System;
using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    [ExecuteInEditMode]
    public class TilemapWindow : MonoBehaviour {
        [SerializeField]
        private RectTransform m_RectTransform = null;
        [SerializeField]
        private UI_Item_Button_Tile m_ButtonPrefab = null;
        [SerializeField]
        private GridManager m_GridManager = null;
        [SerializeField]
        private Tile m_Tile = null;

        private bool m_Active = false;

        public Directions[] Directions = { Traffic.Directions.Left, Traffic.Directions.Right, Traffic.Directions.Up, Traffic.Directions.Down };

        public int TileSize { get; private set; } = 10;
        public List<SO_Tile> SOTileList { get; private set; } = null;
        public List<TileRoadGUI> TileList { get; private set; } = new();
        public Dictionary<Directions, SO_RoadPointer> Pointers { get; private set; } = null;
        public Dictionary<SO_Tile, UI_Item_Button_Tile> Buttons { get; private set; } = new();
        public GridManager GridManager { get => m_GridManager; }
        public Tile CursorTile { get; private set; } = null;
        public Tile Tile { get => m_Tile; }
        public Texture2D[] Textures { get; private set; }

        public Action<int> OnNewCursorTile = null;

        public void ToggleShowCursorTile(bool show) {
            if (CursorTile == null) {
                CursorTile = Instantiate(Tile);
                CursorTile.Initialize(SOTileList[0]);
            }

            CursorTile.ImgTile.sortingOrder = 4;
            CursorTile.ImgPointer.sortingOrder = 5;
            CursorTile.gameObject.SetActive(show);
        }

        public void ToggleShowPointers(bool show) {
            var tiles = GridManager.GetActiveTiles();
            for (int i = 0; i < tiles.Count; i++) {
                if (tiles[i].Data.Rotatable == true) {
                    tiles[i].TogglePointer(show);
                }
            }
        }

        public void UpdateCursorPos(Vector2 pos) {
            CursorTile.transform.position = pos;
        }

        public void SelectNewTileType(SO_Tile tile) {
            CursorTile.Initialize(tile);
        }

        public void SelectNewTileType(int index) {
            CursorTile.Initialize(SOTileList[index]);
            if(SOTileList[index].Rotatable == false) {
                CursorTile.SetDirection(Traffic.Directions.Up);
            }
            Debug.Log("select new tile");
        }

        public void SelectNewLayer(int index) {

        }

        public void Refresh() {
            SOTileList = GridManager.TileList.Tiles;
            TileSize = GridManager.TileSize;
            Pointers = GridManager.TileList.GetPointers();
            Textures = new Texture2D[SOTileList.Count];
            OnNewCursorTile = SelectNewTileType;

            for (int i = 0; i < SOTileList.Count; i++) {
                TileRoadGUI newTile = new TileRoadGUI();
                newTile.Texture2D = SOTileList[i].Sprite.texture;
                TileList.Add(newTile);
                Textures[i] = SOTileList[i].Sprite.texture;

            }
        }

        public UI_Item_Button_Tile CreateButton(SO_Tile tile) {
            UI_Item_Button_Tile button = Instantiate(m_ButtonPrefab, m_RectTransform);
            button.Initialize(tile);

            return button;
        }

        public void ToggleDropdown() {
            m_Active = !m_Active;
            m_RectTransform.gameObject.SetActive(m_Active);
        }

    }
}