using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyMockupGPU : MyShape
{
	public bool wireframe = true;

	private void Awake() {
		Application.targetFrameRate = 30;
	}


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

		// setup lights
		foreach (var root in rootObjects) {
			if (!root.activeInHierarchy) continue;

			var lights = root.GetComponentsInChildren<Light>(false);
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

		// draw renderers
		foreach (var root in rootObjects) {
			if (!root.activeInHierarchy) continue;

			var renderers = root.GetComponentsInChildren<Renderer>(false);
			foreach (var renderer in renderers) {
				draw(canvas, renderer, uniforms);
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
			o.pos = Mathf.Approximately(o.pos.w, 0) ? Vector4.zero : (o.pos / o.pos.w);

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
						DrawTriangle(canvas, v0, v1, v2, uniforms, material, true);
					}
				}
			}
		}
	}

	static void DrawBresenhamLine(MyCanvas canvas, in Vector2 a, in Vector2 b, in Color color) {
		// Bresenham's line algorithm - https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

		int ax = (int)a.x;
		int ay = (int)a.y;

		int bx = (int)b.x;
		int by = (int)b.y;

		int dx =  System.Math.Abs(bx - ax);
		int dy = -System.Math.Abs(by - ay);

		int signX = a.x < b.x ? 1 : -1;
		int signY = a.y < b.y ? 1 : -1;

		int err = dx + dy;

		int x = ax;
		int y = ay;

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

	void DrawTriangle(MyCanvas canvas, in VSOutput v0, in VSOutput v1, in VSOutput v2, in ShaderParameters uniforms, Material material, bool reorder) {
		var a = v0._screenPos;
		var b = v1._screenPos;
		var c = v2._screenPos;

		int canvasWidth  = canvas.canvasSize.x;
		int canvasHeight = canvas.canvasSize.y;

		// out of screen test
		if (a.x < 0   && b.x < 0   && c.x < 0  ) return;
		if (a.x >= canvasWidth && b.x >= canvasWidth && c.x >= canvasWidth) return;

		if (a.y < 0   && b.y < 0   && c.y < 0  ) return;
		if (a.y >= canvasHeight && b.y >= canvasHeight && c.y >= canvasHeight) return;

		//if (v0.pos.z < 0 && v1.pos.z < 0   && v2.pos.z < 0) return;
		//if (v0.pos.z > 1 && v1.pos.z > 1   && v2.pos.z > 1) return;

		if (reorder) {
			// re-order vertices by Y axis
			if (c.y < a.y && c.y < b.y) {
				if (a.y < b.y) {
					DrawTriangle(canvas, v2, v0, v1, uniforms, material, false);
				} else {
					DrawTriangle(canvas, v2, v1, v0, uniforms, material, false);
				}
				return;
			}

			if (b.y < a.y && b.y < c.y) {
				if (a.y < c.y) {
					DrawTriangle(canvas, v1, v0, v2, uniforms, material, false);
				} else {
					DrawTriangle(canvas, v1, v2, v0, uniforms, material, false);
				}
				return;
			}

			if (c.y < b.y) {
				DrawTriangle(canvas, v0, v2, v1, uniforms, material, false);
				return;
			}
		}

		VSOutput psInput = v0;

		if (b.y == c.y) {
			_DrawFlatTrangle(canvas, v0, v1, v2, uniforms, material);

		} else if (a.y == b.y) {
			_DrawFlatTrangle(canvas, v2, v0, v1, uniforms, material);

		} else {
			var midX = lineIntersect(a, c, b.y);
			var ratio = (float)(midX - a.x) / (c.x - a.x);
			var mid = VSOutput.Lerp(v0, v2, ratio);

			mid._screenPos.x = Mathf.RoundToInt(lineIntersect(a, c, b.y));
			mid._screenPos.y = b.y;

			_DrawFlatTrangle(canvas, v0, mid, v1, uniforms, material);
			_DrawFlatTrangle(canvas, v2, mid, v1, uniforms, material);
		}
	}

	float lineIntersect(in Vector2 a, in Vector2 b, float y) {
		float dx = b.x - a.x;
		float dy = b.y - a.y;

		if (dy == 0) return a.x;
		return (y - a.y) * (float)dx/dy + a.x;
	}

	void _DrawFlatTrangle(MyCanvas canvas, in VSOutput v0, in VSOutput v1, in VSOutput v2, in ShaderParameters uniforms, Material material) {
		var a = v0._screenPos;
		var b = v1._screenPos;
		var c = v2._screenPos;
	
		if (c.x < b.x) {
			_DrawFlatTrangle(canvas, v0, v2, v1, uniforms, material);
			return;
		}

		if (b.y != c.y) {
			Debug.LogError("");
		}

		int dy = Mathf.CeilToInt(b.y - a.y);
		int step = 1;
		if (dy < 0) {
			dy = -dy;
			step = -1;
		}

		int canvasWidth  = canvas.canvasSize.x;
		int canvasHeight = canvas.canvasSize.y;

		for (int i = 0; i <= dy; i++) {
			int y = (int)a.y + i * step;

			if (y < 0 || y >= canvasHeight) continue;

			var start = Mathf.FloorToInt(lineIntersect(a, b, y));
			var end   = Mathf.CeilToInt (lineIntersect(a, c, y));

			var barycentric = new Vector2(0, (float)i/dy);

			for (int x = start; x <= end; x++) {
				if (x < 0 || x >= canvasWidth) continue;

				barycentric.x = (float)(x - start) / (end - start);

				var ps01 = VSOutput.Lerp(v0, v1, barycentric.y);
				var ps02 = VSOutput.Lerp(v0, v2, barycentric.y);
				var psInput = VSOutput.Lerp(ps01, ps02, barycentric.x);

				// depth test
				float depth = canvas.GetDepth(x, y);
				float newDepth = psInput.pos.z;

//				if (newDepth < -1 || newDepth > 1) continue;
//				if (newDepth > depth) continue;

				canvas.SetDepth(x, y, newDepth);

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
		var light = Mathf.Clamp01(Vector3.Dot(input.normal, -uniforms._WorldSpaceLightPos0));
		light = Mathf.Max(0, light);

		var o = input.color * light;
		o.a = 1;

		float depth = input.pos.z;
		o.r = depth;
		o.g = depth;
		o.b = depth;

		return o;
	}
}
