Shader "Custom/SpringShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_Bend ("Bend", Range(0,1)) = 0.2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vs_main

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float _Bend;

		sampler2D	_QuatTex;
		float4		_QuatTex_ST;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float3 quatMulVector(float4 quat, float3 v) {
			float3 qv = quat.xyz;
			float3 uv  = cross(qv, v);
			float3 uuv = cross(qv, uv);
			return v + (uv * quat.w + uuv) * 2;
		}

		void vs_main (inout appdata_full v) {
			float4 pos = v.vertex;

			float2 uv = v.texcoord * _QuatTex_ST.xy + _QuatTex_ST.zw;
							
			float4 qTex  = tex2Dlod(_QuatTex, float4(uv.x, uv.y, 0, 0));
			float4 qIden = float4(0,0,0,1);
			float4 q = lerp(qIden, qTex, pos.y * _Bend);
			
			float3 delta = quatMulVector(q, float3(0, pos.y, 0));

			pos.xyz += delta;
			v.vertex = pos;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			//o.Albedo = float3(IN.uv_MainTex.x, IN.uv_MainTex.y, 0);
			//o.Albedo = tex2D(_QuatTex, IN.uv_MainTex);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
