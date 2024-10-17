using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GridManager gm = (GridManager)target;

        if (GUILayout.Button("Refresh")) {
            gm.SetupManager();
        }

        if (GUILayout.Button("Create Grid")) {
            gm.GenerateMap();
        }

        if (GUILayout.Button("Destroy Grid")) {
            gm.DestroyAllTiles();
        }
    }
}