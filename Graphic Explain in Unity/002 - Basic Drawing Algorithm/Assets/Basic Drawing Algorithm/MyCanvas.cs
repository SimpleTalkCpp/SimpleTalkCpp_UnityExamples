using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode]
public class MyCanvas : MonoBehaviour
{
    public int width  = 860;
    public int height = 540;
	public Color backgroundColor = Color.white;

    Texture2D outImage;

	public interface IDemoBase {
		void OnRunDemo(MyCanvas canvas);
	}

	[System.Serializable]
	public class DrawRectDemo : IDemoBase {
		public int x = 10;
		public int y = 10;
		public int width = 300;
		public int height = 200;
		public Color color = new Color(0,1,0);

		public void OnRunDemo(MyCanvas canvas) {
//			Debug.Log("DrawRect");
			for (int j = 0; j < height; j++) {
				for (int i = 0; i < width; i++) {
					canvas.SetPixel(x + i, y + j, color);
				}
			}
		}
	};
	public DrawRectDemo drawRectDemo;

	[System.Serializable]
	public class DrawLineDemo : IDemoBase{
		public Vector2 point0;
		public Vector2 point1;
		public Color color;
		public void OnRunDemo(MyCanvas canvas) {
//			Debug.Log("DrawLineDemo");
		}
	};
	public DrawLineDemo drawLineDemo;

	public void OnRunDemo(string name) {
		if (name == "drawRectDemo") drawRectDemo.OnRunDemo(this);
		if (name == "drawLineDemo") drawLineDemo.OnRunDemo(this);
	}

	public void SetPixel(int x, int y, Color color) {
		if (!outImage) return;
		outImage.SetPixel(x, outImage.height - y - 1, color);
	}

	private void OnGUI() {
		if (outImage) {
			GUI.DrawTexture(new Rect(0,0,outImage.width, outImage.height), outImage);
		}
	}

	public Texture2D CreateOutImage() {
		if (!outImage || outImage.width != width || outImage.height != height) {

			if (Application.isEditor) {
				DestroyImmediate(outImage);
			} else {
				Destroy(outImage);
			}

			outImage = new Texture2D(width, height);
			outImage.filterMode = FilterMode.Point;
		}

		var pixels = new Color[width * height];
		for (int i = 0; i < pixels.Length; i++) {
			pixels[i] = backgroundColor;
		}
		outImage.SetPixels(pixels);
		return outImage;
	}
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(MyCanvas.IDemoBase), true)]
public class MyDemoButton : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.PropertyField(position, property, true);
		position.y += EditorGUI.GetPropertyHeight(property, true) + 10;
		position.height = 30;
		if (GUI.Button(position, "Run")) {
			var canvas = property.serializedObject.targetObject as MyCanvas;
			var outImage = canvas.CreateOutImage();

			if (canvas) {
				canvas.OnRunDemo(property.name);
			}
			outImage.Apply();
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return EditorGUI.GetPropertyHeight(property, true) + 50;
	}
}

#endif