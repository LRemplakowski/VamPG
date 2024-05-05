
//#pragma exclude_renderers d3d11 gles
#define PI 3.14159265359
#define DIFFUSE 0.0
#define NORMAL 1.0
#define AO 2.0
#define SPECULAR 3.0
#define ALPHA -1.0

int _hairCount;
float4 _colorsRoot[3];
float4 _colorsTip[3];
float _colorsBias[3];
int _nbColors;
float _rnd;
float2 _width;
float2 _length;
float4 _wave0;
float4 _wave1;
float4 _wave2;
float _textureID;
float2 _rootOffset;
float _rootThickness;
float4 _noise;
float4 _scale;
float _ao;
float _specularRoot;
float _specularTip;
float3 _specularShift;
float3 _specularMask;
float _specularMode;
float4 _depth;
float _test;

/*
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
*/

float2 hash(float2 p)
{
    p = float2(dot(p, float2(127.1, 311.7)),
		dot(p, float2(269.5, 183.3)));

    return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
}

float noise(in float2 p)
{
    const float K1 = 0.366025404; // (sqrt(3)-1)/2;
    const float K2 = 0.211324865; // (3-sqrt(3))/6;

    float2 i = floor(p + (p.x + p.y) * K1);

    float2 a = p - i + (i.x + i.y) * K2;
    float2 o = step(a.yx, a.xy);
    float2 b = a - o + K2;
    float2 c = a - 1.0 + 2.0 * K2;

    float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
    float3 n = h * h * h * h * float3(dot(a, hash(i + 0.0)), dot(b, hash(i + o)), dot(c, hash(i + 1.0)));
    return dot(n, float3(70.0, 70.0, 70.0));
}


/*
float RND(float seed) {
	seed = round(seed * 65536) / 65536;//fix
	return abs(frac(sin(dot(float2(seed, seed), float2(12.9898, 78.233))) * 43758.5453));
}*/
float RND(float seed)
{
	
    return abs(frac(sin(1000.0 * round(seed * 1000.0) / 1000.0)));
}



