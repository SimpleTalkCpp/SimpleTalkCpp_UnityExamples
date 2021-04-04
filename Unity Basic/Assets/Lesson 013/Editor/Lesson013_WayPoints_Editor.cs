using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Lesson013_WayPoints), true)]
[CanEditMultipleObjects]
public class Lesson013_WayPoints_Editor : Editor
{
    SerializedProperty pointsProp;

    void OnEnable()
    {
        pointsProp = serializedObject.FindProperty("points");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Add Point")) {
                var n = pointsProp.arraySize;
                pointsProp.arraySize = n + 1;
                pointsProp.GetArrayElementAtIndex(n).vector3Value += Vector3.forward;
            }

            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(pointsProp);
        serializedObject.ApplyModifiedProperties();
    }

	void OnSceneGUI()
	{
        var comp = this.target as Lesson013_WayPoints;
        if (!comp) return;

        Handles.matrix = comp.transform.localToWorldMatrix;

        for (int i = 0; i < pointsProp.arraySize; i++) {
            var pos = comp.points[i];

//            Handles.Label(pos, $"{i}");
            MyLabel(pos, $"{i}");

            var newPos = Handles.PositionHandle(pos, Quaternion.identity);

            if (pos != newPos) {
                Undo.RecordObject(comp, "Move WayPoint");
                comp.points[i] = newPos;
            }
        }

        Handles.BeginGUI();
        if (GUI.Button(new Rect(10, 10, 200, 30), "Test")) {
            Debug.Log("Text Button");
        }
        Handles.EndGUI();
    }

    void MyLabel(Vector3 pos, string text) {
        Handles.BeginGUI();
        {
            var cam = SceneView.currentDrawingSceneView.camera;
            var worldPos = Handles.matrix.MultiplyPoint(pos);

            var pos2d = cam.WorldToScreenPoint(worldPos);
            pos2d.y = Screen.height - pos2d.y - 1; // screen coordinate is upside down

            if (GUI.Button(new Rect(pos2d.x, pos2d.y - 20, 40, 20), text)) {
                Debug.Log($"Button {text}");
            }
        }
        Handles.EndGUI();
    }
}


