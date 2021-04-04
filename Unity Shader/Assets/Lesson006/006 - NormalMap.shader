Shader "MyShader/006 - NormalMap"
{
	Properties
	{
		ambientColor		("Ambient Color", Color) = (0,0,0,0)
		diffuseColor		("Diffuse Color", Color) = (1,1,1,1)
		specularColor		("Specular Color", Color) = (1,1,1,1)
		specularShininess	("Specular Shininess", Range(0,128)) = 10

		diffuseMap			("Diffuse Map", 2D) = "white"
		specularMap			("Specular Map", 2D) = "white"
		normalMap			("Normal Map", 2D) = "white"

		uvRepeat			("UV Repeat", Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
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
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 pos : SV_POSITION;
				float4 color : COLOR;

				float3 worldSpaceNormal   : TEXCOORD5;
				float3 worldSpaceTangent  : TEXCOORD6;
				float3 worldSpacePos	  : TEXCOORD7;
			};

			float4 MyLightPos;

			float4 ambientColor;
			float4 diffuseColor;
			float4 specularColor;
			float  specularShininess;

			sampler2D diffuseMap;
			sampler2D specularMap;
			sampler2D normalMap;

			float4 uvRepeat;

			float4 basicLighting(float3 pos, float3 normal, float3 tangent, float2 uv)
			{
				float3 N = normalize(normal);
				float3 L = normalize(MyLightPos - pos);
				float3 V = normalize(pos - _WorldSpaceCameraPos);

				float3 T = normalize(tangent);
				float3x3 TBN = float3x3(T, cross(N, T), N);
				//TBN = transpose(TBN);

				//normal map
				N = tex2D(normalMap, uv).xyz * 2 - 1; // 0..1 -> -1..1
				N.y = -N.y;

				N = mul(TBN, N); // tangent space -> world space

				float3 R = reflect(L,N);

				float4 ambient  = ambientColor;
				float4 diffuse  = diffuseColor * tex2D(diffuseMap, uv) * dot(N,L);
				float  specularAngle = max(0, dot(R,V));				
				float4 specular = specularColor * tex2D(specularMap, uv) * pow(specularAngle, specularShininess);

				float4 color = 0;
				color += ambient;
				color += diffuse;
				color += specular;

				return color;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv * uvRepeat.xy;
				o.normal = v.normal;
				o.color = 1;

				o.worldSpacePos		 = mul(UNITY_MATRIX_M,     v.pos).xyz;
				o.worldSpaceNormal   = mul((float3x3)UNITY_MATRIX_M, v.normal);  // normal has no translation, that's why mul float3x3
				o.worldSpaceTangent  = mul((float3x3)UNITY_MATRIX_M, v.tangent.xyz);

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 o = i.color;
				o = basicLighting(i.worldSpacePos, i.worldSpaceNormal, i.worldSpaceTangent, i.uv);
				return o;
			}
			ENDCG
		}
	}
}
