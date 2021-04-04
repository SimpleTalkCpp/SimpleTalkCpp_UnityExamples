using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Lesson013_EditorWindow : EditorWindow
{
    string myString = "Hello World";
    GameObject myObject;
    bool allowSceneObject = true;
    bool foldout = true;
    bool groupEnabled = true;
    bool myBool = true;
    float myFloat = 1.23f;

	[MenuItem("MyGame/Lesson013_EditorWindow")]
    public static void  ShowWindow () {
        EditorWindow.GetWindow(typeof(Lesson013_EditorWindow));
    }

	private void OnGUI()
	{
        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField ("Text Field", myString);

        allowSceneObject = EditorGUILayout.Toggle("AllowSceneObject", allowSceneObject);

        EditorGUILayout.BeginHorizontal();
        {
            myObject = EditorGUILayout.ObjectField("Object", myObject, typeof(GameObject), allowSceneObject) as GameObject;
            if (GUILayout.Button("Ping Object")) {
                EditorGUIUtility.PingObject(myObject); 
            }
        }
        EditorGUILayout.EndHorizontal();

        {
            var rect = GUILayoutUtility.GetRect(300, 200);
            EditorGUI.DrawRect(rect, new Color(0, 0.3f, 0));
            GUI.Box(rect, $"GetRect => {rect}");
        }

        
        foldout = EditorGUILayout.Foldout(foldout, "Foldout");
        if (foldout) {
            EditorGUI.indentLevel++;

            groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
                myBool = EditorGUILayout.Toggle ("Toggle", myBool);
                myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup ();

            EditorGUI.indentLevel--;
        }
	}
}
