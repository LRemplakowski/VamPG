// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "HairDesigner/FurShell" {
	Properties {
		
		_MainTex ("main color", 2D) = "white" {}
		_DensityTex("Density", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "" {}
		_NormalTex("Normal", 2D) = "bump" {}
		_BrushTex("Brush", 2D) = "white" {}
		_ColorTex("Color", 2D) = "white" {}
		_ColorTexIntensity("Brush color intensity", Range(0,1)) = 0.0

		_RootColor("Root color", Color) = (1,1,1,1)
		_TipColor("Tip color", Color) = (1,1,1,1)
		_ColorBias("Color bias", Range(0,1)) = .5

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_FurLength("Fur length", Range(0,10)) = 1.0
		_FurFactor("Fur factor", Range(0,1)) = 0.0
		_Thickness("Thickness", Range(0,1)) = 0.0
		_CurlNumber("Curl Number", Range(0,10)) = 0.0
		_CurlRadius("Curl Radius", Range(0,10)) = 0.0
		_CurlRnd("Curl Random", Range(0,10)) = 0.0
		_MaskLOD("Mask LOD", Range(0,1)) = 0.0

		_Gravity("Gravity", Range(0,1)) = 0.0
		_Scale("Scale", Range(0,10)) = 0.0
		_BrushFactor("Brush factor", Range(0,1)) = 0.0

		_RimColor("Rim color", Color) = (1,1,1,1)
		_RimPower("Rim power", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		Zwrite On

		CGPROGRAM
		#pragma surface surf Standard  vertex:vert fullforwardshadows addshadow
		#pragma target 3.5


#define HD_GPU_INSTANCING
//#define ENABLE_VERTEXMOTION
//#include "Assets/VertExmotion/Shaders/VertExmotion.cginc"

#ifdef HD_GPU_INSTANCING 	
		#pragma multi_compile_instancing
		#pragma instancing_options maxcount:300
#endif


		

		#include "HairDesigner.cginc"
		// Config maxcount. See manual page.
		// #pragma instancing_options

		sampler2D _MainTex;
		sampler2D _DensityTex;
		sampler2D _MaskTex;
		sampler2D _BrushTex;
		sampler2D _ColorTex;
		sampler2D _NormalTex;

		sampler2D _VertexSkinningTex;
		sampler2D _NormalSkinningTex;
		sampler2D _TangentSkinningTex;
		int _GPUSkinning;
		//sampler2D _DataTex;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_DensityTex;
			float2 uv_MaskTex;
			float2 uv_ColorTex;
			//float uv_DataTex;			
			half3 viewDir;
			float3 normal;
			half3 worldNormal;
			float furVertexLength;
			INTERNAL_DATA
		};

#if UNITY_VERSION > 550
		struct appdata_fur
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;			
			uint vertexId : SV_VertexID;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
#endif

		fixed4 _Color;
		half _Glossiness;
		half _Metallic;
		half _Gravity;
		half _FurLength;
		half _Scale;
		half _BrushFactor;
		half _Thickness;
		half _Emission; 
		half _EmissionPower;
		half _AO;
		float4 _RimColor;
		float _RimPower;
		float _CurlNumber;
		float _CurlRadius;
		float _CurlRnd;

		float4 _RootColor;
		float4 _TipColor;
		float _ColorBias;
		float _ColorBiasLength;

		int _BrushTextureSize;
		float2 _BrushBend;
		float3 _RootPosition;

		float _MaskLOD;
		float2 _NormalStrength;

#ifdef HD_GPU_INSTANCING 	

		UNITY_INSTANCING_BUFFER_START(Props)	
		
		UNITY_DEFINE_INSTANCED_PROP(float, _FurFactor)
#define _FurFactor_arr Props
/*
		UNITY_DEFINE_INSTANCED_PROP(float, _MaskLOD)
#define _MaskLOD_arr Props
*/
		UNITY_INSTANCING_BUFFER_END(Props)
#else
		float _FurFactor;		
#endif				






		float2 GetUVFromIdx(int vID)
		{
			float textureSize = float(_BrushTextureSize);
			float halfPixel = (1.0 / textureSize)*.5;
			float idx = float(vID) / textureSize;
			float idy = floor(idx) / textureSize;
			idx -= floor(idx);

			return float2(idx + halfPixel, 1 - idy - halfPixel);
		}


		float3 BrushDir( /*int vID,*/ float2 uv, out float length )
		{
			/*
			float textureSize = float(_BrushTextureSize);
			float halfPixel = (1.0 / textureSize)*.5;
			float idx = float(vID) / textureSize;
			float idy = floor(idx) / textureSize;
			idx -= floor(idx);
			
			float4 brush = tex2Dlod(_BrushTex, float4(idx + halfPixel, 1-idy- halfPixel, 0, 0));
			*/
			float4 brush = tex2Dlod(_BrushTex, float4(uv, 0, 0));
			length = brush.a;
			return normalize(brush.rgb * 2 - 1);
		}


#if UNITY_VERSION > 550
		void vert(inout appdata_fur v, out Input o)
#else
		void vert(inout appdata_full v, out Input o)
#endif
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);			
			
#ifdef HD_GPU_INSTANCING 	
			UNITY_SETUP_INSTANCE_ID(v);
			//UNITY_TRANSFER_INSTANCE_ID(v, o);			
			float f = UNITY_ACCESS_INSTANCED_PROP(_FurFactor_arr, _FurFactor);
#else
			float f = _FurFactor;

#endif


			float2 vUV = GetUVFromIdx(v.vertexId);

			if (_GPUSkinning == 1)
			{
				
				v.vertex += tex2Dlod(_VertexSkinningTex, float4(vUV, 0, 0));
				v.normal = normalize(tex2Dlod(_NormalSkinningTex, float4(vUV, 0, 0)).xyz);
				v.tangent = length(v.tangent) > 0 ? normalize(tex2Dlod(_TangentSkinningTex, float4(vUV, 0, 0))) : v.tangent;
				
			}


#ifdef ENABLE_VERTEXMOTION
			v.vertex = VertExmotion(v.vertex, v.color);
#endif

			float4 vtx = v.vertex;

			//curl			
			float3 R = normalize(cross(v.tangent.xyz, v.normal.xyz));				
			float r = RND(v.texcoord.x + v.texcoord.y);			
			vtx += mul(rotationMatrix(v.normal, 2 * pi * f * _CurlNumber * (r*_CurlRnd + (1 - _CurlRnd))), float4(R,0)*_CurlRadius*.1 *r);
			
			float furVertexLength = 1;		
			


#if UNITY_VERSION > 550			
			//float3 brush = BrushDir(v.vertexId, furVertexLength);
			float3 brush = BrushDir(vUV, furVertexLength);
			brush *= _BrushFactor;
#else
			float3 brush = float3(0, 0, 0);
#endif			
			float4 world_space_vertex = mul(unity_ObjectToWorld, vtx);						
			float4 world_space_normal = mul(unity_ObjectToWorld, float4(v.vertex.xyz + v.normal.xyz,0)) - mul(unity_ObjectToWorld, float4(v.vertex.xyz,0));
			float4 world_space_brush = mul(unity_ObjectToWorld,
				brush.x*v.normal.xyz
				+ brush.y*normalize(v.tangent.xyz)
				+ brush.z*normalize(cross(v.normal, v.tangent).xyz)
				);		
			
			float3 worldSpaceViewDir = _WorldSpaceCameraPos.xyz - world_space_vertex;
			
			float3 worldMotion = float3(0, 0, 0);
			//------------------
			//motion zone
			
			for (int i = 0; i < NB_MOTIONZONE; ++i)			
			{
				float dist = length(world_space_vertex.xyz - KHD_motionZonePos[i].xyz);
				
				if (dist <= KHD_motionZonePos[i].w && KHD_motionZonePos[i].w>0)
				{					
					float3 motion = KHD_motionZoneDir[i].xyz;
					worldMotion -= motion * (1 - (dist / KHD_motionZonePos[i].w)) * f * f;
				}				
			}
			
			//------------------
			float3 g = KHD_gravity*f;
			float dg = dot(normalize(g), normalize(world_space_normal));
			dg = abs((dg + 1)*.5);

			

			float3 worldG = g*dg*_Gravity;
			float3 worldWind = f*ComputeWind(1, v.texcoord.x + v.texcoord.y, world_space_normal, world_space_vertex);
			float dw = dot(normalize(worldWind), normalize(world_space_normal));
				
				
			f = (1 - (1 - f)*(1 - f)*(1 - f));//more shells at the top

			float3 worldBrush = normalize(lerp(world_space_normal+ world_space_brush,world_space_brush*5.0,f*_BrushFactor));//normalize(world_space_normal + world_space_brush*5) *f;
			float worldBrushLength = length(worldBrush);
			worldBrush -= dot(normalize(worldBrush), world_space_normal)*world_space_normal * min(lerp(_BrushBend.x, _BrushBend.y, furVertexLength*furVertexLength*furVertexLength),lerp(.9,.85, f));
			worldBrush = normalize(worldBrush) * worldBrushLength;


			float3 worldExtForce = worldG + worldWind;

			//G vs Wind
			if(length(worldWind) > 0 && length(worldWind)>length(worldG))
				worldExtForce =	worldWind + worldG * length(worldG)/(length(worldWind));	

			//(G+Wind) vs motion zone
			if (length(worldExtForce*.1 + worldMotion) > 0.0)
				worldExtForce = worldExtForce + worldMotion *length(worldMotion) / length(worldExtForce*.1 + worldMotion);
			
			world_space_vertex.xyz += worldExtForce + worldBrush;
	
			vtx = mul(unity_WorldToObject, world_space_vertex);

			float furLengh = furVertexLength*_FurLength;
			
			float3 delta = normalize(vtx.xyz - v.vertex.xyz)*furLengh*f;					
			
			delta = normalize(delta) * clamp(length(delta), 0, furLengh*f*f);
			float dd = dot(normalize(delta), v.normal);
			//if (dd < -(1 - f))
			if (dd < 0)
			{
				delta *= (1 + dd);//modify displacement when wind -> normal
			}

			v.vertex.xyz += delta;
			//v.vertex.xyz = vtx;
			o.furVertexLength = furVertexLength;
			o.normal = v.normal;
		}
		

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			float4 maskCol = tex2D(_MaskTex, IN.uv_MaskTex);
			float mask = maskCol.a;			
			float maskSmoothness = maskCol.r;

