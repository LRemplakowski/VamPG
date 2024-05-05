
Shader "HairDesigner/AdvancedFur_Shell" {
	Properties{
		
		[MaterialToggle] _EnableShell("Enable Shell", Float) = 0
		[MaterialToggle] _EnableStrands("Enable Strand", Float) = 0


		[Header(GLOBAL)]
		_MainTex("Diffuse", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "white" {}
		_ColorTex("Color", 2D) = "white" {}
		_ColorTexIntensity("Color Tex intensity", Range(0,1)) = 0.0



		[Header(SHELL)]
		_ShellHeight("Shell height", Range(0,1)) = 0.3
		_DensityTex("Fur Density", 2D) = "white" {}
		_DensityNormalTex("Normal Map", 2D) = "bump" {}
		_ShellThickness("Shell thickness", Range(0.0, 1.0)) = 0.2
		_MaskLOD("Coverage", Range(0,1)) = 0.0
		_ShellColorStength("Color strength", Range(0,10)) = 2.0
		_ShellAO("AO strength", Range(0,1)) = .5

		[Header(STRAND)]
		_Cutout("Cutout", Range(0.0, 1.0)) = 0.2
		_CutoutAlpha("Cutout alpha", Range(0.0, 1.0)) = 0.2
		//_ShadowsStrength("Shadows strength", Range(0.0, 1.0)) = .8

		_ShellRootColor("Root Color", Color) = (1, 1, 1, 1)
		_ShellTipColor("Tip Color", Color) = (1, 1, 1, 1)

		_ColorThreshold("Color threshold", Range(0,10)) = 2
		_RootEmissionColor("Root Emission Color", Color) = (0, 0, 0, 0)
		_TipEmissionColor("Tip Emission Color", Color) = (0, 0, 0, 0)
		_EmissionThreshold("Emission threshold", Range(0,10)) = 2
		_RimColor("Rim color", Color) = (1,1,1,1)
		_RimPower("Rim power", Range(0,1)) = 0.0

		[Space]
		_TessEdge("Tessellation", Range(1,32)) = 2
		_StrandHeightMin("Height min", Range(0,1)) = 0.1

		[Space]
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0, 1)) = 0
		_OcclusionMap("Occlusion Map", 2D) = "white" {}
		_OcclusionStrength("Occlusion Strength", Range(0, 1)) = 1

		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalStrength("Normal strength", Range(0, 10)) = 1
		_NormalSwitch("Normal switch", Range(0, 1)) = .5

		_FurTex("Fur Texture", 2D) = "white" {}
		_BrushTex("Brush Texture", 2D) = "white" {}
		_BrushTextureSize("Brush Texture size", int) = 16
		_BrushStrength("Brush strength", Range(0, 10)) = 0
		_Length("Length", Range(0., 20.)) = 0.2
		_RandomLength("Rnd Length", Range(0, 1)) = 0.1
		_Size("Size", Range(0.001, 100)) = 0.05
		_SizeFaceFactor("Size face factor", Range(0.0, 1.0)) = 0		
		_Pinch("Pinch", Range(-1, 1)) = .5

		[Space]
		_RandomSeed("Rnd seed", Range(0.01, 0.99)) = .5
		
		_Density("Density", Range(1, 10)) = 1
		_Subdivision("Subdivisions", Range(1, 5)) = 1
		_Gravity("Gravity", Range(0, 100)) = 0.05
		_Wind("Wind", Range(0, 1)) = 0.0
		
		[Header(DEBUG)]
		_Test("Test", Range(0., 1.)) = 1
		
	}
		SubShader{

		//Tags{ "Queue" = "Geometry" "RenderType" = "Opaque"  }
		Tags{ "Queue" = "AlphaTest+1" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }
		//Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		
			//Tags { "RenderType" = "Opaque" }
			LOD 400
			//Cull off
						

			
		//FWD SHELL
		Pass 
		{
			Tags{ "LightMode" = "ForwardBase" }
			AlphaToMask On
			Zwrite On
			//Cull off
			//Blend SrcAlpha OneMinusSrcAlpha
			LOD 100

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex VS
			#pragma fragment PS_FRWD
			//#pragma hull HS
			//#pragma domain DS
			#pragma geometry GS_SHELL
			#pragma multi_compile_fwdbase			
			#define UNITY_PASS_FORWARDBASE 1
			
			#include "AdvancedFur_Common.cginc"			
			#include "AdvancedFur_Shell.cginc"
			#include "AdvancedFur_PBR.cginc"
			ENDCG
		}
		
			
		Pass
		{
			Tags{ "LightMode" = "ForwardAdd" }			
			Zwrite Off
			Blend One One			

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex VS			
			#pragma fragment PS_FRWD
			//#pragma hull HS
			//#pragma domain DS
			#pragma geometry GS_SHELL
			#pragma multi_compile_fwdadd_fullshadows
			#pragma skip_variants INSTANCING_ON
			#define UNITY_PASS_FORWARDADD		
			//#define UNITY_PASS_FORWARDBASE 1
			#include "AdvancedFur_Common.cginc"			
			#include "AdvancedFur_Shell.cginc"	
			#include "AdvancedFur_PBR.cginc"
			ENDCG
		}
		
			
				

		//DFRD SHELL
		Pass
		{
			Tags{ "LightMode" = "Deferred" }
			Zwrite On
			Lighting On
			
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex VS
			#pragma fragment PS_DFRD
			//#pragma hull HS
			//#pragma domain DS
			#pragma geometry GS_SHELL

			#pragma exclude_renderers nomrt
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#pragma multi_compile_prepassfinal
			#pragma multi_compile_instancing
			
			#define UNITY_PASS_DEFERRED
			#include "AdvancedFur_Common.cginc"			
			#include "AdvancedFur_Shell.cginc"
			#include "AdvancedFur_PBR.cginc"
			
			ENDCG
		}
		


			
							
			
		//SHADOW CASTER SHELL
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }
			Zwrite On

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex VS
			#pragma fragment PS_SHELL_SHADOWS
			//#pragma hull HS
			//#pragma domain DS
			#pragma geometry GS_SHELL
			#pragma multi_compile_shadowcaster noshadowmask nodynlightmap nodirlightmap nolightmap
			#define UNITY_PASS_SHADOWCASTER
			#include "AdvancedFur_Common.cginc"
			#include "AdvancedFur_Shell.cginc"
			#include "AdvancedFur_Shadows.cginc"			
			ENDCG
		}
		
			
			

			
			
	}
}