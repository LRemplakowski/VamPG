Shader "HairDesigner/Atlas"
{
	Properties
	{
		_MainTex ("Diffuse", 2D) = "white" {}
		_NormalTex("Normal", 2D) = "white" {}
		_AOTex("AO", 2D) = "white" {}
		_SpecularTex("Specular", 2D) = "white" {}

		_AtlasSize("Atlas size", Range(1,8)) = 1
		_AO("AO factor", Range(0,1.0)) = 0.5
		_Smoothness("Smoothness", Range(0,1.0)) = 0.5		
		_RootColor("Start Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_TipColor("End Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_ColorBias("Color bias", Range(0.0, 1.0)) = 1

		_NormalPower("Normal Power", Range(0,1.0)) = 0.5

		_Stripes("Stripes", Range(0.0, 100.0)) = 1
		_Emission("Emission", Range(0, 1)) = 1
		_AlphaCutoff("Alpha Cutoff", Range(0, 1)) = .5
		_Alpha("Alpha", Range(0, 1)) = .5
		_RndSeed("Rnd seed", Range(0, 1000)) = 0
					
		_Shift1("Specular Shift 1", Range(-2.0, 2.0)) = .5		
		_SpecularExp1("Specular 1", Range(0.0, 100.0)) = 1.0		
		_SpecularColor1("Specular Color 1", Color) = (1,1,1,1)
		
		_Shift2("Specular Shift 2", Range(-2.0, 2.0)) = .7
		_SpecularExp2("Specular 2", Range(0, 100.0)) = 1.0
		_SpecularColor2("Specular Color 2", Color) = (1,1,1,1)

			_TranslucencyStrength("Translucency Strength", Range(0, 100.0)) = 1.0
			_TransluscencyNormal("Translucency Normal", Range(0, 1.0)) = 0.2
			_TransluscencyPower("Translucency Power", Range(0, 100.0)) = 2.0
			_TransluscencyDirect("Translucency Direct", Range(0, 1.0)) = 1.0
			_TransluscencyIndirect("Translucency Indirect", Range(0, 1.0)) = 0.2

	}
		SubShader
		{

			Tags{ "Queue" = "AlphaTest+1" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }			
			LOD 200
			Cull Off
			ZWrite On
			Lighting On
			//Offset -.5,-.5

			CGPROGRAM
			#pragma target 3.0
			#pragma surface surf HairAtlas vertex:vert addshadow fullforwardshadows	
			
			#define TRANSPARENT_PASS 0
			//#include "HairDesigner_UnityPBSLighting.cginc"
			#include "HairDesigner.cginc"
			#include "HairDesigner_Atlas.cginc"			
			ENDCG			

	}

	
}
