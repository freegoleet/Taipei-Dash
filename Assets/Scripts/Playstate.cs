using System;
using UnityEditor;

[InitializeOnLoadAttribute]
public static class PlayModeStateChangedExample
{
    public static Action OnEditMode = null;

    // register an event handler when the class is initialized
    static PlayModeStateChangedExample() {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    private static void LogPlayModeState(PlayModeStateChange state) {
        if (state == PlayModeStateChange.EnteredEditMode) { 
            OnEditMode?.Invoke();
        }
    }
}