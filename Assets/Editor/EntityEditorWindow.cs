using Traffic;
using UnityEditor;
using UnityEngine;

public class EntityEditorWindow : EditorWindow
{
    // Buttons
    private float m_ButtonWidth = 140f;
    private EntityEditor Editor { get; set; } = null;
    private TilemapEditor TilemapEditor { get; set; } = null;

    // Edit
    private bool m_IsEditing = false;
    private bool m_IsEditingPrev = false;

    [MenuItem("Traffic/Entity Editor")]
    public static void ShowWindow() {
        EntityEditorWindow wnd = GetWindow<EntityEditorWindow>();
        wnd.titleContent = new GUIContent("Entity Editor");
    }

    public void CreateGUI() {
        RefreshSystem();
        ToggleEditing(m_IsEditing);
    }

    private void OnGUI() {
        Controls();
    }

    private void Controls() {
        EditorGUILayout.LabelField("Controls:");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Refresh", GUILayout.Width(m_ButtonWidth))) {
            RefreshSystem();
        }

        m_IsEditing = GUILayout.Toggle(m_IsEditing, "Toggle Editing", GUILayout.Width(m_ButtonWidth));
        if (m_IsEditing != m_IsEditingPrev) {
            m_IsEditingPrev = m_IsEditing;
            ToggleEditing(m_IsEditing);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    public void RefreshSystem() {
        if (Editor == null) {
            Editor = FindFirstObjectByType<EntityEditor>();
        }
        if (TilemapEditor == null) {
            TilemapEditor = GameService.Instance.TilemapEditor;
        }
    }

    public void ToggleEditing(bool editing) {
        if (editing == true) {
            SceneView.duringSceneGui += OnSceneMouseOver;
        }
        else {
            SceneView.duringSceneGui -= OnSceneMouseOver;
        }
        TilemapEditor.TilemapCursor.ToggleShowCursorTile(false);
        TilemapEditor.TilemapCursor.ToggleShowCursor(editing);
    }

    private void OnSceneMouseOver(SceneView view) {
        if (Event.current.type == EventType.Repaint) {
            Editor.Tick(Time.deltaTime);
        }

        if (Event.current.type == EventType.MouseMove) {
            Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            NodeBase nodebase = TilemapEditor.GridManager.GetTileClosestToPosition(pos);
            Tile tile = (Tile)nodebase;

            TilemapEditor.NewTileHovered(tile, nodebase.GridPosition);

            if (Editor.SelectedStartTile != null) {
                if(TilemapEditor.HoveredTile is TileGameplay tilegp) {
                    Editor.ShowPathToHoveredTile(tilegp);
                }
            }

        }

        if (Event.current.type == EventType.MouseDown) {
            if (Event.current.keyCode == KeyCode.Mouse0) {
                if (TilemapEditor.HoveredTile != null) {
                    if (TilemapEditor.HoveredTile is TileGameplay tilegp == false) {
                        return;
                    }
                    if (Editor.SelectedStartTile != null) {
                        Editor.SelectTargetTile(tilegp);
                        return;
                    }
                    Editor.SelectStartTile(tilegp);
                    return;
                }
            }
            if (Event.current.keyCode == KeyCode.Mouse1) {
                Editor.UnselectTiles();
            }
        }

        if (Event.current.type == EventType.KeyDown) {
           
        }
    }
}
