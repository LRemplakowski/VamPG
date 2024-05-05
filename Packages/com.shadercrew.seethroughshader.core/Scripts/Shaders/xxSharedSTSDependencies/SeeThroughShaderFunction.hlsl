#ifndef SEETHROUGHSHADER_FUNCTION
#define SEETHROUGHSHADER_FUNCTION


#ifndef UNITY_MATRIX_I_M
        #define UNITY_MATRIX_I_M   unity_WorldToObject
#endif




void DoSeeThroughShading(
                                    float3 l0,
                                    float3 ll0, float3 lll0,
                                    float3 llll0,
                                    float lllll0, float llllll0, float lllllll0, float llllllll0,
                                    float lllllllll0, float llllllllll0,
                                    float lllllllllll0,
                                    float4 llllllllllll0, float lllllllllllll0, float llllllllllllll0,
                                    float lllllllllllllll0, float llllllllllllllll0, float lllllllllllllllll0, float llllllllllllllllll0,
                                    bool lllllllllllllllllll0,
                                    float llllllllllllllllllll0,
                                    float lllllllllllllllllllll0,
                                    float llllllllllllllllllllll0, float lllllllllllllllllllllll0,
                                    float llllllllllllllllllllllll0, float lllllllllllllllllllllllll0,
                                    float llllllllllllllllllllllllll0, float lllllllllllllllllllllllllll0,
                                    float llllllllllllllllllllllllllll0, float lllllllllllllllllllllllllllll0,
                                    float llllllllllllllllllllllllllllll0,
                                    float lllllllllllllllllllllllllllllll0,
                                    float l1,
                                    float ll1,
                                    float lll1, float llll1,
                                    float lllll1, float llllll1, float lllllll1,
                                    float llllllll1, float lllllllll1, float llllllllll1, float lllllllllll1, float llllllllllll1, float lllllllllllll1,
                                    float llllllllllllll1, float lllllllllllllll1, float llllllllllllllll1, float lllllllllllllllll1, float llllllllllllllllll1, float lllllllllllllllllll1,
                                    float llllllllllllllllllll1, float lllllllllllllllllllll1,
                                    float llllllllllllllllllllll1,
                                    float lllllllllllllllllllllll1, float llllllllllllllllllllllll1, float lllllllllllllllllllllllll1, float llllllllllllllllllllllllll1,
                                    float lllllllllllllllllllllllllll1, float llllllllllllllllllllllllllll1,
                                    float lllllllllllllllllllllllllllll1,
                                    float llllllllllllllllllllllllllllll1,
#ifdef USE_UNITY_TEXTURE_2D_TYPE
                                    UnityTexture2D lllllllllllllllllllllllllllllll1,
                                    UnityTexture2D l2,
                                    UnityTexture2D ll2,
#else
                                    sampler2D lllllllllllllllllllllllllllllll1,
                                    sampler2D l2,
                                    sampler2D ll2,
                                    float4 lll2,
                                    float4 llll2,
#endif
                                    out half3 lllll2,
                                    out half3 llllll2,
                                    out float lllllll2
)
{
        ShaderData d;
        d.worldSpaceNormal = llll0;
        d.worldSpacePosition = lll0;
        Surface o;
        o.Normal = ll0;
        o.Albedo = half3(0, 0, 0) + l0;
        o.Emission = half3(0, 0, 0);
        lllll2 = half3(0, 0, 0);
        llllll2 = half3(0, 0, 0);
        lllllll2 = 1;
        bool llllllll2;
        llllllll2 = (lllll0 > 0 || llllll0 == -1 && _Time.y - lllllll0 < llllllllllllllllllllll1) || (lllll0 >= 0 && llllll0 == 1);
        bool lllllllll2 = !lllllllll0 && !llllllllll0;
        float llllllllll2 = 0;
        half4 lllllllllll2 = half4(0, 0, 0, 0);
        if (!lllllllllll0 && (llllllll2 || lllllllll2))
        {
        float4 llllllllllll2;
        float4 lllllllllllll2;
#ifdef USE_UNITY_TEXTURE_2D_TYPE
        lllllllllllll2 = ll2.texelSize;
        llllllllllll2 = l2.texelSize;
#else
        lllllllllllll2 = llll2;
        llllllllllll2 = lll2;
#endif
            if (ll1 < 0)
            {
                ll1 = 0;
            }
            float3 llllllllllllll2;
            float3 lllllllllllllll2 = d.worldSpacePosition / (-1.0 * abs(llllllllllllll0));
            if (llllllllllllllllllll1)
            {
                lllllllllllllll2 = lllllllllllllll2 + abs(((_Time.y) * lllllllllllllllllllll1));
            }
            float3 llllllllllllllll2 = float3(0, 0, 0);
            float3 lllllllllllllllll2 = float3(0, 0, 0);
            float3 llllllllllllllllll2 = float3(0, 0, 0);
            llllllllllllllll2 = tex2D(lllllllllllllllllllllllllllllll1, lllllllllllllll2.yz).rgb;
            lllllllllllllllll2 = tex2D(lllllllllllllllllllllllllllllll1, lllllllllllllll2.xz).rgb;
            llllllllllllllllll2 = tex2D(lllllllllllllllllllllllllllllll1, lllllllllllllll2.xy).rgb;
            float lllllllllllllllllll2 = abs(d.worldSpaceNormal.x);
            float llllllllllllllllllll2 = abs(d.worldSpaceNormal.z);
            float3 lllllllllllllllllllll2 = lerp(lllllllllllllllll2, llllllllllllllll2, lllllllllllllllllll2).rgb;
            llllllllllllll2 = lerp(lllllllllllllllllllll2, llllllllllllllllll2, llllllllllllllllllll2).rgb;
            half llllllllllllllllllllll2 = llllllllllllll2.r;
            float3 lllllllllllllllllllllll2 = UNITY_MATRIX_V[2].xyz;
#ifdef _HDRP
                lllllllllllllllllllllll2 =  mul(UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2]).xyz;
#else
        lllllllllllllllllllllll2 = mul(UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V))[2]).xyz;
#endif
            float llllllllllllllllllllllll2 = 0;
            float lllllllllllllllllllllllll2 = 0;
            float llllllllllllllllllllllllll2 = 1;
            bool lllllllllllllllllllllllllll2 = false;
            float llllllllllllllllllllllllllll2 = 0;
            float lllllllllllllllllllllllllllll2 = 0;
            float llllllllllllllllllllllllllllll2 = 0;
            float lllllllllllllllllllllllllllllll2 = 0;
            float l3 = 0;
            float ll3 = 0;
