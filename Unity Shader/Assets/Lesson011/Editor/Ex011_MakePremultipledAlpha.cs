using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class Ex011_MakePremultipledAlpha : AssetPostprocessor {	
	public override int GetPostprocessOrder() {
		return 100;
	}

	void OnPostprocessTexture(Texture2D tex) {
		if (assetPath.Contains("[pma]")) {
			DoMulAlpha(tex);
		}
	}

	void DoMulAlpha(Texture2D tex) {
		Debug.Log("DoMulAlpha " + assetPath);

		var pixels = tex.GetPixels32();
		var n = pixels.Length;
		for (int i = 0; i < n; i++) {
			var p = pixels[i];
			var a = (int)pixels[i].a;

			p.r = (byte)((int)p.r * a / 255);
			p.g = (byte)((int)p.g * a / 255);
			p.b = (byte)((int)p.b * a / 255);

			pixels[i] = p;
		}

		tex.SetPixels32(pixels);
		tex.Apply(true);
	}
}
