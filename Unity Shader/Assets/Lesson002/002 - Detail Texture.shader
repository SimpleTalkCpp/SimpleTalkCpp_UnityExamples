Shader "MyShader/002 - Detail Texture"
{
	Properties {
		tex0("tex0", 2D) = "white"
		tex1("tex1", 2D) = "white"
		tex2("tex2", 2D) = "white"
		tex3("tex3", 2D) = "white"
		tex4("tex4", 2D) = "white"
		texMask("texMask", 2D) = "white"
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
			
			sampler2D texMask;

			sampler2D tex0;
			sampler2D tex1;
			sampler2D tex2;
			sampler2D tex3;
			sampler2D tex4;

			float4    tex0_ST;
			float4    tex1_ST;
			float4    tex2_ST;
			float4    tex3_ST;
			float4    tex4_ST;

			float4 detailTex(sampler2D tex, float4 st, float2 uv) {
				float2 tiling = st.xy;
				float2 offset = st.zw;
				float4 o = tex2D(tex, uv * tiling + offset);
				return o;
			}

			float4 ps_main (v2f i) : SV_Target
			{
				float4 t0 = detailTex(tex0, tex0_ST, i.uv);
				float4 t1 = detailTex(tex1, tex1_ST, i.uv);
				float4 t2 = detailTex(tex2, tex2_ST, i.uv);
				float4 t3 = detailTex(tex3, tex3_ST, i.uv);
				float4 t4 = detailTex(tex4, tex4_ST, i.uv);

				float4 mask = tex2D(texMask, i.uv);
				float parity = 1 - mask.r - mask.g - mask.b - mask.a;

				float4 o = t0 * mask.r
					     + t1 * mask.g
						 + t2 * mask.b
						 + t3 * mask.a
						 + t4 * parity;
				return  o;
			}
			ENDCG
		}
	}
}
