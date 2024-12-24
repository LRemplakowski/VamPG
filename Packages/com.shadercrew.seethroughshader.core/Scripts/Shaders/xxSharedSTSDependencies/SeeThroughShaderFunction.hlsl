#ifndef SEETHROUGHSHADER_FUNCTION
#define SEETHROUGHSHADER_FUNCTION


#ifndef UNITY_MATRIX_I_M
        #define UNITY_MATRIX_I_M   unity_WorldToObject
#endif






void DoSeeThroughShading(
                                    float3 l0,
                                    float3 ll0, float3 lll0,
                                    float3 llll0, float4 lllll0,
                                    float llllll0, float lllllll0, float llllllll0, float lllllllll0,
                                    float llllllllll0, float lllllllllll0,
                                    float llllllllllll0,
                                    half lllllllllllll0, half llllllllllllll0,
                                    float4 lllllllllllllll0, float llllllllllllllll0, float lllllllllllllllll0,
                                    float llllllllllllllllll0, float lllllllllllllllllll0, float llllllllllllllllllll0, float lllllllllllllllllllll0,
                                    bool llllllllllllllllllllll0,
                                    float lllllllllllllllllllllll0,
                                    float llllllllllllllllllllllll0,
                                    float lllllllllllllllllllllllll0, float llllllllllllllllllllllllll0,
                                    float lllllllllllllllllllllllllll0, float llllllllllllllllllllllllllll0,
                                    float lllllllllllllllllllllllllllll0, float llllllllllllllllllllllllllllll0,
                                    float lllllllllllllllllllllllllllllll0, float l1,
                                    float ll1,
                                    float lll1,
                                    float llll1,
                                    float lllll1,
                                    float llllll1, float lllllll1,
                                    float llllllll1, float lllllllll1, float llllllllll1,
                                    float lllllllllll1, float llllllllllll1, float lllllllllllll1, float llllllllllllll1, float lllllllllllllll1, float llllllllllllllll1,
                                    float lllllllllllllllll1, float llllllllllllllllll1, float lllllllllllllllllll1, float llllllllllllllllllll1, float lllllllllllllllllllll1, float llllllllllllllllllllll1,
                                    float lllllllllllllllllllllll1, float llllllllllllllllllllllll1,
                                    float lllllllllllllllllllllllll1,
                                    bool llllllllllllllllllllllllll1,
                                    float lllllllllllllllllllllllllll1, float llllllllllllllllllllllllllll1, float lllllllllllllllllllllllllllll1, float llllllllllllllllllllllllllllll1,
                                    float lllllllllllllllllllllllllllllll1, float l2,
                                    float ll2,
                                    float lll2,
#ifdef USE_UNITY_TEXTURE_2D_TYPE
                                    UnityTexture2D llll2,
                                    UnityTexture2D lllll2,
                                    UnityTexture2D llllll2,
#else
                                    sampler2D llll2,
                                    sampler2D lllll2,
                                    sampler2D llllll2,
                                    float4 lllllll2,
                                    float4 llllllll2,
                                    float4 lllllllll2,
#endif
                                    out half3 llllllllll2,
                                    out half3 lllllllllll2,
                                    out float llllllllllll2
)
{
    ShaderData d;
    d.worldSpaceNormal = llll0;
    d.worldSpacePosition = lll0;
    float3 lllllllllllll2 = float3(0, 0, 0);
#ifdef _HDRP
        lllllllllllll2 = mul(UNITY_MATRIX_I_M, float4(GetCameraRelativePositionWS(d.worldSpacePosition), 1)).xyz;
#else
    lllllllllllll2 = mul(UNITY_MATRIX_I_M, float4(d.worldSpacePosition, 1)).xyz;
#endif
    Surface o;
    o.Normal = ll0;
    o.Albedo = half3(0, 0, 0) + l0;
    o.Emission = half3(0, 0, 0);
    llllllllll2 = half3(0, 0, 0);
    lllllllllll2 = half3(0, 0, 0);
    llllllllllll2 = 1;
    float llllllllllllll2 = _Time.y;
    if (llllllllllllllllllllllllll1)
    {
        llllllllllllll2 = _STSCustomTime;
    }
    bool lllllllllllllll2 = (llllll0 > 0 || lllllll0 == -1 && llllllllllllll2 - llllllll0 < lllllllllllllllllllllllll1) || (llllll0 >= 0 && lllllll0 == 1);
    bool llllllllllllllll2 = !llllllllll0 && !lllllllllll0;
    float lllllllllllllllll2 = 0;
    half4 llllllllllllllllll2 = half4(0, 0, 0, 0);
    if (!llllllllllll0 && (lllllllllllllll2 || llllllllllllllll2))
    {
        float4 lllllllllllllllllll2 = float4(0, 0, 0, 0);
        float4 llllllllllllllllllll2 = float4(0, 0, 0, 0);
        float4 lllllllllllllllllllll2 = float4(0, 0, 0, 0);
#ifdef USE_UNITY_TEXTURE_2D_TYPE
        lllllllllllllllllll2 = llll2.texelSize;
        lllllllllllllllllllll2 = llllll2.texelSize;
        llllllllllllllllllll2 = lllll2.texelSize;
#else
        lllllllllllllllllll2 = lllllll2;
        lllllllllllllllllllll2 = lllllllll2;
        llllllllllllllllllll2 = llllllll2;
#endif
        if (lllll1 < 0)
        {
            lllll1 = 0;
        }
        half llllllllllllllllllllll2 = 0;
        if (lllllllllllll0 == 0) 
        {
            if (llllllllllllll0 == 0 || llllllllllllll0 == 1)
            {
                float3 lllllllllllllllllllllll2 = float3(0, 0, 0);
                if (llllllllllllll0 == 0) 
                {
                    lllllllllllllllllllllll2 = lllllllllllll2 / (-1.0 * abs(lllllllllllllllll0));
                }
                else 
                {
                    lllllllllllllllllllllll2 = d.worldSpacePosition / (-1.0 * abs(lllllllllllllllll0));
                }
                if (lllllllllllllllllllllll1)
                {
                    lllllllllllllllllllllll2 = lllllllllllllllllllllll2 + abs(((llllllllllllll2) * llllllllllllllllllllllll1));
                }
                float3 llllllllllllllllllllllll2 = tex2D(llll2, lllllllllllllllllllllll2.yz).rgb;
                float3 lllllllllllllllllllllllll2 = tex2D(llll2, lllllllllllllllllllllll2.xz).rgb;
                float3 llllllllllllllllllllllllll2 = tex2D(llll2, lllllllllllllllllllllll2.xy).rgb;
                float lllllllllllllllllllllllllll2 = abs(d.worldSpaceNormal.x);
                float llllllllllllllllllllllllllll2 = abs(d.worldSpaceNormal.z);
                float3 lllllllllllllllllllllllllllll2 = lerp(lllllllllllllllllllllllll2, llllllllllllllllllllllll2, lllllllllllllllllllllllllll2).rgb;
                float3 llllllllllllllllllllllllllllll2 = lerp(lllllllllllllllllllllllllllll2, llllllllllllllllllllllllll2, llllllllllllllllllllllllllll2).rgb;
                llllllllllllllllllllll2 = llllllllllllllllllllllllllllll2.r;
            }
            else if (llllllllllllll0 == 2) 
            {
                float2 lllllllllllllllllllllllllllllll2 = lllll0.xy / max(0.01, lllll0.w);
                if (lllllllllllllllllllllll1)
                {
                    lllllllllllllllllllllllllllllll2 = lllllllllllllllllllllllllllllll2 + abs(((llllllllllllll2) * llllllllllllllllllllllll1));
                }
                float2 l3 = lllllllllllllllllllllllllllllll2 * _ScreenParams.xy;                
                float2 ll3 = frac(l3 * lllllllllllllllllll2.xy); 
                float lll3 = tex2D(llll2, ll3 * abs(lllllllllllllllll0)).r;
                llllllllllllllllllllll2 = lll3;
            }
        }
        else if (lllllllllllll0 == 1) 
        {
            float4 llll3 = float4(lllll0.xy / max(0.01, lllll0.w), 0, 0);
            float2 lllll3 = llll3 * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0, 9.0 / 17.0, 3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0, 5.0 / 17.0, 15.0 / 17.0, 7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0, 2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0, 8.0 / 17.0, 14.0 / 17.0, 6.0 / 17.0
            };
            uint llllllllllllllllll7 = (uint(lllll3.x) % 4) * 4 + uint(lllll3.y) % 4;
            llllllllllllllllllllll2 = DITHER_THRESHOLDS[llllllllllllllllll7];
        }
        else 
        {
            llllllllllllllllllllll2 = 0.5; 
        }
        float3 lllllll3 = UNITY_MATRIX_V[2].xyz;
#ifdef _HDRP
                lllllll3 =  mul(UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2]).xyz;
#else
        lllllll3 = mul(UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V))[2]).xyz;
#endif
        float llllllll3 = 0;
        float lllllllll3 = 0;
        float llllllllll3 = 1;
        bool lllllllllll3 = false;
        float llllllllllll3 = 0;
        float lllllllllllll3 = 0;
        float llllllllllllll3 = 0;
        float lllllllllllllll3 = 0;
        float llllllllllllllll3 = 0;
        float lllllllllllllllll3 = 0;
