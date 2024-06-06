using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoadAttribute]
public static class PlayModeStateChangedExample
{
    public static Action OnEditMode = null;
    
    static PlayModeStateChangedExample() {
        EditorApplication.playModeStateChanged += LogPlayModeState;
        EditorSceneManager.activeSceneChangedInEditMode += ActiveSceneChangedInEditMode;
    }

    private static void LogPlayModeState(PlayModeStateChange state) {
        if (state == PlayModeStateChange.EnteredEditMode) { 
            OnEditMode?.Invoke();
        }
    }

    private static void ActiveSceneChangedInEditMode(Scene one, Scene two) {

    }
}