float4 DrawHair(float2 uv, float id, float rndId, float rOffset, bool drawBG)
{
	//uv.y = uv.y*.95 + .025;//avoid unclamp problem

	//uv.y /= 1-_rootOffset.y;
    uv.y -= rOffset;
    float offset = 2;
	
	//---------------------------------------
    float xScaleRoot = 1.0 / _scale.y;
    float xScaleTip = 1.0 / _scale.z;
    float xScaleFactor = _scale.w;
	

	//---------------------------------------
	
    float rnd = RND(rndId);
    rnd = RND(rnd);



	//-----------------------------------------------------
	
    float2 uvCenter = float2(uv.x - .5, uv.y - .5);
    float a = atan2(uvCenter.x, uvCenter.y);
    float r = sqrt(length(uvCenter)) * .5;
    float2 uvRadial = float2(a / (PI * 2.0), r);
	
	//uv.y += uvRadial.y ;
	//uv.x += uvRadial.x ;



	//scale
    uv.y *= 1 / _scale.x;

		

	//-----------------------------------------------------

	

	//strand waves
    uv.x += sin(_wave0.x * PI * uv.y) * _wave0.y * uv.y;

    float2 l = clamp(_length - float2(rOffset, rOffset), 0, 1);
    uv.y /= rnd * (l.y - l.x) + l.x;

	//uv.y /= rnd*(_length.y - _length.x) + _length.x;
	
	
    uv.y = clamp(uv.y, 0, 1);

    float xScale = lerp(xScaleRoot, xScaleTip, uv.y);
    uv.x = uv.x * xScale - xScale * .5 + .5;
	

	//hair waves
    rnd = RND(rnd);
    float wave = (rnd * (_wave1.y - _wave1.x) + _wave1.x) * _wave1.w;

    float waveSin = sin(uv.y * PI * wave);
    float waveOffset = waveSin * _wave1.z * _wave1.z * _wave1.z * (uv.y * uv.y);

    rnd = RND(rnd);
    wave = (rnd * (_wave2.y - _wave2.x) + _wave2.x) * _wave2.w;
    waveSin += sin(uv.y * PI * wave);
    waveOffset += sin(uv.y * PI * wave) * _wave2.z * _wave2.z * _wave2.z * (uv.y * uv.y);

    uv.x += waveOffset;


    float width = lerp(0, _width.y * .1, (1 - lerp(uv.y * uv.y * uv.y, uv.y, 1 - _width.x))) * (drawBG ? 10.0 : 1.0);


    width *= lerp(0, 1, uv.y < rOffset ? uv.y - rOffset : 1); //clamp root min
    width *= lerp(0, 1, uv.y < pow(uv.y + rOffset, 1 - _rootThickness) ? uv.y / pow(uv.y + rOffset, 1 - _rootThickness) : 1);
    width = clamp(width, 0, 1);
	
    rnd = RND(rnd);
    float center = (rnd * .8 + .1);
	//center = id * .1;
	
	//float f = sin(uv.x*PI) * (1-uv.y);
    float f = 0;

    if (abs(uv.x - center) < width)
    {
        f = abs(uv.x - center) / width;
        f = 1 - f * f * f;
    }
	
    float nFactor = (uv.x - center) / width;
    float4 normal = lerp(float4(0.5, 1, 1, 1), float4(1, 0.5, 1, 1), nFactor * .5 + .5);
	//normal.z = lerp(.4, .6, noise(float2(uv.x * 100, uv.y * 4)));
    normal.xyz = normalize(normal.xyz);

    float alpha = f > 0 ? lerp(f, 0, pow(uv.y, 5)) : 0;
    float4 c = float4(f, f, f, alpha);

	//c.a = 1;


    if (_textureID == ALPHA)
    {
        c.r = c.g = c.b = c.a = alpha * alpha;
    }


    if (_textureID == DIFFUSE)
    {
        int cId = fmod(id, _nbColors);
		//c = lerp(_colorsTip[cId], _colorsRoot[cId], 1-pow(uv.y, 10-_colorsBias[cId]));
        c = lerp(_colorsRoot[cId], _colorsTip[cId], clamp(uv.y * uv.y - .5 + _colorsBias[cId], 0, 1));
        c *= 1 - clamp((noise(float2(uv.x * _noise.x, uv.y * _noise.y))) * _noise.z, 0, 1);

        c.rgb *= lerp(1.0 - _depth.x, 1.0, id / _hairCount);
		//c.rgb *= clamp(waveOffset,0,1)*.2+.8;
		
		
        c.a *= alpha;
    }

    if (_textureID == NORMAL)
    {
        c.rgb = c.a > 0 ? normal.rgb : float3(0.5, 0.5, 1);
		
    }
	
    if (_textureID == AO)
    {
        c.rgb = (1 - _noise.w * (1 - uv.y) + noise(float2(uv.x * _noise.x, uv.y * _noise.y)) * _noise.w * (1 - uv.y));
        c.rgb = lerp(1, c.rgb - _ao * (1 - uv.y), _ao);
        c.rgb *= lerp(1.0 - _depth.y, 1.0, id / _hairCount);
    }

	

    if (_textureID == SPECULAR)
    {
		//specular
        c.r = lerp(_specularRoot, _specularTip, uv.y);
		//c.r = (1 - _noise.w*(1 - uv.y) + noise(float2(uv.x * _noise.x, uv.y * _noise.y)) * _noise.w*(1 - uv.y));
		
		//shift
        c.g = 1 - noise(float2(uv.x * _specularShift.x, uv.y * _specularShift.y)); // *sin(10 * PI * uv.y);
        c.g = lerp(.5, c.g, _specularShift.z);
																				  //c.g = sin(1 * PI * uv.y);
		//c.a = 1;

		//Mask
        c.b = 1 - noise(float2(uv.x * _specularMask.x, uv.y * _specularMask.y)) + noise(float2(uv.x * _specularMask.x * .5, uv.y * _specularMask.y * 2.0));
        c.b = lerp(.5, c.b, _specularMask.z);

        if (_specularMode == 1)
            c.rgb = c.rrr;
        if (_specularMode == 2)
            c.rgb = c.ggg;
        if (_specularMode == 3)
            c.rgb = c.bbb;
		


    }

	

    return c;
}





float4 Diffuse(float2 uv)
{
    float4 c = float4(0.5, 0.5, 0.5, 0);

    if( _textureID != NORMAL )
    {
        for (float i = 0; i < _hairCount; ++i)
        {
            float rOffset = (1 - ((float) (i + 1) / (float) _hairCount)) * (_rootOffset.y - _rootOffset.x) + _rootOffset.x;
            float4 c2 = DrawHair(uv, i, i + RND(i + _rnd), rOffset, true);
            //c = c2.a > 0 ? c2 : c;
            c = c.a*.6 <= c2.a ? c2 : lerp(c, c2, c2.a / (c2.a + c.a));
        }
       
        c.a = 0;        
    }	
    else
    {
        c = float4(0.5, 0.5, 1, 1.0);
    }
	
    for (float i = 0; i < _hairCount; ++i)
    {
		
		
        float rOffset = (1 - ((float) (i + 1) / (float) _hairCount)) * (_rootOffset.y - _rootOffset.x) + _rootOffset.x;
        float4 c2 = DrawHair(uv, i, i + RND(i + _rnd), rOffset, false);
        c = (c.a * .6 <= c2.a) ? c2 : lerp(c, c2, c2.a / (c2.a + c.a));
    }
    return c;
}




