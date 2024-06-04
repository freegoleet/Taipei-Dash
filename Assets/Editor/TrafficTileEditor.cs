using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrafficTileManager))]
public class TrafficTileEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        TrafficTileManager ttm = (TrafficTileManager)target;

        Event e = Event.current;

        if (GUILayout.Button("Toggle Directions")) {
            ttm.ToggleDirections();
        }
    }
}