#ifdef HD_GPU_INSTANCING 
			float f = UNITY_ACCESS_INSTANCED_PROP(_FurFactor_arr, _FurFactor);
			//float lod = UNITY_ACCESS_INSTANCED_PROP(_MaskLOD_arr, _MaskLOD);
#else
			float f = _FurFactor;
						
#endif
			float lod = _MaskLOD;
			
			//fixed4 c = tex2D(_DensityTex, IN.uv_DensityTex + brush.xy)*mask;
			fixed4 density = tex2D(_DensityTex, IN.uv_DensityTex)*mask;

			clip(sqrt(length(density.xyz))- f*(1 + _Thickness *(1-f)) + step(.0001,mask)*(lod)*(1-f) );

			
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);			
			fixed4 color = tex2D(_ColorTex, IN.uv_ColorTex);
			
			c.rgb = (c.rgb * color.rgb * color.a) + color.rgb * (1 - color.a);

			float bias = f*(_ColorBias + 1.0) / 2.0;
			bias = lerp(bias,  bias/mask, f*_ColorBiasLength);
			c.rgb *= lerp(_RootColor, _TipColor, lerp(clamp(bias,0,1)*.1+.9,clamp(bias,0,1),IN.furVertexLength) );
			
			
			
			float3 n = IN.worldNormal;
			float dotViewDir = saturate(max(dot(normalize(IN.viewDir), n), dot(normalize(IN.viewDir), -n)));
			half rim = 1.0 - dotViewDir;
			

			o.Albedo = c.rgb +(_RimColor.rgb * pow(rim, _RimPower) * f);			
			
			float3 norm = o.Normal;

			//if( _NormalStrength>0)
				o.Normal = normalize( lerp(norm, UnpackNormal(tex2D(_NormalTex, IN.uv_DensityTex)), lerp(_NormalStrength.x,_NormalStrength.y,f)) );
			//else
			//	o.Normal = norm;


			o.Metallic = _Metallic * f;
			//o.Specular = o.Albedo  * _Metallic * f;
			o.Smoothness = max(_Glossiness, maskSmoothness) * f;
			o.Alpha = c.a;						 
			o.Occlusion = lerp(1, lerp(f, 1 - mask / f, 1 - mask), _AO*dot(KHD_windZoneDir.xyz, o.Normal));
			o.Occlusion += (lod*mask)*(1 - f);
			o.Emission = c*_Emission*pow(f/mask,_EmissionPower);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
