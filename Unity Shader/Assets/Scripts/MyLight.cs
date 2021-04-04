using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyLight : MonoBehaviour {
	void Update () {
		Shader.SetGlobalVector("MyLightPos", transform.position);
	}
}
