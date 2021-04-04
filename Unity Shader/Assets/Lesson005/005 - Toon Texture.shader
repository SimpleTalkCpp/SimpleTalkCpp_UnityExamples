// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/005 - Toon Texture"
{
	Properties {
		toonOutlineWidth	("Toon Outline Width", Range(0,1)) = 0.04
		toonOutlineColor	("Toon Outline Color 0", Color) = (0,0,0,0) 

		toonTexture			("Toon Texture", 2D) = "white"
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
			// make fog work
			#pragma multi_compile_fog
			
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
			sampler2D toonTexture;

			float4 toonTextureShading(float3 pos, float3 normal) 
			{
				float3 N = normalize(normal);
				float3 L = normalize(MyLightPos - pos);
				float angle = dot(N,L) * 0.5 + 0.5; //-1..1 -> 0..1

				return tex2D(toonTexture, float2(0.5, angle));
			}

			float4 ps_main (v2f i) : SV_Target {
				return toonTextureShading(i.wpos, i.normal);
			}
			ENDCG
		}
	}
}
