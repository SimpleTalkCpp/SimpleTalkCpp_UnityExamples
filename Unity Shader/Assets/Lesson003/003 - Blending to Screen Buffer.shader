Shader "MyShader/003 - Blending to Screen Buffer"
{
	Properties {
		testColor("test Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Tags { "RenderType"="Transparent" }

		Pass
		{
			Cull Off
			ZWrite Off
			//ZTest Always

			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			BlendOp Add, Add

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

			float4 testColor;

			float4 ps_main (v2f i) : SV_Target
			{
				if (testColor.a < 0.1)
					discard;

				return testColor;
			}
			ENDCG
		}
	}
}
