Shader "MyShader/Ex007_ScreenSpaceNormal"
{
	Properties
	{
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
				float3 normal: NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float intensity;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.normal = mul(UNITY_MATRIX_IT_MV, v.normal); //screen space normal
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				float3 nl = normalize(i.normal) * 0.5 + (128.0 / 255.0);
				return float4(nl, 1);
			}
			ENDCG
		}
	}
}
