using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
	static public void Label(in Vector3 pos, string text) {
#if UNITY_EDITOR
		UnityEditor.Handles.Label(pos, text);
#endif
	}
}
