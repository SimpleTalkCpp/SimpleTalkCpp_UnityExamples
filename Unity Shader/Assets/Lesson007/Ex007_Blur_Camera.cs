using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex007_Blur_Camera : MonoBehaviour {

	public enum Example {
		Blur3x3,
		Blur5x5,
		Blur7x7,

		Blur3x3_TwoPass,
		Blur5x5_TwoPass,
		Blur7x7_TwoPass,
	};

	public Example example = Example.Blur7x7;

	public Shader   blur3x3Shader;
	public Shader   blur5x5Shader;
	public Shader   blur7x7Shader;

	[Range(0,1)]
	public float intensity = 0;

	[Range(0,16)]
	public float blur1 = 1;

	[Range(0,2)]
	public float blur2 = 1.6f;

	private Material blur3x3Mat1;
	private Material blur3x3Mat2;

	private Material blur5x5Mat1;
	private Material blur5x5Mat2;

	private Material blur7x7Mat1H;
	private Material blur7x7Mat1V;

	private Material blur7x7Mat2H;
	private Material blur7x7Mat2V;

	public RenderTexture renderTex;
	public RenderTexture renderTex2;

	private void Start() {
		blur3x3Mat1 = new Material(blur3x3Shader);
		blur3x3Mat2 = new Material(blur3x3Shader);

		blur5x5Mat1 = new Material(blur5x5Shader);
		blur5x5Mat2 = new Material(blur5x5Shader);

		blur7x7Mat1H = new Material(blur7x7Shader);
		blur7x7Mat1V = new Material(blur7x7Shader);
		blur7x7Mat2H = new Material(blur7x7Shader);
		blur7x7Mat2V = new Material(blur7x7Shader);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		MyUtil.CreateRenderTexture(ref renderTex,  Screen.width, Screen.height, 0);
		MyUtil.CreateRenderTexture(ref renderTex2, Screen.width, Screen.height, 0);

		renderTex.name = "MyRenderTex1";
		renderTex2.name = "MyRenderTex2";

		switch (example) {
			case Example.Blur3x3: {
				blur3x3Mat1.SetFloat("intensity", intensity * blur1);
				Graphics.Blit(src, dst, blur3x3Mat1);
			} break;

			case Example.Blur5x5: {
				blur5x5Mat1.SetFloat("intensity", intensity * blur1);
				Graphics.Blit(src, dst, blur5x5Mat1);
			} break;

			case Example.Blur3x3_TwoPass: {
				blur3x3Mat1.SetFloat("intensity", intensity * blur1);
				blur3x3Mat2.SetFloat("intensity", intensity * blur1 * blur2);

				Graphics.Blit(src, renderTex, blur3x3Mat1);
				Graphics.Blit(renderTex, dst, blur3x3Mat2);
			} break;

			case Example.Blur5x5_TwoPass: {
				blur5x5Mat1.SetFloat("intensity", intensity * blur1);
				blur5x5Mat2.SetFloat("intensity", intensity * blur1 * blur2);

				Graphics.Blit(src, renderTex, blur5x5Mat1);
				Graphics.Blit(renderTex, dst, blur5x5Mat2);
			} break;

			case Example.Blur7x7: {
				blur7x7Mat1H.SetVector("intensity", new Vector4(1,0,0,0) * intensity * blur1);
				blur7x7Mat1V.SetVector("intensity", new Vector4(0,1,0,0) * intensity * blur1);

				Graphics.Blit(src, renderTex, blur7x7Mat1H);
				Graphics.Blit(renderTex, dst, blur7x7Mat1V);
			} break;

			case Example.Blur7x7_TwoPass: {
				blur7x7Mat1H.SetVector("intensity", new Vector4(1,0,0,0) * intensity * blur1);
				blur7x7Mat1V.SetVector("intensity", new Vector4(0,1,0,0) * intensity * blur1);

				blur7x7Mat2H.SetVector("intensity", new Vector4(1,0,0,0) * intensity * blur1 * blur2);
				blur7x7Mat2V.SetVector("intensity", new Vector4(0,1,0,0) * intensity * blur1 * blur2);

				Graphics.Blit(src, renderTex, blur7x7Mat1H);
				Graphics.Blit(renderTex, renderTex2, blur7x7Mat1V);

				Graphics.Blit(renderTex2, renderTex, blur7x7Mat2H);
				Graphics.Blit(renderTex, dst, blur7x7Mat2V);
			} break;
		}
	}

	private void OnDestroy() {
		MyUtil.Destroy(ref blur3x3Mat1);
		MyUtil.Destroy(ref blur3x3Mat2);

		MyUtil.Destroy(ref blur5x5Mat1);
		MyUtil.Destroy(ref blur5x5Mat2);

		MyUtil.Destroy(ref blur7x7Mat1H);
		MyUtil.Destroy(ref blur7x7Mat1V);
		MyUtil.Destroy(ref blur7x7Mat2H);
		MyUtil.Destroy(ref blur7x7Mat2V);

		MyUtil.Destroy(ref renderTex);
		MyUtil.Destroy(ref renderTex2);
	}
}