#if defined(_ZONING)  
                if(lllllllllllllllllllllll1) {
                    float lll3 = 0;
                    for (int z = 0; z < _ZonesDataCount; z++){
                        bool llll3 = false;
                        float lllll3 = lll3;
                        if (_ZDFA[lll3 + 1] == 0) { 
#if !_EXCLUDE_ZONEBOXES
                            float llllll3 = lll3 + 2; 
                            float3 lllllll3 = d.worldSpacePosition - float3(_ZDFA[llllll3],_ZDFA[llllll3+1], _ZDFA[llllll3+2]);
                            float3 llllllll3 =     float3(_ZDFA[llllll3+ 3],_ZDFA[llllll3+ 4], _ZDFA[llllll3+ 5]);
                            float3 lllllllll3 =     float3(_ZDFA[llllll3+ 6],_ZDFA[llllll3+ 7], _ZDFA[llllll3+ 8]);
                            float3 llllllllll3 =     float3(_ZDFA[llllll3+ 9],_ZDFA[llllll3+10], _ZDFA[llllll3+11]);
                            float3 lllllllllll3 = float3(_ZDFA[llllll3+12],_ZDFA[llllll3+13], _ZDFA[llllll3+14]);
                            float llllllllllll3 = abs(dot(lllllll3, llllllll3));
                            float lllllllllllll3 = abs(dot(lllllll3, lllllllll3));
                            float llllllllllllll3 = abs(dot(lllllll3, llllllllll3));
                            llll3 =    llllllllllll3 <= lllllllllll3.x &&
                                        lllllllllllll3 <= lllllllllll3.y &&
                                        llllllllllllll3 <= lllllllllll3.z;
                            if(llll3 && llllllllllllll1 == 1 && lllllllllllllllllllllllllll1) {
                                llllllllllllllllllllllllllllll2 = _ZDFA[llllll3+1] - _ZDFA[llllll3+13];  
                                if(lllllllllllllll1 == 0) {                                    
                                    bool lllllllllllllll3 = ((llllllllllllllllllllllllllllll2 - llllllllllllllllllllllllllll1)  <= llllllllllllllll1); 
                                    if(!lllllllllllllll3) {
                                        llll3 = false;
                                    }
                                }
                            }
                            if(llll3) {
                                float llllllllllllllll3 = lllllllllll3.x - llllllllllll3;
                                float lllllllllllllllll3 = lllllllllll3.y - lllllllllllll3;
                                float llllllllllllllllll3 = lllllllllll3.z - llllllllllllll3;
                                float lllllllllllllllllll3 = min(lllllllllllllllll3,llllllllllllllll3);
                                lllllllllllllllllll3 = min(lllllllllllllllllll3,llllllllllllllllll3);
                                lllllllllllllllllllllllllllll2 = max(lllllllllllllllllll3,lllllllllllllllllllllllllllll2);
                                if(lllllllllllllllllll3<0) {
                                    lllllllllllllllllllllllllllll2 = 0;
                                }
                            }
                            if (llll3)
                            {
                                if (lllllllllllllllllllllllllll2 == false)
                                {
                                    llllllllllllllllllllllllllll2 = _ZDFA[lllll3];
                                    lllllllllllllllllllllllllll2 = true;
                                    lllllllllllllllllllllllllllllll2 = _ZDFA[lllll3 + 17];
                                    ll3 = _ZDFA[lllll3 + 18];
                                    l3 = _ZDFA[lllll3 + 19];
                                }                     
                            }
#endif        
                            lll3 = lll3 + 17 + 3;
                        } else if (_ZDFA[lll3 + 1] == 1) { 
#if !_EXCLUDE_ZONESPHERES
                            float llllllllllllllllllll3 = lll3 + 2; 
                            float3 lllllllllllllllllllll3 = float3(_ZDFA[llllllllllllllllllll3], _ZDFA[llllllllllllllllllll3 + 1], _ZDFA[llllllllllllllllllll3 + 2]);
                            float llllllllllllllllllllll3 = _ZDFA[llllllllllllllllllll3 + 3];
                            float lllllllllllllllllllllll3 = distance(d.worldSpacePosition, lllllllllllllllllllll3);
                            llll3 = lllllllllllllllllllllll3 < llllllllllllllllllllll3;
                            if (llll3 && llllllllllllll1 == 1 && lllllllllllllllllllllllllll1)
                            {
                                llllllllllllllllllllllllllllll2 = _ZDFA[llllllllllllllllllll3 + 1] - _ZDFA[llllllllllllllllllll3 + 3];
                                if (lllllllllllllll1 == 0)
                                {
                                    bool lllllllllllllll3 = ((llllllllllllllllllllllllllllll2 - llllllllllllllllllllllllllll1) <= llllllllllllllll1);
                                    if (!lllllllllllllll3)
                                    {
                                        llll3 = false;
                                    }
                                }
                            }
                            if (llll3)
                            {
                                if (lllllllllllllllllllllllllll2 == false)
                                {
                                    llllllllllllllllllllllllllll2 = _ZDFA[lllll3];
                                    lllllllllllllllllllllllllll2 = true;
                                    lllllllllllllllllllllllllllllll2 = _ZDFA[lllll3 + 6];
                                    ll3 = _ZDFA[lllll3 + 7];
                                    l3 = _ZDFA[lllll3 + 8];
                                }
                            }
                            if (llll3)
                             {
                                float lllllllllllllllllll3 = max(0, (llllllllllllllllllllll3 - lllllllllllllllllllllll3));
                                lllllllllllllllllllllllllllll2 = max(lllllllllllllllllll3, lllllllllllllllllllllllllllll2);
                            }
#endif
                            lll3 = lll3 + 6 + 3;
                        } else if (_ZDFA[lll3 + 1] == 2) { 
#if !_EXCLUDE_ZONECYLINDERS
                            float llllllllllllllllllllllllll3 = lll3 + 2;
                            float3 lllllllllllllllllllllllllll3 = float3(_ZDFA[llllllllllllllllllllllllll3], _ZDFA[llllllllllllllllllllllllll3 + 1], _ZDFA[llllllllllllllllllllllllll3 + 2]);
                            float3 llllllllllllllllllllllllllll3 = float3(_ZDFA[llllllllllllllllllllllllll3 + 3], _ZDFA[llllllllllllllllllllllllll3 + 4], _ZDFA[llllllllllllllllllllllllll3 + 5]);
                            float lllllllllllllllllllllllllllll3 = dot(d.worldSpacePosition.xyz - lllllllllllllllllllllllllll3, llllllllllllllllllllllllllll3);
                            float llllllllllllllllllllllllllllll3 = _ZDFA[llllllllllllllllllllllllll3 + 6];
                            float lllllllllllllllllllllllllllllll3 = _ZDFA[llllllllllllllllllllllllll3 + 7];
                            float l4 = length((d.worldSpacePosition.xyz - lllllllllllllllllllllllllll3) - lllllllllllllllllllllllllllll3 * llllllllllllllllllllllllllll3);
                            llll3 = (abs(lllllllllllllllllllllllllllll3) < lllllllllllllllllllllllllllllll3/2) && (l4 < llllllllllllllllllllllllllllll3);
                            if (llll3)
                            {
                                if (lllllllllllllllllllllllllll2 == false)
                                {
                                    llllllllllllllllllllllllllll2 = _ZDFA[lllll3];
                                    lllllllllllllllllllllllllll2 = true;
                                    lllllllllllllllllllllllllllllll2 = _ZDFA[lllll3 + 10];
                                    ll3 = _ZDFA[lllll3 + 11];
                                    l3 = _ZDFA[lllll3 + 12];
                                }
                            }
                            if (llll3)
                            {
                                float lllllllllllllllllll3 = max(0, (llllllllllllllllllllllllllllll3 - l4));
                                lllllllllllllllllll3 = min(lllllllllllllllllll3, (lllllllllllllllllllllllllllllll3/2 - abs(lllllllllllllllllllllllllllll3)));
                                lllllllllllllllllllllllllllll2 = max(lllllllllllllllllll3, lllllllllllllllllllllllllllll2);
                            }
#endif
                            lll3 = lll3 + 10 + 3;
                        }
                        else if (_ZDFA[lll3 + 1] == 3) { 
#if !_EXCLUDE_ZONECONES
                            float llllllllllllllllllllllllll3 = lll3 + 2;
                            float3 lllllllllllllllllllllllllll3 = float3(_ZDFA[llllllllllllllllllllllllll3], _ZDFA[llllllllllllllllllllllllll3 + 1], _ZDFA[llllllllllllllllllllllllll3 + 2]);
                            float3 llllllllllllllllllllllllllll3 = float3(_ZDFA[llllllllllllllllllllllllll3 + 3], _ZDFA[llllllllllllllllllllllllll3 + 4], _ZDFA[llllllllllllllllllllllllll3 + 5]);
                            float lllllllllllllllllllllllllllll3 = dot(d.worldSpacePosition.xyz - lllllllllllllllllllllllllll3, llllllllllllllllllllllllllll3);
                            float lllllll4 = _ZDFA[llllllllllllllllllllllllll3 + 6];
                            float llllllll4 = _ZDFA[llllllllllllllllllllllllll3 + 7];
                            float3 lllllllll4 = lllllllllllllllllllllllllll3 + (llllllllllllllllllllllllllll3 * llllllll4/2); 
                            float llllllllll4 = dot(lllllllll4 - d.worldSpacePosition.xyz, llllllllllllllllllllllllllll3);
                            float lllllllllll4 = (llllllllll4 / llllllll4) * lllllll4;
                            float l4 = length((lllllllll4 - d.worldSpacePosition.xyz) - llllllllll4 * llllllllllllllllllllllllllll3);        
                            llll3 = (abs(lllllllllllllllllllllllllllll3) < llllllll4/2) && (l4 < lllllllllll4);
                            if (llll3)
                            {
                                if (lllllllllllllllllllllllllll2 == false)
                                {
                                    llllllllllllllllllllllllllll2 = _ZDFA[lllll3];
                                    lllllllllllllllllllllllllll2 = true;
                                    lllllllllllllllllllllllllllllll2 = _ZDFA[lllll3 + 10];
                                    ll3 = _ZDFA[lllll3 + 11];
                                    l3 = _ZDFA[lllll3 + 12];
                                }
                            }
                            if (llll3)
                            {
                                float lllllllllllllllllll3 = max(0, (lllllllllll4 - l4));
                                lllllllllllllllllll3 = min(lllllllllllllllllll3, (llllllll4 - llllllllll4));
                                lllllllllllllllllllllllllllll2 = max(lllllllllllllllllll3, lllllllllllllllllllllllllllll2);
                            }
#endif
                            lll3 = lll3 + 10 + 3;
                        }
                }
            }
