// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "MyShader/Ex008 - Shadow Map"
{
	Properties {
		ambientColor		("Ambient Color", Color) = (0,0,0,0)
		diffuseColor		("Diffuse Color", Color) = (1,1,1,1)
		specularColor		("Specular Color", Color) = (1,1,1,1)
		specularShininess	("Specular Shininess", Range(1,128)) = 10
		shadowBias			("shadow bias", Range(0, 0.1)) = 0.05
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{			
		//---------------
			CGPROGRAM
			#pragma vertex vs_main
			#pragma fragment ps_main
			
			#include "UnityCG.cginc"

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
				float4 shadowPos : TEXCOORD1;
			};

			float4x4 MyShadowVP;

			v2f vs_main (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.wpos = mul(UNITY_MATRIX_M, v.pos).xyz;

				o.color = v.color;
				o.uv = v.uv;
				o.normal = mul((float3x3)UNITY_MATRIX_M, v.normal); // normal has no translation, that's why mul float3x3

				float4 wpos = mul(unity_ObjectToWorld, v.pos);
				o.shadowPos = mul(MyShadowVP, wpos);
				o.shadowPos.xyz /= o.shadowPos.w;
				return o;
			}

			float4 MyLightDir;

			float4 ambientColor;
			float4 diffuseColor;
			float4 specularColor;
			float  specularShininess;

			sampler2D uvChecker;
			sampler2D MyShadowMap;
			float shadowBias;

			float4 basicLighting(float3 wpos, float3 normal)
			{
				float3 N = normalize(normal);
				float3 L = normalize(-MyLightDir.xyz);
				float3 V = normalize(wpos - _WorldSpaceCameraPos);

				float3 R = reflect(L,N);

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

			float4 shadow(v2f i) {
				float4 s = i.shadowPos;
				float3 uv = s.xyz * 0.5 + 0.5;
				float d = uv.z;

				if (true) {
					float3 N = normalize(i.normal);
					float3 L = normalize(-MyLightDir.xyz);
					float slope = tan(acos(dot(N,L)));

					d -= shadowBias * slope;
				} else {
					d -= shadowBias;
				}

				// depth checking
				if (false) {
					if (d < 0) return float4(1,0,0,1);
					if (d > 1) return float4(0,1,0,1);
					return float4(0, 0, d, 1);
				}

				//return tex2D(uvChecker, uv); // projection checking

				float m = tex2D(MyShadowMap, uv).r;
				//return float4(m,m,m,1); // shadowMap checking
				//return float4(d, m, 0, 1);

				float c = 0;
				if (d > m.r)
					return float4(c,c,c,1);
				return float4(1,1,1,1);
			}

			float4 ps_main (v2f i) : SV_Target {
				float4 s = shadow(i);
				//return s;

				float4 c = basicLighting(i.wpos, i.normal);
				return c * s;
			}
			ENDCG
		}
	}
}
