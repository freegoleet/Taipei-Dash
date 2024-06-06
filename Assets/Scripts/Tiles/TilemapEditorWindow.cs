using Traffic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TilemapEditorWindow : EditorWindow
{
    private int m_CursorTileIndex = 0;
    private int m_CursorTileIndexOld = 1;
    private TilemapEditor m_Editor = null;

    [MenuItem("Traffic/Tilemap Editor")]
    public static void ShowWindow() {
        TilemapEditorWindow wnd = GetWindow<TilemapEditorWindow>();
        wnd.titleContent = new GUIContent("Tilemap Editor");
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy
        Label label = new Label("Hello World!");
        root.Add(label);

        // Create button
        Button button = new Button();
        button.name = "button";
        button.text = "Button";
        root.Add(button);

        // Create toggle
        Toggle toggle = new Toggle();
        toggle.name = "toggle";
        toggle.label = "Toggle";
        root.Add(toggle);

        // Github boi
        
        
    }

    private void OnGUI() {
        RefreshTiles();
    }

    public void RefreshTiles() {
        if (m_Editor == null) {
            m_Editor = FindFirstObjectByType<TilemapEditor>();
        }

        GUILayout.BeginHorizontal();
        if (m_Editor.Textures != null) {
            m_CursorTileIndex = GUILayout.SelectionGrid(m_CursorTileIndex, m_Editor.Textures, 4);

        }
        GUILayout.EndHorizontal();
    }
}
