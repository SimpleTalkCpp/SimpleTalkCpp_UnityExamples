Shader "MyShader/Ex007_Blur3x3"
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

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				return o;
			}

			float intensity;

			float4 blur3x3(v2f i) {
				float2 d = intensity / _ScreenParams.xy;
				float4 c = 0;
		
				c += tex2D(_MainTex, i.uv + float2(-1,-1) * d) * 0.077847;
				c += tex2D(_MainTex, i.uv + float2( 0,-1) * d) * 0.123317;
				c += tex2D(_MainTex, i.uv + float2( 1,-1) * d) * 0.077847;

				c += tex2D(_MainTex, i.uv + float2(-1, 0) * d) * 0.123317;
				c += tex2D(_MainTex, i.uv + float2( 0, 0) * d) * 0.195346;
				c += tex2D(_MainTex, i.uv + float2( 1, 0) * d) * 0.123317;

				c += tex2D(_MainTex, i.uv + float2(-1, 1) * d) * 0.077847;
				c += tex2D(_MainTex, i.uv + float2( 0, 1) * d) * 0.123317;
				c += tex2D(_MainTex, i.uv + float2( 1, 1) * d) * 0.077847;

				c.a = 1;

				return c;
			}

			float4 frag (v2f i) : SV_Target
			{
				return blur3x3(i);
			}
			ENDCG
		}
	}
}
