using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Lesson009_MenuItem
{
#if UNITY_EDITOR
    [MenuItem("MyGame/Lesson009/Test")]
    static void TestMenuItem() {
        Debug.Log("Test MenuItem");
    }

    [MenuItem("MyGame/Lesson009/Distribute X axis %T")]
    [MenuItem("CONTEXT/Transform/Lesson009/Distribute X axis")]
    static void DistributeX() {
        var list = Selection.gameObjects;
        var n = list.Length;
        if (n < 3) {
            Debug.Log("Please select objects");
            return;
        }

        float min = list[0].transform.position.x;
        float max = min;

        for (int i = 1; i < n; i++) {
            var x = list[i].transform.position.x;
            if (x < min) min = x;
            if (x > max) max = x;
        }

        for (int i = 0; i < n; i++) {
            var pos = list[i].transform.position;
            pos.x = min + (max - min) * (float)i/(n-1);

            Undo.RecordObject(list[i].transform, "Distribute X axis");
            list[i].transform.position = pos;
        }
    }

    [MenuItem("MyGame/Lesson009/Create Test Object")]
    static void CreateObject() {
        var obj = new GameObject("Test");
        Undo.RegisterCreatedObjectUndo(obj, "Create Test Object");
    }

#endif

}
