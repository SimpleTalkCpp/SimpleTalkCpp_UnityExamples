Shader "MyShader/006 - MatCap"
{
	Properties {
		matCapTex("MatCap Texture", 2D) = "black"
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
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;

				float3 viewDir :  TEXCOORD4;
				float3 viewNormal : TEXCOORD5;
			};

			sampler2D matCapTex;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				o.normal = v.normal;

				o.viewDir  = normalize( mul( UNITY_MATRIX_MV, v.pos ) );
				o.viewNormal = mul((float3x3)UNITY_MATRIX_MV, v.normal );

				return o;
			}
			float4 matCap(float3 viewNormal, float3 viewDir)
			{
				float3 N = normalize(viewNormal);
				float3 V = normalize(viewDir);
				
				N -= V * dot(V,N);
				float2 uv = N.xy * 0.5 * 0.99 + 0.5;

				return tex2D(matCapTex,  uv);
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 o = 0;
				o += matCap(i.viewNormal, i.viewDir);

				return o;
			}
			ENDCG
		}
	}
}
