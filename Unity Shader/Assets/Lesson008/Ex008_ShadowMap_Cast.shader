// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/Ex008 - ShadowMap Cast"
{
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		//Cull Front
		Pass
		{
			CGPROGRAM
			#pragma vertex vs_main
			#pragma fragment ps_main
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 pos		: POSITION;
			};

			struct v2f {
				float4 pos		: SV_POSITION;
				float depth		: TEXCOORD0;
			};
	
			float4x4  MyShadowVP;

			v2f vs_main (appdata v) {
				v2f o;

			#if true
				float4 wpos = mul(unity_ObjectToWorld, v.pos);
				o.pos = mul(MyShadowVP, wpos);
				float d = o.pos.z / o.pos.w;
				d = d * 0.5 + 0.5;
			#else
				o.pos = UnityObjectToClipPos(v.pos);				
				float d = o.pos.z / o.pos.w;
				if (UNITY_NEAR_CLIP_VALUE == -1) {
					d = d * 0.5 + 0.5;
				}
				#if UNITY_REVERSED_Z
					d = 1 - d;
				#endif
			#endif

				o.depth = d;
				return o;
			}

			float4 ps_main (v2f i) : SV_Target {
				return float4(i.depth, 0,0,1);
			}
			ENDCG
		}
	}
}
