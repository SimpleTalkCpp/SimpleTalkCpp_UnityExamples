using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ApplyTransformationToMeshWhenImport : AssetPostprocessor {

	void OnPostprocessModel(GameObject obj) {
		if (assetPath.StartsWith("Assets/ApplyTransformationToMeshWhenImport/MeshesToConvert/")) {
			applyTransformationToMesh(obj);
		}
	}

	void applyTransformationToMesh(GameObject obj) {
		foreach(Transform c in obj.transform) {
			applyTransformationToMesh(c.gameObject);
		}

		var mf = obj.GetComponent<MeshFilter>();
		if (mf != null) {
			var mesh = mf.sharedMesh;
			if (mesh != null) {
				Debug.Log("applyTransformationToMesh " + assetPath);

				var vertices = mesh.vertices;
				for (int i = 0; i < vertices.Length; i++) {
					vertices[i] = obj.transform.TransformPoint(vertices[i]);
				}

				mesh.vertices = vertices;
			}
		}

		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = new Quaternion();
		obj.transform.localScale = Vector3.one;		
	}

}
