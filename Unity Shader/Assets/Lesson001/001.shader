Shader "MyShader/001"
{
	Properties {
		testFloat("testFloat", range(0,360)) = 1
		testColor("testColor", color) = (1,1,1,1)
		testVec("testVec", vector) = (0,0,0,0)
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
			
			// uniform
			float testFloat;
			float my_time;
			float4 testColor;
			float4 testVec;

			sampler2D testTex;

			float4 ps_main (v2f i) : SV_Target
			{
				float x = i.uv.x - 0.5;
				float y = i.uv.y - 0.5;

//				float r = testFloat / 360.0 * 2 * 3.1416;
				float r = radians(testFloat);

				float a = cos(r);
				float b = -sin(r);
				float c = sin(r);
				float d = cos(r);

				float nx = a*x + b*y + 0.5;
				float ny = c*x + d*y + 0.5;

				float2 uv = float2(nx, ny);

				//--- calc by matrix
				/*
				float2x2 rotateMat = float2x2( cos(r), -sin(r),
											   sin(r),  cos(r));
				float2 uv = mul(rotateMat, i.uv - 0.5) + 0.5;
				*/
				//----------
				float4 o = tex2D(testTex, uv);
				return o;
			}
			ENDCG
		}
	}
}
