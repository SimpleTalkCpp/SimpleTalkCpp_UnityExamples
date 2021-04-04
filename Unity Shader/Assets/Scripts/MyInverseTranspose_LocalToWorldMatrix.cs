using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyInverseTranspose_LocalToWorldMatrix : MonoBehaviour {
	public Material mat;

	void Update () {
		if (mat != null) {
			mat.SetMatrix("MyInverseTranspose_LocalToWorldMatrix", transform.localToWorldMatrix.inverse.transpose);
		}
	}
}
