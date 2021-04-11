using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderParameters {
	// https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
	public Matrix4x4 unity_ObjectToWorld;	// Current model matrix.
	public Matrix4x4 unity_WorldToObject;	// Inverse of current world matrix.

	public Matrix4x4 UNITY_MATRIX_V;		// Current view matrix.
	public Matrix4x4 UNITY_MATRIX_P;		// Current projection matrix.
	public Matrix4x4 UNITY_MATRIX_MV;		// Current model * view matrix.
	public Matrix4x4 UNITY_MATRIX_MVP;		// Current model * view * projection matrix.

	public Matrix4x4 UNITY_MATRIX_T_MV;		// Transpose of model * view matrix.
	public Matrix4x4 UNITY_MATRIX_IT_MV;	// Inverse transpose of model * view matrix.

	public Vector3   _WorldSpaceCameraPos;
	public Vector4   _ScreenParams;			// x is the width  of the camera’s target texture in pixels, 
											// y is the height of the camera’s target texture in pixels, 
											// z is 1.0 + 1.0/width 
											// w is 1.0 + 1.0/height.

	public Vector4	_WorldSpaceLightPos0;	// Directional lights: (world space direction, 0). Other lights: (world space position, 1).
}

public struct VSInput {
	public Vector3 pos;
	public Vector3 normal;
	public Color   color;
}

public struct VSOutput {
	public Vector4 pos;
	public Vector3 worldPos;

	public Vector3 normal;
	public Color   color;
	public Vector2 _screenPos;

	static public VSOutput Lerp(in VSOutput a, in VSOutput b, float w) {
		var o = new VSOutput();
		o.pos    = Vector4.Lerp(a.pos, b.pos, w);
		o.normal = Vector3.Lerp(a.normal, b.normal, w);
		o.color  = Color.Lerp(a.color, b.color, w);
		return o;
	}
}

public class MyShader : MonoBehaviour
{
}
