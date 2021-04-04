// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/005 - Lighting"
{
	Properties {
		ambientColor		("Ambient Color", Color) = (0,0,0,0)
		diffuseColor		("Diffuse Color", Color) = (1,1,1,1)
		specularColor		("Specular Color", Color) = (1,1,1,1)
		specularShininess	("Specular Shininess", Range(1,128)) = 10
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

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

			float4 ambientColor;
			float4 diffuseColor;
			float4 specularColor;
			float  specularShininess; 

			float4 basicLighting(float3 wpos, float3 normal)
			{
				float3 N = normalize(normal);
				float3 L = normalize(MyLightPos - wpos);
				float3 V = normalize(wpos - _WorldSpaceCameraPos);

				float3 R = reflect(L,N);
				// same as float3 R = L - dot(N,L) * 2 * N;

				float4 ambient  = ambientColor;
				float4 diffuse  = diffuseColor * max(0, dot(N,L));

				float  specularAngle = max(0, dot(R,V));
				float4 specular = specularColor * pow(specularAngle, specularShininess);

				float4 color = 0;
				color += ambient;
				color += diffuse;
				color += specular;

				return color;
			}

			float4 ps_main (v2f i) : SV_Target {
				return basicLighting(i.wpos, i.normal);
			}
			ENDCG
		}
	}
}
