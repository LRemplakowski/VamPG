Shader "HairDesigner/Fire"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NbFlame("Details", Range(1.0, 50.0)) = 2.0
		_AlphaTop("AlphaTop", Range(0.0, 1.0)) = 1.0
		_AlphaBottom("AlphaBottom", Range(0.0, 1.0)) = 1.0
		_Power("Power", Range(0.0, 1.0)) = 1.0
		_Amplitude("Amplitude", Range(0.0, 10.0)) = 1.0
		_Period("Period", Range(0.0, 10.0)) = 1.0
		_Color1("Color1", Color) = (0.3, 0.4, 0.7, 1.0)
		_Color2("Color2", Color) = (0.3, 0.4, 0.7, 1.0)
		_ColorWeight("Color Weight", Range(0.0, 1.0)) = 0.5

			//_Refraction("Refraction", Range(0.00, 100.0)) = 1.0
			//_DistortTex("Distord", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Overlay" }		
		LOD 100
		Cull off
		AlphaToMask On
		ZWrite Off		
		
		
		//----------------------------------------------
		//TEST Distorsion
		/*

		GrabPass
		{

		}


		CGPROGRAM
#pragma exclude_renderers gles

#pragma surface surf NoLighting
#pragma vertex vert
#include "HairDesigner.cginc"
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}

	sampler2D _GrabTexture : register(s0);
	sampler2D _DistortTex : register(s2);
	float _Refraction;
	float _NbFlame;

	float4 _GrabTexture_TexelSize;

	struct Input {
		float2 uv_MainTex;
		float2 uv_DistortTex;
		float3 color;
		float3 worldRefl;
		float4 screenPos;
		INTERNAL_DATA
	};

	void vert(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);
		HairDesigner(v);
		v.vertex.xyz += float3(sin(cos(_Time.z*v.texcoord.y + 2)), cos(sin(_Time.z*v.texcoord.y + 3)), cos(_Time.z))*v.texcoord.y*v.texcoord.y*v.texcoord.y*0.005;
		o.color = v.color;
	}

	void surf(Input IN, inout SurfaceOutput o)
	{
		float2 uv = IN.uv_MainTex;
		uv.x += (sin(uv.x / 10.0 - _Time.y*10.0 + uv.y*10.0 * (sin(uv.x*20.0 + _Time.y * 10 * .2)*.3 + 1.0)) + 1.0)*.03;
		float t = uv.x * 4.0;

		//shape
		float f = abs(sin(t*pi*.25*(_NbFlame)))*10.0;
		f *= (1 - IN.uv_MainTex.y) * IN.uv_MainTex.y;//alpha Y
		f *= (1 - IN.uv_MainTex.x) * IN.uv_MainTex.x;//alpha X	
		float l = length(IN.uv_MainTex.xy - float2(.5, .5)) * 2;
		f *= f*f*f * 10;


		float3 distort = tex2D(_DistortTex, IN.uv_DistortTex) * IN.color.rgb;
		float2 offset = distort * _Refraction * f * _GrabTexture_TexelSize.xy;
		IN.screenPos.xy = offset * IN.screenPos.z + IN.screenPos.xy;
		float4 refrColor = tex2Dproj(_GrabTexture, IN.screenPos);



		o.Alpha = refrColor.a;
		o.Emission = refrColor.rgb;
	}
	ENDCG
	*/

		//------------------------------------------------------------------
		


		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf NoLighting vertex:vert decal:add 
		#pragma multi_compile_fog			
		#include "HairDesigner.cginc"		
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}		
			

			sampler2D _MainTex;
			float _NbFlame;
			float _AlphaTop;
			float _AlphaBottom;
			float4 _Color1;
			float4 _Color2;
			float _ColorWeight;
			float _Power;
			float _Amplitude;
			float _Period;

			struct Input
			{
				float2 uv_MainTex;
				float3 viewDir;
				float4 color;
				INTERNAL_DATA
			};


			void vert (inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				HairDesigner(v);
				v.vertex.xyz += 
					float3(
						sin(_Period*(_Time.z +2 * v.color.a))+cos(_Period*((_Time.z + 5 * v.color.a))),
						cos(_Period*((_Time.z +3 * v.color.a)))+ sin(_Period*((_Time.z + 5 * v.color.a))),
						cos(_Period*_Time.z)
						)*v.texcoord.y*v.texcoord.y*v.texcoord.y*0.005 * _Amplitude;
				o.color = v.color;				
			}
			
											

			void surf(Input IN, inout SurfaceOutput o)
			{
				float2 uv = IN.uv_MainTex;
				float time = _Time.y*_Period;
				//warp
				uv.x += (sin(uv.x / 10.0 - time*10.0 + uv.y*10.0 * (sin(uv.x*20.0 + time *10*.2)*.3 + 1.0)) + 1.0)*.03;
				float t = uv.x * 4.0;

				//shape
				float f = abs(sin(t*pi*.25*(_NbFlame)))*10.0;
				f *= (1-IN.uv_MainTex.y) * IN.uv_MainTex.y;//alpha Y
				f *= (1-IN.uv_MainTex.x) * IN.uv_MainTex.x;//alpha X	
				float l = length(IN.uv_MainTex.xy - float2(.5, .5)) * 2;
				f *= f*f*f*10;

				float3 col = lerp(_Color1, _Color2, f*uv.y*10* _ColorWeight)*f;;
								
				//col *= lerp(_AlphaBottom, _AlphaTop, IN.uv_MainTex.y)*_Power;
				float top = IN.uv_MainTex.y;
				float b = (1-IN.uv_MainTex.y);
				col *= _AlphaBottom*b*b*b + _AlphaTop*top*top*top;
				col *= 10 * _Power;

				o.Albedo = col;
				o.Emission = col;
			}


			ENDCG
		
	}

}