#endif
            float llllllllllllll4 = 0;
            float lllllllllllllll4 = lllllllllllllllllllllllllll2;
#if !defined(_PLAYERINDEPENDENT)
#if defined(_ZONING)
                    if(lllllllllllllllllllllllllll2 && llllllllllllll1 == 1 && lllllllllllllll1 == 1 && lllllllllllllllllllllllllll1) {
                        float llllllllllllllll4 = 0;
                        bool lllllllllllllllll4 = false;
                        for (int i = 0; i < _ArrayLength; i++){
                            float llllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllll4+1]; 
                            float3 lllllllllllllllllll4 = _PlayersPosVectorArray[llllllllllllllllll4].xyz - _WorldSpaceCameraPos;               
                            if(dot(lllllllllllllllllllllll2,lllllllllllllllllll4) <= 0) {       
                                if(!lllllllll2) {
                                    float llllllllllllllllllll4 = llllllllllllllll4 + 3;
                                    float lllllllllllllllllllll4 = 4;
                                    for (int lllllllllllllllllllllll7 = 0; lllllllllllllllllllllll7 < _PlayersDataFloatArray[llllllllllllllll4 + 2]; lllllllllllllllllllllll7++){
                                        float llllllllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4 + 2];
                                        if (llllllllllllllllllllll4 != 0 && llllllllllllllllllllll4 == llllllll0) {
                                            float lllllllllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4 ];
                                            float llllllllllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4 + 1];
                                            if ((llllllllllllllllllllllll4 == -1 && _Time.y - lllllllllllllllllllllll4 < llllllllllllllllllllll1 )|| (llllllllllllllllllllllll4 == 1) ) {
                                                float lllllllllllllllllllllllll4 = _PlayersPosVectorArray[llllllllllllllllll4].y+ lllllllllllllllll1;
                                                if(lllllllllllllllllllllllllllll1) {
                                                    if(i==0) {
                                                        llllllllllllll4 = lllllllllllllllllllllllll4;
                                                    } else {
                                                        llllllllllllll4 = max(llllllllllllll4,lllllllllllllllllllllllll4);
                                                    }
                                                }
                                                bool llllllllllllllllllllllllll4 = llllllllllllllllllllllllllllll2 >= lllllllllllllllllllllllll4 + llllllllllllllllllllllllllll1; 
                                                if(!llllllllllllllllllllllllll4) {
                                                    lllllllllllllllll4 = true;
                                                } 
                                            }                        
                                        }
                                    }
                                } else if (llll1 == 0 || distance(_PlayersPosVectorArray[llllllllllllllllll4].xyz, d.worldSpacePosition.xyz) < lll1) {
                                    float lllllllllllllllllllllllll4 = _PlayersPosVectorArray[llllllllllllllllll4].y+ lllllllllllllllll1;
                                    if(lllllllllllllllllllllllllllll1) {
                                        if(i==0) {
                                            llllllllllllll4 = lllllllllllllllllllllllll4;
                                        } else {
                                            llllllllllllll4 = max(llllllllllllll4,lllllllllllllllllllllllll4);
                                        }
                                    }
                                    bool llllllllllllllllllllllllll4 = llllllllllllllllllllllllllllll2 >= lllllllllllllllllllllllll4 + llllllllllllllllllllllllllll1; 
                                    if(!llllllllllllllllllllllllll4) {
                                        lllllllllllllllll4 = true;
                                    } 
                                }
                                llllllllllllllll4 = llllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllll4 + 2]*4 + 3; 
                                llllllllllllllll4 = llllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllll4]*4 + 1; 
                            }
                        }
                        if(!lllllllllllllllll4) {
                            lllllllllllllllllllllllllll2 = false;
                        }
                    }
#endif
            float llllllllllllllll4 = 0;
            for (int i = 0; i < _ArrayLength; i++)
            {
            float llllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllll4 + 1];
            if (sign(_PlayersPosVectorArray[llllllllllllllllll4].w) != -1) 
            {
                float3 lllllllllllllllllll4 = _PlayersPosVectorArray[llllllllllllllllll4].xyz - _WorldSpaceCameraPos;
                float l5 = 0;
                float lllllllllllllllllllll4 = 4;
                if (!lllllllll2)
                {
                    float llllllllllllllllllll4 = llllllllllllllll4 + 3;
                    for (int lllllllllllllllllllllll7 = 0; lllllllllllllllllllllll7 < _PlayersDataFloatArray[llllllllllllllll4 + 2]; lllllllllllllllllllllll7++)
                    {
                        float llllllllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4 + 2];
                        if (llllllllllllllllllllll4 != 0 && llllllllllllllllllllll4 == llllllll0)
                        {
                            float lllllllllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4];
                            float llllllllllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4 + 1];
                            l5 = 1;
                            if (llllllllllllllllllllllll4 != 0 && lllllllllllllllllllllll4 != 0 && _Time.y - lllllllllllllllllllllll4 < llllllllllllllllllllll1)
                            {
                                if (llllllllllllllllllllllll4 == 1)
                                {
                                    l5 = ((llllllllllllllllllllll1 - (_Time.y - lllllllllllllllllllllll4)) / llllllllllllllllllllll1);
                                }
                                else
                                {
                                    l5 = ((_Time.y - lllllllllllllllllllllll4) / llllllllllllllllllllll1);
                                }
                            }
                            else if (llllllllllllllllllllllll4 == -1)
                            {
                                l5 = 1;
                            }
                            else if (llllllllllllllllllllllll4 == 1)
                            {
                                l5 = 0;
                            }
                            else
                            {
                                l5 = 1;
                            }
                            l5 = 1 - l5;
                        }
                    }
                }
                llllllllllllllll4 = llllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllll4 + 2] * 4 + 3;
                float lllllll5 = 0;
                float llllllll5 = 0;
                float lllllllll5 = 0;
                float llllllllll5 = llllllll5;
                bool lllllllllll5 = distance(_PlayersPosVectorArray[llllllllllllllllll4].xyz, d.worldSpacePosition) > lll1;
                if ((l5 != 0) || ((!lllllllll0 && !llllllllll0) && (llll1 == 0 || !lllllllllll5) ))
                {
#if defined(_ZONING)
                            if(lllllllllllllllllllllll1) {
                                if(lllllllllllllllllllllllllll2) 
                                {
                                    if(lllllllllllllllllllllllll1) {
                                        float llllllllllllllllllll4 = llllllllllllllll4 + 1;
                                        for (int lllllllllllllllllllllll7 = 0; lllllllllllllllllllllll7 < _PlayersDataFloatArray[llllllllllllllll4]; lllllllllllllllllllllll7++){
                                            float llllllllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4 + 2];
                                            if (llllllllllllllllllllll4 != 0 && llllllllllllllllllllll4 == llllllllllllllllllllllllllll2) {
                                                float lllllllllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4 ];
                                                float llllllllllllllllllllllll4 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4 + 1];
                                                lllllll5 = 1;
                                                float llllllllllllllll5 = _PlayersDataFloatArray[llllllllllllllllllll4 + lllllllllllllllllllllll7 * lllllllllllllllllllll4 + 3];
                                                if( llllllllllllllllllllllll4!= 0 && lllllllllllllllllllllll4 != 0 && _Time.y-lllllllllllllllllllllll4 < llllllllllllllll5) {
                                                    if(llllllllllllllllllllllll4 == 1) {
                                                        lllllll5 = ((llllllllllllllll5-(_Time.y-lllllllllllllllllllllll4))/llllllllllllllll5);
                                                    } else {
                                                        lllllll5 = ((_Time.y-lllllllllllllllllllllll4)/llllllllllllllll5);
                                                    }
                                                } else if(llllllllllllllllllllllll4 ==-1) {
                                                    lllllll5 = 1;
                                                } else if(llllllllllllllllllllllll4 == 1) {
                                                    lllllll5 = 0;
                                                } else {
                                                    lllllll5 = 1;
                                                }
                                                lllllll5 = 1 - lllllll5;
                                            }
                                            if(llllllllllllllllllllllll1 == 0 && lllllllllllllllllllllllll1) {
                                                float lllllllllllllllll5 = 1 / llllllllllllllllllllllllll1;
                                                if (lllllllllllllllllllllllllllll2 < llllllllllllllllllllllllll1)  {
                                                    float llllllllllllllllll5 = ((llllllllllllllllllllllllll1-lllllllllllllllllllllllllllll2) * lllllllllllllllll5);
                                                    lllllll5 =  max(lllllll5,llllllllllllllllll5);
                                                }
                                            }
                                        }
                                    } else { 
                                    }
                                } else {
                                }
                            }
