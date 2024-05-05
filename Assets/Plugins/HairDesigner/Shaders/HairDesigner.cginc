#ifndef HAIR_DESIGNER_INCLUDED
#define HAIR_DESIGNER_INCLUDED

#include "UnityCG.cginc"

#define pi 3.1415926

#if SHADER_TARGET < 30
	static const int NB_MOTIONZONE = 10;
#else
	static const int NB_MOTIONZONE = 50;
#endif

float3 KHD_startPosition = float3(0, 0, 0);
float3 KHD_startTangent = float3(0, 1, 0);
float3 KHD_endPosition = float3(0, 0, 1);
float3 KHD_endTangent = float3(1, -1, 0);
float2 KHD_taper = float2(1,1);
float KHD_bendX = 1;
float KHD_strandLength = 1;
float KHD_rigidity = 1;
float KHD_scale = 1;
float3 KHD_motionDirection = float3(0, 0, 0);
float3 KHD_gravity = float3(0, 0, 0);
float3 KHD_lossyScale = float3(1, 1, 1);
float KHD_gravityFactor = 0;
float4 KHD_rotation = float4(0, 0, 0, 1);
float4 KHD_motionZonePos[NB_MOTIONZONE];
float4 KHD_motionZoneDir[NB_MOTIONZONE];
float KHD_editor = 0;
float _NormalFactor;
float KHD_windFactor;
float4 KHD_windZoneDir;
float4 KHD_windZoneParam;
float KHD_windZoneTurbulenceFrequency;
float KHD_maxStrandLength;

/*
// PRNG function
float nrand(float2 uv, float salt)
{
	uv += float2(salt, 0);
	return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}
*/

// Quaternion multiplication
// http://mathworld.wolfram.com/Quaternion.html
float4 qmul(float4 q1, float4 q2)
{
	return float4(
		q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
		q1.w * q2.w - dot(q1.xyz, q2.xyz)
		);
}


