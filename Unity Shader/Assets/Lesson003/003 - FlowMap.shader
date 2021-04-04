Shader "MyShader/003 - FlowMap"
{
	Properties {
		flowIntensity("flow Intensity", range(0, 0.1)) = 0.01

		testTex("testTex", 2D) = "White"
		flowMap("FlowMap", 2D) = "Black"
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
				float4 color  : COLOR;
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

			sampler2D	testTex;
			sampler2D	flowMap;
			float flowIntensity;

			float4 ps_main (v2f i) : SV_Target {
				float4 f = tex2D(flowMap, i.uv) * 2 + 1;
				float4 t = tex2D(testTex, i.uv + f.xy * flowIntensity + float2(0, _Time.y * 0.1));
				return t;
			}
			ENDCG
		}
	}
}
