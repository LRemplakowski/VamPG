// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "HairDesigner/Standard" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Specular("Specular", Range(0,1)) = 0.0
		_Cutout("Cutout", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" }
		LOD 200
		Cull Off
		ZWrite On
		//Lighting On

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular vertex:vert fullforwardshadows addshadow 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "HairDesigner.cginc"
		#include "UnityPBSLighting.cginc"

		

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float4 color;
		};


		sampler2D _MainTex;
		half _Glossiness;
		half _Specular;
		fixed4 _Color;
		half _Cutout;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)					

		

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			HairDesigner(v);
			o.color = v.color;
		}

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			clip(c.a-_Cutout);
			
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Specular = _Specular;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
