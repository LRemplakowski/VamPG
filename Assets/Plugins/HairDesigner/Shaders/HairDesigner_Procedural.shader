Shader "HairDesigner/Procedural"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_Smoothness("Smoothness", Range(0,1.0)) = 0.5
		_AO("AO", Range(0,1.0)) = 0.5
		_StartColor("Start Color", Color) = (0.3, 0.4, 0.7, 1.0)
		_EndColor("End Color", Color) = (0.7, 0.4, 0.3, 1.0)
		_ColorBalance("Color balance", Range(0.0, 1.0)) = 1
		_Stripes("Stripes", Range(1.0, 100.0)) = 1
		_HairDensity("Hair Density", Range(1,40)) = 20
		_HairSize("Hair Size", Range(0, 1)) = .4
		_HairWidth("Hair Width", Range(0, 1)) = 1
		_Wave("Wave", Range(0,20)) = 0
		_WavePower("Wave Power", Range(.01,.2)) = 0
		_Chaos("Chaos", Range(0.0, 1.0)) = 1
		_Cut("Cut", Range(0.0, 1.0)) = 1
		_RimColor("Rim Color", Color) = (0.2, 0.2, 0.2, 0.0)
		_RimPower("Rim Power", Range(0.5, 8.0)) = 2.5
		_NormalFactor( "_Normal",Range(-1.0, 1.0)) = 0
		_Emission("Emission", Range(0, 1)) = 1
		_NormalMode("Normal mode", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True"  }		
		//Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200
		Cull off
		ZWrite On
		
			CGPROGRAM
//#if SHADER_TARGET < 30
//			#pragma surface surf StandardSpecular vertex:vert alpha:fade
//#else
			#pragma surface surf StandardSpecular vertex:vert addshadow fullforwardshadows
//#endif

			#pragma target 3.0
			//#pragma multi_compile_fog			
			#include "HairDesigner.cginc"
						
			struct Input
			{
				float2 uv_MainTex;
				//float3 worldNormal;
				float3 viewDir;
				float4 color;				
				//INTERNAL_DATA
			};

			sampler2D _MainTex;
			float _Smoothness;
			float _AO;
			float4 _StartColor;
			float4 _EndColor;
			float _ColorSlider;
			float4 _RimColor;
			float _RimPower;
			float _HairDensity;
			float _Wave;
			float _WavePower;
			float _Chaos;
			float _Cut;
			float _Stripes;
			float _Emission;
			float _ColorBalance;
			float _NormalMode;

			float _HairSize = 1;
			float _HairWidth = 1;
			float _UVX = 1;

			/*
			float RND(float seed)
			{
				return abs(frac(cos(seed*seed*pi)* 43758.5453));
			}*/

			void StrandValueCurv(float seed, float nbHair, float2 uv, float2 wave, float chaos, float scale, out float strandFactor, out float3 strandNormal, out float strandColor)
			{
				half rnd = KHD_editor == 0 ? nbHair + seed : nbHair + KHD_scale*KHD_strandLength;
				rnd *= .001;//fix some weird rendering issue
				uv.x *= _UVX;
				int hairCount = ceil(nbHair);
				uv.x = uv.x + sin(uv.y*pi * wave.x*uv.y) * wave.y * (1 - sqrt(uv.y));//wave
				half res = -1;
				half3 n = half3(0, 0, 0);
				//uv.x = lerp((uv.x - 0.5) * 10 + 0.5, uv.x, sqrt(sqrt(uv.y)));

				for (int i = 1; i <= hairCount; ++i)
				{
					rnd = RND(rnd + i);
					half2 SP = half2(rnd*.8 + .1, 0);
					rnd = RND(rnd);
					half2 EP = half2(rnd*.8 + .1, 0);
					EP = lerp(SP, EP, chaos);
					rnd = RND(rnd);
					half2 ST = half2(-.1*chaos, 0);
					half2 ET = half2(.1*chaos, 0);
					half2 uv2 = Curv2D(uv.y, uv, SP, ST, EP, ET);
					//hair shape
					//half shapeMin = .11;
					//half shapeMin = .0950;
					half shapeMin = lerp(.089, .11, _HairWidth*(1 - uv.y*uv.y));
					//half shapeMin = lerp(.095, .11, _HairWidth);
					rnd = RND(rnd);
					half f = (1 - abs(sin(uv2.x * pi)))*lerp(shapeMin, 0.09, uv.y*uv.y)*lerp(0.91, 1.1, _HairSize) * (9 + rnd*scale);

					//root
					f = uv.y < .04 ? f * (.9 + .1 *(uv.y * 25)) : f;

					half3 normal = lerp(half3(1, 0, _NormalFactor), half3(0, 1, _NormalFactor), sin(uv2.x * pi) * 10 * .5 + .5);
					res = max(res, f);
					if (res == f)
						n = normal;


					strandColor = max(strandColor, lerp(f*uv.y * 2, f*uv.y, _HairSize));
				}

				strandFactor = res>1 ? res : 0;
				strandFactor = uv.y > .98 ? 0 : strandFactor;
				strandNormal = normalize(n);
			}



			void vert (inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				HairDesigner(v);
				o.color = v.color;
			}
			
			void surf(Input IN, inout SurfaceOutputStandardSpecular o)
			{
				float strandFactor = 0;
				float strandColor = 0;
				float3 n = float3(0,0,0);
				StrandValueCurv(IN.color.a, _HairDensity, IN.uv_MainTex, float2(_Wave, _WavePower), _Chaos, 1, strandFactor, n, strandColor);
				
				clip(strandFactor-.0001);
				clip((1-IN.uv_MainTex.y)- _Cut);

				o.Albedo = lerp(_StartColor,_EndColor, _ColorBalance * 2.0 * IN.uv_MainTex.y * (abs(cos(IN.uv_MainTex.y*pi*_Stripes))));
				//o.Albedo = lerp(_StartColor, _EndColor, strandColor);
				
				//long hair
				n = lerp( n, lerp( -n,n,dot(IN.viewDir, n)), _NormalMode);
				//short hair
				o.Normal = n;


				//o.Smoothness = _Smoothness * strandFactor;
				o.Occlusion = 1 - (1 - IN.uv_MainTex.y*strandColor) * strandFactor * _AO* _AO;				
				half rim = 1.0 - max(saturate(dot(normalize(IN.viewDir), n)), saturate(dot(normalize(IN.viewDir), -n))) ;
				o.Specular = clamp(_RimColor.rgb * min(pow(rim, _RimPower), 0.9), half3 (0, 0, 0), half3 (1, 1, 1));
				o.Smoothness = _Smoothness * strandFactor;
				o.Alpha = strandFactor - .1;
				o.Emission = o.Albedo * _Emission;
			}


			ENDCG
		
	}
	//FallBack "Diffuse"
	
}
