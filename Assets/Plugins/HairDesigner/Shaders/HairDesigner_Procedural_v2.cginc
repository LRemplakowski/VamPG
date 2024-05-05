//#ifndef HAIR_DESIGNER_PROCEDURAL_INCLUDED
//#define HAIR_DESIGNER_PROCEDURAL_INCLUDED



struct Input
{	
	float2 uv_MainTex;	
	float3 viewDir;
	float4 color;
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
float _Wave1;
float _WavePower1;
float _WaveRnd1;
float _Wave2;
float _WavePower2;
float _WaveRnd2;
float _WaveDir;
float _Chaos;
float _Cut;
float _Stripes;
float _Emission;
float _ColorBias;
float _ColorPower;
float _NormalMode;
float _NormalPower;
float _AlphaCutoff;
float _Alpha;
float _HairLength = 1;
float _HairWidth = 1;
float _RndLength = 0;
float _UVX = 1;
float _RootThickness;
float _TipThickness;
float _TipWidth;
float _HairPerPass;
float _RndSeed;



#define f3_f(c) (dot(round((c) * 255), float3(65536, 256, 1)))
#define f_f3(f) (frac((f) / float3(16777216, 65536, 256)))



void StrandValueCurv(float seed, float nbHair, float2 uv, float2 wave1, float2 wave2, float chaos, float scale, float3 viewDir, out float strandFactor, out float3 strandNormal, out float strandColor)
{

	half rnd = KHD_editor == 0 ? seed : KHD_scale*KHD_strandLength/ KHD_maxStrandLength;
	
	/*
	strandFactor = rnd;
	strandColor = 1;
	strandNormal = float3(0, 1, 0);
	return;
	*/
	//rnd = round(rnd*255.0)/255.0;//fix some weird rendering issue	
	rnd += _RndSeed;
	uv.x *= _UVX;
	int hairCount = ceil(nbHair);

	half res = -1;
	half3 n = half3(0, 0, 0);
	
	_TipThickness *= TRANSPARENT_PASS == 0 ? .8:1;
	_HairWidth *= lerp(1, _TipWidth, uv.y);

	//Thickness
	//_HairWidth = uv.y > _RootThickness ? _HairWidth : _HairWidth* lerp(abs(sin(uv.x*pi / _RootThickness))*(uv.y / _RootThickness), 1, (uv.y / _RootThickness));
	_HairWidth = uv.y > _RootThickness ? _HairWidth : _HairWidth * lerp(uv.y / _RootThickness, 1, (uv.y / _RootThickness));
	_HairWidth = uv.y < (1 - _TipThickness) ? _HairWidth : _HairWidth*lerp(1,(1-uv.y)*(1-uv.y), uv.y - (1 - _TipThickness));
	
	
	//_HairWidth *= TRANSPARENT_PASS==0?.9:1;
	_HairWidth /= 10;
	_HairWidth *= _HairPerPass;
		
	_HairLength *= TRANSPARENT_PASS == 0 ? 1-_AlphaCutoff : 1;
	_HairLength = clamp(_HairLength, 0, 1);

	float uvX = uv.x;
	
	for (int i = 1; i <= hairCount; ++i)
	{
		half hairDir = sign(RND(rnd) - _WaveDir);
		
		float2 w1 = (1-rnd*_WaveRnd1)*wave1;
		rnd = RND(rnd);
		float2 w2 = (1-rnd*_WaveRnd2)*wave2;
		rnd = RND(rnd);
		
		uv.x = uvX + hairDir*sin(uv.y*pi * w1.x*uv.y) * w1.y * (1 - sqrt(uv.y)) + hairDir*cos(uv.y*pi * w2.x*uv.y) * w2.y * (1 - sqrt(uv.y));//wave

		rnd = RND(rnd + i);		
		half2 SP = half2(rnd*.8 + .1, 0);
		rnd = RND(rnd);		
		half2 EP = half2(rnd*.8 + .1, 0);
		EP = lerp(SP, EP, chaos);
		//wind param
		EP.x += cos(rnd*_Time.y*KHD_windFactor) *min(KHD_windFactor, .1);
		
		rnd = RND(rnd);		
		half2 ST = half2(-.1*rnd*chaos, 0);
		half2 ET = half2(.1*rnd*chaos, 0);
		rnd = RND(rnd);
	

		half2 uv2 = Curv2D((uv.y*.9) , uv, SP, ST, EP, ET);
			
		half f = (1 - abs(sin(uv2.x * pi * _HairPerPass)) / (_HairWidth + .0001)) * 10;// *(9 + rnd*scale);		
		f *= (1 - (uv.y))*_HairLength* lerp(1, RND(rnd)*RND(rnd), _RndLength);//random length
				
		
		float xf = (1 - (sin(uv2.x * pi* _HairPerPass * 2.0)) / (_HairWidth + .0001))*_NormalPower;
		half3 normal = normalize( lerp(half3(_NormalPower, 0, 1.01- _NormalPower), half3(0, _NormalPower, 1.01- _NormalPower), xf) );
		res = max(res, f);
		if (res == f )
		{
			n = max( n, normalize(normal) );
			strandColor = 1;
		}	
		
	}

	

	strandFactor = res>1 ? res : 0;
	strandFactor = uv.y > .98 ? 0 : strandFactor;
	strandNormal = normalize(n);
	
}



void vert(inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	HairDesigner(v);
	o.color = v.color;
	//o.color.a = v.texcoord1.x;
	//o.color.a = 10;
	float3 viewDir = normalize(_WorldSpaceCameraPos- mul(unity_ObjectToWorld, v.vertex).xyz);
	
}



void surf(Input IN, inout SurfaceOutputStandardSpecular o)
{
	float strandFactor = 0;
	float4 strandFactor4 = float4(0,0,0,0);
	float strandColor = 0;
	float3 n = float3(0, 0, 0);
	
	float2 uv = IN.uv_MainTex;
	//uv.x -= ddx(uv);
	//uv.y -= ddx(uv);

	StrandValueCurv(IN.color.a, _HairDensity, uv, float2(_Wave1, _WavePower1), float2(_Wave2, _WavePower2), _Chaos, 1, IN.viewDir, strandFactor4.x, n, strandColor);
	//StrandValueCurv(IN.color.a, _HairDensity, uv, float2(_Wave1, _WavePower1), float2(_Wave2, _WavePower2), _Chaos, 1, IN.viewDir, strandFactor4.y, n, strandColor);
	//StrandValueCurv(IN.color.a, _HairDensity, uv, float2(_Wave1, _WavePower1), float2(_Wave2, _WavePower2), _Chaos, 1, IN.viewDir, strandFactor, n, strandColor);
	//StrandValueCurv(IN.color.a, _HairDensity, uv, float2(_Wave1, _WavePower1), float2(_Wave2, _WavePower2), _Chaos, 1, IN.viewDir, strandFactor, n, strandColor);

	strandFactor = strandFactor4.x;// + strandFactor4.y;
	//strandFactor /= 2;
	
	clip(strandFactor-.0001);
	clip((1 - uv.y) - _Cut);// +(TRANSPARENT_PASS*_AlphaCutoff)*(1 - _Cut));
	
	
	float f = (uv.y + _ColorBias);
	o.Albedo = lerp(_StartColor, _EndColor, clamp(  pow(f, _ColorPower)  * (abs(cos(uv.y*uv.y*pi*_Stripes))), 0, 1));
	
	_NormalMode = 1;
	//n = lerp(n, lerp(n, -n, dot(IN.viewDir, n)), _NormalMode);
	
	o.Normal = n;
	
	o.Occlusion = 1 - (1 - uv.y*uv.y) * _AO;
	//o.Occlusion = 1 - (1 - strandFactor*.2) * _AO * _AO;
	//o.Occlusion = 1 - (1 - IN.uv_MainTex.y) * _AO * _AO;
	

	float dotViewDir = saturate(max(dot(normalize(IN.viewDir), n), dot(normalize(IN.viewDir), -n)));
	half rim = 1.0 - dotViewDir;
	//o.Specular = (clamp(_RimColor.rgb * min(pow(rim, _RimPower), 0.9), half3 (0, 0, 0), half3 (1, 1, 1)))*dotViewDir;// *(1 - TRANSPARENT_PASS);
	o.Specular = _RimColor.rgb * pow(rim, _RimPower);// *(1 - TRANSPARENT_PASS);
	
	o.Smoothness = _Smoothness;// *strandFactor;// *dot(IN.viewDir, n);// *(1 - TRANSPARENT_PASS);

	o.Alpha = strandFactor*strandFactor* ( (1-min(_AlphaCutoff, uv.y))*.5) *_Alpha;
	o.Emission = o.Albedo * _Emission;
}


half4 LightingTwoSided(SurfaceOutputStandardSpecular s, half3 lightDir, half atten) {
	half NdotL = dot(s.Normal, lightDir);
	half INdotL = dot(-s.Normal, lightDir);
	half diff = (NdotL < 0) ? INdotL : NdotL;
	diff = _TwoSidedLight==1 ? diff : NdotL;

	half4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ( max(diff , s.Emission) * atten);
	c.a = s.Alpha;

	return c;
}

//#endif
