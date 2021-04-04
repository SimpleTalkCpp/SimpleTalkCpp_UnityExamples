Shader "MyShader/Ex007_Blur7x7"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		intensity("intensity", Vector) = (0,0,0,0)
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

			float4 intensity;

			float4 blur7x7(v2f i) {
				float2 d = intensity.xy / _ScreenParams.xy;
				float4 c = 0;

				c += tex2D(_MainTex, i.uv + float2(-3,-3) * d) * 0.106288522;
				c += tex2D(_MainTex, i.uv + float2(-2,-2) * d) * 0.140321344;
				c += tex2D(_MainTex, i.uv + float2(-1,-1) * d) * 0.165770069;
				c += tex2D(_MainTex, i.uv + float2( 0, 0) * d) * 0.175240144;
				c += tex2D(_MainTex, i.uv + float2( 1, 1) * d) * 0.165770069;
				c += tex2D(_MainTex, i.uv + float2( 2, 2) * d) * 0.140321344;
				c += tex2D(_MainTex, i.uv + float2( 3, 3) * d) * 0.106288522;
				c.a = 0;

				return c;
			}

			float4 frag (v2f i) : SV_Target
			{
				return blur7x7(i);
			}
			ENDCG
		}
	}
}
