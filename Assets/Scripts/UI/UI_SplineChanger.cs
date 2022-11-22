using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(SplinePointChanger))]
public class UI_SplineChanger : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SplinePointChanger myTarget = (SplinePointChanger)target;

        if (GUILayout.Button("SplineForward"))
        {
            myTarget.SplineForward();
        }

        if (GUILayout.Button("SplineBackward"))
        {
            myTarget.SplineBackward();
        }
    }
}
#endif
