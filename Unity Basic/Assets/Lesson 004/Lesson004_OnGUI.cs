using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson004_OnGUI : MonoBehaviour
{
    public float guiRotate = 0;
    public Color guiColor = Color.red;
    public bool restoreColor = true;
    public GUIStyle style;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

	private void OnGUI()
	{
        GUI.matrix = Matrix4x4.Rotate(Quaternion.Euler(0,0, guiRotate));

        { // draw at screen center
            float w = 200;
            float h = 40;
            var rect = new Rect((Screen.width - w) /2, (Screen.height - h) / 2, w, h);
            
            var oldColor = GUI.color;
            GUI.color = guiColor;
		    // GUI.Label(rect,"Testing");
		    GUI.Box(rect,"Testing");

            if (restoreColor) {
                GUI.color = oldColor;
            }
        }

        {// Button
            if (GUI.Button(new Rect(20, Screen.height - 40 - 20, 150, 40), "Button")) {
                Debug.Log("Button !!");
            }
        }

        { //Layout
            GUILayout.Box("Test 01");
            GUILayout.Box("Test 02");
            GUILayout.Box("Test 03");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Box("Test 04");
                GUILayout.Box("Test 05");
                GUILayout.Box("Test 06");
            }
            GUILayout.EndHorizontal();
        }

        { // Style
            // style = new GUIStyle(GUI.skin.box);
		    GUILayout.Label("Style Test Case", style);
        }
	}
}
