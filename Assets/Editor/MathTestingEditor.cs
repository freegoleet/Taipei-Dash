using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MathTesting))]
public class MathTestingEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        MathTesting math = (MathTesting)target;

        if (GUILayout.Button("Draw Curve")) {
            math.DrawCurve();
        }
    }
}
