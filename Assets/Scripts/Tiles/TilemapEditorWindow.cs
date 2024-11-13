using System;
using Traffic;
using UnityEditor;
using UnityEngine;

public class TilemapEditorWindow : EditorWindow
{
    private int m_SelectedTileIndex = 0;
    private int m_CursorTileIndexOld = 1;

    private TilemapEditor Editor { get; set; } = null;
    private Tile m_HoveredTile = null;

    private bool m_IsEditing = false;
    private bool m_IsEditingOld = false;
    private bool m_ShowConnections = false;
    private bool m_ShowConnectionsOld = false;
    private bool m_ModifyTiles = false;
    private bool m_ModifyTilesOld = false;

    private bool[] m_DecorativeLayers = null;
    private bool[] m_DecorativeLayersLast = null;

    private int m_LayerToEdit = 0;
    private int m_LayerToEditOld = 1;
    private string[] m_LayerNames = null;

    private int m_TileModType = 0;
    private int m_TileModTypeOld = 1;
    private string[] m_TileModTypes = null;

    private Vector2 m_ScrollPosition = Vector2.zero;

    [MenuItem("Traffic/Tilemap Editor")]
    public static void ShowWindow() {
        TilemapEditorWindow wnd = GetWindow<TilemapEditorWindow>();
        wnd.titleContent = new GUIContent("Tilemap Editor");
    }

    public void CreateGUI() {
        if (Editor == null) {
            Editor = FindFirstObjectByType<TilemapEditor>();
        }

        m_DecorativeLayers = new bool[Editor.GridManager.Rows];
        m_DecorativeLayersLast = new bool[m_DecorativeLayers.Length];

        m_LayerNames = new string[Editor.GridManager.Rows + 1];
        for (int i = 0; i < m_LayerNames.Length; i++) {
            if (i == 0) {
                m_LayerNames[i] = "Gameplay";
                continue;
            }

            m_LayerNames[i] = "Deco " + i;
        }

        m_TileModTypes = new string[Enum.GetNames(typeof(TileModType)).Length];
        for (int i = 0; i < m_TileModTypes.Length; i++) {
            m_TileModTypes[i] = Enum.GetValues(typeof(TileModType)).GetValue(i).ToString();
        }

        RefreshSystem();

        for (int i = 0; i < m_DecorativeLayers.Length; i++) {
            m_DecorativeLayers[i] = true;
            m_DecorativeLayersLast[i] = false;
            ToggleShowDecoLayer(i, true);
        }

        ToggleEditing(m_IsEditing);
        Editor.ToggleShowConnections(m_ShowConnections);
    }

    private void OnGUI() {
        if (m_SelectedTileIndex != m_CursorTileIndexOld) {
            m_CursorTileIndexOld = m_SelectedTileIndex;
            Editor.SelectNewTileType(m_SelectedTileIndex);
        }

        Controls();

        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, false, true);

        TileModification();
        DecorativeLayers();
        LayerToEdit();
        TileSelector();

