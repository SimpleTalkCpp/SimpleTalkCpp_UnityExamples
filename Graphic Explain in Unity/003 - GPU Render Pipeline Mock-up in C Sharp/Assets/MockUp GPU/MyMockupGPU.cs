using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyMockupGPU : MyShape
{
	public bool wireframe = true;

	public MyShader shader = new MyShader();

	[System.Serializable]
	public struct DebugInfo {
		public int vertexShaderCount;
		public int pixelShaderCount;
		public int triangleCount;
	}
	public DebugInfo debugInfo;

	private void Awake() {
		Application.targetFrameRate = 15;
	}


	void Update() {
		if (Input.GetKeyDown(KeyCode.W)) {
			wireframe = !wireframe;
		}
	}

	public override void OnDraw(MyCanvas canvas) {
		debugInfo = new DebugInfo();

		var cam = Camera.main;

		var shaderParams = new MyShader.Parameters();
		shaderParams.UNITY_MATRIX_V = cam.transform.worldToLocalMatrix;

		{
			// matrix embeds a z-flip operation whose purpose is to cancel the z-flip performed by the camera view matrix
			var tmp = cam.projectionMatrix;
			tmp.SetColumn(2, -tmp.GetColumn(2));
			shaderParams.UNITY_MATRIX_P = tmp;
		}

		var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

		var light = Object.FindObjectOfType<Light>();
		if (light) {
			if (light.type == LightType.Directional) {
				shaderParams._WorldSpaceLightPos0 = -light.transform.forward;
				shaderParams._WorldSpaceLightPos0.w = 0;

			} else {
				shaderParams._WorldSpaceLightPos0 = light.transform.position;
				shaderParams._WorldSpaceLightPos0.w = 1;
			}
		}

		// draw renderers
		foreach (var root in rootObjects) {
			if (!root.activeInHierarchy) continue;

			var renderers = root.GetComponentsInChildren<Renderer>(false);
			foreach (var renderer in renderers) {
				draw(canvas, renderer, shaderParams);
			}
		}
	}

	void draw(MyCanvas canvas, Renderer renderer, MyShader.Parameters shaderParams) {
		shaderParams.unity_ObjectToWorld = renderer.transform.localToWorldMatrix;
		shaderParams.UNITY_MATRIX_MV  = shaderParams.UNITY_MATRIX_V * shaderParams.unity_ObjectToWorld;
		shaderParams.UNITY_MATRIX_MVP = shaderParams.UNITY_MATRIX_P * shaderParams.UNITY_MATRIX_MV;

		var mtl = renderer.sharedMaterial;
		if (mtl) {
			shaderParams.materialColor = mtl.color;
		}

		var meshRenderer = renderer as MeshRenderer;
		if (meshRenderer) {
		 	drawMesh(canvas, meshRenderer, shaderParams);
			return;
		}
	}

	List<int    > _meshIndices  = new List<int>();
	List<Vector3> _meshVertices = new List<Vector3>();
	List<Vector3> _meshNormals  = new List<Vector3>();
	List<Color  > _meshColors   = new List<Color>();

	void drawMesh(MyCanvas canvas, MeshRenderer meshRenderer, MyShader.Parameters shaderParams) {
		var meshFilter = meshRenderer.GetComponent<MeshFilter>();
		if (!meshFilter) return;

		var mesh = meshFilter.sharedMesh;
		if (!mesh) return;

		mesh.GetVertices(_meshVertices);
		mesh.GetNormals(_meshNormals);
		mesh.GetColors(_meshColors);

		MyShader.VsOutput handleVertex(int vertexIndex) {
			var vsInput		= new MyShader.VsInput();
			vsInput.vertex		= _meshVertices[vertexIndex];
			vsInput.normal	= vertexIndex < _meshNormals.Count ? _meshNormals[vertexIndex] : Vector3.zero; 
			vsInput.color   = vertexIndex < _meshColors.Count  ? _meshColors[vertexIndex]  : Color.white;

			debugInfo.vertexShaderCount++;
			var o = shader.vertexShader(vsInput, shaderParams);
			if (!Mathf.Approximately(o.vertex.w, 0)) {
				o.vertex /= o.vertex.w;
			}

			o.vertex.z = (o.vertex.z + 1) * 0.5f; // [-1 ~ +1] => [0 ~ 1]

//			Debug.Log($"pos[{vertexIndex}] = {o.pos} {o.pos.z:0.0000000} {o.color}");
			o._screenPos = new Vector2Int((int)((1 + o.vertex.x) * 0.5f * canvas.canvasSize.x),
										  (int)((1 - o.vertex.y) * 0.5f * canvas.canvasSize.y));
			return o;
		}

		for (int sub = 0; sub < mesh.subMeshCount; sub++) {
			mesh.GetIndices(_meshIndices, sub, true);

			int triCount = _meshIndices.Count / 3;
			for (int tri = 0; tri < triCount; tri++) {
				int i = tri * 3;

				var v0 = handleVertex(_meshIndices[i]);
				var v1 = handleVertex(_meshIndices[i+1]);
				var v2 = handleVertex(_meshIndices[i+2]);

				if (wireframe) {
					canvas.DrawLine(v0._screenPos, v1._screenPos, Color.red);
					canvas.DrawLine(v1._screenPos, v2._screenPos, Color.red);
					canvas.DrawLine(v2._screenPos, v0._screenPos, Color.red);

				} else {
			
					var v01 = v0.vertex - v1.vertex;
					var v02 = v0.vertex - v2.vertex;

					var faceDirection = Vector3.Dot(Vector3.Cross(v02, v01), Vector3.forward);
					if (shader.cull == MyShader.Cull.Back  && faceDirection < 0) continue;
					if (shader.cull == MyShader.Cull.Front && faceDirection > 0) continue;

					debugInfo.triangleCount++;
					DrawTriangle(canvas, v0, v1, v2, shaderParams, true);
				}
			}
		}
	}

	void DrawTriangle(	MyCanvas canvas, 
						in MyShader.VsOutput v0, 
						in MyShader.VsOutput v1, 
						in MyShader.VsOutput v2, 
						in MyShader.Parameters shaderParams,
						bool reorder)
	{
		var a = v0._screenPos;
		var b = v1._screenPos;
		var c = v2._screenPos;

		// re-order vertices by Y axis
		if (reorder) {
			if (c.y < a.y && c.y < b.y) {
				if (a.y < b.y) {
					DrawTriangle(canvas, v2, v0, v1, shaderParams, false);
				} else {
					DrawTriangle(canvas, v2, v1, v0, shaderParams, false);
				}
				return;
			}

			if (b.y < a.y && b.y < c.y) {
				if (a.y < c.y) {
					DrawTriangle(canvas, v1, v0, v2, shaderParams, false);
				} else {
					DrawTriangle(canvas, v1, v2, v0, shaderParams, false);
				}
				return;
			}

			if (c.y < b.y) {
				DrawTriangle(canvas, v0, v2, v1, shaderParams, false);
				return;
			}
		}

		if (b.y == c.y) { // only top triangle
			_DrawFlatTrangle(canvas, v0, v1, v2, shaderParams);
			return;
		}
		
		if (a.y == b.y) { // only bottom triangle
			_DrawFlatTrangle(canvas, v2, v0, v1, shaderParams);
			return;
		}
		
		var midX = lineIntersect(a, c, b.y);

		var ratio = (float)(b.y - a.y) / (c.y - a.y);
		var mid = MyShader.VsOutput.Lerp(v0, v2, ratio);

		mid._screenPos.x = (int)lineIntersect(a, c, b.y);
		mid._screenPos.y = b.y;

		_DrawFlatTrangle(canvas, v0, mid, v1, shaderParams);
		_DrawFlatTrangle(canvas, v2, mid, v1, shaderParams);
	}

	float lineIntersect(in Vector2 a, in Vector2 b, float y) {
		float dx = b.x - a.x;
		float dy = b.y - a.y;

		if (dy == 0) return a.x;
		return (y - a.y) * (float)dx/dy + a.x;
	}

	void _DrawFlatTrangle(MyCanvas canvas, in MyShader.VsOutput v0, in MyShader.VsOutput v1, in MyShader.VsOutput v2, in MyShader.Parameters shaderParams) {
		var a = v0._screenPos;
		var b = v1._screenPos;
		var c = v2._screenPos;
	
		if (c.x < b.x) {
			_DrawFlatTrangle(canvas, v0, v2, v1, shaderParams);
			return;
		}

		if (b.y != c.y) {
			Debug.LogError("");
		}

		int dy = (int)(b.y - a.y);
		int ySign = 1;
		if (dy < 0) {
			dy = -dy;
			ySign = -1;
		}

		int canvasWidth  = canvas.canvasSize.x;
		int canvasHeight = canvas.canvasSize.y;

		for (int i = 0; i <= dy; i++) {
			int y = (int)a.y + i * ySign;

			if (y < 0 || y >= canvasHeight) continue;

			var start = (int)lineIntersect(a, b, y);
			var end   = (int)lineIntersect(a, c, y);

			for (int x = start; x <= end; x++) {
				if (x < 0 || x >= canvasWidth) continue;

				var barycentric = new Vector2(0, (float)i/dy);

				if (end != start) {
					barycentric.x = (float)(x - start) / (end - start);
				}

				var ps01    = MyShader.VsOutput.Lerp(v0,   v1,   barycentric.y);
				var ps02    = MyShader.VsOutput.Lerp(v0,   v2,   barycentric.y);
				var psInput = MyShader.VsOutput.Lerp(ps01, ps02, barycentric.x);

				float depth = canvas.GetDepth(x, y);
				float newDepth = psInput.vertex.z;

				if (newDepth < 0 || newDepth > 1) continue;

				switch (shader.depthTestFunc) {
					case MyShader.DepthTestFunc.Less:		if (newDepth > depth) continue; break;
					case MyShader.DepthTestFunc.Greater:	if (newDepth < depth) continue; break;
				}
				
				debugInfo.pixelShaderCount++;
				var col = shader.pixelShader(psInput, shaderParams);

				if (shader.depthWrite) {
					canvas.SetDepth(x, y, newDepth);
				}

				col.r = Mathf.Clamp01(col.r);
				col.g = Mathf.Clamp01(col.g);
				col.b = Mathf.Clamp01(col.b);
				col.a = Mathf.Clamp01(col.a);

				canvas.BlendPixel(x, y, col);
			}
		}
	}
}
