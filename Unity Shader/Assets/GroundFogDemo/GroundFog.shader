Shader "Custom/GroundFog" {
	Properties {
		_Bias("Bias",   Range(-1,1)) = 0
		_Scale("Scale", Range(0.001, 2)) = 1
	}
	SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			uniform sampler2D _CameraDepthTexture; 

			float _Bias;
            float _Scale;

            struct v2f {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD0;
                float depth : TEXCOORD1;
            };

            v2f vert (appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.projPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.depth);
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r;
                depth = LinearEyeDepth(depth);
                depth -= i.depth + _Bias;
                depth *= _Scale;
                depth = saturate(depth);
                depth *= depth;
                                
                return float4(0, .5, 0, depth);
            }
            ENDCG
		}
	}
	FallBack "Diffuse"
}
