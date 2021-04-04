using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

public class Lesson012_AssetPostprocessor_Model : AssetPostprocessor
{
	override public int GetPostprocessOrder()
	{
		return 100;
	}

	void OnPreprocessModel() {
		if (assetPath.StartsWith("Assets/Lesson 012")) {
			var importer = assetImporter as ModelImporter;
			importer.generateSecondaryUV = true;
		}
	}


	void OnPostprocessModel(GameObject obj) {
		if (assetPath.StartsWith("Assets/Lesson 012")) {
			// auto add box collider

			var bounds = new Bounds();
			foreach (var r in obj.GetComponentsInChildren<Renderer>()) {
				bounds.Encapsulate(r.bounds);
			}
			
			var box = obj.AddComponent<BoxCollider>();
			box.center = bounds.center;
			box.size   = bounds.size;
		}
	}
}