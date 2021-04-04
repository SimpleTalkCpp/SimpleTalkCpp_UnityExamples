using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

public class Lesson012_AssetPostprocessor_Animation : AssetPostprocessor
{
	override public int GetPostprocessOrder()
	{
		return 101;
	}

	bool assetNeedProcess() {
		if (assetPath.StartsWith("Assets/Lesson 010/mixamo/Sword and Shield Pack/")) {
			if (assetPath.Contains("run") || assetPath.Contains("walk")) {
				return true;
			}
		}
		return false;
	}

	void OnPreprocessAnimation() {
		if (assetNeedProcess()) {
			var importer = assetImporter as ModelImporter;
			var list = importer.defaultClipAnimations;
			foreach (var clip in list) {
				clip.loopTime = true;
			}

			importer.clipAnimations = list;
		}
	}

	void OnPostprocessAnimation(GameObject obj, AnimationClip clip){
		if (assetNeedProcess()) {
			Debug.Log($"OnPostprocessAnimation [{assetPath}] [{obj.name}] [{clip.name}]");
			PrintAllCurves(clip);
			RemoveCurve(clip, "Hips", "m_LocalPosition.z");
			AddAnimationEndEvent(clip);
		}
	}

	void PrintAllCurves(AnimationClip clip) {
		var sb = new StringBuilder();
		sb.Append($"Clip = {clip.name}");

        foreach (var binding in AnimationUtility.GetCurveBindings(clip))
        {
			if (binding.path != "Hips") continue;

            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
			sb.Append($"binding path={binding.path} prop={binding.propertyName} type={binding.type.Name}\n");
        }

		Debug.Log(sb.ToString());
	}

	void RemoveCurve(AnimationClip clip, string path, string propName) {
		var binding = new EditorCurveBinding();
		binding.path = "Hips";
		binding.propertyName = "m_LocalPosition.z";
		binding.type = typeof(Transform);
		
		var curve = AnimationUtility.GetEditorCurve(clip, binding);
		if (curve == null) {
			Debug.LogWarning("Cannot find curve");
			return;
		}

		curve.keys = new Keyframe[0];
		AnimationUtility.SetEditorCurve(clip, binding, curve);
	}

	void AddAnimationEndEvent(AnimationClip clip) {
		var list = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(clip));

		var ev = new AnimationEvent();
		ev.time = clip.length;
		ev.functionName = "OnAnimEnd_Test";
		ev.stringParameter = clip.name;
		list.Add(ev);

		AnimationUtility.SetAnimationEvents(clip, list.ToArray());
	}
}
