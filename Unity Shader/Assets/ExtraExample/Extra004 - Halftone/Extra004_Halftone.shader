Shader "Unlit/Extra004_Halftone"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Halftone("Halftone", 2D) = "white" {}
        _Test("Test", float) = 0
    }
    SubShader
    {
        Tags { 
            "Queue"="Transparent"
            "RenderType"="Transparent" 
           }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _Halftone;
            float4 _Halftone_ST;

            float _Test;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                
                float h = tex2D(_Halftone, i.uv * _Halftone_ST.xy + _Halftone_ST.zw).r;

                h = (h + col.r) < _Test ? 0 : 1;

                col.a *= clamp(h, 0, 1);

                return col;
            }
            ENDCG
        }
    }
}
