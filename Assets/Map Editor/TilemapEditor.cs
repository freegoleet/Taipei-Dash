using System;
using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
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
        [SerializeField]
        private UI_Connections m_Connections = null;

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
        public Tile HoveredTile { get; set; } = null;
        public List<TileGUI> GUITileList { get; private set; } = new();
        public Dictionary<RoadPointer, SO_RoadPointer> Pointers { get; private set; } = null;
        public Dictionary<SO_Tile, UI_Item_Button_Tile> Buttons { get; private set; } = new();
        public GridManager GridManager { get => m_GridManager; }
        public GameObject HoverOverlay { get; private set; } = null;
        public UI_Neighbors UI_Neighbors { get { return m_Neighbors; } }
        public UI_Connections UI_Connections { get { return m_Connections; } }
        public Texture2D[] Textures { get; private set; } = null;
        public int LayerToEdit { get; private set; } = 0;
        public bool ShowPointer { get; private set; } = false;
        public TilemapCursor TilemapCursor { get => m_TilemapCursor; private set => m_TilemapCursor = value; }

        // TrafficLight
        private TrafficLight m_FirstTL = null;

        public Action<Tile> OnNewTileHovered = null;
        public Action<int> OnNewCursorTile = null;
        public Action<int, bool> OnShowLayer = null;

#if UNITY_EDITOR
        public void EditorTick(float dt) {
            ShowTrafficLightConnections();
        }
#endif

        public void NewTileHovered(Tile tile, Vector2Int newPos) {
            if(HoveredTile != null) {
                if (HoveredTile.GridPosition == newPos) {
                    return;
                }
            }

            HoveredTile = tile;
            TilemapCursor.UpdateCursorPos(tile);
            OnNewTileHovered?.Invoke(tile);

            if (TilemapCursor.GetCurrentCursorTile() is Tile cursorTile == false) {
                return;
            }
            if(tile is TileRoad tileRoad) {
                Direction[] dirs = new Direction[4];
                for (int i = 0; i < tileRoad.Connections.OutConnections.Length; i++) {
                    if (tileRoad.Connections.OutConnections[i] == true) {
                        dirs[i] = (Direction)i;
                        continue;
                    }
                    dirs[i] = Direction.None;
                }
                UI_Connections.ShowConnections(dirs);
            }
            if (tile.Data == cursorTile.Data) {
                UI_Neighbors.ShowNeighbors(tile.NeighborSystem.GetAllFittableNeighbors());
            }
            else {
                UI_Neighbors.ShowNeighbors(tile.NeighborSystem.GetAllUnfittableNeighbors());
            }
            
        }

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
                if (cursorTile != null) {
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
            UI_Connections.Initialize();

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
                case TileModType.TrafficLight:
                    TilemapCursor.ToggleShowCursorTile(false);
                    break;
            }
        }

        public void EditTile(bool standard) {
            switch (CurrentTileModType) {
                case TileModType.Placement:
                    PlaceTile(HoveredTile);
                    break;
            }
            if (HoveredTile is TileRoad tileRoad == true) {
                switch (CurrentTileModType) {
                    case TileModType.Connections:
                        GridManager.TileManager.RoadUtils.ToggleDrivableDirections(tileRoad, standard);
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
                    case TileModType.TrafficLight:
                        GridManager.TileManager.RoadUtils.ToggleTrafficLight(tileRoad);
                        break;
                }
            }
        }

        public void EditTileSecondary() {
            switch (CurrentTileModType) {
                case TileModType.Placement:
                    PlaceTile(HoveredTile);
                    break;
            }
            if (HoveredTile is TileRoad tileRoad == true) {
                switch (CurrentTileModType) {
                    case TileModType.Connections:
                        break;
                    case TileModType.Crosswalk:
                        break;
                    case TileModType.Stopline:
                        break;
                    case TileModType.Pointer:
                        break;
                    case TileModType.TrafficLight:
                        TrafficLight light = GetTrafficLight(HoveredTile);
                        ConnectTrafficLights(light);
                        break;
                }
            }
        }

        public TrafficLight GetTrafficLight(Tile tile) {
            if (tile is TileRoad road == false) {
                return null;
            }
            return road.TrafficLight;
        }

        public void ConnectTrafficLights(TrafficLight trafficLight) {
            if (m_FirstTL == null) {
                m_FirstTL = trafficLight;
                m_FirstTL.Road.ToggleHighlight(true);
                return;
            }

            if(trafficLight.Road.Facing == m_FirstTL.Road.Facing ||
                TrafficUtilities.ReverseDirections(trafficLight.Road.Facing) == m_FirstTL.Road.Facing) {
                trafficLight.SyncNew(m_FirstTL);
            }
            else
            {
                trafficLight.SyncNewReverse(m_FirstTL);
            }

            m_FirstTL.SyncTimers();
            m_FirstTL.Road.ToggleHighlight(false);
            m_FirstTL = null;
        }

        public void ShowTrafficLightConnections() {
            TrafficLight trafficLight = GetTrafficLight(HoveredTile);
            if (trafficLight == null) {
                return;
            }
            foreach (TrafficLight light in trafficLight.SyncedLights) {
                Debug.DrawLine(trafficLight.transform.position, light.transform.position, Color.green);
            }
            foreach (TrafficLight light in trafficLight.SyncedReverseLights) {
                Debug.DrawLine(trafficLight.transform.position, light.transform.position, Color.red);
            }
        }
    }
}