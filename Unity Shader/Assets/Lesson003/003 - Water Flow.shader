Shader "MyShader/003 - Water Flow"
{
	Properties {
		flowIntensity("flow Intensity", range(-0.2, 0.2)) = 0.05

		duration("duration", range(0,5)) = 3

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
			float duration;
			float flowIntensity;

			float4 getLayer(float2 uv, float t) 
			{
				float4 f = tex2D(flowMap, uv) * 2 - 1;
				return tex2D(testTex, uv - f.xy * t * flowIntensity);
			}

			float4 ps_main (v2f i) : SV_Target {
				float t = _Time.y / duration;
				float t0 =  t    % 2;
				float t1 = (t+1) % 2;

				float w  = abs(t % 2 - 1);

				float4 c0 = getLayer(i.uv, t0);
				float4 c1 = getLayer(i.uv, t1);

//				c0 = float4(1,0,0,1);
//				c1 = float4(0,0,1,1);

				return lerp(c0, c1, w);
			}
			ENDCG
		}
	}
}