#endif
                    if (dot(lllllllllllllllllllllll2, lllllllllllllllllll4) <= 0)
                    {
                        if (llllllllllllllllllll0 == 2 || llllllllllllllllllll0 == 3 || llllllllllllllllllll0 == 4 || llllllllllllllllllll0 == 5 || llllllllllllllllllll0 == 6 || llllllllllllllllllll0 == 7)
                        {
                            float4 lllllllllllllllllll5 = float4(0, 0, 0, 0);
                            float4 llllllllllllllllllll5 = float4(0, 0, 0, 0);
                            float lllllllllllllllllllll5 = 0;
                            if (lllllllllllllllllllllllllllllll0 || llllllllllllllllllll0 == 6)
                            {
                                float llllllllllllllllllllll5 = _ScreenParams.x / _ScreenParams.y;
#ifdef _HDRP
                                        float4 lllllllllllllllllllllll5 = mul(UNITY_MATRIX_VP, float4(GetCameraRelativePositionWS(_PlayersPosVectorArray[llllllllllllllllll4].xyz), 1.0));
                                        llllllllllllllllllll5 = ComputeScreenPos(lllllllllllllllllllllll5 , _ProjectionParams.x);
#else
                                float4 lllllllllllllllllllllll5 = mul(UNITY_MATRIX_VP, float4(_PlayersPosVectorArray[llllllllllllllllll4].xyz, 1.0));
                                llllllllllllllllllll5 = ComputeScreenPos(lllllllllllllllllllllll5);
#endif
                                llllllllllllllllllll5.xy /= llllllllllllllllllll5.w;
                                llllllllllllllllllll5.x *= llllllllllllllllllllll5;
#ifdef _HDRP
                                        float4 lllllllllllllllllllllllll5 = mul(UNITY_MATRIX_VP, float4(GetCameraRelativePositionWS(d.worldSpacePosition.xyz), 1.0));
                                        lllllllllllllllllll5 = ComputeScreenPos(lllllllllllllllllllllllll5 , _ProjectionParams.x);
#else
                                float4 lllllllllllllllllllllllll5 = mul(UNITY_MATRIX_VP, float4(d.worldSpacePosition.xyz, 1.0));
                                lllllllllllllllllll5 = ComputeScreenPos(lllllllllllllllllllllllll5);
#endif
                                lllllllllllllllllll5.xy /= lllllllllllllllllll5.w;
                                lllllllllllllllllll5.x *= llllllllllllllllllllll5;
#if defined(_DISSOLVEMASK)
                                        if(lllllllllllllllllllllllllllllll0) {
                                                lllllllllllllllllllll5 = max(llllllllllll2.z,llllllllllll2.w);
                                        }
#endif
                            }
                            float3 lllllllllllllllllllllllllll5 = _WorldSpaceCameraPos - _PlayersPosVectorArray[llllllllllllllllll4].xyz;
                            float3 llllllllllllllllllllllllllll5 = normalize(lllllllllllllllllllllllllll5);
                            float lllllllllllllllllllllllllllll3 = dot(d.worldSpacePosition.xyz - _PlayersPosVectorArray[llllllllllllllllll4].xyz, llllllllllllllllllllllllllll5);
                            float llllllllllllllllllllllllllllll5 = 0;
                            float lllllllllllllllllllllllllllllll5 = 0;
                            float2 l6 = float2(0, 0);
                            if (llllllllllllllllllll0 == 2 || llllllllllllllllllll0 == 3)
                            {
                                llllllllllllllllllllllllllllll5 = llllllllllllllllllllll0;
                                float l4 = length((d.worldSpacePosition.xyz - _PlayersPosVectorArray[llllllllllllllllll4].xyz) - lllllllllllllllllllllllllllll3 * llllllllllllllllllllllllllll5);
                                float llllllll4 = length(lllllllllllllllllllllllllll5);
                                float lllllll4 = lllllllllllllllllllllll0;
                                float lllllllllll4 = (lllllllllllllllllllllllllllll3 / llllllll4) * lllllll4;
#if _DISSOLVEMASK
                                        float llllll6 = (2*lllllllllll4) / lllllllllllllllllllll5;
                                        float2 lllllll6 = lllllllllllllllllll5.xy - llllllllllllllllllll5.xy;
                                        lllllll6 =  normalize(lllllll6)*l4;
                                        l6 = lllllll6 /llllll6;
#else
                                float llllllll6 = l4 < lllllllllll4;
                                if (llllllll6)
                                {
                                    float lllllllll6 = l4 / lllllllllll4;
                                    lllllllllllllllllllllllllllllll5 = lllllllll6;
                                }
                                else
                                {
                                    lllllllllllllllllllllllllllllll5 = -1;
                                }
#endif
                            }
                            else if (llllllllllllllllllll0 == 4 || llllllllllllllllllll0 == 5)
                            {
                                llllllllllllllllllllllllllllll5 = llllllllllllllllllllllll0;
                                float l4 = length((d.worldSpacePosition.xyz - _PlayersPosVectorArray[llllllllllllllllll4].xyz) - lllllllllllllllllllllllllllll3 * llllllllllllllllllllllllllll5);
                                float llllllllllllllllllllllllllllll3 = lllllllllllllllllllllllll0;
                                float llllllllllll6 = (l4 < llllllllllllllllllllllllllllll3) && lllllllllllllllllllllllllllll3 > 0;
#if _DISSOLVEMASK
                                        float llllll6 = (2*llllllllllllllllllllllllllllll3) / lllllllllllllllllllll5;
                                        float2 lllllll6 = lllllllllllllllllll5.xy - llllllllllllllllllll5.xy;
                                        lllllll6 =  normalize(lllllll6)*l4;
                                        l6 = lllllll6 /llllll6;
#else
                                if (llllllllllll6)
                                {
                                    float lllllllll6 = l4 / llllllllllllllllllllllllllllll3;
                                    lllllllllllllllllllllllllllllll5 = lllllllll6;
                                }
                                else
                                {
                                    lllllllllllllllllllllllllllllll5 = -1;
                                }
#endif
                            }
                            else if (llllllllllllllllllll0 == 6)
                            {
                                llllllllllllllllllllllllllllll5 = llllllllllllllllllllllllll0;
                                float llllllllllllllll6 = length(lllllllllllllllllllllllllll5);
                                float llllllllllllllllllllll5 = _ScreenParams.x / _ScreenParams.y;
                                float llllllllllllllllll6 = min(1, llllllllllllllllllllll5);
                                float lllllllllllllllllll6 = distance(lllllllllllllllllll5.xy, llllllllllllllllllll5.xy) < lllllllllllllllllllllllllll0 / llllllllllllllll6 * llllllllllllllllll6;
                                float llllllllllllllllllll6 = (lllllllllllllllllll6) && lllllllllllllllllllllllllllll3 > 0;
#if _DISSOLVEMASK
                                        float lllllllllllllllllllll6 = lllllllllllllllllllllllllll0/llllllllllllllll6*llllllllllllllllll6;
                                        float llllll6 = (2*lllllllllllllllllllll6) / lllllllllllllllllllll5;
                                        float2 lllllll6 = lllllllllllllllllll5.xy - llllllllllllllllllll5.xy;
                                        l6 = lllllll6 /llllll6;
#else
                                if (llllllllllllllllllll6)
                                {
                                    float llllllllllllllllllllllll6 = (distance(lllllllllllllllllll5.xy, llllllllllllllllllll5.xy) / (lllllllllllllllllllllllllll0 / llllllllllllllll6 * llllllllllllllllll6));
                                    lllllllllllllllllllllllllllllll5 = llllllllllllllllllllllll6;
                                }
                                else
                                {
                                    lllllllllllllllllllllllllllllll5 = -1;
                                }
#endif
                            }
                            else if (llllllllllllllllllll0 == 7)
                            {
#if _OBSTRUCTION_CURVE
                                        llllllllllllllllllllllllllllll5 = llllllllllllllllllllllllllll0;
                                        float l4 = length((d.worldSpacePosition.xyz  - _PlayersPosVectorArray[llllllllllllllllll4].xyz) - lllllllllllllllllllllllllllll3 * llllllllllllllllllllllllllll5);
                                        float llllllllllllllll6 = length(lllllllllllllllllllllllllll5);
                                        float4 lllllllllllllllllllllllllll6 = float4(0,0,0,0);
                                        float llllllllllllllllllllllllllll6 = lllllllllllll2.z;
                                        float lllllllllllllllllllllllllllll6 = (lllllllllllllllllllllllllllll3/llllllllllllllll6) * llllllllllllllllllllllllllll6;
                                        float4 llllllllllllllllllllllllllllll6 = float4(0,0,0,0);
                                        llllllllllllllllllllllllllllll6 = lllllllllllll2;
                                        float2 lllllllllllllllllllllllllllllll6 = (lllllllllllllllllllllllllllll6+0.5) * llllllllllllllllllllllllllllll6.xy;
                                            lllllllllllllllllllllllllll6 = tex2D(ll2, lllllllllllllllllllllllllllllll6);
                                        float l7 = lllllllllllllllllllllllllll6.r * lllllllllllllllllllllllllllll0;
                                        float ll7 = (l4 < l7) && lllllllllllllllllllllllllllll3 > 0 ;
#if _DISSOLVEMASK
                                            float llllll6 = (2*l7) / lllllllllllllllllllll5;
                                            float2 lllllll6 = lllllllllllllllllll5.xy - llllllllllllllllllll5.xy;
                                            lllllll6 =  normalize(lllllll6)*l4;
                                            l6 = lllllll6 /llllll6;
#else
                                            if(ll7){
                                                float lllllllll6 = l4/l7;
                                                lllllllllllllllllllllllllllllll5 = lllllllll6;
                                            } else {
                                                lllllllllllllllllllllllllllllll5 = -1;
                                            }
#endif
#endif
                            }
#if defined(_DISSOLVEMASK)
                                    if(lllllllllllllllllllllllllllllll0) {
                                        float4 llllll7 = float4(0,0,0,0);
                                        llllll7 = llllllllllll2;
                                        float2 lllllll7 = float2(llllll7.z/2,llllll7.w/2);
                                        float2 llllllll7 = lllllll7 + l6;
                                        float2 lllllllll7 = (llllllll7+0.5) * llllll7.xy;
                                        float4 llllllllll7 = float4(0,0,0,0);
                                            llllllllll7 = tex2D(l2, lllllllll7);
                                        float lllllllllll7 = -1;
                                        if(llllllll7.x <= llllll7.z && llllllll7.x >= 0 && llllllll7.y <= llllll7.w && llllllll7.y >= 0 && llllllllll7.x <= 0 && lllllllllllllllllllllllllllll3 > 0 ){
                                            float llllllllllll7 = sqrt(pow(llllll7.z,2)+pow(llllll7.w,2))/2;
                                            float lllllllllllll7 = 40;
                                            float llllllllllllll7 = llllllllllll7/lllllllllllll7;
                                            float lllllllllllllll7 = 0;
                                            lllllllllll7 = 0;     
                                                for (int i = 0; i < lllllllllllll7; i++){
                                                    float2 llllllllllllllll7 = lllllll7 + (l6 + ( normalize(l6)*llllllllllllll7*i));
                                                    float2 lllllllllllllllll7 = (llllllllllllllll7+0.5) * llllll7.xy;
                                                    float4 llllllllllllllllll7 = tex2Dlod(l2, float4(lllllllllllllllll7, 0.0, 0.0)); 
                                                    float2 lllllllllllllllllll7 = step(float2(0,0), llllllllllllllll7) - step(float2(llllll7.z,llllll7.w), llllllllllllllll7);
                                                    if(llllllllllllllllll7.x <= 0) {
                                                        lllllllllllllll7 +=  (1/lllllllllllll7) * (lllllllllllllllllll7.x * lllllllllllllllllll7.y);
                                                    }                                            
                                                }   
                                            lllllllllll7 = 1-lllllllllllllll7;  
                                        }         
                                        lllllllllllllllllllllllllllllll5 = lllllllllll7;
                                    }
#endif
                            if (llllllllllllllllllllllllllllll0 <= 1)
                            {
                                if (lllllllllllllllllllllllllllllll5 != -1)
                                {
                                    float llllllllllllllllllll7 = max(llllllllllllllllllllllllllllll0, 0.00001);
                                    float lllllllllllllllllllll7 = 1 - llllllllllllllllllllllllllllll5;
                                    float llllllllllllllllllllll7 = exp(llllllllllllllllllll7 * 6);
                                    float lllllllllllllllllllllll7 = lllllllllllllllllllllllllllllll5;
                                    float llllllllllllllllllllllll7 = lllllllllllllllllllll7 / (llllllllllllllllllll7 / (llllllllllllllllllll7 * lllllllllllllllllllll7 - 0.15 * (llllllllllllllllllll7 - lllllllllllllllllllll7)));
                                    float lllllllllllllllllllllllll7 = ((lllllllllllllllllllllll7 - llllllllllllllllllllllll7) / (llllllllllllllllllllll7 * (1 - lllllllllllllllllllllll7) + lllllllllllllllllllllll7)) + llllllllllllllllllllllll7;
                                    lllllllllllllllllllllllll7 = 1 - lllllllllllllllllllllllll7;
                                    llllllll5 = lllllllllllllllllllllllll7 * sign(llllllllllllllllllllllllllllll5);
                                }
                            }
                            else
                            {
                                llllllll5 = lllllllllllllllllllllllllllllll5;
                            }
                        }
                        if (llllllllllllllllllll0 == 1 || llllllllllllllllllll0 == 3 || llllllllllllllllllll0 == 5)
                        {
                            float llllllllllllllllllllllllll7 = distance(_WorldSpaceCameraPos, _PlayersPosVectorArray[llllllllllllllllll4].xyz);
                            float lllllllllllllllllllllllllll7 = distance(_WorldSpaceCameraPos, d.worldSpacePosition.xyz);
                            float3 llllllllllllllllllllllllllll7 = d.worldSpacePosition.xyz - _PlayersPosVectorArray[llllllllllllllllll4].xyz;
                            float3 lllllllllllllllllllllllllllll7 = d.worldSpaceNormal;
                            float llllllllllllllllllllllllllllll7 = acos(dot(llllllllllllllllllllllllllll7, lllllllllllllllllllllllllllll7) / (length(llllllllllllllllllllllllllll7) * length(lllllllllllllllllllllllllllll7)));
                            if (llllllllllllllllllllllllllllll7 <= 1.5 && llllllllllllllllllllllllll7 > lllllllllllllllllllllllllll7)
                            {
                                float lllllllllllllllllllllllllllllll7 = (sqrt((llllllllllllllllllllllllll7 - lllllllllllllllllllllllllll7)) * 25 / llllllllllllllllllllllllllllll7) * lllllllllllllllllllll0;
                                llllllll5 += max(0, log(lllllllllllllllllllllllllllllll7 * 0.2));
                            }
                        }
                    }
                    float l8 = llllllll5;
                    float ll8 = 0;
                    float lll8 = 0;
                    if (l1 == 1 && llllllllllllllllllllllll1 == 0 && !lllllllllllllllllllllllll1)
                    {
                        llllllll5 = min((1 * ll1), 1);
                        lllllllll5 = llllllll5;
                    }
                    else
                    {
                        llllllll5 = min(llllllll5 + (1 * ll1), 1);
                        lllllllll5 = min((1 * ll1), 1);
                    }
                    if (lllllllllllllllllllllllllll2)
                    {
                        if (llllllllllllllllllllllll1 == 1)
                        {
                            float lllllllllllllllll5 = 1 / llllllllllllllllllllllllll1;
                            if (lllllllllllllllllllllllllllll2 < llllllllllllllllllllllllll1)
                            {
                                float lllll8 = 1 - ((llllllllllllllllllllllllll1 - lllllllllllllllllllllllllllll2) * lllllllllllllllll5);
                                llllllll5 = min(llllllll5, lllll8);
                                lllllllll5 = min(lllllllll5, lllll8);
                            }
                        }
                        else if (llllllllllllllllllllllll1 == 0 && !lllllllllllllllllllllllll1)
                        {
                            if (l1 == 1)
                            {    
                                float llllll8 = ((l8) / llllllllllllllllllllllllll1);
                                if (lllllllllllllllllllllllllllll2 < llllllllllllllllllllllllll1 && l8 > 0 && saturate(l8) > ll1)
                                {
                                    float lllll8 = ((llllllllllllllllllllllllll1- lllllllllllllllllllllllllllll2) * (llllll8));
                                    l8 = l8 - (lllll8);
                                }
                                else
                                {
                                }
                            }
                            if (lllllllllllllllllllllllllllll2 < llllllllllllllllllllllllll1)
                            {
                                float lllllllllllllllll5 = llllllll5 / llllllllllllllllllllllllll1;
                                float lllll8 = ((llllllllllllllllllllllllll1 - lllllllllllllllllllllllllllll2) * lllllllllllllllll5);
                                llllllll5 = max(0, lllll8);
                                float llllllllll8 = lllllllll5 / llllllllllllllllllllllllll1;
                                float lllllllllll8 = ((llllllllllllllllllllllllll1 - lllllllllllllllllllllllllllll2) * llllllllll8);
                                lllllllll5 = max(0, lllllllllll8);
                                ll8 = llllllll5;
                                lll8 = lllllllll5;
                                if (l1 == 0 || l1 == 1)
                                {
                                    llllllll5 = max(l8, lllll8);
                                }
                            }
                            else
                            {
                                llllllll5 = 0;      
                                lllllllll5 = 0;
                                ll8 = llllllll5;
                                lll8 = lllllllll5;
                                if (l1 == 0 || l1 == 1)
                                {
                                    llllllll5 = max(l8, llllllll5);
                                }
                            }
                        }
                    }
                    if (lllll1)
                    {
                        float llllllllllll8 = llllllll5 / lllllll1;                    
                        float lllllllllllll8 = lllllllll5 / lllllll1;                       
                        float3 lllllllllllllllllll4 = _PlayersPosVectorArray[llllllllllllllllll4].xyz - _WorldSpaceCameraPos;
                        float3 lllllllllllllll8 = d.worldSpacePosition.xyz - _WorldSpaceCameraPos;
                        float llllllllllllllll8 = dot(lllllllllllllll8, normalize(lllllllllllllllllll4));
                        if (llllllllllllllll8 - llllll1 >= length(lllllllllllllllllll4))
                        {
                            float lllllllllllllllll8 = llllllllllllllll8 - llllll1 - length(lllllllllllllllllll4);
                            if (lllllllllllllllll8 < 0)
                            {
                                lllllllllllllllll8 = 0;
                            }
                            if (lllllllllllllllll8 < lllllll1)
                            {
                                llllllll5 = (lllllll1 - lllllllllllllllll8) * llllllllllll8;
                                lllllllll5 = (lllllll1 - lllllllllllllllll8) * lllllllllllll8;
                            }
                            else
                            {
                                llllllll5 = 0;
                                lllllllll5 = 0;
                            }
                        }
                    }
                    if (lllllllllllllllllllllll1 && !lllllllllllllllllllllllllll2)
                    {
                        if (llllllllllllllllllllllll1 == 1)
                        {
                            llllllll5 = 0;
                            lllllllll5 = 0;
                        }
                    }
                    if (llllllll1 == 1)
                    {
                        float llllllllllllllllll8 = 0;
                        float lllllllllllllllllll8 = 0;
                        if (llllllllll1 == 0)
                        {
                            llllllllllllllllll8 = llllllll5 / lllllllllllll1;
                            lllllllllllllllllll8 = lllllllll5 / lllllllllllll1;
                        }
                        else if (llllllllll1 == 1)
                        {
                            float llllllllllllllllllll8 = 1 - llllllll5;
                            float lllllllllllllllllllll8 = 1 - lllllllll5;
                            if (lllllllllllllllllllllll1 && lllllllllllllllllllllllllll2 && lllllllllllllllllllllllll1)
                            {
                                llllllllllllllllllll8 = max(1 - llllllll5, 1 - (llllllll5 * lllllll5));
                                lllllllllllllllllllll8 = max(1 - lllllllll5, 1 - (lllllllll5 * lllllll5));
                            }
                            llllllllllllllllll8 = llllllllllllllllllll8 / lllllllllllll1;
                            lllllllllllllllllll8 = lllllllllllllllllllll8 / lllllllllllll1;
                        }
                        if (lllllllll1 == 1)
                        {
                            if (d.worldSpacePosition.y > (_PlayersPosVectorArray[llllllllllllllllll4].y + llllllllllll1))
                            {
                                float lllllllllllllllll8 = d.worldSpacePosition.y - (_PlayersPosVectorArray[llllllllllllllllll4].y + llllllllllll1);
                                if (lllllllllllllllll8 < 0)
                                {
                                    lllllllllllllllll8 = 0;
                                }
                                if (llllllllll1 == 0)
                                {
                                    if (lllllllllllllllll8 < lllllllllllll1)
                                    {
                                        llllllll5 = ((lllllllllllll1 - lllllllllllllllll8) * llllllllllllllllll8);
                                        lllllllll5 = ((lllllllllllll1 - lllllllllllllllll8) * lllllllllllllllllll8);
                                    }
                                    else
                                    {
                                        llllllll5 = 0;
                                        lllllllll5 = 0;
                                    }
                                }
                                else
                                {
                                    if (lllllllllllllllll8 < lllllllllllll1)
                                    {
                                        llllllll5 = 1 - ((lllllllllllll1 - lllllllllllllllll8) * llllllllllllllllll8);
                                        lllllllll5 = 1 - ((lllllllllllll1 - lllllllllllllllll8) * lllllllllllllllllll8);
                                    }
                                    else
                                    {
                                        llllllll5 = 1;
                                        lllllllll5 = 1;
                                    }
                                    lllllll5 = 1;
                                }
                            }
                        }
                        else
                        {
                            if (d.worldSpacePosition.y > lllllllllll1)
                            {
                                float lllllllllllllllll8 = d.worldSpacePosition.y - lllllllllll1;
                                if (lllllllllllllllll8 < 0)
                                {
                                    lllllllllllllllll8 = 0;
                                }
                                if (llllllllll1 == 0)
                                {
                                    if (lllllllllllllllll8 < lllllllllllll1)
                                    {
                                        llllllll5 = ((lllllllllllll1 - lllllllllllllllll8) * llllllllllllllllll8);
                                        lllllllll5 = ((lllllllllllll1 - lllllllllllllllll8) * lllllllllllllllllll8);
                                    }
                                    else
                                    {
                                        llllllll5 = 0;
                                        lllllllll5 = 0;
                                    }
                                }
                                else
                                {
                                    if (lllllllllllllllll8 < lllllllllllll1)
                                    {
                                        llllllll5 = 1 - ((lllllllllllll1 - lllllllllllllllll8) * llllllllllllllllll8);
                                        lllllllll5 = 1 - ((lllllllllllll1 - lllllllllllllllll8) * lllllllllllllllllll8);
                                    }
                                    else
                                    {
                                        llllllll5 = 1;
                                        lllllllll5 = 1;
                                    }
                                    lllllll5 = 1;
                                }
                            }
                        }
                    }
                    float llllllllllllllllllllllll8 = llllllll5;
                    float lllllllllllllllllllllllll8 = lllllllll5;
                    if (llllllllllllll1 == 1)
                    {
                        float llllllllllllllllllllllllll8 = llllllll5 / llllllllllllllllll1;
                        float lllllllllllllllllllllllllll8 = lllllllll5 / llllllllllllllllll1;
                        if (lllllllllllllll1 == 1)
                        {
                            if (d.worldSpacePosition.y < (_PlayersPosVectorArray[llllllllllllllllll4].y + lllllllllllllllll1))
                            {
                                float lllllllllllllllll8 = (_PlayersPosVectorArray[llllllllllllllllll4].y + lllllllllllllllll1) - d.worldSpacePosition.y;
                                if (lllllllllllllllll8 < 0)
                                {
                                    lllllllllllllllll8 = 0;
                                }
                                if (lllllllllllllllll8 < llllllllllllllllll1)
                                {
                                    llllllll5 = (llllllllllllllllll1 - lllllllllllllllll8) * llllllllllllllllllllllllll8;
                                    lllllllll5 = (llllllllllllllllll1 - lllllllllllllllll8) * lllllllllllllllllllllllllll8;
                                }
                                else
                                {
                                    llllllll5 = 0;
                                    lllllllll5 = 0;
                                }
                            }
                        }
                        else
                        {
                            if (d.worldSpacePosition.y < llllllllllllllll1)
                            {
                                float lllllllllllllllll8 = llllllllllllllll1 - d.worldSpacePosition.y;
                                if (lllllllllllllllll8 < 0)
                                {
                                    lllllllllllllllll8 = 0;
                                }
                                if (lllllllllllllllll8 < llllllllllllllllll1)
                                {
                                    llllllll5 = (llllllllllllllllll1 - lllllllllllllllll8) * llllllllllllllllllllllllll8;
                                    lllllllll5 = (llllllllllllllllll1 - lllllllllllllllll8) * lllllllllllllllllllllllllll8;
                                }
                                else
                                {
                                    llllllll5 = 0;
                                    lllllllll5 = 0;
                                }
                            }
                        }
                        if (lllllllllllllllllll1 == 0) 
                        {
                        }
                        else if (lllllllllllllllllll1 == 1) 
                        {
                            if (lllllllllllllllllllllllllll2)
                            {
                                llllllll5 = max(ll8, llllllll5);
                                lllllllll5 = max(lll8, lllllllll5);
                            }
                            else
                            {
                                llllllll5 = llllllllllllllllllllllll8;
                                lllllllll5 = lllllllllllllllllllllllll8;
                            }
                        }
                        else if (lllllllllllllllllll1 == 2) 
                        {
                            if (lllllllllllllllllllllllllll2)
                            {
                                llllllll5 = min(ll8, llllllll5);
                                llllllll5 = max(l8, llllllll5);
                                lllllllll5 = min(lll8, lllllllll5);
                            }
                        }
                    }
                    if (!lllllllll0 && !llllllllll0)
                    {
                        if (llll1 == 1 && distance(_PlayersPosVectorArray[llllllllllllllllll4].xyz, d.worldSpacePosition) > lll1)
                        {
                            llllllll5 = 0;
                            lllllllll5 = 0;
                        }
                    }
                }
                llllllllllllllll4 = llllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllll4] * 4 + 1;
                if (lllllllllllllllllllllll1 && lllllllllllllllllllllllllll2 && lllllllllllllllllllllllll1)
                {
                    l5 = l5 * lllllll5;
                }
                if (lllllllll0 || llllllllll0)
                {
                    llllllll5 = l5 * llllllll5;
                    lllllllll5 = l5 * lllllllll5;
                }
                else
                {
                    if (lllllllllllllllllllllll1)
                    {
                        if (lllllllllllllllllllllllllll2)
                        {
                            if (lllllllllllllllllllllllll1)
                            {
                                llllllll5 = lllllll5 * llllllll5;
                                lllllllll5 = lllllll5 * lllllllll5;
                            }
                        }
                        else
                        {
                            if (llllllllllllllllllllllll1 == 1)
                            {
                                llllllll5 = lllllll5 * llllllll5;
                                lllllllll5 = lllllll5 * lllllllll5;
                            }
                        }
                    }
                }
                llllllllllllllllllllllll2 = max(llllllllllllllllllllllll2, llllllll5);
                lllllllllllllllllllllllll2 = max(lllllllllllllllllllllllll2, lllllllll5);
            }
            else
            {
                llllllllllllllll4 = llllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllll4 + 2] * 4 + 3;
                llllllllllllllll4 = llllllllllllllll4 + _PlayersDataFloatArray[llllllllllllllll4] * 4 + 1;
            }
            }
