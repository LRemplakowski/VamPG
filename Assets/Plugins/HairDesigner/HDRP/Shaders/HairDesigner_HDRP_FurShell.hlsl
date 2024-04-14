#define pi 3.1415926

#if SHADER_TARGET < 30
static const int NB_MOTIONZONE = 10;
#else
static const int NB_MOTIONZONE = 50;
#endif

float4 KHD_motionZonePos[NB_MOTIONZONE];
float4 KHD_motionZoneDir[NB_MOTIONZONE];
float KHD_windFactor;
float4 KHD_windZoneDir;
float4 KHD_windZoneParam;
float KHD_windZoneTurbulenceFrequency;


float RND(float seed) {
	seed = round(seed * 255) / 255;//fix
	return frac(sin(dot(float2(seed,seed*seed), float2(12.9898, 78.233))) * 43758.5453);
}


float4x4 rotationMatrix(float3 axis, float angle)
{
	axis = normalize(axis);
	float s = sin(angle);
	float c = cos(angle);
	float oc = 1.0 - c;

	return float4x4(oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s, 0.0,
		oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s, 0.0,
		oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c, 0.0,
		0.0, 0.0, 0.0, 1.0);
}



void Curl_float( float3 position, float3 n, float3 t,float f, float id, float _CurlNumber, float _CurlRadius, inout float3 newpos)
{
	float3 R = normalize(cross(t.xyz, n.xyz));				
	float r = RND(id);				
	newpos = position + mul(rotationMatrix(n, 2.0 * pi * f * _CurlNumber * r), float4(R,0)*_CurlRadius*.1*(1-f));				
}






void VertexIDToUV_float( float vID, float textureSize, inout float2 UV)
{
	//float textureSize = 64;


	float halfPixel = (1.0 / textureSize)*.5;
	float idx = float(vID) / textureSize;
	float idy = floor(idx) / textureSize;
	idx -= floor(idx);

	UV = float2(idx + halfPixel, 1-idy- halfPixel);

}



float3 ComputeWind(float t, float rnd, float3 tan, float3 worldPos)
{
	if (length(KHD_windZoneDir)*KHD_windZoneParam.x == 0)
		return float3(0, 0, 0);

	float3 wind = KHD_windZoneDir.xyz;

	if (KHD_windZoneDir.w != 0)
	{
		wind = normalize(cross(worldPos - KHD_windZoneDir.xyz, float3(0.0, 1.0, 0.0)));
		if (length((worldPos - KHD_windZoneDir.xyz)) < abs(KHD_windZoneDir.w))
		{
			float f = length(worldPos - KHD_windZoneDir.xyz) / KHD_windZoneDir.w;
			KHD_windZoneParam.x *= f > .5 ? 1.0 - (f - .5)*2.0 : f * 2.0;
		}
		else
			return float3(0, 0, 0);
	}

	//return 0;//WIP

	//rotationMatrix(cross(wind, tan), sin(_Time.y*KHD_windZoneParam.y));
	float angle = clamp(KHD_windZoneParam.y*(KHD_windZoneParam.y + KHD_windZoneParam.x), -80.0, 80.0)* (3.14 / 180.0);
	if (angle != 0)
		wind = mul(
			rotationMatrix(
				cross(float3(0, 1, 0), wind),
				cos((rnd*_Time.w + rnd * _Time.y)*KHD_windZoneTurbulenceFrequency*KHD_windZoneParam.y*.1)
				* sin((rnd*_Time.w*10.0 + rnd * _Time.y)*KHD_windZoneTurbulenceFrequency*KHD_windZoneParam.y*.1)
				*angle
			)
			, float4(wind, 1)).xyz;
	//wind += lerp(wind, tan, dot(KHD_windZoneDir.xyz, tan)) * .1;


	wind = normalize(wind) * KHD_windZoneParam.x * 0.1;
	float dotWindTan = dot(KHD_windZoneDir.xyz, tan);
	float omt = 1 - t;
	float3 winDir = lerp( KHD_windZoneDir.xyz, normalize(tan+ KHD_windZoneDir.xyz), 1.0-omt*omt*omt);
	wind += winDir
		* (
			sin(KHD_windZoneParam.w *  _Time.y * 10.0 + cos(rnd*_Time.y*KHD_windZoneParam.z) * 2.0) * 2.0
			* cos(KHD_windZoneParam.w * _Time.y * 20.0 + sin(rnd*_Time.y*KHD_windZoneParam.z) * 4.0) * 0.1
			)
		* KHD_windZoneParam.z * KHD_windZoneParam.x  * 0.1;

	float f = (1 + dotWindTan);
	//float f = 1.0;
	return wind * KHD_windFactor *t * t * f;

	
}



