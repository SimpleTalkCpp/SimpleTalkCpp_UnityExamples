Shader "MyShader/Ex011 - LinearColorSpaceBlending"
{
	Properties {
		_color0 ("Color 0", Color) = (1,0,0,1)
		_color1 ("Color 1", Color) = (0,1,0,1)
		_gamma  ("Gamma", Range(1, 2.2)) = 2.2

		[Toggle(sRGB_Accurate_Conversion)]
		sRGB_Accurate_Conversion("sRGB_Accurate_Conversion", Int) = 0
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
			
		//---------------
			CGPROGRAM
			#pragma vertex vs_main
			#pragma fragment ps_main
			#pragma shader_feature sRGB_Accurate_Conversion
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 pos		: POSITION;
				float2 uv		: TEXCOORD0;
			};

			struct v2f {
				float4 pos		: SV_POSITION;
				float2 uv		: TEXCOORD0;
			};

	
			v2f vs_main (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				return o;
			}

			float _gamma;

			float sRGB_to_Linear(float c) {
				//#if defined(sRGB_Accurate_Conversion)
				#ifdef sRGB_Accurate_Conversion
					if (c <= 0.04045)
						return c / 12.92;
					return pow((c + 0.055) / 1.055, 2.4);
				#else
					return pow(c, _gamma);
				#endif
			}

			float Linear_to_sRGB(float c) {
				#ifdef sRGB_Accurate_Conversion
					if (c <= 0.0031308)
						return c * 12.92;
					return 1.055 * pow(c, 1.0 / 2.4) - 0.055;
				#else
					return pow(c, 1 / _gamma);
				#endif
			}

			float4 sRGB_to_Linear(float4 c) { 
				return float4(
					sRGB_to_Linear(c.r), 
					sRGB_to_Linear(c.g), 
					sRGB_to_Linear(c.b), 
					c.a); 
			}
			float4 Linear_to_sRGB(float4 c) { 
				return float4(
					Linear_to_sRGB(c.r), 
					Linear_to_sRGB(c.g), 
					Linear_to_sRGB(c.b), 
					c.a); 
			}

			float4 _color0;
			float4 _color1;

			float4 ps_main (v2f i) : SV_Target {
				float4 c0 = sRGB_to_Linear(_color0);
				float4 c1 = sRGB_to_Linear(_color1);
				float weight = i.uv.x;

				float4 o = lerp(c0, c1, weight); // blending in linear color space

				o = Linear_to_sRGB(o);
				return o;
			}
			ENDCG
		}
	}
}
