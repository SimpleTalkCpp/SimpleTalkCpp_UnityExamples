Shader "MyShader/002 - ScreenSpace"
{
	Properties {
		testTex("testTex", 2D) = "white" // "black"
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vs_main
			#pragma fragment ps_main
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 pos : POSITION;
				float4 color  : COLOR;
				float2 uv : TEXCOORD0;
			};

			// varying
			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};
		
			v2f vs_main (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.color = v.color;
				o.uv = v.uv;
				return o;
			}
			
			sampler2D testTex;

			float4 ps_main (v2f i) : SV_Target
			{
				float2 uv = i.pos.xy / _ScreenParams.y;

				float4 o = tex2D(testTex, uv);
				return o;
			}
			ENDCG
		}
	}
}
