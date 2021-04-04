using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyLocator : MonoBehaviour {
	public Material mat;

	void Update () {
		if (mat != null) {
			var p = transform.position;
			var v = new Vector4(p.x, p.y, p.z, 1);
			mat.SetVector("myLocator", v);
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color(1,0,0,1);
		float size = 0.2f;
        Gizmos.DrawCube(transform.position, Vector3.one * size);
	}
}
