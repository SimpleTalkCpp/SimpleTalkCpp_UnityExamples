// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/TestShader"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {	"RenderType"="Opaque" }

		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct VsInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			struct VsOutput
			{
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD7;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			float4 _Color;

			VsOutput vert (VsInput input)
			{
				VsOutput o;
				o.vertex   = UnityObjectToClipPos(input.vertex);
				o.worldPos = mul(unity_ObjectToWorld, input.vertex);
				o.normal   = mul((float3x3)unity_ObjectToWorld, input.normal);
				o.color    = input.color;
				return o;
			}

			float4 frag (VsOutput input) : SV_Target
			{
				float light = dot(input.normal, _WorldSpaceLightPos0); // _WorldSpaceLightPos0 updated by TestShader.cs
				light = max(0, light);

				float4 o = _Color * input.color * light;
				o.a = 1;

				return o;
			}
			ENDCG
		}
	}
}
