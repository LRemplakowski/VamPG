Shader "HairDesigner/Procedural_v2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Cull("Cull", Float) = 0.0
		_Smoothness("Smoothness", Range(0,1.0)) = 0.5
		_AO("AO", Range(0,1.0)) = 0.5
		_StartColor("Start Color", Color) = (0.3, 0.4, 0.7, 1.0)
		_EndColor("End Color", Color) = (0.7, 0.4, 0.3, 1.0)
		_ColorBias("Color bias", Range(0.0, 1.0)) = 1
		_Stripes("Stripes", Range(1.0, 100.0)) = 1
		_HairDensity("Hair Density", Range(1,40)) = 20
		_HairPerPass("Hair per pass", Range(1,40)) = 1
		_HairLength("Hair Length", Range(0, 1)) = .4
		_HairWidth("Hair Width", Range(0, 1)) = 1
		_Wave1("Wave 1", Range(0,20)) = 0
		_WavePower1("Wave power 1", Range(.01,.2)) = 0
		_WaveRnd1("Wave rnd 1", Range(0.,1.)) = 0
		_Wave2("Wave 2", Range(0,20)) = 0
		_WavePower2("Wave power 2", Range(.01,.2)) = 0
		_WaveRnd2("Wave rnd 2", Range(0.,1.)) = 0
		_WaveDir("Wave dir", Range(0,1)) = 0.5
		_Chaos("Chaos", Range(0.0, 1.0)) = 1
		_Cut("Cut", Range(0.0, 1.0)) = 1
		_RootThickness("Root thickness", Range(0.0, 1.0)) = .2
		_TipThickness("Tip thickness", Range(0.0, 1.0)) = .2
		_RimColor("Rim Color", Color) = (0.2, 0.2, 0.2, 0.0)
		_RimPower("Rim Power", Range(0.5, 8.0)) = 2.5
		_NormalFactor("_Normal",Range(-1.0, 1.0)) = 0
		_Emission("Emission", Range(0, 1)) = 1
		_NormalMode("Normal mode", Range(0, 1)) = 1
		_NormalPower("Normal power", Range(0, 1)) = 1
		_AlphaCutoff("Alpha Cutoff", Range(0, 1)) = .5
		_Alpha("Alpha", Range(0, 1)) = .5
		_RndSeed("Rnd seed", Range(0, 1000)) = 0


	}
		SubShader
		{
			Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" "IgnoreProjector" = "True" }
			LOD 200
			Cull Off
			ZWrite On
			Lighting On
			CGPROGRAM
			#pragma target 3.0
			//#pragma surface surf StandardSpecular vertex:vert addshadow fullforwardshadows 
			//#pragma surface surf TwoSided vertex:vert addshadow fullforwardshadows 
			#pragma surface surf HairDesigner vertex:vert addshadow fullforwardshadows
			#define TRANSPARENT_PASS 0
			#include "HairDesigner_UnityPBSLighting.cginc"
			#include "HairDesigner.cginc"
			#include "HairDesigner_Procedural_v2.cginc"	
			ENDCG
			
			
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
			LOD 200
			Cull Off
			ZWrite On
			Lighting On
			//AlphaToMask On
			

			CGPROGRAM
			#pragma target 3.0
			#pragma surface surf HairDesigner vertex:vert alpha:blend
			#define TRANSPARENT_PASS 1
			#include "HairDesigner_UnityPBSLighting.cginc"
			#include "HairDesigner.cginc"
			#include "HairDesigner_Procedural_v2.cginc"
			ENDCG
			
	}

	
}
