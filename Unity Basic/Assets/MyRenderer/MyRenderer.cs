using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyRenderer : MonoBehaviour
{
	static MyRenderer s_instance;

	public Texture2D _tex;

	MyRenderer() {
		s_instance = this;
	}

	[UnityEditor.MenuItem("MyRenderer/Render %F1")]
	static void MenuItem_Render() {
		if (s_instance) s_instance.MyRender();
	}

	void MyRender() {
		Debug.Log("Render");

		_tex = new Texture2D(512, 512);
		var pixels = new Color[512 * 512];

		for (int i = 0; i < pixels.Length; i++) {
			pixels[i] = new Color(0, 0, 0, 1);
		}
		_tex.SetPixels(pixels);


		DrawRect(_tex, 10, 10, 400, 200, new Color(0, 1, 0, 1));

		_tex.Apply(false);
	}

	void DrawRect(Texture2D tex, int x, int y, int w, int h, Color color) {
		for (int j = 0; j < h; j++) {
			for (int i = 0; i < w; i++) {
				tex.SetPixel(x + i, y + j, color);
			}
		}
	}

	private void OnGUI() {
		GUILayout.Box("======== testing =======");
		if (_tex) {
			GUI.DrawTexture(new Rect(0,0, _tex.width, _tex.height), _tex);
		}
	}

}
