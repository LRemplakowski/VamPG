#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"
#include "../HairDesigner.cginc"


#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

#line 10 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif


#ifdef UNITY_PASS_FORWARDBASE	
#define UNITY_PASS_FORWARD
#endif

#ifdef UNITY_PASS_FORWARDADD	
#define UNITY_PASS_FORWARD
#endif

float _GlobalScale;
int _TessEdge;
sampler2D _MainTex;
float4 _MainTex_ST;
sampler2D _FurTex;
float4 _FurTex_ST;
sampler2D _MaskTex;
float4 _MaskTex_ST;
sampler2D _DensityTex;
float4 _DensityTex_ST;
sampler2D _DensityNormalTex;

sampler2D _MotionTex;
sampler2D _ExtraTex;
sampler2D _ColorTex;
sampler2D _BrushTex;

half _Smoothness;
half _Metallic;

half4 _StrandRootColor;
half4 _StrandTipColor;
float _StrandColorThreshold;

half4 _ShellRootColor;
half4 _ShellTipColor;
float _ShellColorThreshold;

float _ColorTexIntensity;

float _ShellCount;
float _StrandFlexibility;
float _StrandOrientation;
float _StrandWaveFrequency;
float _StrandWaveAmplitude;

half4 _RootEmissionColor;
half4 _TipEmissionColor;
float _EmissionThreshold;
float _Emission;

float4 _RimColor;
float _RimPower;

sampler2D _NormalMap;
float _NormalStrength;
float _NormalSwitch;

//sampler2D _OcclusionMap;
//float _OcclusionStrength;
float _Cutout;
float _CutoutAlpha;
//float _ShadowsStrength;

float _AtlasSize;
float _Length;
float _StrandRandomLength;
float _Size;
float _SizeFaceFactor;
float _Pinch;
float _RandomSeed;
float _Test;
float _Density;
float _Subdivision;
float _Gravity;
float _Wind;

float _localRnd;
int _BrushTextureSize;
int _MotionTextureSize;

float _BrushStrength;
float _StrandAO;

float _ShellThickness;
float _ShellHeight;
float _MaskLOD;
float _ShellColorStength;
float _ShellAO;

float _StrandHeightMin;
float _MotionFactor;

float _Translucency;

struct VS_Input
{
	float4 position : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	uint vertexId : SV_VertexID;
	//fixed facing : VFACE;
};


struct HS_Input
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 texcoord : TEXCOORD0;
	float4 motionDir : TEXCOORD1;
	float4 brushDir : TEXCOORD2;	
	float4 extra : TEXCOORD3;
	float rnd : TEXCOORD4;
};

struct HS_ConstantOutput
{
	float TessFactor[3]    : SV_TessFactor;
	float InsideTessFactor : SV_InsideTessFactor;
};


/*
struct HS_ControlPointOutput
{
	float4 position : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 texcoord : TEXCOORD0;
	float4 motionDir : TEXCOORD1;
	float4 brushDir : TEXCOORD2;	
	float4 extra : TEXCOORD3;
};
*/
/*
struct DS_Output
{
	float4 position   : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 texcoord : TEXCOORD0;
	//float2 texcoord2 : TEXCOORD1;
	float4 brushDir : TEXCOORD2;
};
*/

struct FS_Output
{
	fixed4 color : SV_Target0;
};


struct GEOM_Input
{
	UNITY_POSITION(pos);
	float4 texcoord : TEXCOORD0;
	
	//float3 worldPos: TEXCOORD2;
	

#if defined(UNITY_PASS_SHADOWCASTER)	
	//float3 worldNormal: TEXCOORD1;
	float4 surfaceNormal : TEXCOORD8;
#else
	float4 tSpace0 : TEXCOORD1;
	float4 tSpace1 : TEXCOORD2;
	float4 tSpace2 : TEXCOORD3;
	float4 surfaceNormal : TEXCOORD8;
#endif		
	

#if defined(UNITY_PASS_FORWARDBASE) 			
	
