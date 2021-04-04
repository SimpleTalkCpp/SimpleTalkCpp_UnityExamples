Shader "MyShader/Ex007_GrayScale"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		intensity("intensity", Range(0,1)) = 1
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Cull Off
		ZTest Always
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float intensity;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				return o;
			}

			float4 grayScale(float4 v) {
				float g = v.r * 0.299 + v.g * 0.587 + v.b * 0.114;
				return float4(g,g,g, v.a);
			}


			float4 frag (v2f i) : SV_Target
			{
				float4 c = tex2D(_MainTex, i.uv);
				return lerp(c, grayScale(c), intensity);
			}
			ENDCG
		}
	}
}