// Rotate a vector with a rotation quaternion.
// http://mathworld.wolfram.com/Quaternion.html
float3 rotate_vector(float3 v, float4 r)
{
	float4 r_c = r * float4(-1, -1, -1, 1);
	return qmul(r, qmul(float4(v, 0), r_c)).xyz;
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


float RND(float seed) {
	seed = round(seed * 255) / 255;//fix
	return frac(sin(dot(float2(seed,seed*seed), float2(12.9898, 78.233))) * 43758.5453);
}



void GetPosition(float t, inout float3 vertex, inout float3 tangent )
{
	
	float3 a;
	float3 b;
	float3 c;

	c.x = 3 * ((KHD_startPosition.x + KHD_startTangent.x) - KHD_startPosition.x);
	b.x = 3 * ((KHD_endPosition.x + KHD_endTangent.x) - (KHD_startPosition.x + KHD_startTangent.x)) - c.x;
	a.x = KHD_endPosition.x - KHD_startPosition.x - c.x - b.x;

	c.y = 3 * ((KHD_startPosition.y + KHD_startTangent.y) - KHD_startPosition.y);
	b.y = 3 * ((KHD_endPosition.y + KHD_endTangent.y) - (KHD_startPosition.y + KHD_startTangent.y)) - c.y;
	a.y = KHD_endPosition.y - KHD_startPosition.y - c.y - b.y;

	c.z = 3 * ((KHD_startPosition.z + KHD_startTangent.z) - KHD_startPosition.z);
	b.z = 3 * ((KHD_endPosition.z + KHD_endTangent.z) - (KHD_startPosition.z + KHD_startTangent.z)) - c.z;
	a.z = KHD_endPosition.z - KHD_startPosition.z - c.z - b.z;

	float t2 = t*t;
	float t3 = t*t*t;
	float x = a.x * t3 + b.x * t2 + c.x * t + KHD_startPosition.x;
	float y = a.y * t3 + b.y * t2 + c.y * t + KHD_startPosition.y;
	float z = a.z * t3 + b.z * t2 + c.z * t + KHD_startPosition.z;

	float tanx = 3 * a.x*t2 + 2 * b.x*t + c.x;
	float tany = 3 * a.y*t2 + 2 * b.y*t + c.y;
	float tanz = 3 * a.z*t2 + 2 * b.z*t + c.z;

	//float3 crss = cross( normalize(float3(tanx, tany, tanz)) , float3(0,1,0));
	//float3 crss = cross(normalize(endTangent), float3(1, 0, 0));

	vertex.x *= lerp(KHD_taper.x, KHD_taper.y,  t);//taper
	vertex.y -= abs(vertex.x)*abs(vertex.x) * KHD_bendX * (.1f+t*.9);//bend
	//vertex.z *= KHD_strandLength;//length
	//vertex.xyz += KHD_strandLength * float3(tanx, tany, tanz);

	//float3 n = lerp(float3(x, y,z), float3(x*t3, y*t3,z), KHD_rigidity*t);
	float3 n = float3(x, y, z*KHD_strandLength);
	//float3 n = float3(x*t3, y*t3, z);

	vertex.xy = (vertex.xy + n.xy);// +crss*.1;// +float3(tanx, tany, tanz);
	vertex.z = n.z;
	tangent = normalize(float3(tanx, tany, tanz));
}

//--------------------------



float3 ComputeWind(float t, float rnd, float3 tan, float3 worldPos)
{
	if (length(KHD_windZoneDir)*KHD_windZoneParam.x == 0 )
		return float3(0,0,0);

	float3 wind = KHD_windZoneDir.xyz;

	if (KHD_windZoneDir.w != 0)
	{
		wind = normalize(cross(worldPos - KHD_windZoneDir.xyz, float3(0.0, 1.0, 0.0)));
		if (length((worldPos - KHD_windZoneDir.xyz)) < abs(KHD_windZoneDir.w))
		{
			float f = length(worldPos - KHD_windZoneDir.xyz) / KHD_windZoneDir.w;
			KHD_windZoneParam.x *= f>.5? 1.0 - (f-.5)*2.0 : f*2.0;
		}
		else
			return float3(0, 0, 0);
	}

	//return 0;//WIP
	
	//rotationMatrix(cross(wind, tan), sin(_Time.y*KHD_windZoneParam.y));
	float angle = clamp(KHD_windZoneParam.y*(KHD_windZoneParam.y+KHD_windZoneParam.x), -80.0, 80.0)* (3.14 / 180.0);
	if(angle!=0)
	wind = mul(
		rotationMatrix(
			cross(float3(0, 1, 0), wind),
			cos((rnd*_Time.w + rnd*_Time.y)*KHD_windZoneTurbulenceFrequency*KHD_windZoneParam.y*.1)
			* sin((rnd*_Time.w*10.0 + rnd*_Time.y)*KHD_windZoneTurbulenceFrequency*KHD_windZoneParam.y*.1)
			*angle
			)						
		, float4(wind,1)).xyz;
	//wind += lerp(wind, tan, dot(KHD_windZoneDir.xyz, tan)) * .1;
	

	wind = normalize(wind) * KHD_windZoneParam.x * 0.1;
	//wind += KHD_windZoneDir.xyz * KHD_windZoneParam.x * 0.1;
	//rnd += dot(KHD_windZoneDir.xyz, tan);
	//rnd += t;
	
	wind += KHD_windZoneDir.xyz
		* (
			sin(KHD_windZoneParam.w *  _Time.y * 10.0 + cos(rnd*_Time.y*KHD_windZoneParam.z) * 2.0) * 2.0
			* cos(KHD_windZoneParam.w * _Time.y * 20.0 + sin(rnd*_Time.y*KHD_windZoneParam.z) * 4.0) * 0.1
			)
		* KHD_windZoneParam.z * KHD_windZoneParam.x  * 0.1;
	
	float f = (1 + dot(KHD_windZoneDir.xyz, tan)*.1);
	//f = 1.0;
	return wind * KHD_windFactor * t * f;

	/*
	float A = 1;//amplitude
	float P = 10;//periode
	float D = .1;//wave distance
	KHD_windFactor = 10.0;

	float3 windDir = float3 (1, sin(_Time.x*.1*A)*.1,0);

	float3 wind = windDir;
	wind *= max(1-dot(tan, wind),0.01);
	
	wind = normalize(wind);	

	wind *= cos(rnd*_Time.x*P) * sin(_Time.x*P) * cos(rnd*_Time.x*P*.001) * abs(cos(rnd*_Time.x))* A;

	//wave distance
	wind *= pow(abs(cos(2*pi*_Time.x*D))*.9 + abs(sin(2 * pi*_Time.x*D))*.1,3);

	wind *= dot(normalize(wind), normalize(windDir)) > 0 ? 1 : .2;

	//waves = frac(waves);
	
	//return wind * t * ( pow(t,1-t) )* KHD_windFactor;
	return wind * KHD_windFactor * rnd * t;
	*/
}
//--------------------------

float3 GetMotion(float t, float4 vertex, float s, float3 wrldTan)
{
	
	float f = KHD_lossyScale.x;
	float _length = KHD_strandLength;

	float4 world_space_vertex = mul(unity_ObjectToWorld, vertex);
	//float4 world_origin = world_space_vertex - wrldTan *s*f;
	//float3 g = KHD_gravity*s*f;	
	float3 g = KHD_gravity*f;
	float4 newWorldPos = world_space_vertex;
	

	float3 motion =  g* KHD_gravityFactor*.1 + ComputeWind(t, s, wrldTan, world_space_vertex.xyz);
	float dist = 0;
	for (int i = 0; i < NB_MOTIONZONE; ++i)
	{
		dist = length(world_space_vertex.xyz - KHD_motionZonePos[i].xyz);
		if(dist<=KHD_motionZonePos[i].w && KHD_motionZonePos[i].w>0)
			motion -= KHD_motionZoneDir[i].xyz * (1-(dist / KHD_motionZonePos[i].w ));
	}
	motion = normalize(motion + .0000001) * clamp(length(motion), 0, length(wrldTan*s*f));
	newWorldPos.xyz += motion*t*t*t;
	
	//float3 ref = world_space_vertex - wrldTan*s*f;
	//newWorldPos.xyz = normalize(newWorldPos.xyz - ref)*length(wrldTan*s*f) + ref;

	return mul(unity_WorldToObject, newWorldPos);	
}





float2x2 rot2D(float r)
{
	float c = cos(r), s = sin(r);
	return float2x2(c, s, -s, c);
}







//-------------------------------------------------------------------------------
float2 hash(float2 p)
{
	p = float2(dot(p, float2(127.1, 311.7)),
		dot(p, float2(269.5, 183.3)));
	return -1.0 + 2.0*frac(sin(p)*43758.5453123);
}


// returns -.5 to 1.5. i think.
float noise(in float2 p)
{
	const float K1 = 0.366025404; // (sqrt(3)-1)/2;
	const float K2 = 0.211324865; // (3-sqrt(3))/6;

	float2 i = floor(p + (p.x + p.y)*K1);

	float2 a = p - i + (i.x + i.y)*K2;
	float2 o = (a.x>a.y) ? float2(1.0, 0.0) : float2(0.0, 1.0); //float2 of = 0.5 + 0.5*float2(sign(a.x-a.y), sign(a.y-a.x));
	float2 b = a - o + K2;
	float2 c = a - 1.0 + 2.0*K2;

	float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);

	float3 n = h*h*h*h*float3(dot(a, hash(i + 0.0)), dot(b, hash(i + o)), dot(c, hash(i + 1.0)));

	return dot(n, float3(70.0, 70., 70.));
}