#if defined(_ZONING)  
                if(lllllllllllllllllllllllllll1) {
                    float llllllllllllllllll3 = 0;
                    for (int z = 0; z < _ZonesDataCount; z++){
                        bool lllllllllllllllllll3 = false;
                        float llllllllllllllllllll3 = llllllllllllllllll3;
                        if (_ZDFA[llllllllllllllllll3 + 1] == 0) { 
#if !_EXCLUDE_ZONEBOXES
                            float lllllllllllllllllllll3 = llllllllllllllllll3 + 2; 
                            float3 llllllllllllllllllllll3 = d.worldSpacePosition - float3(_ZDFA[lllllllllllllllllllll3],_ZDFA[lllllllllllllllllllll3+1], _ZDFA[lllllllllllllllllllll3+2]);
                            float3 lllllllllllllllllllllll3 =     float3(_ZDFA[lllllllllllllllllllll3+ 3],_ZDFA[lllllllllllllllllllll3+ 4], _ZDFA[lllllllllllllllllllll3+ 5]);
                            float3 llllllllllllllllllllllll3 =     float3(_ZDFA[lllllllllllllllllllll3+ 6],_ZDFA[lllllllllllllllllllll3+ 7], _ZDFA[lllllllllllllllllllll3+ 8]);
                            float3 lllllllllllllllllllllllll3 =     float3(_ZDFA[lllllllllllllllllllll3+ 9],_ZDFA[lllllllllllllllllllll3+10], _ZDFA[lllllllllllllllllllll3+11]);
                            float3 llllllllllllllllllllllllll3 = float3(_ZDFA[lllllllllllllllllllll3+12],_ZDFA[lllllllllllllllllllll3+13], _ZDFA[lllllllllllllllllllll3+14]);
                            float lllllllllllllllllllllllllll3 = abs(dot(llllllllllllllllllllll3, lllllllllllllllllllllll3));
                            float llllllllllllllllllllllllllll3 = abs(dot(llllllllllllllllllllll3, llllllllllllllllllllllll3));
                            float lllllllllllllllllllllllllllll3 = abs(dot(llllllllllllllllllllll3, lllllllllllllllllllllllll3));
                            lllllllllllllllllll3 =    lllllllllllllllllllllllllll3 <= llllllllllllllllllllllllll3.x &&
                                        llllllllllllllllllllllllllll3 <= llllllllllllllllllllllllll3.y &&
                                        lllllllllllllllllllllllllllll3 <= llllllllllllllllllllllllll3.z;
                            if(lllllllllllllllllll3 && lllllllllllllllll1 == 1 && lllllllllllllllllllllllllllllll1) {
                                llllllllllllll3 = _ZDFA[lllllllllllllllllllll3+1] - _ZDFA[lllllllllllllllllllll3+13];  
                                if(llllllllllllllllll1 == 0) {                                    
                                    bool llllllllllllllllllllllllllllll3 = ((llllllllllllll3 - l2)  <= lllllllllllllllllll1); 
                                    if(!llllllllllllllllllllllllllllll3) {
                                        lllllllllllllllllll3 = false;
                                    }
                                }
                            }
                            if(lllllllllllllllllll3) {
                                float lllllllllllllllllllllllllllllll3 = llllllllllllllllllllllllll3.x - lllllllllllllllllllllllllll3;
                                float l4 = llllllllllllllllllllllllll3.y - llllllllllllllllllllllllllll3;
                                float ll4 = llllllllllllllllllllllllll3.z - lllllllllllllllllllllllllllll3;
                                float lll4 = min(l4,lllllllllllllllllllllllllllllll3);
                                lll4 = min(lll4,ll4);
                                lllllllllllll3 = max(lll4,lllllllllllll3);
                                if(lll4<0) {
                                    lllllllllllll3 = 0;
                                }
                            }
                            if (lllllllllllllllllll3)
                            {
                                if (lllllllllll3 == false)
                                {
                                    llllllllllll3 = _ZDFA[llllllllllllllllllll3];
                                    lllllllllll3 = true;
                                    lllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 17];
                                    lllllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 18];
                                    llllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 19];
                                }                     
                            }
#endif        
                            llllllllllllllllll3 = llllllllllllllllll3 + 17 + 3;
                        } else if (_ZDFA[llllllllllllllllll3 + 1] == 1) { 
#if !_EXCLUDE_ZONESPHERES
                            float llll4 = llllllllllllllllll3 + 2; 
                            float3 lllll4 = float3(_ZDFA[llll4], _ZDFA[llll4 + 1], _ZDFA[llll4 + 2]);
                            float llllll4 = _ZDFA[llll4 + 3];
                            float lllllll4 = distance(d.worldSpacePosition, lllll4);
                            lllllllllllllllllll3 = lllllll4 < llllll4;
                            if (lllllllllllllllllll3 && lllllllllllllllll1 == 1 && lllllllllllllllllllllllllllllll1)
                            {
                                llllllllllllll3 = _ZDFA[llll4 + 1] - _ZDFA[llll4 + 3];
                                if (llllllllllllllllll1 == 0)
                                {
                                    bool llllllllllllllllllllllllllllll3 = ((llllllllllllll3 - l2) <= lllllllllllllllllll1);
                                    if (!llllllllllllllllllllllllllllll3)
                                    {
                                        lllllllllllllllllll3 = false;
                                    }
                                }
                            }
                            if (lllllllllllllllllll3)
                            {
                                if (lllllllllll3 == false)
                                {
                                    llllllllllll3 = _ZDFA[llllllllllllllllllll3];
                                    lllllllllll3 = true;
                                    lllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 6];
                                    lllllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 7];
                                    llllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 8];
                                }
                            }
                            if (lllllllllllllllllll3)
                             {
                                float lll4 = max(0, (llllll4 - lllllll4));
                                lllllllllllll3 = max(lll4, lllllllllllll3);
                            }
#endif
                            llllllllllllllllll3 = llllllllllllllllll3 + 6 + 3;
                        } else if (_ZDFA[llllllllllllllllll3 + 1] == 2) { 
#if !_EXCLUDE_ZONECYLINDERS
                            float llllllllll4 = llllllllllllllllll3 + 2;
                            float3 lllllllllll4 = float3(_ZDFA[llllllllll4], _ZDFA[llllllllll4 + 1], _ZDFA[llllllllll4 + 2]);
                            float3 llllllllllll4 = float3(_ZDFA[llllllllll4 + 3], _ZDFA[llllllllll4 + 4], _ZDFA[llllllllll4 + 5]);
                            float lllllllllllll4 = dot(d.worldSpacePosition.xyz - lllllllllll4, llllllllllll4);
                            float llllllllllllll4 = _ZDFA[llllllllll4 + 6];
                            float lllllllllllllll4 = _ZDFA[llllllllll4 + 7];
                            float llllllllllllllll4 = length((d.worldSpacePosition.xyz - lllllllllll4) - lllllllllllll4 * llllllllllll4);
                            lllllllllllllllllll3 = (abs(lllllllllllll4) < lllllllllllllll4/2) && (llllllllllllllll4 < llllllllllllll4);
                            if (lllllllllllllllllll3)
                            {
                                if (lllllllllll3 == false)
                                {
                                    llllllllllll3 = _ZDFA[llllllllllllllllllll3];
                                    lllllllllll3 = true;
                                    lllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 10];
                                    lllllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 11];
                                    llllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 12];
                                }
                            }
                            if (lllllllllllllllllll3)
                            {
                                float lll4 = max(0, (llllllllllllll4 - llllllllllllllll4));
                                lll4 = min(lll4, (lllllllllllllll4/2 - abs(lllllllllllll4)));
                                lllllllllllll3 = max(lll4, lllllllllllll3);
                            }
#endif
                            llllllllllllllllll3 = llllllllllllllllll3 + 10 + 3;
                        }
                        else if (_ZDFA[llllllllllllllllll3 + 1] == 3) { 
#if !_EXCLUDE_ZONECONES
                            float llllllllll4 = llllllllllllllllll3 + 2;
                            float3 lllllllllll4 = float3(_ZDFA[llllllllll4], _ZDFA[llllllllll4 + 1], _ZDFA[llllllllll4 + 2]);
                            float3 llllllllllll4 = float3(_ZDFA[llllllllll4 + 3], _ZDFA[llllllllll4 + 4], _ZDFA[llllllllll4 + 5]);
                            float lllllllllllll4 = dot(d.worldSpacePosition.xyz - lllllllllll4, llllllllllll4);
                            float llllllllllllllllllllll4 = _ZDFA[llllllllll4 + 6];
                            float lllllllllllllllllllllll4 = _ZDFA[llllllllll4 + 7];
                            float3 llllllllllllllllllllllll4 = lllllllllll4 + (llllllllllll4 * lllllllllllllllllllllll4/2); 
                            float lllllllllllllllllllllllll4 = dot(llllllllllllllllllllllll4 - d.worldSpacePosition.xyz, llllllllllll4);
                            float llllllllllllllllllllllllll4 = (lllllllllllllllllllllllll4 / lllllllllllllllllllllll4) * llllllllllllllllllllll4;
                            float llllllllllllllll4 = length((llllllllllllllllllllllll4 - d.worldSpacePosition.xyz) - lllllllllllllllllllllllll4 * llllllllllll4);        
                            lllllllllllllllllll3 = (abs(lllllllllllll4) < lllllllllllllllllllllll4/2) && (llllllllllllllll4 < llllllllllllllllllllllllll4);
                            if (lllllllllllllllllll3)
                            {
                                if (lllllllllll3 == false)
                                {
                                    llllllllllll3 = _ZDFA[llllllllllllllllllll3];
                                    lllllllllll3 = true;
                                    lllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 10];
                                    lllllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 11];
                                    llllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 12];
                                }
                            }
                            if (lllllllllllllllllll3)
                            {
                                float lll4 = max(0, (llllllllllllllllllllllllll4 - llllllllllllllll4));
                                lll4 = min(lll4, (lllllllllllllllllllllll4 - lllllllllllllllllllllllll4));
                                lllllllllllll3 = max(lll4, lllllllllllll3);
                            }