        GUILayout.EndScrollView();

    }

    private void LayerToEdit() {
        //EditorGUILayout.LabelField("Layer to edit:");
        //EditorGUILayout.BeginHorizontal();
        //if (Editor.Textures != null) {
        //    m_LayerToEdit = GUILayout.SelectionGrid(m_LayerToEdit, m_LayerNames, 4, GUILayout.Width(300));

        //    if (m_LayerToEdit != m_LayerToEditOld) {
        //        m_LayerToEditOld = m_LayerToEdit;
        //        Editor.SelectLayerToEdit(m_LayerToEdit);
        //        if (m_LayerToEdit > 0 && m_DecorativeLayers[m_LayerToEdit - 1] == false) {
        //            ToggleShowDecoLayer(m_LayerToEdit - 1, true);
        //        }
        //    }
        //}
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    private void Controls() {
        EditorGUILayout.LabelField("Controls:");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh", GUILayout.Width(100))) {
            RefreshSystem();
        }

        if (GUILayout.Button("Create Grid", GUILayout.Width(100))) {
            Editor.GridManager.GenerateMap();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        m_IsEditing = GUILayout.Toggle(m_IsEditing, "Toggle Editing", GUILayout.Width(100));
        if (m_IsEditing != m_IsEditingOld) {
            m_IsEditingOld = m_IsEditing;
            ToggleEditing(m_IsEditing);
        }

        m_ShowConnections = GUILayout.Toggle(m_ShowConnections, "Show Connections", GUILayout.Width(140));
        if (m_ShowConnections != m_ShowConnectionsOld) {
            m_ShowConnectionsOld = m_ShowConnections;
            Editor.ToggleShowConnections(m_ShowConnections);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    private void TileModification() {
        EditorGUILayout.LabelField("Tile Modification:");
        EditorGUILayout.BeginHorizontal();
        if (Editor.Textures != null) {
            m_TileModType = GUILayout.SelectionGrid(m_TileModType, m_TileModTypes, 4, GUILayout.Width(300));
            if (m_TileModType != m_TileModTypeOld) {
                m_TileModTypeOld = m_TileModType;
                Editor.SwitchTileModType((TileModType)m_TileModTypeOld);
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    private void DecorativeLayers() {
        EditorGUILayout.LabelField("Decolayer:");
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < m_DecorativeLayers.Length; i++) {
            m_DecorativeLayers[i] = GUILayout.Toggle(m_DecorativeLayers[i], "Layer " + (i + 1), GUILayout.Width(80));
            if ((i + 1) % 4 == 0) {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
            if (m_DecorativeLayers[i] != m_DecorativeLayersLast[i]) {
                ToggleShowDecoLayer(i, m_DecorativeLayers[i]);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    private void ToggleShowDecoLayer(int layer, bool show) {
        m_DecorativeLayers[layer] = show;
        m_DecorativeLayersLast[layer] = show;
        Editor.ShowLayer(layer, show);
    }

    public void TileSelector() {
        if (Editor == null) {
            Editor = FindFirstObjectByType<TilemapEditor>();
        }

        EditorGUILayout.LabelField("Tile to use:");

        EditorGUILayout.BeginHorizontal();
        if (Editor.Textures != null) {
            m_SelectedTileIndex = GUILayout.SelectionGrid(m_SelectedTileIndex, Editor.Textures, 4, GUILayout.Width(400));

        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    public void RefreshSystem() {
        Editor.GridManager.SetupManager();
        Editor.Refresh();
    }

    public void ToggleModifyTiles(bool show) {
        Editor.TilemapCursor.ToggleShowCursorTile(!show);
    }

    public void ToggleEditing(bool editing) {
        if (editing == true) {
            SceneView.duringSceneGui += OnSceneMouseOver;
        }
        else {
            SceneView.duringSceneGui -= OnSceneMouseOver;
        }
        Editor.TilemapCursor.ToggleShowCursorTile(editing);
        Editor.TilemapCursor.ToggleShowCursor(editing);
    }

    private void OnSceneMouseOver(SceneView view) {
        if (Event.current.type == EventType.MouseMove) {
            Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            NodeBase nodebase = Editor.GridManager.GetTileClosestToPosition(pos);
            Tile tile = (Tile)nodebase;

            if (m_HoveredTile == null) {
                m_HoveredTile = tile;
            }

            if (m_HoveredTile.GridPosition != nodebase.GridPosition) {
                m_HoveredTile = tile;
                Editor.TilemapCursor.UpdateCursorPos(tile);

                if(Editor.TilemapCursor.GetCurrentCursorTile() is Tile cursorTile == false) {
                    return;
                }
                if (tile.Data == cursorTile) {
                    Editor.UI_Neighbors.ShowNeighbors(tile.NeighborSystem.GetAllFittableNeighbors());
                }
                else {
                    Editor.UI_Neighbors.ShowNeighbors(tile.NeighborSystem.GetAllUnfittableNeighbors());
                }
            }
        }

        if (Event.current.type == EventType.MouseDown) {
            if (Event.current.keyCode == KeyCode.Mouse0) {
                Editor.EditTile(m_HoveredTile, true);
            }

            if (Event.current.keyCode == KeyCode.Mouse1) {
                Editor.EditTile(m_HoveredTile, false);
            }
        }

        if (Event.current.type == EventType.KeyDown) {
            Tile tile = Editor.TilemapCursor.GetCurrentCursorTile();
            if (Event.current.keyCode == KeyCode.R) {
                if (tile.Data.Rotatable == true) {
                    tile.SetFacing(TrafficLib.GetNextDirection(tile.Facing));
                }
            }
        }
    }
}
