Shader "MyShader/Ex007_DualFilterBlurUpScale"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
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
			float4 halfPixel;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				float4 o;
				o  = tex2D(_MainTex, i.uv + float2(-halfPixel.x * 2, 0));
				o += tex2D(_MainTex, i.uv + float2( halfPixel.x * 2, 0));
				o += tex2D(_MainTex, i.uv + float2(0, -halfPixel.y * 2));
				o += tex2D(_MainTex, i.uv + float2(0,  halfPixel.y * 2));

				o += tex2D(_MainTex, i.uv + halfPixel.xy) * 2;
				o += tex2D(_MainTex, i.uv - halfPixel.xy) * 2;
				o += tex2D(_MainTex, i.uv + halfPixel.zw) * 2;
				o += tex2D(_MainTex, i.uv - halfPixel.zw) * 2;
	
				o /= 12;
				return o;
			}
			ENDCG
		}
	}
}