#endif
                            llllllllllllllllll3 = llllllllllllllllll3 + 10 + 3;
                        }
                        else if (_ZDFA[llllllllllllllllll3 + 1] == 4) { 
#if !_EXCLUDE_ZONEPLANES
                            float lllllllllllllllllllllllllllll4 = llllllllllllllllll3 + 2;
                            float3 llllllllllllllllllllllllllllll4 = float3(_ZDFA[lllllllllllllllllllllllllllll4], _ZDFA[lllllllllllllllllllllllllllll4 + 1], _ZDFA[lllllllllllllllllllllllllllll4 + 2]);
                            float lllllllllllllllllllllllllllllll4 = _ZDFA[lllllllllllllllllllllllllllll4 + 3];       
                            float l5 = dot(d.worldSpacePosition.xyz, llllllllllllllllllllllllllllll4.xyz) + lllllllllllllllllllllllllllllll4;
                            lllllllllllllllllll3 = l5 < 0;
                            if (lllllllllllllllllll3)
                            {
                                if (lllllllllll3 == false)
                                {
                                    llllllllllll3 = _ZDFA[llllllllllllllllllll3];
                                    lllllllllll3 = true;
                                    lllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 6];
                                    lllllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 7];
                                    llllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 8];
                                }
                            }
                            if (lllllllllllllllllll3)
                            {
                                float lll4 = max(0, 0 - l5);
                                lllllllllllll3 = max(lll4, lllllllllllll3);
                            }
#endif
                            llllllllllllllllll3 = llllllllllllllllll3 + 6 + 3;
                        }
                }
            }
#endif
        float lll5 = 0;
        float llll5 = lllllllllll3;
#if !defined(_PLAYERINDEPENDENT)
#if defined(_ZONING)
                    if(lllllllllll3 && lllllllllllllllll1 == 1 && llllllllllllllllll1 == 1 && lllllllllllllllllllllllllllllll1) {
                        float lllll5 = 0;
                        bool llllll5 = false;
                        for (int i = 0; i < _ArrayLength; i++){
                            float lllllll5 = _PlayersDataFloatArray[lllll5+1]; 
                            float3 llllllll5 = _PlayersPosVectorArray[lllllll5].xyz - _WorldSpaceCameraPos;               
                            if(dot(lllllll3,llllllll5) <= 0) {       
                                if(!llllllllllllllll2) {
                                    float lllllllll5 = lllll5 + 3;
                                    float llllllllll5 = 4;
                                    for (int llllllllllll8 = 0; llllllllllll8 < _PlayersDataFloatArray[lllll5 + 2]; llllllllllll8++){
                                        float lllllllllll5 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5 + 2];
                                        if (lllllllllll5 != 0 && lllllllllll5 == lllllllll0) {
                                            float llllllllllll5 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5 ];
                                            float lllllllllllll5 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5 + 1];
                                            if ((lllllllllllll5 == -1 && llllllllllllll2 - llllllllllll5 < lllllllllllllllllllllllll1 )|| (lllllllllllll5 == 1) ) {
                                                float llllllllllllll5 = _PlayersPosVectorArray[lllllll5].y+ llllllllllllllllllll1;
                                                if(ll2) {
                                                    if(i==0) {
                                                        lll5 = llllllllllllll5;
                                                    } else {
                                                        lll5 = max(lll5,llllllllllllll5);
                                                    }
                                                }
                                                bool lllllllllllllll5 = llllllllllllll3 >= llllllllllllll5 + l2; 
                                                if(!lllllllllllllll5) {
                                                    llllll5 = true;
                                                } 
                                            }                        
                                        }
                                    }
                                } else if (lllllll1 == 0 || distance(_PlayersPosVectorArray[lllllll5].xyz, d.worldSpacePosition.xyz) < llllll1) {
                                    float llllllllllllll5 = _PlayersPosVectorArray[lllllll5].y+ llllllllllllllllllll1;
                                    if(ll2) {
                                        if(i==0) {
                                            lll5 = llllllllllllll5;
                                        } else {
                                            lll5 = max(lll5,llllllllllllll5);
                                        }
                                    }
                                    bool lllllllllllllll5 = llllllllllllll3 >= llllllllllllll5 + l2; 
                                    if(!lllllllllllllll5) {
                                        llllll5 = true;
                                    } 
                                }
                                lllll5 = lllll5 + _PlayersDataFloatArray[lllll5 + 2]*4 + 3; 
                                lllll5 = lllll5 + _PlayersDataFloatArray[lllll5]*4 + 1; 
                            }
                        }
                        if(!llllll5) {
                            lllllllllll3 = false;
                        }
                    }
#endif
        float lllll5 = 0;
        for (int i = 0; i < _ArrayLength; i++)
        {
            float lllllll5 = _PlayersDataFloatArray[lllll5 + 1];
            if (sign(_PlayersPosVectorArray[lllllll5].w) != -1) 
            {
                float3 llllllll5 = _PlayersPosVectorArray[lllllll5].xyz - _WorldSpaceCameraPos;
                float lllllllllllllllllllll5 = 0;
                float llllllllll5 = 4;
                if (!llllllllllllllll2)
                {
                    float lllllllll5 = lllll5 + 3;
                    for (int llllllllllll8 = 0; llllllllllll8 < _PlayersDataFloatArray[lllll5 + 2]; llllllllllll8++)
                    {
                        float lllllllllll5 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5 + 2];
                        if (lllllllllll5 != 0 && lllllllllll5 == lllllllll0)
                        {
                            float llllllllllll5 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5];
                            float lllllllllllll5 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5 + 1];
                            lllllllllllllllllllll5 = 1;
                            if (lllllllllllll5 != 0 && llllllllllll5 != 0 && llllllllllllll2 - llllllllllll5 < lllllllllllllllllllllllll1)
                            {
                                if (lllllllllllll5 == 1)
                                {
                                    lllllllllllllllllllll5 = ((lllllllllllllllllllllllll1 - (llllllllllllll2 - llllllllllll5)) / lllllllllllllllllllllllll1);
                                }
                                else
                                {
                                    lllllllllllllllllllll5 = ((llllllllllllll2 - llllllllllll5) / lllllllllllllllllllllllll1);
                                }
                            }
                            else if (lllllllllllll5 == -1)
                            {
                                lllllllllllllllllllll5 = 1;
                            }
                            else if (lllllllllllll5 == 1)
                            {
                                lllllllllllllllllllll5 = 0;
                            }
                            else
                            {
                                lllllllllllllllllllll5 = 1;
                            }
                            lllllllllllllllllllll5 = 1 - lllllllllllllllllllll5;
                        }
                    }
                }
                lllll5 = lllll5 + _PlayersDataFloatArray[lllll5 + 2] * 4 + 3;
                float lllllllllllllllllllllllllll5 = 0;
                float llllllllllllllllllllllllllll5 = 0;
                float lllllllllllllllllllllllllllll5 = 0;
                float llllllllllllllllllllllllllllll5 = llllllllllllllllllllllllllll5;
                bool lllllllllllllllllllllllllllllll5 = distance(_PlayersPosVectorArray[lllllll5].xyz, d.worldSpacePosition) > llllll1;
                if ((lllllllllllllllllllll5 != 0) || ((!llllllllll0 && !lllllllllll0) && (lllllll1 == 0 || !lllllllllllllllllllllllllllllll5)))
                {
#if defined(_ZONING)
                            if(lllllllllllllllllllllllllll1) {
                                if(lllllllllll3) 
                                {
                                    if(lllllllllllllllllllllllllllll1) {
                                        float lllllllll5 = lllll5 + 1;
                                        for (int llllllllllll8 = 0; llllllllllll8 < _PlayersDataFloatArray[lllll5]; llllllllllll8++){
                                            float lllllllllll5 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5 + 2];
                                            if (lllllllllll5 != 0 && lllllllllll5 == llllllllllll3) {
                                                float llllllllllll5 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5 ];
                                                float lllllllllllll5 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5 + 1];
                                                lllllllllllllllllllllllllll5 = 1;
                                                float lllll6 = _PlayersDataFloatArray[lllllllll5 + llllllllllll8 * llllllllll5 + 3];
                                                if( lllllllllllll5!= 0 && llllllllllll5 != 0 && llllllllllllll2-llllllllllll5 < lllll6) {
                                                    if(lllllllllllll5 == 1) {
                                                        lllllllllllllllllllllllllll5 = ((lllll6-(llllllllllllll2-llllllllllll5))/lllll6);
                                                    } else {
                                                        lllllllllllllllllllllllllll5 = ((llllllllllllll2-llllllllllll5)/lllll6);
                                                    }
                                                } else if(lllllllllllll5 ==-1) {
                                                    lllllllllllllllllllllllllll5 = 1;
                                                } else if(lllllllllllll5 == 1) {
                                                    lllllllllllllllllllllllllll5 = 0;
                                                } else {
                                                    lllllllllllllllllllllllllll5 = 1;
                                                }
                                                lllllllllllllllllllllllllll5 = 1 - lllllllllllllllllllllllllll5;
                                            }
                                            if(llllllllllllllllllllllllllll1 == 0 && lllllllllllllllllllllllllllll1) {
                                                float llllll6 = 1 / llllllllllllllllllllllllllllll1;
                                                if (lllllllllllll3 < llllllllllllllllllllllllllllll1)  {
                                                    float lllllll6 = ((llllllllllllllllllllllllllllll1-lllllllllllll3) * llllll6);
                                                    lllllllllllllllllllllllllll5 =  max(lllllllllllllllllllllllllll5,lllllll6);
                                                }
                                            }
                                        }
                                    } else { 
                                    }
                                } else {
                                }
                            }