float noise01(float2 p)
{
	return clamp((noise(p) + .5)*.5, 0., 1.);
}
//-------------------------------------------------------------------------------


float2 Curv2D(float t, float2 uv, float2 SP, float2 ST, float2 EP, float2 ET )
{
	float2 a;
	float2 b;
	float2 c;
	c.x = 3 * ((SP.x + ST.x) - SP.x);
	b.x = 3 * ((EP.x + ET.x) - (SP.x + ST.x)) - c.x;
	a.x = EP.x - SP.x - c.x - b.x;
	c.y = 3 * ((SP.y + ST.y) - SP.y);
	b.y = 3 * ((EP.y + ET.y) - (SP.y + ST.y)) - c.y;
	a.y = EP.y - SP.y - c.y - b.y;
	float t2 = t*t;
	float t3 = t*t*t;
	float x = a.x * t3 + b.x * t2 + c.x * t + SP.x;
	float y = a.y * t3 + b.y * t2 + c.y * t + SP.y;
	return   float2(x,y) - uv;
}




//-------------------------------------------------------------------------------


//---------------------------------------------------------------------------
/*
float RND( float seed )
{
	return abs(frac(cos(seed*seed*pi)* 43758.5453));
}


float _HairSize = 1;
float _HairWidth = 1;
float _UVX = 1;
void StrandValueCurv(float seed, float nbHair, float2 uv, float2 wave, float chaos, float scale, out float strandFactor, out float3 strandNormal, out float strandColor)
{	
	half rnd = KHD_editor == 0 ? nbHair + seed : nbHair + KHD_scale*KHD_strandLength;
	rnd *= .001;//fix some weird rendering issue
	uv.x *= _UVX;
	int hairCount = ceil(nbHair);
	uv.x = uv.x + sin(uv.y*pi * wave.x*uv.y) * wave.y * (1 - sqrt(uv.y));//wave
	half res = -1;
	half3 n = half3(0,0,0);
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
		half shapeMin = lerp( .089, .11, _HairWidth*(1-uv.y*uv.y));
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
	
	strandFactor = res>1?res:0;
	strandFactor = uv.y > .98 ? 0 : strandFactor;
	strandNormal = normalize(n);
}
*/

