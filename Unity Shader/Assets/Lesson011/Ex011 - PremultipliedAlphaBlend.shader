Shader "MyShader/Ex011 - PremultipliedAlphaBlend"
{
	Properties {
		_MainTex("Main Tex", 2D) = "Black"
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		Pass
		{
			Blend One OneMinusSrcAlpha // Premultiplied transparency
			
		//---------------
			CGPROGRAM
			#pragma vertex vs_main
			#pragma fragment ps_main
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 pos		: POSITION;
				float2 uv		: TEXCOORD0;
			};

			struct v2f {
				float4 pos		: SV_POSITION;
				float2 uv		: TEXCOORD0;
			};

	
			v2f vs_main (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				return o;
			}

			sampler2D	_MainTex;

			float4 ps_main (v2f i) : SV_Target {
				float4 o = tex2D(_MainTex, i.uv);
				return o;
			}
			ENDCG
		}
	}
}