#endif
                    if (dot(lllllll3, llllllll5) <= 0)
                    {
                        if (lllllllllllllllllllllll0 == 2 || lllllllllllllllllllllll0 == 3 || lllllllllllllllllllllll0 == 4 || lllllllllllllllllllllll0 == 5 || lllllllllllllllllllllll0 == 6 || lllllllllllllllllllllll0 == 7)
                        {
                            float4 llllllll6 = float4(0, 0, 0, 0);
                            float4 lllllllll6 = float4(0, 0, 0, 0);
                            float llllllllll6 = 0;
                            if (lll1 || lllllllllllllllllllllll0 == 6)
                            {
                                float lllllllllll6 = _ScreenParams.x / _ScreenParams.y;
#ifdef _HDRP
                                        float4 llllllllllll6 = mul(UNITY_MATRIX_VP, float4(GetCameraRelativePositionWS(_PlayersPosVectorArray[lllllll5].xyz), 1.0));
                                        lllllllll6 = ComputeScreenPos(llllllllllll6 , _ProjectionParams.x);
#else
                                float4 llllllllllll6 = mul(UNITY_MATRIX_VP, float4(_PlayersPosVectorArray[lllllll5].xyz, 1.0));
                                lllllllll6 = ComputeScreenPos(llllllllllll6);
#endif
                                lllllllll6.xy /= lllllllll6.w;
                                lllllllll6.x *= lllllllllll6;
#ifdef _HDRP
                                        float4 llllllllllllll6 = mul(UNITY_MATRIX_VP, float4(GetCameraRelativePositionWS(d.worldSpacePosition.xyz), 1.0));
                                        llllllll6 = ComputeScreenPos(llllllllllllll6 , _ProjectionParams.x);
#else
                                float4 llllllllllllll6 = mul(UNITY_MATRIX_VP, float4(d.worldSpacePosition.xyz, 1.0));
                                llllllll6 = ComputeScreenPos(llllllllllllll6);
#endif
                                llllllll6.xy /= llllllll6.w;
                                llllllll6.x *= lllllllllll6;
#if defined(_DISSOLVEMASK)
                                        if(lll1) {
                                                llllllllll6 = max(llllllllllllllllllll2.z,llllllllllllllllllll2.w);
                                        }
#endif
                            }
                            float3 llllllllllllllll6 = _WorldSpaceCameraPos - _PlayersPosVectorArray[lllllll5].xyz;
                            float3 lllllllllllllllll6 = normalize(llllllllllllllll6);
                            float lllllllllllll4 = dot(d.worldSpacePosition.xyz - _PlayersPosVectorArray[lllllll5].xyz, lllllllllllllllll6);
                            float lllllllllllllllllll6 = 0;
                            float llllllllllllllllllll6 = 0;
                            float2 lllllllllllllllllllll6 = float2(0, 0);
                            if (lllllllllllllllllllllll0 == 2 || lllllllllllllllllllllll0 == 3)
                            {
                                lllllllllllllllllll6 = lllllllllllllllllllllllll0;
                                float llllllllllllllll4 = length((d.worldSpacePosition.xyz - _PlayersPosVectorArray[lllllll5].xyz) - lllllllllllll4 * lllllllllllllllll6);
                                float lllllllllllllllllllllll4 = length(llllllllllllllll6);
                                float llllllllllllllllllllll4 = llllllllllllllllllllllllll0;
                                float llllllllllllllllllllllllll4 = (lllllllllllll4 / lllllllllllllllllllllll4) * llllllllllllllllllllll4;
#if _DISSOLVEMASK
                                        float llllllllllllllllllllllllll6 = (2*llllllllllllllllllllllllll4) / llllllllll6;
                                        float2 lllllllllllllllllllllllllll6 = llllllll6.xy - lllllllll6.xy;
                                        lllllllllllllllllllllllllll6 =  normalize(lllllllllllllllllllllllllll6)*llllllllllllllll4;
                                        lllllllllllllllllllll6 = lllllllllllllllllllllllllll6 /llllllllllllllllllllllllll6;
#else
                                float llllllllllllllllllllllllllll6 = llllllllllllllll4 < llllllllllllllllllllllllll4;
                                if (llllllllllllllllllllllllllll6)
                                {
                                    float lllllllllllllllllllllllllllll6 = llllllllllllllll4 / llllllllllllllllllllllllll4;
                                    llllllllllllllllllll6 = lllllllllllllllllllllllllllll6;
                                }
                                else
                                {
                                    llllllllllllllllllll6 = -1;
                                }
#endif
                            }
                            else if (lllllllllllllllllllllll0 == 4 || lllllllllllllllllllllll0 == 5)
                            {
                                lllllllllllllllllll6 = lllllllllllllllllllllllllll0;
                                float llllllllllllllll4 = length((d.worldSpacePosition.xyz - _PlayersPosVectorArray[lllllll5].xyz) - lllllllllllll4 * lllllllllllllllll6);
                                float llllllllllllll4 = llllllllllllllllllllllllllll0;
                                float l7 = (llllllllllllllll4 < llllllllllllll4) && lllllllllllll4 > 0;
#if _DISSOLVEMASK
                                        float llllllllllllllllllllllllll6 = (2*llllllllllllll4) / llllllllll6;
                                        float2 lllllllllllllllllllllllllll6 = llllllll6.xy - lllllllll6.xy;
                                        lllllllllllllllllllllllllll6 =  normalize(lllllllllllllllllllllllllll6)*llllllllllllllll4;
                                        lllllllllllllllllllll6 = lllllllllllllllllllllllllll6 /llllllllllllllllllllllllll6;
#else
                                if (l7)
                                {
                                    float lllllllllllllllllllllllllllll6 = llllllllllllllll4 / llllllllllllll4;
                                    llllllllllllllllllll6 = lllllllllllllllllllllllllllll6;
                                }
                                else
                                {
                                    llllllllllllllllllll6 = -1;
                                }
#endif
                            }
                            else if (lllllllllllllllllllllll0 == 6)
                            {
                                lllllllllllllllllll6 = lllllllllllllllllllllllllllll0;
                                float lllll7 = length(llllllllllllllll6);
                                float lllllllllll6 = _ScreenParams.x / _ScreenParams.y;
                                float lllllll7 = min(1, lllllllllll6);
                                float llllllll7 = distance(llllllll6.xy, lllllllll6.xy) < llllllllllllllllllllllllllllll0 / lllll7 * lllllll7;
                                float lllllllll7 = (llllllll7) && lllllllllllll4 > 0;
#if _DISSOLVEMASK
                                        float llllllllll7 = llllllllllllllllllllllllllllll0/lllll7*lllllll7;
                                        float llllllllllllllllllllllllll6 = (2*llllllllll7) / llllllllll6;
                                        float2 lllllllllllllllllllllllllll6 = llllllll6.xy - lllllllll6.xy;
                                        lllllllllllllllllllll6 = lllllllllllllllllllllllllll6 /llllllllllllllllllllllllll6;
#else
                                if (lllllllll7)
                                {
                                    float lllllllllllll7 = (distance(llllllll6.xy, lllllllll6.xy) / (llllllllllllllllllllllllllllll0 / lllll7 * lllllll7));
                                    llllllllllllllllllll6 = lllllllllllll7;
                                }
                                else
                                {
                                    llllllllllllllllllll6 = -1;
                                }
#endif
                            }
                            else if (lllllllllllllllllllllll0 == 7)
                            {
#if _OBSTRUCTION_CURVE
                                        lllllllllllllllllll6 = lllllllllllllllllllllllllllllll0;
                                        float llllllllllllllll4 = length((d.worldSpacePosition.xyz  - _PlayersPosVectorArray[lllllll5].xyz) - lllllllllllll4 * lllllllllllllllll6);
                                        float lllll7 = length(llllllllllllllll6);
                                        float4 llllllllllllllll7 = float4(0,0,0,0);
                                        float lllllllllllllllll7 = lllllllllllllllllllll2.z;
                                        float llllllllllllllllll7 = (lllllllllllll4/lllll7) * lllllllllllllllll7;
                                        float4 lllllllllllllllllll7 = float4(0,0,0,0);
                                        lllllllllllllllllll7 = lllllllllllllllllllll2;
                                        float2 llllllllllllllllllll7 = (llllllllllllllllll7+0.5) * lllllllllllllllllll7.xy;
                                            llllllllllllllll7 = tex2D(llllll2, llllllllllllllllllll7);
                                        float lllllllllllllllllllll7 = llllllllllllllll7.r * l1;
                                        float llllllllllllllllllllll7 = (llllllllllllllll4 < lllllllllllllllllllll7) && lllllllllllll4 > 0 ;
#if _DISSOLVEMASK
                                            float llllllllllllllllllllllllll6 = (2*lllllllllllllllllllll7) / llllllllll6;
                                            float2 lllllllllllllllllllllllllll6 = llllllll6.xy - lllllllll6.xy;
                                            lllllllllllllllllllllllllll6 =  normalize(lllllllllllllllllllllllllll6)*llllllllllllllll4;
                                            lllllllllllllllllllll6 = lllllllllllllllllllllllllll6 /llllllllllllllllllllllllll6;
#else
                                            if(llllllllllllllllllllll7){
                                                float lllllllllllllllllllllllllllll6 = llllllllllllllll4/lllllllllllllllllllll7;
                                                llllllllllllllllllll6 = lllllllllllllllllllllllllllll6;
                                            } else {
                                                llllllllllllllllllll6 = -1;
                                            }
#endif
#endif
                            }
#if defined(_DISSOLVEMASK)
                                    if(lll1) {
                                        float4 llllllllllllllllllllllllll7 = float4(0,0,0,0);
                                        llllllllllllllllllllllllll7 = llllllllllllllllllll2;
                                        float2 lllllllllllllllllllllllllll7 = float2(llllllllllllllllllllllllll7.z/2,llllllllllllllllllllllllll7.w/2);
                                        float2 llllllllllllllllllllllllllll7 = lllllllllllllllllllllllllll7 + lllllllllllllllllllll6;
                                        float2 lllllllllllllllllllllllllllll7 = (llllllllllllllllllllllllllll7+0.5) * llllllllllllllllllllllllll7.xy;
                                        float4 llllllllllllllllllllllllllllll7 = float4(0,0,0,0);
                                            llllllllllllllllllllllllllllll7 = tex2D(lllll2, lllllllllllllllllllllllllllll7);
                                        float lllllllllllllllllllllllllllllll7 = -1;
                                        if(llllllllllllllllllllllllllll7.x <= llllllllllllllllllllllllll7.z && llllllllllllllllllllllllllll7.x >= 0 && llllllllllllllllllllllllllll7.y <= llllllllllllllllllllllllll7.w && llllllllllllllllllllllllllll7.y >= 0 && llllllllllllllllllllllllllllll7.x <= 0 && lllllllllllll4 > 0 ){
                                            float l8 = sqrt(pow(llllllllllllllllllllllllll7.z,2)+pow(llllllllllllllllllllllllll7.w,2))/2;
                                            float ll8 = 40;
                                            float lll8 = l8/ll8;
                                            float llll8 = 0;
                                            lllllllllllllllllllllllllllllll7 = 0;     
                                                for (int i = 0; i < ll8; i++){
                                                    float2 lllll8 = lllllllllllllllllllllllllll7 + (lllllllllllllllllllll6 + ( normalize(lllllllllllllllllllll6)*lll8*i));
                                                    float2 llllll8 = (lllll8+0.5) * llllllllllllllllllllllllll7.xy;
                                                    float4 lllllll8 = tex2Dlod(lllll2, float4(llllll8, 0.0, 0.0)); 
                                                    float2 llllllll8 = step(float2(0,0), lllll8) - step(float2(llllllllllllllllllllllllll7.z,llllllllllllllllllllllllll7.w), lllll8);
                                                    if(lllllll8.x <= 0) {
                                                        llll8 +=  (1/ll8) * (llllllll8.x * llllllll8.y);
                                                    }                                            
                                                }   
                                            lllllllllllllllllllllllllllllll7 = 1-llll8;  
                                        }         
                                        llllllllllllllllllll6 = lllllllllllllllllllllllllllllll7;
                                    }
#endif
                            if (ll1 <= 1)
                            {
                                if (llllllllllllllllllll6 != -1)
                                {
                                    float lllllllll8 = max(ll1, 0.00001);
                                    float llllllllll8 = 1 - lllllllllllllllllll6;
                                    float lllllllllll8 = exp(lllllllll8 * 6);
                                    float llllllllllll8 = llllllllllllllllllll6;
                                    float lllllllllllll8 = llllllllll8 / (lllllllll8 / (lllllllll8 * llllllllll8 - 0.15 * (lllllllll8 - llllllllll8)));
                                    float llllllllllllll8 = ((llllllllllll8 - lllllllllllll8) / (lllllllllll8 * (1 - llllllllllll8) + llllllllllll8)) + lllllllllllll8;
                                    llllllllllllll8 = 1 - llllllllllllll8;
                                    llllllllllllllllllllllllllll5 = llllllllllllll8 * sign(lllllllllllllllllll6);
                                }
                            }
                            else
                            {
                                llllllllllllllllllllllllllll5 = llllllllllllllllllll6;
                            }
                        }
                        if (lllllllllllllllllllllll0 == 1 || lllllllllllllllllllllll0 == 3 || lllllllllllllllllllllll0 == 5)
                        {
                            float lllllllllllllll8 = distance(_WorldSpaceCameraPos, _PlayersPosVectorArray[lllllll5].xyz);
                            float llllllllllllllll8 = distance(_WorldSpaceCameraPos, d.worldSpacePosition.xyz);
                            float3 lllllllllllllllll8 = d.worldSpacePosition.xyz - _PlayersPosVectorArray[lllllll5].xyz;
                            float3 llllllllllllllllll8 = d.worldSpaceNormal;
                            float lllllllllllllllllll8 = acos(dot(lllllllllllllllll8, llllllllllllllllll8) / (length(lllllllllllllllll8) * length(llllllllllllllllll8)));
                            if (lllllllllllllllllll8 <= 1.5 && lllllllllllllll8 > llllllllllllllll8)
                            {
                                float llllllllllllllllllll8 = (sqrt((lllllllllllllll8 - llllllllllllllll8)) * 25 / lllllllllllllllllll8) * llllllllllllllllllllllll0;
                                llllllllllllllllllllllllllll5 += max(0, log(llllllllllllllllllll8 * 0.2));
                            }
                        }
                    }
                    float lllllllllllllllllllll8 = llllllllllllllllllllllllllll5;
                    float llllllllllllllllllllll8 = 0;
                    float lllllllllllllllllllllll8 = 0;
                    if (llll1 == 1 && llllllllllllllllllllllllllll1 == 0 && !lllllllllllllllllllllllllllll1)
                    {
                        llllllllllllllllllllllllllll5 = min((1 * lllll1), 1);
                        lllllllllllllllllllllllllllll5 = llllllllllllllllllllllllllll5;
                    }
                    else
                    {
                        llllllllllllllllllllllllllll5 = min(llllllllllllllllllllllllllll5 + (1 * lllll1), 1);
                        lllllllllllllllllllllllllllll5 = min((1 * lllll1), 1);
                    }
                    if (lllllllllll3)
                    {
                        if (llllllllllllllllllllllllllll1 == 1)
                        {
                            float llllll6 = 1 / llllllllllllllllllllllllllllll1;
                            if (lllllllllllll3 < llllllllllllllllllllllllllllll1)
                            {
                                float lllllllllllllllllllllllll8 = 1 - ((llllllllllllllllllllllllllllll1 - lllllllllllll3) * llllll6);
                                llllllllllllllllllllllllllll5 = min(llllllllllllllllllllllllllll5, lllllllllllllllllllllllll8);
                                lllllllllllllllllllllllllllll5 = min(lllllllllllllllllllllllllllll5, lllllllllllllllllllllllll8);
                            }
                        }
                        else if (llllllllllllllllllllllllllll1 == 0 && !lllllllllllllllllllllllllllll1)
                        {
                            if (llll1 == 1)
                            {
                                float llllllllllllllllllllllllll8 = ((lllllllllllllllllllll8) / llllllllllllllllllllllllllllll1);
                                if (lllllllllllll3 < llllllllllllllllllllllllllllll1 && lllllllllllllllllllll8 > 0 && saturate(lllllllllllllllllllll8) > lllll1)
                                {
                                    float lllllllllllllllllllllllll8 = ((llllllllllllllllllllllllllllll1 - lllllllllllll3) * (llllllllllllllllllllllllll8));
                                    lllllllllllllllllllll8 = lllllllllllllllllllll8 - (lllllllllllllllllllllllll8);
                                }
                                else
                                {
                                }
                            }
                            if (lllllllllllll3 < llllllllllllllllllllllllllllll1)
                            {
                                float llllll6 = llllllllllllllllllllllllllll5 / llllllllllllllllllllllllllllll1;
                                float lllllllllllllllllllllllll8 = ((llllllllllllllllllllllllllllll1 - lllllllllllll3) * llllll6);
                                llllllllllllllllllllllllllll5 = max(0, lllllllllllllllllllllllll8);
                                float llllllllllllllllllllllllllllll8 = lllllllllllllllllllllllllllll5 / llllllllllllllllllllllllllllll1;
                                float lllllllllllllllllllllllllllllll8 = ((llllllllllllllllllllllllllllll1 - lllllllllllll3) * llllllllllllllllllllllllllllll8);
                                lllllllllllllllllllllllllllll5 = max(0, lllllllllllllllllllllllllllllll8);
                                llllllllllllllllllllll8 = llllllllllllllllllllllllllll5;
                                lllllllllllllllllllllll8 = lllllllllllllllllllllllllllll5;
                                if (llll1 == 0 || llll1 == 1)
                                {
                                    llllllllllllllllllllllllllll5 = max(lllllllllllllllllllll8, lllllllllllllllllllllllll8);
                                }
                            }
                            else
                            {
                                llllllllllllllllllllllllllll5 = 0;
                                lllllllllllllllllllllllllllll5 = 0;
                                llllllllllllllllllllll8 = llllllllllllllllllllllllllll5;
                                lllllllllllllllllllllll8 = lllllllllllllllllllllllllllll5;
                                if (llll1 == 0 || llll1 == 1)
                                {
                                    llllllllllllllllllllllllllll5 = max(lllllllllllllllllllll8, llllllllllllllllllllllllllll5);
                                }
                            }
                        }
                    }
                    if (llllllll1)
                    {
                        float l9 = llllllllllllllllllllllllllll5 / llllllllll1;
                        float ll9 = lllllllllllllllllllllllllllll5 / llllllllll1;
                        float3 llllllll5 = _PlayersPosVectorArray[lllllll5].xyz - _WorldSpaceCameraPos;
                        float3 llll9 = d.worldSpacePosition.xyz - _WorldSpaceCameraPos;
                        float lllll9 = dot(llll9, normalize(llllllll5));
                        if (lllll9 - lllllllll1 >= length(llllllll5))
                        {
                            float llllll9 = lllll9 - lllllllll1 - length(llllllll5);
                            if (llllll9 < 0)
                            {
                                llllll9 = 0;
                            }
                            if (llllll9 < llllllllll1)
                            {
                                llllllllllllllllllllllllllll5 = (llllllllll1 - llllll9) * l9;
                                lllllllllllllllllllllllllllll5 = (llllllllll1 - llllll9) * ll9;
                            }
                            else
                            {
                                llllllllllllllllllllllllllll5 = 0;
                                lllllllllllllllllllllllllllll5 = 0;
                            }
                        }
                    }
                    if (lllllllllllllllllllllllllll1 && !lllllllllll3)
                    {
                        if (llllllllllllllllllllllllllll1 == 1)
                        {
                            llllllllllllllllllllllllllll5 = 0;
                            lllllllllllllllllllllllllllll5 = 0;
                        }
                    }
                    if (lllllllllll1 == 1)
                    {
                        float lllllll9 = 0;
                        float llllllll9 = 0;
                        if (lllllllllllll1 == 0)
                        {
                            lllllll9 = llllllllllllllllllllllllllll5 / llllllllllllllll1;
                            llllllll9 = lllllllllllllllllllllllllllll5 / llllllllllllllll1;
                        }
                        else if (lllllllllllll1 == 1)
                        {
                            float lllllllll9 = 1 - llllllllllllllllllllllllllll5;
                            float llllllllll9 = 1 - lllllllllllllllllllllllllllll5;
                            if (lllllllllllllllllllllllllll1 && lllllllllll3 && lllllllllllllllllllllllllllll1)
                            {
                                lllllllll9 = max(1 - llllllllllllllllllllllllllll5, 1 - (llllllllllllllllllllllllllll5 * lllllllllllllllllllllllllll5));
                                llllllllll9 = max(1 - lllllllllllllllllllllllllllll5, 1 - (lllllllllllllllllllllllllllll5 * lllllllllllllllllllllllllll5));
                            }
                            lllllll9 = lllllllll9 / llllllllllllllll1;
                            llllllll9 = llllllllll9 / llllllllllllllll1;
                        }
                        if (llllllllllll1 == 1)
                        {
                            if (d.worldSpacePosition.y > (_PlayersPosVectorArray[lllllll5].y + lllllllllllllll1))
                            {
                                float llllll9 = d.worldSpacePosition.y - (_PlayersPosVectorArray[lllllll5].y + lllllllllllllll1);
                                if (llllll9 < 0)
                                {
                                    llllll9 = 0;
                                }
                                if (lllllllllllll1 == 0)
                                {
                                    if (llllll9 < llllllllllllllll1)
                                    {
                                        llllllllllllllllllllllllllll5 = ((llllllllllllllll1 - llllll9) * lllllll9);
                                        lllllllllllllllllllllllllllll5 = ((llllllllllllllll1 - llllll9) * llllllll9);
                                    }
                                    else
                                    {
                                        llllllllllllllllllllllllllll5 = 0;
                                        lllllllllllllllllllllllllllll5 = 0;
                                    }
                                }
                                else
                                {
                                    if (llllll9 < llllllllllllllll1)
                                    {
                                        llllllllllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - llllll9) * lllllll9);
                                        lllllllllllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - llllll9) * llllllll9);
                                    }
                                    else
                                    {
                                        llllllllllllllllllllllllllll5 = 1;
                                        lllllllllllllllllllllllllllll5 = 1;
                                    }
                                    lllllllllllllllllllllllllll5 = 1;
                                }
                            }
                        }
                        else
                        {
                            if (d.worldSpacePosition.y > llllllllllllll1)
                            {
                                float llllll9 = d.worldSpacePosition.y - llllllllllllll1;
                                if (llllll9 < 0)
                                {
                                    llllll9 = 0;
                                }
                                if (lllllllllllll1 == 0)
                                {
                                    if (llllll9 < llllllllllllllll1)
                                    {
                                        llllllllllllllllllllllllllll5 = ((llllllllllllllll1 - llllll9) * lllllll9);
                                        lllllllllllllllllllllllllllll5 = ((llllllllllllllll1 - llllll9) * llllllll9);
                                    }
                                    else
                                    {
                                        llllllllllllllllllllllllllll5 = 0;
                                        lllllllllllllllllllllllllllll5 = 0;
                                    }
                                }
                                else
                                {
                                    if (llllll9 < llllllllllllllll1)
                                    {
                                        llllllllllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - llllll9) * lllllll9);
                                        lllllllllllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - llllll9) * llllllll9);
                                    }
                                    else
                                    {
                                        llllllllllllllllllllllllllll5 = 1;
                                        lllllllllllllllllllllllllllll5 = 1;
                                    }
                                    lllllllllllllllllllllllllll5 = 1;
                                }
                            }
                        }
                    }
                    float lllllllllllll9 = llllllllllllllllllllllllllll5;
                    float llllllllllllll9 = lllllllllllllllllllllllllllll5;
                    if (lllllllllllllllll1 == 1)
                    {
                        float lllllllllllllll9 = llllllllllllllllllllllllllll5 / lllllllllllllllllllll1;
                        float llllllllllllllll9 = lllllllllllllllllllllllllllll5 / lllllllllllllllllllll1;
                        if (llllllllllllllllll1 == 1)
                        {
                            if (d.worldSpacePosition.y < (_PlayersPosVectorArray[lllllll5].y + llllllllllllllllllll1))
                            {
                                float llllll9 = (_PlayersPosVectorArray[lllllll5].y + llllllllllllllllllll1) - d.worldSpacePosition.y;
                                if (llllll9 < 0)
                                {
                                    llllll9 = 0;
                                }
                                if (llllll9 < lllllllllllllllllllll1)
                                {
                                    llllllllllllllllllllllllllll5 = (lllllllllllllllllllll1 - llllll9) * lllllllllllllll9;
                                    lllllllllllllllllllllllllllll5 = (lllllllllllllllllllll1 - llllll9) * llllllllllllllll9;
                                }
                                else
                                {
                                    llllllllllllllllllllllllllll5 = 0;
                                    lllllllllllllllllllllllllllll5 = 0;
                                }
                            }
                        }
                        else
                        {
                            if (d.worldSpacePosition.y < lllllllllllllllllll1)
                            {
                                float llllll9 = lllllllllllllllllll1 - d.worldSpacePosition.y;
                                if (llllll9 < 0)
                                {
                                    llllll9 = 0;
                                }
                                if (llllll9 < lllllllllllllllllllll1)
                                {
                                    llllllllllllllllllllllllllll5 = (lllllllllllllllllllll1 - llllll9) * lllllllllllllll9;
                                    lllllllllllllllllllllllllllll5 = (lllllllllllllllllllll1 - llllll9) * llllllllllllllll9;
                                }
                                else
                                {
                                    llllllllllllllllllllllllllll5 = 0;
                                    lllllllllllllllllllllllllllll5 = 0;
                                }
                            }
                        }
                        if (llllllllllllllllllllll1 == 0) 
                        {
                        }
                        else if (llllllllllllllllllllll1 == 1) 
                        {
                            if (lllllllllll3)
                            {
                                llllllllllllllllllllllllllll5 = max(llllllllllllllllllllll8, llllllllllllllllllllllllllll5);
                                lllllllllllllllllllllllllllll5 = max(lllllllllllllllllllllll8, lllllllllllllllllllllllllllll5);
                            }
                            else
                            {
                                llllllllllllllllllllllllllll5 = lllllllllllll9;
                                lllllllllllllllllllllllllllll5 = llllllllllllll9;
                            }
                        }
                        else if (llllllllllllllllllllll1 == 2) 
                        {
                            if (lllllllllll3)
                            {
                                llllllllllllllllllllllllllll5 = min(llllllllllllllllllllll8, llllllllllllllllllllllllllll5);
                                llllllllllllllllllllllllllll5 = max(lllllllllllllllllllll8, llllllllllllllllllllllllllll5);
                                lllllllllllllllllllllllllllll5 = min(lllllllllllllllllllllll8, lllllllllllllllllllllllllllll5);
                            }
                        }
                    }
                    if (!llllllllll0 && !lllllllllll0)
                    {
                        if (lllllll1 == 1 && distance(_PlayersPosVectorArray[lllllll5].xyz, d.worldSpacePosition) > llllll1)
                        {
                            llllllllllllllllllllllllllll5 = 0;
                            lllllllllllllllllllllllllllll5 = 0;
                        }
                    }
                }
                lllll5 = lllll5 + _PlayersDataFloatArray[lllll5] * 4 + 1;
                if (lllllllllllllllllllllllllll1 && lllllllllll3 && lllllllllllllllllllllllllllll1)
                {
                    lllllllllllllllllllll5 = lllllllllllllllllllll5 * lllllllllllllllllllllllllll5;
                }
                if (llllllllll0 || lllllllllll0)
                {
                    llllllllllllllllllllllllllll5 = lllllllllllllllllllll5 * llllllllllllllllllllllllllll5;
                    lllllllllllllllllllllllllllll5 = lllllllllllllllllllll5 * lllllllllllllllllllllllllllll5;
                }
                else
                {
                    if (lllllllllllllllllllllllllll1)
                    {
                        if (lllllllllll3)
                        {
                            if (lllllllllllllllllllllllllllll1)
                            {
                                llllllllllllllllllllllllllll5 = lllllllllllllllllllllllllll5 * llllllllllllllllllllllllllll5;
                                lllllllllllllllllllllllllllll5 = lllllllllllllllllllllllllll5 * lllllllllllllllllllllllllllll5;
                            }
                        }
                        else
                        {
                            if (llllllllllllllllllllllllllll1 == 1)
                            {
                                llllllllllllllllllllllllllll5 = lllllllllllllllllllllllllll5 * llllllllllllllllllllllllllll5;
                                lllllllllllllllllllllllllllll5 = lllllllllllllllllllllllllll5 * lllllllllllllllllllllllllllll5;
                            }
                        }
                    }
                }
                llllllll3 = max(llllllll3, llllllllllllllllllllllllllll5);
                lllllllll3 = max(lllllllll3, lllllllllllllllllllllllllllll5);
            }
            else
            {
                lllll5 = lllll5 + _PlayersDataFloatArray[lllll5 + 2] * 4 + 3;
                lllll5 = lllll5 + _PlayersDataFloatArray[lllll5] * 4 + 1;
            }
        }