void MotionZoneAnimation_float(float3 worldpos, float worldTan, float2 uv, float furLength, float furFactor, inout float3 newWorldpos)
{		
	float3 worldMotion = float3(0,0,0);
	for (int i = 0; i < NB_MOTIONZONE; ++i)
	{
		float dist = length(worldpos - KHD_motionZonePos[i].xyz);

		if (dist <= KHD_motionZonePos[i].w && KHD_motionZonePos[i].w > 0)
		{
			float3 motion = KHD_motionZoneDir[i].xyz;
			worldMotion -= motion * (1 - (dist / KHD_motionZonePos[i].w)) * furFactor * furFactor;
		}
	}


	worldMotion +=  ComputeWind(furFactor, uv.x + uv.y, worldTan, worldpos) * (1.0-(1.0- furFactor)*(1.0- furFactor));

	if(length(worldMotion)>0)
		worldMotion = normalize(worldMotion) * clamp(length(worldMotion), 0.0, furLength* furFactor);
		
	newWorldpos = worldpos + worldMotion;

	
}

float4x4 _O2W;
float4x4 _W2O;



float4x4 inverse(float4x4 m)
{
    float n11 = m[0][0], n12 = m[1][0], n13 = m[2][0], n14 = m[3][0];
    float n21 = m[0][1], n22 = m[1][1], n23 = m[2][1], n24 = m[3][1];
    float n31 = m[0][2], n32 = m[1][2], n33 = m[2][2], n34 = m[3][2];
    float n41 = m[0][3], n42 = m[1][3], n43 = m[2][3], n44 = m[3][3];

    float t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
    float t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
    float t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
    float t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

    float det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;
    float idet = 1.0f / det;

    float4x4 ret;

    ret[0][0] = t11 * idet;
    ret[0][1] = (n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44) * idet;
    ret[0][2] = (n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44) * idet;
    ret[0][3] = (n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43) * idet;

    ret[1][0] = t12 * idet;
    ret[1][1] = (n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44) * idet;
    ret[1][2] = (n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44) * idet;
    ret[1][3] = (n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43) * idet;

    ret[2][0] = t13 * idet;
    ret[2][1] = (n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44) * idet;
    ret[2][2] = (n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44) * idet;
    ret[2][3] = (n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43) * idet;

    ret[3][0] = t14 * idet;
    ret[3][1] = (n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34) * idet;
    ret[3][2] = (n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34) * idet;
    ret[3][3] = (n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33) * idet;

    return ret;
}

float3 getPosition(float4x4 m)
{    
    return float3(m[0][3], m[1][3], m[2][3]);
}


float3 rotate(float4x4 m, float3 pos)
{    
    m[3] = float4(0,0,0,1);
    m = inverse(m);
    return mul(m, pos);
}

float3 transform(float4x4 m,float3 pos)
{    
    float3 scale = m[3].xyz;
    m[3] = float4(0, 0, 0, 1);
    return (rotate(m, pos - getPosition(m))) / scale;
	
}



void InjectSetup_float(float3 A, out float3 Out) 
{
	Out = A;
}


#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED


struct FurInstanceData
{
	float furFactor;
};
#if defined(SHADER_API_GLCORE) || defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN) || defined(SHADER_API_PSSL) || defined(SHADER_API_XBOXONE)
uniform StructuredBuffer<FurInstanceData> FurInstanceDataBuffer;
#endif

#endif

uniform float _FurFactorInstance;

void InitInstancing()
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

#ifdef unity_ObjectToWorld
#undef unity_ObjectToWorld
#endif

#ifdef unity_WorldToObject
#undef unity_WorldToObject
#endif
	
	_FurFactorInstance = FurInstanceDataBuffer[unity_InstanceID].furFactor;

	unity_ObjectToWorld = _O2W;
	unity_WorldToObject = _W2O;
	
#else
	_FurFactorInstance = 1.0;
#endif
}


float _InstancedTrick;
void FurFactor_float(float f,inout float FurFactor)
{
   //FurFactor = lerp(f, UNITY_MATRIX_M[3][3], _InstancedTrick);
   #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
   
	   if( _InstancedTrick >= 0 )
	   {
		FurFactor = _InstancedTrick == 0 ? f : UNITY_MATRIX_M[3][3];
		}
		else
		{
			FurFactor =  _FurFactorInstance;
		}

	#else
		FurFactor = _InstancedTrick == 0 ? f : UNITY_MATRIX_M[3][3];
	#endif


}

void UpdateVertexData_float(float3 InPos, float3 InNormal, float3 INTangent, out float3 OutPosition, out float3 OutNormal, out float3 OutTangent) 
{
	//Out = A;
	//Out = mul(_W2O,A);
	//Out = A - getPosition(_W2O);
	//Out = rotate(  _W2O,A );
	//Out = transform( _W2O, A );
	//#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	InPos *= 1;//_FurFactorInstance;
	//#endif
	OutPosition = rotate(_W2O, InPos - getPosition(_W2O));
	OutNormal = normalize(rotate(_W2O, InPos + InNormal  - getPosition(_W2O))- OutPosition );
	OutTangent = normalize(rotate(_W2O, InPos + INTangent - getPosition(_W2O))- OutPosition );

	
}

float2 _FurNoise;
void FurNoise_float(float vID, float furLength, inout float newFurLength)
{
    newFurLength = furLength - furLength * _FurNoise.x * sin(vID * 10.0 + _FurNoise.y);
}

void NoiseSide_float(float furFactor, inout float noise)
{
    noise =  sin(furFactor) + cos(furFactor*furFactor) * .5;
}