using UnityEditor;
using UnityEngine;

public class SaveEditorWindow : EditorWindow
{
    // Buttons
    private float m_ButtonWidth = 140f;
    private SaveEditor Editor { get; set; } = null;
    private Vector2 m_ScrollPosition = Vector2.zero;
    private int m_SelectedLevelIndex = 0;

    [MenuItem("Traffic/Save Editor")]
    public static void ShowWindow() {
        SaveEditorWindow wnd = GetWindow<SaveEditorWindow>();
        wnd.titleContent = new GUIContent("Save Editor");
    }

    public void CreateGUI() {
        if (Editor == null) {
            Editor = FindFirstObjectByType<SaveEditor>();
        }
    }

    private void OnGUI() {
        SaveLoad();
    }

    public void SaveLoad() {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Level", GUILayout.Width(m_ButtonWidth))) {
            Editor.SaveLevel();
        }
        Editor.SetLevelName(GUILayout.TextField(Editor.InputLevelName, GUILayout.Width(m_ButtonWidth)));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Level", GUILayout.Width(m_ButtonWidth))) {
            Editor.LoadLevel(m_SelectedLevelIndex);
        }
        GUILayout.Label(Editor.CurrentLevelData.Name, GUILayout.Width(m_ButtonWidth));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Get Levels", GUILayout.Width(m_ButtonWidth))) {
            Editor.Initialize();
            Editor.GetAllLevels();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, false, true);
        Levels();
        GUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }

    public void Levels() {
        if (Editor.LevelNames.Length > 0) {
            m_SelectedLevelIndex = GUILayout.SelectionGrid(m_SelectedLevelIndex, Editor.LevelNames, 4, GUILayout.Width(400));
        }
    }
}
