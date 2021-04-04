using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex007_RenderToTexture : MonoBehaviour {

	public Material mat;
	public Camera renderTexCamera;

	public RenderTexture renderTex;

	private void Update() {
		if (renderTex == null || renderTex.width != Screen.width || renderTex.height != Screen.height) {
			MyUtil.Destroy(ref renderTex);

			Debug.Log("create RenderTexture");
			renderTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
			renderTex.name = "MyRenderTex";
		}

		if (renderTexCamera)
			renderTexCamera.targetTexture = renderTex;
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (!mat || !renderTex)
			return;

		mat.SetTexture("inputTex", renderTex);
		Graphics.Blit(src, dst, mat);
	}

	private void OnDestroy() {
		MyUtil.Destroy(ref renderTex);
	}
}
