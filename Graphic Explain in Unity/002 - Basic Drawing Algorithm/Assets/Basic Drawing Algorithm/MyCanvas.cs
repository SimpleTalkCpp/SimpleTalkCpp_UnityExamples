using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MyShape : MonoBehaviour {
	public virtual void OnDraw(MyCanvas canvas) {}
}

public class MyCanvas : MonoBehaviour
{
    public Vector2Int canvasSize = new Vector2Int(640, 480);
	public Color backgroundColor = Color.white;

	public int drawOffsetX;
	public int drawOffsetY;

	[Range(1, 8)]
	public int drawScale = 1;

    Texture2D outImage;

	static MyCanvas _instance;

	private void Awake() {
		_instance = this;
		Application.targetFrameRate = 30;
	}

	private void Update() {
		CreateOutImage();

		foreach (Transform c in transform) {
			var p = c.GetComponent<MyShape>();
			if (p && p.isActiveAndEnabled) p.OnDraw(this);
		}

		outImage.Apply();
	}

	private void OnGUI() {
		if (outImage) {
			var rect = new Rect(drawOffsetX    * drawScale,
								drawOffsetY    * drawScale,
								outImage.width  * drawScale,
								outImage.height * drawScale);
			GUI.DrawTexture(rect, outImage);
		}
	}

	public void SetPixel(int x, int y, in Color color) {
		if (!outImage) return;
		if (x < 0 || x >= canvasSize.x) return;
		if (y < 0 || y >= canvasSize.y) return;

		int iy  = outImage.height - y - 1;
		outImage.SetPixel(x, iy, color);
	}

	public void BlendPixel(int x, int y, in Color color) {
		if (!outImage) return;
		if (x < 0 || x >= canvasSize.x) return;
		if (y < 0 || y >= canvasSize.y) return;

		int iy  = outImage.height - y - 1;
		var old = outImage.GetPixel(x, iy);

		var newColor  = (color * color.a) + (old * (1-color.a));
		newColor.a = 1;

		outImage.SetPixel(x, iy, newColor);
	}

	public void DrawPoint(in Vector2Int pos, int size, in Color color) {
		int n = size / 2;
		for (int y = 0; y < n; y++) {
			for (int x = 0; x < n; x++) {
				BlendPixel(pos.x - x, pos.y - y, color);
				BlendPixel(pos.x + x, pos.y - y, color);
				BlendPixel(pos.x - x, pos.y + y, color);
				BlendPixel(pos.x + x, pos.y + y, color);
			}
		}
	}

	public Texture2D CreateOutImage() {
		if (!outImage || outImage.width != canvasSize.x || outImage.height != canvasSize.y) {

			if (Application.isEditor) {
				DestroyImmediate(outImage);
			} else {
				Destroy(outImage);
			}

			outImage = new Texture2D(canvasSize.x, canvasSize.y);
			outImage.filterMode = FilterMode.Point;
		}

		var pixels = new Color[canvasSize.x * canvasSize.y];
		for (int i = 0; i < pixels.Length; i++) {
			pixels[i] = backgroundColor;
		}
		outImage.SetPixels(pixels);
		return outImage;
	}
}