	float3  sh : TEXCOORD4;	
	UNITY_SHADOW_COORDS(5)
	UNITY_FOG_COORDS(6)
	float4 lmap : TEXCOORD7;
#endif
	

#if defined(UNITY_PASS_FORWARDADD) 
	UNITY_SHADOW_COORDS(4)
	UNITY_FOG_COORDS(5)
#endif


	//UNITY_SHADOW_COORDS(3)
	//UNITY_FOG_COORDS(4)

#if defined(UNITY_PASS_DEFERRED) 	

	#ifndef DIRLIGHTMAP_OFF
			half3 viewDir : TEXCOORD6;
	#endif
		float4 lmap : TEXCOORD4;
	#ifndef LIGHTMAP_ON
		#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
			half3 sh : TEXCOORD5; // SH
		#endif
	#else
		#ifdef DIRLIGHTMAP_OFF
			float4 lmapFadePos : TEXCOORD5;
		#endif
	#endif

	#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
	

};







//----------------------------------------------
//FUNCTIONS

void InitRandom()
{
    _localRnd = _RandomSeed;
}


half Random(float a)
{
	_localRnd = abs( sin(a + UNITY_HALF_PI * frac(a) + UNITY_HALF_PI * frac(_localRnd)));
	return _localRnd;
}



float4 PerVertexTextureCoord(int vID, float textureSize)
{
	float halfPixel = (1.0 / textureSize)*.5;
	float idx = float(vID) / textureSize;
	float idy = floor(idx) / textureSize;
	idx -= floor(idx);

	return float4(idx + halfPixel, 1 - idy - halfPixel, 0, 0);
}



float3 BrushDir( out float length, float4 uv)
{

	float4 brush = tex2Dlod(_BrushTex, uv);
	length = brush.a;

    return normalize(brush.rgb * 2 - float3(1, 1, 1));
}



float3 FurDir(float3 brush, float3 motion)
{	
	//return lerp(brush.xyz+ motion.xyz*motion.w, motion.xyz, motion.w);
	//return lerp((brush + motion)*.5, motion, length(motion));
    return lerp((brush + motion) * .5, motion, length(motion));
}


float2 AtlasUV(float id, float atlasSize, float2 uv)
{    
    if (atlasSize > 0)
    {        
        float as = floor(atlasSize);
        uv = uv / as;
        //InitRandom();
        _RandomSeed = 10;
        uv.x += clamp(round(Random(.1 + id * (_RandomSeed)) * (as - 1)) * (1 / as), 0, atlasSize - 1);
        uv.y += clamp(round(Random(.1 + id * ((_RandomSeed) + (_RandomSeed))) * (as - 1)) * (1 / as), 0, atlasSize - 1);
    }
    return uv;
}



//----------------------------------------------




//----------------------------------------------
//VERTEX
//----------------------------------------------
HS_Input VS(VS_Input input)
{
	/*
	HS_Input Output;
	Output.position = input.position;
	return Output;
	*/


	HS_Input o = (HS_Input)0;

	o.vertex = mul(unity_ObjectToWorld, input.position);
	o.normal = UnityObjectToWorldNormal(input.normal);
	o.tangent.xyz = UnityObjectToWorldDir(input.tangent.xyz);


	o.texcoord.xy = input.texcoord.xy; //TRANSFORM_TEX(input.texcoord.xy, _MainTex);
	o.texcoord.z = input.vertexId;

	
	float4 vtxUV = PerVertexTextureCoord(input.vertexId, float(_BrushTextureSize > _MotionTextureSize ? _BrushTextureSize: _MotionTextureSize));

	float length = 1;
	float3 brush = BrushDir(length, vtxUV);


	o.brushDir.xyz = normalize(brush.x*o.normal.xyz
		+ brush.y*normalize(o.tangent.xyz)
		+ brush.z*normalize(cross(o.normal, o.tangent).xyz));

	o.brushDir.xyz = lerp(o.normal,o.brushDir.xyz,_BrushStrength);
	o.brushDir.w = length;	
	    
	
	o.motionDir = tex2Dlod(_MotionTex, vtxUV) - float4(.5, .5, .5,0);
	o.motionDir.xyz *= 2.0;


	o.extra = tex2Dlod(_ExtraTex, vtxUV);
	o.rnd = (float)(input.vertexId%_AtlasSize);

	//float dotMotion = dot(normalize(o.normal.xyz), normalize(motion.xyz));
	//if (dotMotion > 0)
	//	o.brushDir.xyz = lerp(o.brushDir.xyz, motion.xyz, motion.w);// *_MotionFactor);
	//else
	//	o.brushDir.xyz = lerp(o.brushDir.xyz, motion.xyz, motion.w* (1 + dotMotion)) ;// *_MotionFactor;


	//o.brushDir.xyz = float3(0, 10, 0);

	//o.brushDir.xyz = normalize(o.brushDir.xyz);
	//o.texcoord.w =  motion.w;

	return o;

}












