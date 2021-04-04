Shader "MyShader/Ex007_DualFilterBlurDownScale"
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
				float4 o = tex2D(_MainTex, i.uv) * 4;
				o += tex2D(_MainTex, i.uv - halfPixel.xy);
				o += tex2D(_MainTex, i.uv + halfPixel.xy);
				o += tex2D(_MainTex, i.uv - halfPixel.zw);
				o += tex2D(_MainTex, i.uv + halfPixel.zw);
				o /= 8;
				return o;
			}
			ENDCG
		}
	}
}
