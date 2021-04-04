Shader "MyShader/Ex007_RenderToTexture"
{
	Properties
	{
		intensity("intensity", Range(0,300)) = 0
		ratio("ratio", float) = 1
		_MainTex("MainTex", 2D) = "black"
		inputTex("InputTex", 2D) = "black"
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
			sampler2D inputTex;
			float intensity;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				return o;
			}

			float ratio; // _ScreenParams doesn't avaliable in OnRenderImage()

			float4 frag (v2f i) : SV_Target
			{
			#if UNITY_UV_STARTS_AT_TOP
				// DirectX
				float2 uv = float2(i.uv.x, 1 - i.uv.y);
			#else
				// OpenGL
				float2 uv = i.uv;
			#endif

				float4 t = tex2D(inputTex, uv);

				float2 nl = (t.xy * 255 - 128) / 255; // instead of '* 2 - 1'

			#if UNITY_UV_STARTS_AT_TOP
				nl.y = -nl.y;
			#endif

				nl /= _ScreenParams.xy;

				float4 c = tex2D(_MainTex, i.uv + nl * intensity);
				return c;
			}
			ENDCG
		}
	}
}
