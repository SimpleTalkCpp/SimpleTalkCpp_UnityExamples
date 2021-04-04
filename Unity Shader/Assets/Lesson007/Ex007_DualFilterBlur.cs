using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ref: Siggraph2015 - Bandwidth-Efficient Rendering by Marius Bjorge (ARM)
// https://community.arm.com/cfs-file/__key/communityserver-blogs-components-weblogfiles/00-00-00-20-66/siggraph2015_2D00_mmg_2D00_marius_2D00_notes.pdf

public class Ex007_DualFilterBlur : MonoBehaviour {

	public Material	downScaleMat;
	public Material	upScaleMat;

	const int maxPassCount = 16;

	private RenderTexture[] renderTextures = new RenderTexture[maxPassCount];

	[Range(1,maxPassCount)]
	public int passCount = 4;

	private void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (!downScaleMat) return;
		if (!upScaleMat) return;

		if (passCount >= maxPassCount)
			passCount = maxPassCount;

		const int minRenderTexSize = 4; // some platform doesn't support render texture < 4, for example PlayStation4
		
		for (int i = 0; i < passCount; i++) {
			int w = Screen.width  >> i;
			int h = Screen.height >> i;
			if (w < minRenderTexSize || h < minRenderTexSize)
				continue;

			MyUtil.CreateRenderTexture(ref renderTextures[i], w, h, 0);
			renderTextures[i].name = "dualFilterBlur" + i;

			// Down Sample Pass
			var halfPixel = new Vector2(0.5f/w, 0.5f/h);
			downScaleMat.SetVector("halfPixel", new Vector4(halfPixel.x, halfPixel.y, halfPixel.x, -halfPixel.y));
			Graphics.Blit(i == 0 ? src : renderTextures[i-1], renderTextures[i], downScaleMat);
		}

		for (int i = passCount - 1; i >= 0; i--) {
			int w = Screen.width  >> i;
			int h = Screen.height >> i;
			if (w < minRenderTexSize || h < minRenderTexSize)
				continue;

			// Up Sample Pass
			var halfPixel = new Vector2(0.25f/w, 0.25f/h);
			upScaleMat.SetVector("halfPixel", new Vector4(halfPixel.x, halfPixel.y, halfPixel.x, -halfPixel.y));
			Graphics.Blit(renderTextures[i], i == 0 ? dst : renderTextures[i-1], upScaleMat);
		}
	}

	private void OnDestroy() {
		for (int i = 0; i < maxPassCount; i++) {
			MyUtil.Destroy(ref renderTextures[i]);
		}
	}
}
