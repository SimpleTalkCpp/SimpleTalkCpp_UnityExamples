using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Lesson012_AssetPostprocessor_Texture : AssetPostprocessor
{
	override public int GetPostprocessOrder()
	{
		return 100;
	}

	void OnPreprocessTexture() {
		if (assetPath.StartsWith("Assets/Lesson 012")) {
			var importer = assetImporter as TextureImporter;

			importer.filterMode = FilterMode.Trilinear;
		}
	}


	void OnPostprocessTexture(Texture2D tex) {
		if (assetPath.StartsWith("Assets/Lesson 012")) {
			Color32[] pixels = tex.GetPixels32();
			for (int i = 0; i < pixels.Length; i++) {
				pixels[i].r = 255;
			}

			tex.SetPixels32(pixels);
		}
	}
}