#else
        float lllllllllllllllllllll5 = 0;
        if (!llllllllllllllll2)
        {
            lllllllllllllllllllll5 = 1;
            if (lllllll0 != 0 && llllllll0 != 0 && llllllllllllll2 - llllllll0 < lllllllllllllllllllllllll1)
            {
                if (lllllll0 == 1)
                {
                    lllllllllllllllllllll5 = ((lllllllllllllllllllllllll1 - (llllllllllllll2 - llllllll0)) / lllllllllllllllllllllllll1);
                }
                else
                {
                    lllllllllllllllllllll5 = ((llllllllllllll2 - llllllll0) / lllllllllllllllllllllllll1);
                }
            }
            else if (lllllll0 == -1)
            {
                lllllllllllllllllllll5 = 1;
            }
            else if (lllllll0 == 1)
            {
                lllllllllllllllllllll5 = 0;
            }
            else
            {
                lllllllllllllllllllll5 = 1;
            }
            lllllllllllllllllllll5 = 1 - lllllllllllllllllllll5;
        }
        float llllllllllllllllllllllllllll5 = 0;
        float lllllllllllllllllllllllllll5 = 0;
        bool lllllllllllllllllllllllllllllll5 = distance(_WorldSpaceCameraPos, d.worldSpacePosition) > llllll1;
        if ((lllllllllllllllllllll5 != 0) || ((!llllllllll0 && !lllllllllll0) && (lllllll1 == 0 || !lllllllllllllllllllllllllllllll5) ))
        {
#if defined(_ZONING)
                        if(lllllllllllllllllllllllllll1) {
                            if(lllllllllll3) 
                            {
                                if(lllllllllllllllllllllllllllll1) {
                                    float llllllllllll5 = lllllllllllllll3;
                                    float lllllllllllll5 = lllllllllllllllll3;
                                    lllllllllllllllllllllllllll5 = 1;
                                    float lllll6 = llllllllllllllll3;
                                    if( lllllllllllll5!= 0 && llllllllllll5 != 0 && llllllllllllll2-llllllllllll5 < lllll6) {
                                        if(lllllllllllll5 == 1) {
                                            lllllllllllllllllllllllllll5 = ((lllll6-(llllllllllllll2-llllllllllll5))/lllll6);
                                        } else {
                                            lllllllllllllllllllllllllll5 = ((llllllllllllll2-llllllllllll5)/lllll6);
                                        }
                                    } else if(lllllllllllll5 ==-1) {
                                        lllllllllllllllllllllllllll5 = 1;
                                    } else if(lllllllllllll5 == 1) {
                                        lllllllllllllllllllllllllll5 = 0;
                                    } else {
                                        lllllllllllllllllllllllllll5 = 1;
                                    }
                                    lllllllllllllllllllllllllll5 = 1 - lllllllllllllllllllllllllll5;
                                    if(llllllllllllllllllllllllllll1 == 0 && lllllllllllllllllllllllllllll1) {
                                        float llllll6 = 1 / llllllllllllllllllllllllllllll1;
                                        if (lllllllllllll3 < llllllllllllllllllllllllllllll1)  {
                                            float lllllll6 = ((llllllllllllllllllllllllllllll1-lllllllllllll3) * llllll6);
                                            lllllllllllllllllllllllllll5 =  max(lllllllllllllllllllllllllll5,lllllll6);
                                        }
                                    }
                                } else { 
                                }
                            } else {
                            }
                        }
#endif
            llllllllllllllllllllllllllll5 = min(llllllllllllllllllllllllllll5 + (1 * lllll1), 1);
            if (lllllllllll3)
            {
                if (llllllllllllllllllllllllllll1 == 1)
                {
                    float llllll6 = 1 / llllllllllllllllllllllllllllll1;
                    if (lllllllllllll3 < llllllllllllllllllllllllllllll1)
                    {
                        float lllllllllllllllllllllllllllll9 = 1 - ((llllllllllllllllllllllllllllll1 - lllllllllllll3) * llllll6);
                        llllllllllllllllllllllllllll5 = min(llllllllllllllllllllllllllll5, lllllllllllllllllllllllllllll9);
                    }
                }
                else if (llllllllllllllllllllllllllll1 == 0 && !lllllllllllllllllllllllllllll1)
                {
                    float llllll6 = llllllllllllllllllllllllllll5 / llllllllllllllllllllllllllllll1;
                    if (lllllllllllll3 < llllllllllllllllllllllllllllll1)
                    {
                        float lllllllllllllllllllllllllllll9 = ((llllllllllllllllllllllllllllll1 - lllllllllllll3) * llllll6);
                        llllllllllllllllllllllllllll5 = max(0, lllllllllllllllllllllllllllll9);
                    }
                    else
                    {
                        llllllllllllllllllllllllllll5 = 0;
                    }
                }
            }
            if (lllllllllllllllllllllllllll1 && !lllllllllll3)
            {
                if (llllllllllllllllllllllllllll1 == 1)
                {
                    llllllllllllllllllllllllllll5 = 0;
                }
            }
            if (lllllllllll1 == 1 && llllllllllll1 == 0)
            {
                float lllllll9 = 0;
                if (lllllllllllll1 == 0)
                {
                    lllllll9 = (llllllllllllllllllllllllllll5) / llllllllllllllll1;
                }
                else if (lllllllllllll1 == 1)
                {
                    float lllllllll9 = 1 - llllllllllllllllllllllllllll5;
                    if (lllllllllllllllllllllllllll1 && lllllllllll3 && lllllllllllllllllllllllllllll1)
                    {
                        lllllllll9 = max(1 - llllllllllllllllllllllllllll5, 1 - (llllllllllllllllllllllllllll5 * lllllllllllllllllllllllllll5));
                    }
                    lllllll9 = lllllllll9 / llllllllllllllll1;
                }
                if (d.worldSpacePosition.y > llllllllllllll1)
                {
                    float llllll9 = d.worldSpacePosition.y - llllllllllllll1;
                    if (llllll9 < 0)
                    {
                        llllll9 = 0;
                    }
                    if (lllllllllllll1 == 0)
                    {
                        if (llllll9 < llllllllllllllll1)
                        {
                            llllllllllllllllllllllllllll5 = ((llllllllllllllll1 - llllll9) * lllllll9);
                        }
                        else
                        {
                            llllllllllllllllllllllllllll5 = 0;
                        }
                    }
                    else
                    {
                        if (llllll9 < llllllllllllllll1)
                        {
                            llllllllllllllllllllllllllll5 = 1 - ((llllllllllllllll1 - llllll9) * lllllll9);
                        }
                        else
                        {
                            llllllllllllllllllllllllllll5 = 1;
                        }
                        lllllllllllllllllllllllllll5 = 1;
                    }
                }
            }
            if (lllllllllllllllll1 == 1 && llllllllllllllllll1 == 0)
            {
                float lllllllllllllll9 = llllllllllllllllllllllllllll5 / lllllllllllllllllllll1;
                if (d.worldSpacePosition.y < lllllllllllllllllll1)
                {
                    float llllll9 = lllllllllllllllllll1 - d.worldSpacePosition.y;
                    if (llllll9 < 0)
                    {
                        llllll9 = 0;
                    }
                    if (llllll9 < lllllllllllllllllllll1)
                    {
                        llllllllllllllllllllllllllll5 = (lllllllllllllllllllll1 - llllll9) * lllllllllllllll9;
                    }
                    else
                    {
                        llllllllllllllllllllllllllll5 = 0;
                    }
                }
            }
        }
        if (lllllllllllllllllllllllllll1 && lllllllllll3 && lllllllllllllllllllllllllllll1)
        {
            lllllllllllllllllllll5 = lllllllllllllllllllll5 * lllllllllllllllllllllllllll5;
        }
        if (llllllllll0 || lllllllllll0)
        {
            llllllllllllllllllllllllllll5 = lllllllllllllllllllll5 * llllllllllllllllllllllllllll5;
        }
        else
        {
            llllllllllllllllllllllllllll5 = llllllllllllllllllllllllllll5;
            if (lllllllllllllllllllllllllll1)
            {
                if (lllllllllll3)
                {
                    if (lllllllllllllllllllllllllllll1)
                    {
                        llllllllllllllllllllllllllll5 = lllllllllllllllllllllllllll5 * llllllllllllllllllllllllllll5;
                    }
                }
                else
                {
                    if (llllllllllllllllllllllllllll1 == 1)
                    {
                        llllllllllllllllllllllllllll5 = lllllllllllllllllllllllllll5 * llllllllllllllllllllllllllll5;
                    }
                }
            }
        }
        llllllll3 = max(llllllll3, llllllllllllllllllllllllllll5);
