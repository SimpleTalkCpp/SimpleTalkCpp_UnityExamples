// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/005 - Toon"
{
	Properties {
		toonAngle0			("Toon Angle 0", Range(-1,1)) = -0.6
		toonColor0			("Toon Color 0", Color) = (0,0,1,0) 

		toonAngle1			("Toon Angle 1", Range(-1,1)) = 0.5
		toonColor1			("Toon Color 1", Color) = (0,1,0,0) 

		toonAngle2			("Toon Angle 2", Range(-1,1)) = 0.9
		toonColor2			("Toon Color 2", Color) = (1,0,0,0) 

		toonColor3			("Toon Color 3", Color) = (0.2, 0.2, 0.2, 0.2) 

		toonOutlineWidth	("Toon Outline Width", Range(0,1)) = 0.04
		toonOutlineColor	("Toon Outline Color 0", Color) = (0,0,0,0) 
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		// outline pass
		Pass 
		{
			Cull front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};	

			float toonOutlineWidth;
			float4 toonOutlineColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);

				float3 N = mul ((float3x3)UNITY_MATRIX_MV, v.normal);
                float2 offset = mul((float2x2)UNITY_MATRIX_P, N.xy);
                o.pos.xy += offset  * toonOutlineWidth;

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				return toonOutlineColor;
			}
			ENDCG
		}

		// color pass
		Pass
		{
			Cull Off
			
		//---------------
			CGPROGRAM
			#pragma vertex vs_main
			#pragma fragment ps_main
			
			#include "UnityCG.cginc"
			#define M_PI 3.1415926535897932384626433832795

			struct appdata {
				float4 pos		: POSITION;
				float4 color	: COLOR;
				float2 uv		: TEXCOORD0;
				float3 normal	: NORMAL;
			};

			struct v2f {
				float4 pos		: SV_POSITION;
				float3 wpos		: TEXCOORD7;
				float4 color	: COLOR;
				float2 uv		: TEXCOORD0;
				float3 normal	: NORMAL;
			};
	
			v2f vs_main (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.wpos = mul(UNITY_MATRIX_M, v.pos).xyz;

				o.color = v.color;
				o.uv = v.uv;
				o.normal = mul((float3x3)UNITY_MATRIX_M, v.normal); // normal has no translation, that's why mul float3x3
				return o;
			}

			float4 MyLightPos;

			float  toonAngle0;
			float4 toonColor0;

			float  toonAngle1;
			float4 toonColor1;

			float  toonAngle2;
			float4 toonColor2;

			float4 toonColor3;

			float4 toonShading(float3 pos, float3 normal) 
			{
				float3 N = normalize(normal);
				float3 L = normalize(MyLightPos - pos);

				float angle = max(0, dot(N,L));

				if (angle > toonAngle2) 
					return toonColor2;
				
				if (angle > toonAngle1) 
					return toonColor1;
				
				if (angle > toonAngle0) 
					return toonColor0;

				return toonColor3;
			}

			float4 ps_main (v2f i) : SV_Target {
				return toonShading(i.wpos, i.normal);
			}
			ENDCG
		}
	}
}