#else
        float l5 = 0;
        if (!lllllllll2)
        {
            l5 = 1;
            if (llllll0 != 0 && lllllll0 != 0 && _Time.y - lllllll0 < llllllllllllllllllllll1)
            {
                if (llllll0 == 1)
                {
                    l5 = ((llllllllllllllllllllll1 - (_Time.y - lllllll0)) / llllllllllllllllllllll1);
                }
                else
                {
                    l5 = ((_Time.y - lllllll0) / llllllllllllllllllllll1);
                }
            }
            else if (llllll0 == -1)
            {
                l5 = 1;
            }
            else if (llllll0 == 1)
            {
                l5 = 0;
            }
            else
            {
                l5 = 1;
            }
            l5 = 1 - l5;
        }
        float llllllll5 = 0;
        float lllllll5 = 0;
        bool lllllllllll5 = distance(_WorldSpaceCameraPos, d.worldSpacePosition) > lll1;
        if ((l5 != 0) || ((!lllllllll0 && !llllllllll0) && (llll1 == 0 || !lllllllllll5) ))
        {
#if defined(_ZONING)
                        if(lllllllllllllllllllllll1) {
                            if(lllllllllllllllllllllllllll2) 
                            {
                                if(lllllllllllllllllllllllll1) {
                                    float lllllllllllllllllllllll4 = lllllllllllllllllllllllllllllll2;
                                    float llllllllllllllllllllllll4 = ll3;
                                    lllllll5 = 1;
                                    float llllllllllllllll5 = l3;
                                    if( llllllllllllllllllllllll4!= 0 && lllllllllllllllllllllll4 != 0 && _Time.y-lllllllllllllllllllllll4 < llllllllllllllll5) {
                                        if(llllllllllllllllllllllll4 == 1) {
                                            lllllll5 = ((llllllllllllllll5-(_Time.y-lllllllllllllllllllllll4))/llllllllllllllll5);
                                        } else {
                                            lllllll5 = ((_Time.y-lllllllllllllllllllllll4)/llllllllllllllll5);
                                        }
                                    } else if(llllllllllllllllllllllll4 ==-1) {
                                        lllllll5 = 1;
                                    } else if(llllllllllllllllllllllll4 == 1) {
                                        lllllll5 = 0;
                                    } else {
                                        lllllll5 = 1;
                                    }
                                    lllllll5 = 1 - lllllll5;
                                    if(llllllllllllllllllllllll1 == 0 && lllllllllllllllllllllllll1) {
                                        float lllllllllllllllll5 = 1 / llllllllllllllllllllllllll1;
                                        if (lllllllllllllllllllllllllllll2 < llllllllllllllllllllllllll1)  {
                                            float llllllllllllllllll5 = ((llllllllllllllllllllllllll1-lllllllllllllllllllllllllllll2) * lllllllllllllllll5);
                                            lllllll5 =  max(lllllll5,llllllllllllllllll5);
                                        }
                                    }
                                } else { 
                                }
                            } else {
                            }
                        }
#endif
            llllllll5 = min(llllllll5 + (1 * ll1), 1);
            if (lllllllllllllllllllllllllll2)
            {
                if (llllllllllllllllllllllll1 == 1)
                {
                    float lllllllllllllllll5 = 1 / llllllllllllllllllllllllll1;
                    if (lllllllllllllllllllllllllllll2 < llllllllllllllllllllllllll1)
                    {
                        float lllllllll9 = 1 - ((llllllllllllllllllllllllll1 - lllllllllllllllllllllllllllll2) * lllllllllllllllll5);
                        llllllll5 = min(llllllll5, lllllllll9);
                    }
                }
                else if (llllllllllllllllllllllll1 == 0 && !lllllllllllllllllllllllll1)
                {
                    float lllllllllllllllll5 = llllllll5 / llllllllllllllllllllllllll1;
                    if (lllllllllllllllllllllllllllll2 < llllllllllllllllllllllllll1)
                    {
                        float lllllllll9 = ((llllllllllllllllllllllllll1 - lllllllllllllllllllllllllllll2) * lllllllllllllllll5);
                        llllllll5 = max(0, lllllllll9);
                    }
                    else
                    {
                        llllllll5 = 0;
                    }
                }
            }
            if (lllllllllllllllllllllll1 && !lllllllllllllllllllllllllll2)
            {
                if (llllllllllllllllllllllll1 == 1)
                {
                    llllllll5 = 0;
                }
            }
            if (llllllll1 == 1 && lllllllll1 == 0)
            {
                float llllllllllllllllll8 = 0;
                if (llllllllll1 == 0)
                {
                    llllllllllllllllll8 = (llllllll5) / lllllllllllll1;
                }
                else if (llllllllll1 == 1)
                {
                    float llllllllllllllllllll8 = 1 - llllllll5;
                    if (lllllllllllllllllllllll1 && lllllllllllllllllllllllllll2 && lllllllllllllllllllllllll1)
                    {
                        llllllllllllllllllll8 = max(1 - llllllll5, 1 - (llllllll5 * lllllll5));
                    }
                    llllllllllllllllll8 = llllllllllllllllllll8 / lllllllllllll1;
                }
                if (d.worldSpacePosition.y > lllllllllll1)
                {
                    float lllllllllllllllll8 = d.worldSpacePosition.y - lllllllllll1;
                    if (lllllllllllllllll8 < 0)
                    {
                        lllllllllllllllll8 = 0;
                    }
                    if (llllllllll1 == 0)
                    {
                        if (lllllllllllllllll8 < lllllllllllll1)
                        {
                            llllllll5 = ((lllllllllllll1 - lllllllllllllllll8) * llllllllllllllllll8);
                        }
                        else
                        {
                            llllllll5 = 0;
                        }
                    }
                    else
                    {
                        if (lllllllllllllllll8 < lllllllllllll1)
                        {
                            llllllll5 = 1 - ((lllllllllllll1 - lllllllllllllllll8) * llllllllllllllllll8);
                        }
                        else
                        {
                            llllllll5 = 1;
                        }
                        lllllll5 = 1;
                    }
                }
            }
            if (llllllllllllll1 == 1 && lllllllllllllll1 == 0)
            {
                float llllllllllllllllllllllllll8 = llllllll5 / llllllllllllllllll1;
                if (d.worldSpacePosition.y < llllllllllllllll1)
                {
                    float lllllllllllllllll8 = llllllllllllllll1 - d.worldSpacePosition.y;
                    if (lllllllllllllllll8 < 0)
                    {
                        lllllllllllllllll8 = 0;
                    }
                    if (lllllllllllllllll8 < llllllllllllllllll1)
                    {
                        llllllll5 = (llllllllllllllllll1 - lllllllllllllllll8) * llllllllllllllllllllllllll8;
                    }
                    else
                    {
                        llllllll5 = 0;
                    }
                }
            }
        }
        if (lllllllllllllllllllllll1 && lllllllllllllllllllllllllll2 && lllllllllllllllllllllllll1)
        {
            l5 = l5 * lllllll5;
        }
        if (lllllllll0 || llllllllll0)
        {
            llllllll5 = l5 * llllllll5;
        }
        else
        {
            llllllll5 = llllllll5;
            if (lllllllllllllllllllllll1)
            {
                if (lllllllllllllllllllllllllll2)
                {
                    if (lllllllllllllllllllllllll1)
                    {
                        llllllll5 = lllllll5 * llllllll5;
                    }
                }
                else
                {
                    if (llllllllllllllllllllllll1 == 1)
                    {
                        llllllll5 = lllllll5 * llllllll5;
                    }
                }
            }
        }
        llllllllllllllllllllllll2 = max(llllllllllllllllllllllll2, llllllll5);