#endif
        float llllllllllllllllllllllllllllll5 = llllllll3;
        if (!ll2)
        {
            if (llllllllllllllllllllllllllllll5 == 1)
            {
                llllllllllllllllllllllllllllll5 = 10;
            }
            if (!llllllllllllllllllllll0) 
            {
#if defined(UNITY_PASS_SHADOWCASTER) 
#if defined(SHADOWS_DEPTH) 
                if (!any(unity_LightShadowBias))
                {
#if !defined(NO_STS_CLIPPING)
                        clip(llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5);
#endif
                    llllllllllll2 = llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5;
                }
                else
                {
                    if(llllllllllllllllllllll0) 
                    {
#if !defined(NO_STS_CLIPPING)
                        clip(llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5);
#endif
                        llllllllllll2 = llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5;
                    }
                }
#endif
#else
#if !defined(NO_STS_CLIPPING)
                clip(llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5);
#endif
                llllllllllll2 = llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5;
#endif
            }
            else
            {
                if (lllllllllllllllllllllll0 == 6 && llllllllllllllllllllll0)
                {
#if defined(UNITY_PASS_SHADOWCASTER) 
#if defined(SHADOWS_DEPTH) 
                    if (!any(unity_LightShadowBias))
                    {
                    }
                    else
                    {
                        llllllllllllllllllllllllllllll5 = lllllllll3;
                        if (llllllllllllllllllllllllllllll5 == 1)
                        {
                            llllllllllllllllllllllllllllll5 = 10;
                        }                    
                    }                
#endif
#endif
                }
#if !defined(NO_STS_CLIPPING)
                clip(llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5);
#endif
                llllllllllll2 = llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5;
            }
            if (llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5 < 0)
            {
                llllllllllll2 = 0;
            }
            else
            {
                llllllllllll2 = 1;
            }
        }
        if (ll2)
        {
            lllllllllllllllll2 = 1;
            if ((llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5) < 0)
            {
                llllllllllllllllll2 = half4(1, 1, 1, 1);
                o.Emission = 1;
            }
            else
            {
                llllllllllllllllll2 = half4(0, 0, 0, 1);
            }
            if (llll5)
            {
                if ((llllllllllllllllllllll2 - llllllllllllllllllllllllllllll5) < 0)
                {
                    llllllllllllllllll2 = half4(0.5, 1, 0.5, 1);
                    o.Emission = 0;
                }
                else
                {
                    llllllllllllllllll2 = half4(0, 0.1, 0, 1);
                }
            }
            if (lllllllllll3 && lllllllllllllllll1 == 1 && lllllllllllllllllllllllllllllll1)
            {
                float lllllll10 = 0;
                if (llllllllllllllllll1 == 1)
                {
                    lll5 = lll5 + l2;
                    lllllll10 = lll5;
                }
                else
                {
                    lllllll10 = lllllllllllllllllll1 + l2;
                }
                if (d.worldSpacePosition.y > (lllllll10 - lll2) && d.worldSpacePosition.y < (lllllll10 + lll2))
                {
                    llllllllllllllllll2 = half4(1, 0, 0, 1);
                }
            }
        }
        else
        {
            half3 llllllll10 = lerp(1, lllllllllllllll0, llllllllllllllll0).rgb;
            if (llllllllllllllllllll0)
            {
                lllllllllllllllllllll0 = 0.2 + (lllllllllllllllllllll0 * (0.8 - 0.2));
                o.Emission = o.Emission + min(clamp(llllllll10 * clamp(((llllllllllllllllllllllllllllll5 / lllllllllllllllllllll0) - llllllllllllllllllllll2), 0, 1), 0, 1) * sqrt(llllllllllllllllll0 * lllllllllllllllllll0), clamp(llllllll10 * llllllllllllllllllllllllllllll5, 0, 1) * sqrt(llllllllllllllllll0 * lllllllllllllllllll0));
            }
            else
            {
                o.Emission = o.Emission + clamp(llllllll10 * llllllllllllllllllllllllllllll5, 0, 1) * sqrt(llllllllllllllllll0 * lllllllllllllllllll0);
            }
        }
    }
    if (lllllllllllllllll2)
    {
        o.Albedo = llllllllllllllllll2.rgb;
    }
    llllllllll2 = o.Albedo;
    lllllllllll2 = o.Emission;
    #ifdef _HDRP  
        float lllllllll10 = 0;
        float llllllllll10 = 0;
    #if SHADEROPTIONS_PRE_EXPOSITION
            llllllllll10 =  LOAD_TEXTURE2D(_ExposureTexture, int2(0, 0)).x * _ProbeExposureScale;
    #else
            llllllllll10 = _ProbeExposureScale;
    #endif
            float lllllllllll10 = 0;
            float llllllllllll10 = llllllllll10;
            lllllllllll10 = rcp(llllllllllll10 + (llllllllllll10 == 0.0));
            float3 lllllllllllll10 = o.Emission * lllllllllll10;
            o.Emission = lerp(lllllllllllll10, o.Emission, lllllllll10);
        lllllllllll2 = o.Emission;
    #endif
}
void DoCrossSection(
                    half llllllllllllll10,
                    half4 lllllllllllllll10,
                    half llllllllllllllll10,
                    sampler2D lllllllllllllllll10,
                    float llllllllllllllllll10,
                    half lllllllllllllllllll10,
                    bool llllllllllllllllllll10,
                    float4 lllll0,
                    inout half4 llllllllllllllllllllll10
                    )
{
    if (llllllllllllll10 == 1)
    {
        if (llllllllllllllllllll10 == false)
        {
            if (llllllllllllllll10 == 1)
            {
                float2 llllll8 = lllll0.xy / lllll0.w;
                if (lllllllllllllllllll10 == 1)
                {
                    float4 llllllllllllllllllllllll10 = mul(UNITY_MATRIX_M, float4(0, 0, 0, 1));
                    llllll8.xy *= distance(_WorldSpaceCameraPos, llllllllllllllllllllllll10);
                }
                float lllllllllll6 = _ScreenParams.x / _ScreenParams.y;
                llllll8.x *= lllllllllll6;
                half3 llllllllllllllllllllllllll10 = tex2D(lllllllllllllllll10, llllll8 * llllllllllllllllll10).rgb;
                llllllllllllllllllllll10 = half4(llllllllllllllllllllllllll10, 1) * lllllllllllllll10;
            }
            else
            {
                llllllllllllllllllllll10 = lllllllllllllll10;
            }
        }
    }
}


#endif
