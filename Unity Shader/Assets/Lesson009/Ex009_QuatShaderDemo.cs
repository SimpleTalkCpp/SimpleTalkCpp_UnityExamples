using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ex009_QuatShaderDemo : MonoBehaviour {

	public Material mat;
	public Texture2D tex;
	public Light dirLight;

	const int texWidth  = 4;
	const int texHeight = 4;

	public GameObject[] controlCubes = new GameObject[texWidth * texHeight];

#if UNITY_EDITOR
	[ContextMenu("Re-Create Control Cubes")]
	void CreateControlCubes() {
		string groupName = "_ControlCubes_";
		var group = transform.Find(groupName);
		if (group) {
			GameObject.DestroyImmediate(group.gameObject);
		}

		if (tex) {
			Object.DestroyImmediate(tex);
		}
		tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBAFloat, false);
		tex.name = "QuatTexture";

		group = new GameObject(groupName).transform;
		group.SetParent(transform, false);

		for (int y = 0; y < texHeight; y++) {
			for (int x = 0; x < texWidth; x++) {
				var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.name = string.Format("cube{0}_{1}", x, y);
				cube.transform.SetParent(group, false);
				cube.transform.localPosition = new Vector3(x,10,y);
				cube.transform.localScale = Vector3.one * 0.5f;
				controlCubes[y * texWidth + x] = cube;
			}
		}
	}
#endif

	void Update () {
		if (controlCubes != null && controlCubes.Length == texWidth * texHeight) {
			var colors = new Color[texWidth * texHeight];
			const int n = texWidth * texHeight;
			for (int i = 0; i < n; i++) {
				var cube = controlCubes[i];
				if (!cube)
					continue;
				var r = cube.transform.rotation;
				colors[i] = new Color(r.x, r.y, r.z, r.w);
			}

			tex.SetPixels(colors);
			tex.Apply();
		}

		if (mat) {
			mat.SetTexture("_QuatTex", tex);

			if (dirLight) {
				mat.SetVector("MyLightDir", dirLight.transform.forward);
			}
		}
	}
}
