using UnityEditor;
using UnityEngine;

namespace Traffic
{
    [CustomEditor(typeof(TilemapEditor))]
    public class TilemapWindowEditor : Editor
    {
        private TilemapEditor window = null;
        private Tile m_HoveredTile = null;

        private bool m_ShowPointers = true;
        private bool m_ShowPointersOld = true;

        private int m_LayerToEdit = 0;
        private int m_LayerToEditOld = 1;
        private string[] m_LayerNames = new string[] { "Tiles", "Deco 1", "Deco 2", "Deco 3" };

        private int m_CursorTileIndex = 0;
        private int m_CursorTileIndexOld = 1;

        private bool[] m_LayersToShow = new bool[] { true, true, true };

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            window = (TilemapEditor)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Editing", GUILayout.Height(20), GUILayout.Width(100))) {
                SceneView.duringSceneGui += OnSceneMouseOver;
                window.ToggleShowCursorTile(true);
            }

            if (GUILayout.Button("Stop Editing", GUILayout.Height(20), GUILayout.Width(100))) {
                SceneView.duringSceneGui -= OnSceneMouseOver;
                if (m_HoveredTile != null) {
                    m_HoveredTile.ToggleHighlight(false);
                    m_HoveredTile = null;
                }

                window.ToggleShowCursorTile(false);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Tiles", GUILayout.Height(20), GUILayout.Width(100))) {
                window.Refresh();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            m_ShowPointers = GUILayout.Toggle(m_ShowPointers, "Show Pointers", GUILayout.Height(20), GUILayout.Width(100));
            if (m_ShowPointers != m_ShowPointersOld) {
                m_ShowPointersOld = m_ShowPointers;
                window.ToggleShowPointers(m_ShowPointers);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.TextArea("Layer to Edit.");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            m_LayerToEdit = GUILayout.SelectionGrid(m_LayerToEdit, m_LayerNames, 4);
            GUILayout.EndHorizontal();
            if (m_LayerToEdit != m_LayerToEditOld) {
                m_LayerToEditOld = m_LayerToEdit;
                window.SelectNewLayer(m_LayerToEdit);
            }

            GUILayout.BeginHorizontal();
            GUILayout.TextArea("Layers to Show.");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            for (int i = 0; i < window.GridManager.DecorativeLayers.Count; i++) {
                if (GUILayout.Toggle(m_LayersToShow[i], new GUIContent("Layer " + i))) {    
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if(window.Textures != null) {
                m_CursorTileIndex = GUILayout.SelectionGrid(m_CursorTileIndex, window.Textures, 4);

            }
            GUILayout.EndHorizontal();

            if(m_CursorTileIndex != m_CursorTileIndexOld) {
                m_CursorTileIndexOld = m_CursorTileIndex;
                window.SelectNewTileType(m_CursorTileIndex);
            }

            GUILayout.BeginHorizontal();
            if (window.SOTileList == null) {
                window.Refresh();
            }
            GUILayout.EndHorizontal();
        }

        void OnSceneMouseOver(SceneView view) {
            if (Event.current.type == EventType.MouseMove) {
                Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                Tile tile = window.GridManager.GetTileClosestToPosition(pos);

                if (m_HoveredTile != tile) {
                    if (m_HoveredTile != null) {
                        m_HoveredTile.ToggleHighlight(false);
                    }
                    tile.ToggleHighlight(true);
                    m_HoveredTile = tile;
                    window.UpdateCursorPos(tile.transform.position);
                }
            }

            if (Event.current.type == EventType.MouseDown) {
                if (Event.current.keyCode == KeyCode.Mouse0) {
                    HandleTileReplacement(m_HoveredTile);
                    if (m_HoveredTile.Data != window.CursorTile.Data) {
                    }
                }
            }


            if (Event.current.type == EventType.KeyDown) {
                if (Event.current.keyCode == KeyCode.R) {
                    if (window.CursorTile.Data.Rotatable == true) {
                        window.CursorTile.SetDirection(TrafficLib.GetNextDirection(window.CursorTile.TileRotation));
                    }
                }
            }
        }

        private void HandleTileReplacement(Tile tile) {
            tile.Initialize(window.CursorTile.Data);
            tile.SetDirection(window.CursorTile.TileRotation);
        }
    }
}