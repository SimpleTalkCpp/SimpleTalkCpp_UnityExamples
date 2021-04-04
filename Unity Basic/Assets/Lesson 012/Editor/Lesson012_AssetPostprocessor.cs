using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Lesson012_AssetPostprocessor : AssetPostprocessor
{
	void OnPreprocessAsset() {
//		Debug.Log($"OnPreprocessAsset {assetPath}");
	}

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {

	}
}
