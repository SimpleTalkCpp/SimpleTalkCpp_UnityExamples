using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson005_PropAttr : MonoBehaviour
{
    [Range(0, 10)] 
    public float range;

    [Min(10)]
    public float min;

    [HideInInspector]
    public float hide;

    [Space(50)]
    public float spaceAbove;

    [Header("Header Test")]
    public float headerAbove;

    [ColorUsage(false)]
    public Color color_NoAlpha;

    [ColorUsage(true, true)]
    public Color color_HDR;

    [TextArea(8, 20)]
    public string textArea = "Hello\n123";

    public AnimationCurve curve;

    [Range(0,1)]
    public float curveTest;

	private void OnDrawGizmos()
	{
#if UNITY_EDITOR
		UnityEditor.Handles.Label(transform.position + Vector3.up, $"  [curve={curveTest}, {curve.Evaluate(curveTest)}]");
#endif
	}
}
