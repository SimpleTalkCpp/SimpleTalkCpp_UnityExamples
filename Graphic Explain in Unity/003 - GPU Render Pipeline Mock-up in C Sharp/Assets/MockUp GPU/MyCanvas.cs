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

	public float clearDepthValue = 1;

	public int drawOffsetX;
	public int drawOffsetY;

	[Range(1, 8)]
	public int drawScale = 1;

	public float fps = 0;

	float[] _depthBuffer;
	Texture2D _tex;

	private void Awake() {
		Application.targetFrameRate = 30;
	}

	private void Update() {
		fps = 1 / Time.deltaTime;

		canvasSize.x = Screen.width;
		canvasSize.y = Screen.height;

		if (canvasSize.x <= 0 || canvasSize.y <= 0) return;

		CreateTexture();

		var shapes = transform.GetComponentsInChildren<MyShape>(false);
		foreach (var shape in shapes) {
			shape.OnDraw(this);
		}

		_tex.Apply();
	}

	public Texture2D CreateTexture() {
		if (!_tex || _tex.width != canvasSize.x || _tex.height != canvasSize.y) {

			if (_tex) {
				if (Application.isEditor) {
					DestroyImmediate(_tex);
				} else {
					Destroy(_tex);
				}
			}

			_tex = new Texture2D(canvasSize.x, canvasSize.y);
			_tex.filterMode = FilterMode.Point;
			_depthBuffer = new float[canvasSize.x * canvasSize.y];
		}

		var pixels = new Color[canvasSize.x * canvasSize.y];
		for (int i = 0; i < pixels.Length; i++) {
			pixels[i] = backgroundColor;
		}
		_tex.SetPixels(pixels);

		for (int i = 0; i < _depthBuffer.Length; i++) {
			_depthBuffer[i] = clearDepthValue;
		}

		return _tex;
	}

	private void OnGUI() {
		if (_tex) {
			var rect = new Rect(drawOffsetX * drawScale,
								drawOffsetY * drawScale,
								_tex.width  * drawScale,
								_tex.height * drawScale);
			GUI.DrawTexture(rect, _tex);
		}

		GUILayout.Box($"Frame = {Time.frameCount}\n FPS = {(int)fps}");
	}

	public void SetPixel(int x, int y, in Color color) {
		if (!_tex) return;
		if (x < 0 || x >= canvasSize.x) return;
		if (y < 0 || y >= canvasSize.y) return;

		int iy  = _tex.height - y - 1;
		_tex.SetPixel(x, iy, color);
	}

	public void BlendPixel(int x, int y, in Color color) {
		if (!_tex) return;
		if (x < 0 || x >= canvasSize.x) return;
		if (y < 0 || y >= canvasSize.y) return;

		int iy  = _tex.height - y - 1;
		var old = _tex.GetPixel(x, iy);

		var newColor  = (color * color.a) + (old * (1-color.a));
		newColor.a = 1;

		_tex.SetPixel(x, iy, newColor);
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

	public float GetDepth(int x, int y) {
		return _depthBuffer[y * canvasSize.x + x];
	}

	public void SetDepth(int x, int y, float depth) {
		_depthBuffer[y * canvasSize.x + x] = depth;
	}
}
