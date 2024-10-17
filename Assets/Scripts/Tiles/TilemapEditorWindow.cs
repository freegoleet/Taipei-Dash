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
    private bool m_ShowPointers = false;
    private bool m_ShowPointersOld = false;

    private bool[] m_DecorativeLayers = null;
    private bool[] m_DecorativeLayersLast = null;

    private int m_LayerToEdit = 0;
    private int m_LayerToEditOld = 1;
    private string[] m_LayerNames = null;

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

        RefreshSystem();

        for (int i = 0; i < m_DecorativeLayers.Length; i++) {
            m_DecorativeLayers[i] = true;
            m_DecorativeLayersLast[i] = false;
            ToggleShowDecoLayer(i, true);
        }


        ToggleEditing(m_IsEditing);
        Editor.ToggleShowPointers(m_ShowPointers);
    }

    private void OnGUI() {
        if (m_SelectedTileIndex != m_CursorTileIndexOld) {
            m_CursorTileIndexOld = m_SelectedTileIndex;
            Editor.SelectNewTileType(m_SelectedTileIndex);
        }

        Controls();

        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, false, true);

        DecorativeLayers();
        LayerToEdit();
        SetupTiles();

        GUILayout.EndScrollView();

    }

    private void LayerToEdit() {
        EditorGUILayout.LabelField("Layer to edit:");
        EditorGUILayout.BeginHorizontal();
        if (Editor.Textures != null) {
            m_LayerToEdit = GUILayout.SelectionGrid(m_LayerToEdit, m_LayerNames, 4, GUILayout.Width(300));

            if (m_LayerToEdit != m_LayerToEditOld) {
                m_LayerToEditOld = m_LayerToEdit;
                Editor.SelectLayerToEdit(m_LayerToEdit);
                if (m_LayerToEdit > 0 && m_DecorativeLayers[m_LayerToEdit - 1] == false) {
                    ToggleShowDecoLayer(m_LayerToEdit - 1, true);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    private void Controls() {
        EditorGUILayout.LabelField("Controls:");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh", GUILayout.Width(100))) {
            RefreshSystem();
        }

        m_IsEditing = GUILayout.Toggle(m_IsEditing, "Toggle Editing", GUILayout.Width(100));
        if (m_IsEditing != m_IsEditingOld) {
            m_IsEditingOld = m_IsEditing;
            ToggleEditing(m_IsEditing);
        }

        m_ShowPointers = GUILayout.Toggle(m_ShowPointers, "Show Pointers", GUILayout.Width(100));
        if (m_ShowPointers != m_ShowPointersOld) {
            m_ShowPointersOld = m_ShowPointers;
            Editor.ToggleShowPointers(m_ShowPointers);
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

    public void SetupTiles() {
        if (Editor == null) {
            Editor = FindFirstObjectByType<TilemapEditor>();
        }

        EditorGUILayout.LabelField("TileGameplay to use:");

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

    public void ToggleEditing(bool editing) {
        if (editing == true) {
            SceneView.duringSceneGui += OnSceneMouseOver;
        }
        else {
            SceneView.duringSceneGui -= OnSceneMouseOver;
        }

        Editor.TilemapCursor.ToggleShowCursorTile(editing);
        //m_HoveredTile.ToggleHighlight(editing);
    }

    private void OnSceneMouseOver(SceneView view) {
        if (Event.current.type == EventType.MouseMove) {
            Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            NodeBase nodebase = Editor.GridManager.GetTileClosestToPosition(pos);
            Tile tile = (Tile)nodebase;

            if(m_HoveredTile == null) {
                m_HoveredTile = tile;
            }

            if (m_HoveredTile.GridPosition != nodebase.GridPosition) {
                m_HoveredTile = tile;
                Editor.TilemapCursor.UpdateCursorPos(tile);
                if(tile is TileAutofit aftile) {
                    Editor.UI_Neighbors.ShowNeighbors(aftile.NeighborSystem.GetAllNeighbors());
                }
            }
        }

        if (Event.current.type == EventType.MouseDown) {
            if (Event.current.keyCode == KeyCode.Mouse0) {
                Editor.PlaceTile(m_HoveredTile);
            }

            if (Event.current.keyCode == KeyCode.Mouse1) {
                Editor.RemoveTile(m_HoveredTile);
            }
        }

        if (Event.current.type == EventType.KeyDown) {
            if (Event.current.keyCode == KeyCode.R) {
                Tile tile = Editor.TilemapCursor.GetCursorTile();
                if (tile.Data.Rotatable == true) {
                    tile.SetFacing(TrafficLib.GetNextDirection(tile.Facing));
                }
            }
        }

    }
}
