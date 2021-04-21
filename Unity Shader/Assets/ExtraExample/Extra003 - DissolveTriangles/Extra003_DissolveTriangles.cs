using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extra003_DissolveTriangles : MonoBehaviour
{
	public int groupCount = 4;

	Material mat;
	float time;

	private void OnEnable() {
		GenMesh();
		mat = GetComponent<Renderer>()?.sharedMaterial;
	}

	private void OnDisable() {
		var mat = GetComponent<Renderer>()?.sharedMaterial;
		if (mat) {
			mat.SetFloat("_MyTime", 0);
		}
	}

	void Update()
	{
		time += Time.deltaTime;
		if (mat) {
			mat.SetFloat("_MyTime", time);
		}
	}

	void GenMesh() {
		var mf = GetComponent<MeshFilter>();
		if (!mf) return;

		var mesh = mf.sharedMesh;

		var vertices  = mesh.vertices;
		var uv		  = mesh.uv;
		var triangles = mesh.triangles;

		var newTriangles = new int[triangles.Length];
		var newVertices  = new Vector3[triangles.Length];
		var newUv        = new Vector2[triangles.Length];
		var newUv2       = new Vector2[triangles.Length];
		var newUv3       = new Vector2[triangles.Length];

		var triCount = triangles.Length / 3;
		var triCenters = new Vector3[triCount];

		var groupId = new float[triCount];

		for (int i = 0; i < triCount; i++) {
			var v0 = vertices[triangles[i * 3   ]];
			var v1 = vertices[triangles[i * 3 + 1]];
			var v2 = vertices[triangles[i * 3 + 2]];

			triCenters[i] = (v0 + v1 + v2) / 3;
			groupId[i] = (float)Random.Range(0, groupCount) / groupCount;
		}

		for (int i = 0; i < triangles.Length; i++) {
			var vi = triangles[i];
			newTriangles[i] = i;
			newVertices[i]  = vertices[vi];
			newUv[i]        = uv[vi];

			var tri = i / 3;
			var center = triCenters[tri];
			newUv2[i]       = new Vector2(center.x, center.y);
			newUv3[i]       = new Vector2(center.z, groupId[tri]);
		}

		var newMesh = new Mesh();
		newMesh.name = "NewMesh";
		newMesh.vertices  = newVertices;
		newMesh.uv        = newUv;
		newMesh.uv2       = newUv2;
		newMesh.uv3       = newUv3;
		newMesh.triangles = newTriangles;
		mf.mesh = newMesh;
	}
}