HS_ConstantOutput HSConstant(InputPatch<HS_Input, 3> Input)
{
	HS_ConstantOutput Output = (HS_ConstantOutput)0;

#if defined(NO_TESS)
	Output.TessFactor[0] = Output.TessFactor[1] = Output.TessFactor[2] = 1;
	Output.InsideTessFactor = 1;
#else		
	Output.TessFactor[0] = Output.TessFactor[1] = Output.TessFactor[2] = _TessEdge;
	Output.InsideTessFactor = _TessEdge;
#endif
	return Output;
}


//----------------------------------------------
// TESS HULL
//----------------------------------------------
[domain("tri")]
[partitioning("integer")]
[outputtopology("triangle_cw")]
[patchconstantfunc("HSConstant")]
[outputcontrolpoints(3)]
HS_Input HS(InputPatch<HS_Input, 3> Input, uint uCPID : SV_OutputControlPointID)
{
	return Input[uCPID];
	/*
	HS_Input o = (HS_Input)0;
	o.vertex = Input[uCPID].vertex;
	o.normal = Input[uCPID].normal;
	o.tangent.xyz = Input[uCPID].tangent.xyz;
	o.texcoord = Input[uCPID].texcoord;
	o.brushDir = Input[uCPID].brushDir;
	o.motionDir = Input[uCPID].motionDir;
	o.extra = Input[uCPID].extra;
	o.rnd = Input[uCPID].rnd;
	return o;
	*/
}


//----------------------------------------------
// TESS DOMAIN
//----------------------------------------------
[domain("tri")]
HS_Input DS(HS_ConstantOutput HSConstantData,
	const OutputPatch<HS_Input, 3> Input,
	float3 BarycentricCoords : SV_DomainLocation)
{
	HS_Input o = (HS_Input)0;

	float BX = BarycentricCoords.x;
	float BY = BarycentricCoords.y;
	float BZ = BarycentricCoords.z;

	o.vertex = Input[0].vertex * BX + Input[1].vertex * BY + Input[2].vertex * BZ;
	o.normal = Input[0].normal * BX + Input[1].normal * BY + Input[2].normal * BZ;
	o.tangent = Input[0].tangent * BX + Input[1].tangent * BY + Input[2].tangent * BZ;
	o.texcoord = Input[0].texcoord * BX + Input[1].texcoord * BY + Input[2].texcoord * BZ;
	o.brushDir = Input[0].brushDir * BX + Input[1].brushDir * BY + Input[2].brushDir * BZ;
	o.motionDir = Input[0].motionDir * BX + Input[1].motionDir * BY + Input[2].motionDir * BZ;
	o.extra = Input[0].extra * BX + Input[1].extra * BY + Input[2].extra * BZ;
	o.rnd = Input[0].rnd * BX + Input[1].rnd * BY + Input[2].rnd * BZ;
	return o;
}





