Shader "MyShader/Ex007_Bloom"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		blurTex ("BlurTex", 2D) = "black" {}
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

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			sampler2D blurTex;
			float intensity;
			float dofStart;
			float dofRange;

			float4 frag (v2f i) : SV_Target
			{
			#if UNITY_UV_STARTS_AT_TOP
				// DirectX
				float2 uv = float2(i.uv.x, 1 - i.uv.y);
			#else
				// OpenGL
				float2 uv = i.uv;
			#endif

				float4 blur = tex2D(blurTex, uv);
				float4 c = tex2D(_MainTex, i.uv);

				//soft lens
				//return lerp(c, blur, intensity);

				return max(c, blur * intensity);
			}
			ENDCG
		}
	}
}