//---------------------------------------------------------------------------





void HairDesigner(inout appdata_full v)
{
	if (KHD_editor)
	{
		float3 pos = v.vertex.xyz;
		float3 tangent = float3(0, 0, 0);
		GetPosition(v.texcoord.y, pos , tangent);
		v.vertex.xyz = pos.xyz *KHD_scale;
		//v.tangent.xyz = tangent*KHD_scale*KHD_strandLength;
		v.tangent.xyz = float3(0, 0, 1);
		v.vertex.xyz *= KHD_lossyScale.xyz;
		v.vertex.xyz = GetMotion(v.texcoord.y, v.vertex,KHD_scale*KHD_strandLength, normalize(mul(unity_ObjectToWorld, v.tangent).xyz));
	}
	else
	{
		//v 1.2.0
		//v.vertex.xyz = GetMotion(v.texcoord.y, v.vertex, v.color.a, normalize(mul(unity_ObjectToWorld, v.tangent).xyz));
		
		//v >1.2.0		
		v.vertex.xyz = GetMotion(v.texcoord.y, v.vertex, v.color.a*KHD_maxStrandLength, normalize(mul(unity_ObjectToWorld, v.tangent).xyz));
		
	}
}


struct HairInput
{
	float2 uv_MainTex;
	float3 viewDir;
	float4 color;
};

void HairVert(inout appdata_full v, out HairInput o)
{
	UNITY_INITIALIZE_OUTPUT(HairInput, o);
	HairDesigner(v);
	o.color = v.color;
	float3 viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);

}

#endif