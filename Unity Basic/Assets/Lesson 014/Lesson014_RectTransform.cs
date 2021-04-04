using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

static public class Lesson014_RectTransform
{
	static void SetSizeToParent(this RectTransform rt) {
		rt.anchoredPosition = Vector2.zero;
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
	}


#if UNITY_EDITOR
	[MenuItem("CONTEXT/RectTransform/Lesson014/Set Size to Parent")]
	static void MenuItem_RectTransformSetSizeToParent() {
		foreach (var obj in Selection.gameObjects) {
			var rt = obj.GetComponent<RectTransform>();
			if (rt) {
				Undo.RecordObject(rt, "SetSizeToParent");
				rt.SetSizeToParent();
			}
		}
	}
#endif
}
