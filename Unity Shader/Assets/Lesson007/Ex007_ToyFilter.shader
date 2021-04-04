Shader "MyShader/Ex007_ToyFilter"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		blurTex ("BlurTex", 2D) = "black" {}
		
		dir_offset("direction & offset", Vector) = (0,0,0,0)
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
			float4 dir_offset;

			float4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;

				float4 blur = tex2D(blurTex, uv);
				float4 c = tex2D(_MainTex, i.uv);

				float2 dir = dir_offset.xy;
				float offset = dir_offset.z;

				float s = dot(i.uv * 2 - 1 + float2(0, offset), dir);
				s = clamp(abs(s), 0, 1);
				//return float4(s,0,0,1);

				return lerp(c, blur, s);
			}
			ENDCG
		}
	}
}