#endif
            float llllllllll5 = llllllllllllllllllllllll2;
            if (!lllllllllllllllllllllllllllll1)
            {
                if (llllllllll5 == 1)
                {
                    llllllllll5 = 10;
                }
            if (!lllllllllllllllllll0) 
            {
#if defined(UNITY_PASS_SHADOWCASTER) 
#if defined(SHADOWS_DEPTH) 
                if (!any(unity_LightShadowBias))
                {
#if !defined(NO_STS_CLIPPING)
                        clip(llllllllllllllllllllll2 - llllllllll5);
#endif
                    lllllll2 = llllllllllllllllllllll2 - llllllllll5;
                }
                else
                {
                    if(lllllllllllllllllll0) 
                    {
#if !defined(NO_STS_CLIPPING)
                        clip(llllllllllllllllllllll2 - llllllllll5);
#endif
                        lllllll2 = llllllllllllllllllllll2 - llllllllll5;
                    }
                }
#endif
#else
#if !defined(NO_STS_CLIPPING)
                    clip(llllllllllllllllllllll2 - llllllllll5);
#endif
                    lllllll2 = llllllllllllllllllllll2 - llllllllll5;
#endif
            }
            else
            {
                if (llllllllllllllllllll0 == 6 && lllllllllllllllllll0)
                {
#if defined(UNITY_PASS_SHADOWCASTER) 
#if defined(SHADOWS_DEPTH) 
                    if (!any(unity_LightShadowBias))
                    {
                    }
                    else
                    {
                        llllllllll5 = lllllllllllllllllllllllll2;
                        if (llllllllll5 == 1)
                        {
                            llllllllll5 = 10;
                        }                    
                    }                
#endif
#endif
                }
#if !defined(NO_STS_CLIPPING)
                    clip(llllllllllllllllllllll2 - llllllllll5);
#endif
                    lllllll2 = llllllllllllllllllllll2 - llllllllll5;
                }
                if (llllllllllllllllllllll2 - llllllllll5 < 0)
                {
                    lllllll2 = 0;
                }
                else
                {
                    lllllll2 = 1;
                }
            }
            if (lllllllllllllllllllllllllllll1)
            {
                llllllllll2 = 1;
                if ((llllllllllllllllllllll2 - llllllllll5) < 0)
                {
                    lllllllllll2 = half4(1, 1, 1, 1);
                    o.Emission = 1;
            }
                else
                {
                    lllllllllll2 = half4(0, 0, 0, 1);
                }
                if (lllllllllllllll4)
                {
                    if ((llllllllllllllllllllll2 - llllllllll5) < 0)
                    {
                        lllllllllll2 = half4(0.5, 1, 0.5, 1);
                        o.Emission = 0; 
                }
                    else
                    {
                        lllllllllll2 = half4(0, 0.1, 0, 1);
                    }
                }
                if (lllllllllllllllllllllllllll2 && llllllllllllll1 == 1 && lllllllllllllllllllllllllll1)
                {
                    float llllllllllllllllll9 = 0;
                    if (lllllllllllllll1 == 1)
                    {
                        llllllllllllll4 = llllllllllllll4 + llllllllllllllllllllllllllll1;
                        llllllllllllllllll9 = llllllllllllll4;
                    }
                    else
                    {
                        llllllllllllllllll9 = llllllllllllllll1 + llllllllllllllllllllllllllll1;
                    }
                    if (d.worldSpacePosition.y > (llllllllllllllllll9 - llllllllllllllllllllllllllllll1) && d.worldSpacePosition.y < (llllllllllllllllll9 + llllllllllllllllllllllllllllll1))
                    {
                        lllllllllll2 = half4(1, 0, 0, 1);
                    }
                }
            }
            else
            {
                half3 lllllllllllllllllll9 = lerp(1, llllllllllll0, lllllllllllll0).rgb;
                if (lllllllllllllllll0)
                {
                    llllllllllllllllll0 = 0.2 + (llllllllllllllllll0 * (0.8 - 0.2));
                    o.Emission = o.Emission + min(clamp(lllllllllllllllllll9 * clamp(((llllllllll5 / llllllllllllllllll0) - llllllllllllllllllllll2), 0, 1), 0, 1) * sqrt(lllllllllllllll0 * llllllllllllllll0), clamp(lllllllllllllllllll9 * llllllllll5, 0, 1) * sqrt(lllllllllllllll0 * llllllllllllllll0));
                }
                else
                {
                    o.Emission = o.Emission + clamp(lllllllllllllllllll9 * llllllllll5, 0, 1) * sqrt(lllllllllllllll0 * llllllllllllllll0);
                }
            }
        }
        if (llllllllll2)
        {
            o.Albedo = lllllllllll2.rgb;
        }
        lllll2 = o.Albedo;
        llllll2 = o.Emission; 
        #ifdef _HDRP  
            float llllllllllllllllllll9 = 0;
            float lllllllllllllllllllll9 = 0;
            #if SHADEROPTIONS_PRE_EXPOSITION
                lllllllllllllllllllll9 =  LOAD_TEXTURE2D(_ExposureTexture, int2(0, 0)).x * _ProbeExposureScale;
            #else
                lllllllllllllllllllll9 = _ProbeExposureScale;
            #endif
                float llllllllllllllllllllll9 = 0;
                float lllllllllllllllllllllll9 = lllllllllllllllllllll9;
                llllllllllllllllllllll9 = rcp(lllllllllllllllllllllll9 + (lllllllllllllllllllllll9 == 0.0));
                float3 llllllllllllllllllllllll9 = o.Emission * llllllllllllllllllllll9;
                o.Emission = lerp(llllllllllllllllllllllll9, o.Emission, llllllllllllllllllll9);
            llllll2 = o.Emission;
        #endif

}

#endif
