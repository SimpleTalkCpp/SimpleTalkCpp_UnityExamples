// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "MyShader/Ex009 - QuatShaderDemo"
{
	Properties {
		ambientColor		("Ambient Color", Color) = (0,0,0,0)
		diffuseColor		("Diffuse Color", Color) = (1,1,1,1)
		specularColor		("Specular Color", Color) = (1,1,1,1)
		specularShininess	("Specular Shininess", Range(1,128)) = 10
		_QuatTex("Quat Tex", 2D) = "black"
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

			struct appdata
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 wpos : TEXCOORD7;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			sampler2D	_QuatTex;
			float4		_QuatTex_ST;

			float3 quatMulVector(float4 quat, float3 v) {
				float3 qv = quat.xyz;
				float3 uv  = cross(qv, v);
				float3 uuv = cross(qv, uv);
				return v + (uv * quat.w + uuv) * 2;
			}

			v2f vs_main (appdata v)
			{
				v2f o;
				float3 wpos = mul(unity_ObjectToWorld, v.pos).xyz;

				float2 uv = 1 - (v.uv * _QuatTex_ST.xy + _QuatTex_ST.zw);
								
				float4 qTex  = tex2Dlod(_QuatTex, float4(uv.x, uv.y, 0, 0));
				float4 qIden = float4(0,0,0,1);
				float4 q = lerp(qIden, qTex, wpos.y);

				wpos = quatMulVector(q, wpos);

				o.pos = mul(UNITY_MATRIX_VP, float4(wpos,1));

				o.uv = v.uv;
				o.normal = v.normal;
				return o;
			}

			float4 ambientColor;
			float4 diffuseColor;
			float4 specularColor;
			float  specularShininess;
			float4 MyLightDir;

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

			float4 ps_main (v2f i) : SV_Target {
				//return tex2D(_QuatTex, 1 - i.uv);

				float4 c = basicLighting(i.wpos, i.normal);
				return c;
			}
			ENDCG
		}
	}
}
