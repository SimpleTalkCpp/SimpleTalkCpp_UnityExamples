using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyShader
{
	public enum Cull {
		None,
		Front,
		Back
	};

	public Cull cull = Cull.Back;

	public enum DepthTestFunc {
		Always,
		Less,
		Greater
	};

	public DepthTestFunc depthTestFunc = DepthTestFunc.Less;
	public bool depthWrite = true;


	public class Parameters {
		// https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
		public Matrix4x4 unity_ObjectToWorld;	// Current model matrix.

		public Matrix4x4 UNITY_MATRIX_V;		// Current view matrix.
		public Matrix4x4 UNITY_MATRIX_P;		// Current projection matrix.
		public Matrix4x4 UNITY_MATRIX_MV;		// Current model * view matrix.
		public Matrix4x4 UNITY_MATRIX_MVP;		// Current model * view * projection matrix.

		public Vector4	_WorldSpaceLightPos0;	// Directional lights: (world space direction, 0). Other lights: (world space position, 1).

		public Color materialColor;
	}

	public struct VsInput {
		public Vector4 vertex;
		public Vector3 normal;
		public Color   color;
	}

	public struct VsOutput {
		public Vector4 vertex;
		public Vector3 worldPos;

		public Vector3 normal;
		public Color   color;
		public Vector2 _screenPos;

		static public VsOutput Lerp(in VsOutput a, in VsOutput b, float w) {
			var o = new VsOutput();
			o.vertex   = Vector4.Lerp(a.vertex,   b.vertex,   w);
			o.worldPos = Vector4.Lerp(a.worldPos, b.worldPos, w);
			o.normal   = Vector3.Lerp(a.normal,   b.normal,   w);
			o.color    =   Color.Lerp(a.color,    b.color,    w);
			return o;
		}
	}

	public VsOutput vertexShader(in VsInput input, in Parameters shaderParams) {
		var o = new VsOutput();
		var pos4 = new Vector4(	input.vertex.x,
								input.vertex.y,
								input.vertex.z,
								1);
		
		o.vertex      = shaderParams.UNITY_MATRIX_MVP * pos4;
		o.worldPos = shaderParams.unity_ObjectToWorld.MultiplyPoint(input.vertex);
		o.normal   = shaderParams.unity_ObjectToWorld.MultiplyVector(input.normal);
		o.color    = input.color;
		return o;
	}

	public Color pixelShader(VsOutput input, in Parameters shaderParams) {
		var light = Vector3.Dot(input.normal, shaderParams._WorldSpaceLightPos0);
		light = Mathf.Max(0, light);

		var o = shaderParams.materialColor * input.color * light;
		o.a = 1;

		return o;
	}
}
