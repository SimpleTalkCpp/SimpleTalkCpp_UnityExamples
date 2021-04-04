Shader "MyShader/002 - Tex Blending"
{
	Properties {
		testFloat("testFloat", range(0, 0.1)) = 0
		texWeight("texWeight", range(-1, 1)) = 0

		testTex("testTex", 2D) = "white"
		testTex2("testTex2", 2D) = "white"
		testMask("testMask", 2D) = "white"
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
			sampler2D	testTex2;
			sampler2D	testMask;
			float		texWeight;

			float		testFloat;

			float4 grayScale(float4 v) {
				float g = v.r * 0.299 + v.g * 0.587 + v.b * 0.114;
				return float4(g,g,g, v.a);
			}

			float4 ps_main (v2f i) : SV_Target
			{
				float4 a;
				a.r = tex2D(testTex, i.uv - float2(testFloat, 0)).r;
				a.g = tex2D(testTex, i.uv).g;
				a.b = tex2D(testTex, i.uv + float2(testFloat, 0)).b;
				a.a = 1;

				float4 b = tex2D(testTex2, i.uv);

				float4 mask = tex2D(testMask, i.uv);

				float w = mask + texWeight;

				//if (w < 0) w = 0;
				//if (w > 1) w = 1;
				w = clamp(w, 0, 1);

				//float4 o = a * (1-w) + b * w;
				float4 o = lerp(a, b, w);

				//o = grayScale(o);
				return o;
			}
			ENDCG
		}
	}
}
