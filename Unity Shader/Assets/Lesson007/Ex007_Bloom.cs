using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex007_Bloom : MonoBehaviour {

	public Material	bloomMat;

	[Range(0,1)]
	public float bloomIntensity = 0.5f;

	[Range(0,16)]
	public float blurIntensity = 3.2f;

	[Range(0,4)]
	public float blur2Factor = 1.6f;

	public Shader blurShader;

	private RenderTexture blurRenderTex1;
	private RenderTexture blurRenderTex2;

	private Material blurMat1H;
	private Material blurMat1V;
	private Material blurMat2H;
	private Material blurMat2V;

	private void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (!bloomMat)
			return;

		MyUtil.CreateRenderTexture(ref blurRenderTex1, Screen.width, Screen.height, 0);
		MyUtil.CreateRenderTexture(ref blurRenderTex2, Screen.width, Screen.height, 0);

		blurRenderTex1.name = "blurRenderTex1";
		blurRenderTex2.name = "blurRenderTex2";


		// 1st pass
		if (!blurMat1H) blurMat1H = new Material(blurShader);
		if (!blurMat1V) blurMat1V = new Material(blurShader);

		blurMat1H.SetVector("intensity", new Vector4(blurIntensity, 0, 0, 0));
		blurMat1V.SetVector("intensity", new Vector4(0, blurIntensity, 0, 0));

		Graphics.Blit(src, blurRenderTex1, blurMat1H);
		Graphics.Blit(blurRenderTex1, blurRenderTex2, blurMat1V);

		// 2nd Pass
		if (blur2Factor > 0) {
			if (!blurMat2H) blurMat2H = new Material(blurShader);
			if (!blurMat2V) blurMat2V = new Material(blurShader);

			blurMat2H.SetVector("intensity", new Vector4(blurIntensity * blur2Factor, 0, 0, 0));
			blurMat2V.SetVector("intensity", new Vector4(0, blurIntensity * blur2Factor, 0, 0));
			Graphics.Blit(blurRenderTex2, blurRenderTex1, blurMat2H);
			Graphics.Blit(blurRenderTex1, blurRenderTex2, blurMat2V);
		}

		bloomMat.SetFloat("intensity", bloomIntensity);
		bloomMat.SetTexture("blurTex", blurRenderTex2);
		Graphics.Blit(src, dst, bloomMat);
	}

	private void OnDestroy() {
		MyUtil.Destroy(ref blurMat1H);
		MyUtil.Destroy(ref blurMat1V);
		MyUtil.Destroy(ref blurMat2H);
		MyUtil.Destroy(ref blurMat2V);

		MyUtil.Destroy(ref blurRenderTex1);
		MyUtil.Destroy(ref blurRenderTex2);
	}
}
