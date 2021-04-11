using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyMockupGPU : MyShape
{
	public bool wireframe = true;

	void Update() {
		if (Input.GetKeyDown(KeyCode.W)) {
			wireframe = !wireframe;
		}		
	}

	public override void OnDraw(MyCanvas canvas) {
		var uniforms = new ShaderParameters();
		uniforms._WorldSpaceCameraPos = Camera.main.transform.position;
		uniforms.UNITY_MATRIX_V = Camera.main.transform.worldToLocalMatrix;
		uniforms.UNITY_MATRIX_P = Camera.main.projectionMatrix;

		uniforms._ScreenParams = new Vector4(canvas.canvasSize.x,
											 canvas.canvasSize.y,
											 1 + 1f / canvas.canvasSize.x,
											 1 + 1f / canvas.canvasSize.y);

		var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (var root in rootObjects) {
			var renderers = root.GetComponentsInChildren<Renderer>();
			foreach (var renderer in renderers) {
				draw(canvas, renderer, uniforms);
			}

			var lights = root.GetComponentsInChildren<Light>();
			foreach (var light in lights) {
				if (light.type == LightType.Directional) {
					uniforms._WorldSpaceLightPos0 = light.transform.forward;
					uniforms._WorldSpaceLightPos0.w = 0;
				} else {
					uniforms._WorldSpaceLightPos0 = light.transform.position;
					uniforms._WorldSpaceLightPos0.w = 1;
				}
			}
		}
	}

	void draw(MyCanvas canvas, Renderer renderer, ShaderParameters uniforms) {
		uniforms.unity_ObjectToWorld = renderer.transform.localToWorldMatrix;
		uniforms.unity_WorldToObject = renderer.transform.worldToLocalMatrix;

		uniforms.UNITY_MATRIX_MV  = uniforms.UNITY_MATRIX_V * uniforms.unity_ObjectToWorld;
		uniforms.UNITY_MATRIX_MVP = uniforms.UNITY_MATRIX_P * uniforms.UNITY_MATRIX_V * uniforms.unity_ObjectToWorld;

		uniforms.UNITY_MATRIX_T_MV  = uniforms.UNITY_MATRIX_MV.transpose;
		uniforms.UNITY_MATRIX_IT_MV = uniforms.UNITY_MATRIX_MV.inverse.transpose;

		var meshRenderer = renderer as MeshRenderer;
		if (meshRenderer) {
		 	drawMesh(canvas, meshRenderer, uniforms);
			return;
		}
	}

	public class ShaderParameters {
		// https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
		public Matrix4x4 unity_ObjectToWorld;	// Current model matrix.
		public Matrix4x4 unity_WorldToObject;	// Inverse of current world matrix.

		public Matrix4x4 UNITY_MATRIX_V;		// Current view matrix.
		public Matrix4x4 UNITY_MATRIX_P;		// Current projection matrix.
		public Matrix4x4 UNITY_MATRIX_MV;		// Current model * view matrix.
		public Matrix4x4 UNITY_MATRIX_MVP;		// Current model * view * projection matrix.

		public Matrix4x4 UNITY_MATRIX_T_MV;		// Transpose of model * view matrix.
		public Matrix4x4 UNITY_MATRIX_IT_MV;	// Inverse transpose of model * view matrix.

		public Vector3   _WorldSpaceCameraPos;
		public Vector4   _ScreenParams;			// x is the width  of the camera’s target texture in pixels, 
												// y is the height of the camera’s target texture in pixels, 
												// z is 1.0 + 1.0/width 
												// w is 1.0 + 1.0/height.

		public Vector4	_WorldSpaceLightPos0;	// Directional lights: (world space direction, 0). Other lights: (world space position, 1).
	}

	public struct VSInput {
		public Vector3 pos;
		public Vector3 normal;
		public Color   color;
	}

	public struct VSOutput {
		public Vector4 pos;
		public Vector3 worldPos;

		public Vector3 normal;
		public Color   color;
		public Vector2Int _screenPos;

		static public VSOutput Lerp(in VSOutput a, in VSOutput b, float w) {
			var o = new VSOutput();
			o.pos    = Vector4.Lerp(a.pos, b.pos, w);
			o.normal = Vector3.Lerp(a.normal, b.normal, w);
			o.color  = Color.Lerp(a.color, b.color, w);
			return o;
		}
	}

	void drawMesh(MyCanvas canvas, MeshRenderer meshRenderer, ShaderParameters uniforms) {
		var material = meshRenderer.sharedMaterial;
		if (!material) return;

		var meshFilter = meshRenderer.GetComponent<MeshFilter>();
		if (!meshFilter) return;

		var mesh = meshFilter.sharedMesh;
		if (!mesh) return;

		var vertices = new List<Vector3>();
		var normals  = new List<Vector3>();
		var colors   = new List<Color>();

		List<int> indices = new List<int>();

		mesh.GetVertices(vertices);
		mesh.GetNormals(normals);
		mesh.GetColors(colors);

		VSOutput handleVertex(int vertexIndex) {
			var vsInput		= new VSInput();
			vsInput.pos		= vertices[vertexIndex];
			vsInput.normal	= vertexIndex < normals.Count ? normals[vertexIndex] : Vector3.zero; 
			vsInput.color   = vertexIndex < colors.Count  ? colors[vertexIndex]  : Color.white;

			// semantic: NORMAL
			vsInput.normal = uniforms.unity_ObjectToWorld.MultiplyVector(vsInput.normal);

			var o = vertexShader(vsInput, uniforms, material);
			o.pos = (o.pos.w != 0) ? o.pos / o.pos.w : Vector4.zero;

			o._screenPos = new Vector2Int((int)((1 - o.pos.x) * 0.5f * canvas.canvasSize.x),
										 (int)((1 + o.pos.y) * 0.5f * canvas.canvasSize.y));
			return o;
		}

		for (int sub = 0; sub < mesh.subMeshCount; sub++) {
			mesh.GetIndices(indices, sub, true);

			int triCount = indices.Count / 3;
			for (int tri = 0; tri < triCount; tri++) {
				int i = tri * 3;

				var v0 = handleVertex(indices[i]);
				var v1 = handleVertex(indices[i+1]);
				var v2 = handleVertex(indices[i+2]);

				if (wireframe) {
					DrawBresenhamLine(canvas, v0._screenPos, v1._screenPos, Color.red);
					DrawBresenhamLine(canvas, v1._screenPos, v2._screenPos, Color.red);
					DrawBresenhamLine(canvas, v2._screenPos, v0._screenPos, Color.red);

				} else {
					var v01 = v0.pos - v1.pos;
					var v02 = v0.pos - v2.pos;

					var faceDirection = Vector3.Dot(Vector3.Cross(v02, v01), Vector3.forward);
					if (faceDirection > 0) {
						DrawTriangle(canvas, v0, v1, v2, uniforms, material);
					}
				}
			}
		}
	}

	static void DrawBresenhamLine(MyCanvas canvas, Vector2Int a, Vector2Int b, in Color color) {
		// Bresenham's line algorithm - https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

		int dx = System.Math.Abs(b.x - a.x);
		int dy = -System.Math.Abs(b.y - a.y);

		int signX = a.x < b.x ? 1 : -1;
		int signY = a.y < b.y ? 1 : -1;

		int err = dx + dy;

		int x = a.x;
		int y = a.y;

		while (true) {
			canvas.SetPixel(x, y, color);
			if (x == b.x && y == b.y) break;

			int e2 = 2 * err;
			if (e2 >= dy) {
				err += dy;
				x += signX;
			}

			if (e2 <= dx) {
				err += dx;
				y += signY;
			}
		}
	}

	void DrawTriangle(MyCanvas canvas, in VSOutput v0, in VSOutput v1, in VSOutput v2, in ShaderParameters uniforms, Material material) {
		var a = v0._screenPos;
		var b = v1._screenPos;
		var c = v2._screenPos;

		if (c.y < a.y && c.y < a.y) {
			if (a.y < b.y) {
				DrawTriangle(canvas, v2, v0, v1, uniforms, material);
			} else {
				DrawTriangle(canvas, v2, v1, v0, uniforms, material);
				return;
			}
		}

		if (b.y < a.y && b.y < c.y) {
			if (a.y < c.y) {
				DrawTriangle(canvas, v1, v0, v2, uniforms, material);
				return;
			} else {
				DrawTriangle(canvas, v1, v2, v0, uniforms, material);
				return;
			}
		}

		if (c.y < b.y) {
			DrawTriangle(canvas, v0, v2, v1, uniforms, material);
			return;
		}

		VSOutput psInput = v0;

		int dx = c.x - b.x;
		int dy = c.y - b.y;

		if (b.y == c.y) {
			_DrawFlatTrangle(canvas, v0, v1, v2, uniforms, material);

		} else if (a.y == b.y) {
			_DrawFlatTrangle(canvas, v2, v0, v1, uniforms, material);

		} else {
			var weight = lineIntersectY(a, c, b.y);
			var mid = VSOutput.Lerp(v0, v1, weight);

			mid._screenPos.x = lineIntersectY(a, c, b.y);
			mid._screenPos.y = b.y;

			_DrawFlatTrangle(canvas, v0, v1, mid, uniforms, material);
			_DrawFlatTrangle(canvas, v2, v1, mid, uniforms, material);
		}
	}

	int lineIntersectY(in Vector2Int a, in Vector2Int b, int y) {
		int dx = b.x - a.x;
		int dy = b.y - a.y;

		if (dy == 0) return a.x;
		return (int)((y - a.y) * ((float)dx/dy) + a.x);
	}

	void _DrawFlatTrangle(MyCanvas canvas, in VSOutput v0, in VSOutput v1, in VSOutput v2, in ShaderParameters uniforms, Material material) {
		var a = v0._screenPos;
		var b = v1._screenPos;
		var c = v2._screenPos;

		if (b.y != c.y) {
			Debug.LogError("");
		}

		int dy = b.y - a.y;
		int step = 1;
		if (dy < 0) {
			dy = -dy;
			step = -1;
		}

		for (int i = 0; i <= dy; i++) {
			int y = a.y + i * step;

			var start = lineIntersectY(a, b, y);
			var end   = lineIntersectY(a, c, y);

			if (start > end) {
				var tmp = start;
				start = end;
				end = tmp;
			}

			var barycentric = new Vector2(0, (float)i/dy);

			for (int x = start; x <= end; x++) {
				barycentric.x = (float)(x - start) / (end - start);

				var psInput = (v1._screenPos.x < v2._screenPos.x) ? VSOutput.Lerp(v1, v2, barycentric.x) : VSOutput.Lerp(v2, v1, barycentric.x);
				psInput = VSOutput.Lerp(v0, psInput, barycentric.y);

				var col = pixelShader(psInput, uniforms, material);
				canvas.BlendPixel(x, y, col);
			}
		}
	}

	VSOutput vertexShader(in VSInput input, in ShaderParameters uniforms, Material material) {
		var o = new VSOutput();
		Vector4 pos = input.pos;
		pos.w = 1;
		
		o.pos = uniforms.UNITY_MATRIX_MVP * pos;
		o.worldPos = uniforms.unity_ObjectToWorld.MultiplyPoint(input.pos);
		o.normal = input.normal;
		o.color = input.color;
		return o;
	}

	Color pixelShader(VSOutput input, in ShaderParameters uniforms, Material material) {
		var light = Mathf.Clamp01(Vector3.Dot(input.normal, uniforms._WorldSpaceCameraPos));
		var o = input.color * light;
		o.a = 1;

		var col = uniforms._WorldSpaceCameraPos;

		return new Color(col.x, col.y, col.z, 1);
	}
}
