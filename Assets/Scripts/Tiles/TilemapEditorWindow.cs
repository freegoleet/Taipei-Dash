using Traffic;
using UnityEditor;
using UnityEngine;

public class TilemapEditorWindow : EditorWindow
{
    private int m_CursorTileIndex = 0;
    private int m_CursorTileIndexOld = 1;

    private TilemapEditor m_Editor = null;
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
        if (m_Editor == null) {
            m_Editor = FindFirstObjectByType<TilemapEditor>();
        }

        m_DecorativeLayers = new bool[m_Editor.GridManager.Rows];
        m_DecorativeLayersLast = new bool[m_DecorativeLayers.Length];

        m_LayerNames = new string[m_Editor.GridManager.Rows + 1];
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
        m_Editor.ToggleShowPointers(m_ShowPointers);
    }

    private void OnGUI() {
        if (m_CursorTileIndex != m_CursorTileIndexOld) {
            m_CursorTileIndexOld = m_CursorTileIndex;
            m_Editor.SelectNewTileType(m_CursorTileIndex);
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
        if (m_Editor.Textures != null) {
            m_LayerToEdit = GUILayout.SelectionGrid(m_LayerToEdit, m_LayerNames, 4, GUILayout.Width(300));

            if (m_LayerToEdit != m_LayerToEditOld) {
                m_LayerToEditOld = m_LayerToEdit;
                m_Editor.SelectLayerToEdit(m_LayerToEdit);
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
            m_Editor.ToggleShowPointers(m_ShowPointers);
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
        m_Editor.ShowLayer(layer, show);
    }

    public void SetupTiles() {
        if (m_Editor == null) {
            m_Editor = FindFirstObjectByType<TilemapEditor>();
        }

        EditorGUILayout.LabelField("Tile to use:");

        EditorGUILayout.BeginHorizontal();
        if (m_Editor.Textures != null) {
            m_CursorTileIndex = GUILayout.SelectionGrid(m_CursorTileIndex, m_Editor.Textures, 4, GUILayout.Width(400));

        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    public void RefreshSystem() {
        m_Editor.GridManager.EnteredEditMode();
        m_Editor.Refresh();
    }

    public void ToggleEditing(bool editing) {
        if (editing == true) {
            SceneView.duringSceneGui += OnSceneMouseOver;
        }
        else {
            SceneView.duringSceneGui -= OnSceneMouseOver;
        }

        m_Editor.ToggleShowCursorTile(editing);
        //m_HoveredTile.ToggleHighlight(editing);
    }

    private void OnSceneMouseOver(SceneView view) {
        if (Event.current.type == EventType.MouseMove) {
            Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Tile tile = m_Editor.GridManager.GetTileClosestToPosition(pos);

            if (m_HoveredTile != tile) {
                if (m_HoveredTile != null) {
                    m_HoveredTile.ToggleHighlight(false);
                }
                tile.ToggleHighlight(true);
                m_HoveredTile = tile;
                m_Editor.UpdateCursorPos(tile.transform.position);
            }
        }

        if (Event.current.type == EventType.MouseDown) {
            if (Event.current.keyCode == KeyCode.Mouse0) {
                m_Editor.PlaceTile(m_HoveredTile);
                if (m_HoveredTile.Data != m_Editor.CursorTile.Data) {
                }
            }

            if (Event.current.keyCode == KeyCode.Mouse1) {
                m_Editor.RemoveTile(m_HoveredTile);
            }
        }

        if (Event.current.type == EventType.KeyDown) {
            if (Event.current.keyCode == KeyCode.R) {
                if (m_Editor.CursorTile.Data.Rotatable == true) {
                    m_Editor.CursorTile.SetDirection(TrafficLib.GetNextDirection(m_Editor.CursorTile.TileRotation));
                }
            }
        }

    }
}
