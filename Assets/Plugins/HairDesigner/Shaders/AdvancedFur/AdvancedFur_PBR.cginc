
inline fixed4 Translucency(fixed3 viewDir, half3 surfaceNormal, UnityGI gi)
{

	//WIP
	return float4(0, 0, 0, 0);
	/*
#ifdef STRAND
	float F = clamp( dot(viewDir, -gi.light.dir),0,1) * _Translucency;
	float dt = clamp(dot(normalize(surfaceNormal), -gi.light.dir), 0, 1);
	F *= 1 - dt* dt;
	return  float4(gi.light.color * F* F,0);
#else
	return float4(0, 0, 0, 0);
#endif
*/
}


//---------------------------------------------------------
//UNITY_PASS_FORWARDBASE
//---------------------------------------------------------

#ifdef UNITY_PASS_FORWARD


	half4 PS_FRWD(GEOM_Input IN, fixed facing : VFACE) : SV_Target
	{
		
		UNITY_SETUP_INSTANCE_ID(IN);	
	/*
#ifdef STRAND	
	IN.tSpace0.z = lerp(IN.surfaceNormal.x, facing > 0 ? IN.tSpace0.z : -IN.tSpace0.z, _NormalSwitch);
	IN.tSpace1.z = lerp(IN.surfaceNormal.y, facing > 0 ? IN.tSpace1.z : -IN.tSpace1.z, _NormalSwitch);
	IN.tSpace2.z = lerp(IN.surfaceNormal.z, facing > 0 ? IN.tSpace2.z : -IN.tSpace2.z, _NormalSwitch);
#endif
*/
		//float3 worldPos = IN.worldPos;
		float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
#ifndef USING_DIRECTIONAL_LIGHT
		fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
		fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
		fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
#ifdef UNITY_COMPILER_HLSL
		SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
		SurfaceOutputStandard o;
#endif
		o.Albedo = 0.0;
		o.Emission = 0.0;
		o.Alpha = 0.0;
		o.Occlusion = 1.0;
		fixed3 normalWorldVertex = fixed3(0, 0, 1);
		//o.Normal = IN.worldNormal;
		o.Normal = fixed3(0, 0, 1);
		//normalWorldVertex = IN.worldNormal;
		
		// call surface function
		surf(IN, o,facing, worldViewDir);
		
		
		
		UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
		fixed4 c = 0;

		fixed3 worldN;
		worldN.x = dot(IN.tSpace0.xyz, o.Normal);
		worldN.y = dot(IN.tSpace1.xyz, o.Normal);
		worldN.z = dot(IN.tSpace2.xyz, o.Normal);
		worldN = normalize(worldN);
		o.Normal = worldN;


		// Setup lighting environment
		UnityGI gi;
		UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
		gi.indirect.diffuse = 0;
		gi.indirect.specular = 0;
		gi.light.color = _LightColor0.rgb;
		gi.light.dir = lightDir;
#ifdef UNITY_PASS_FORWARDADD
		gi.light.color *= atten;
#else

		// Call GI (lightmaps/SH/reflections) lighting function
		UnityGIInput giInput;
		UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
		giInput.light = gi.light;
		giInput.worldPos = worldPos;
		giInput.worldViewDir = worldViewDir;
		giInput.atten = atten;
	
	#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
			giInput.lightmapUV = IN.lmap;
	#else
			giInput.lightmapUV = 0.0;			
	#endif
			
	#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
			giInput.ambient = IN.sh;
	#else
			giInput.ambient.rgb = 0.0;
	#endif

		giInput.probeHDR[0] = unity_SpecCube0_HDR;
		giInput.probeHDR[1] = unity_SpecCube1_HDR;
	#if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
			giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
	#endif
	#ifdef UNITY_SPECCUBE_BOX_PROJECTION
			giInput.boxMax[0] = unity_SpecCube0_BoxMax;
			giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
			giInput.boxMax[1] = unity_SpecCube1_BoxMax;
			giInput.boxMin[1] = unity_SpecCube1_BoxMin;
			giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
	#endif
			LightingStandard_GI(o, giInput, gi);
#endif//UNITY_FORWARDADD				
		

		// realtime lighting: call lighting function
		c += LightingStandard(o, worldViewDir, gi);		
		//c += Translucency( worldViewDir, IN.surfaceNormal, gi);
		c.rgb += o.Emission;
		c.a = 0.0;
		UNITY_APPLY_FOG(IN.fogCoord, c); // apply fog
		UNITY_OPAQUE_ALPHA(c.a);

		return c;
	}
#endif



//---------------------------------------------------------
//UNITY_PASS_DEFERRED
//---------------------------------------------------------
#ifdef UNITY_PASS_DEFERRED

void PS_DFRD(GEOM_Input IN,
	out half4 outGBuffer0 : SV_Target0,
	out half4 outGBuffer1 : SV_Target1,
	out half4 outGBuffer2 : SV_Target2,
	out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
	, out half4 outShadowMask : SV_Target4
#endif
	, fixed facing : VFACE
)
{		
	
	UNITY_SETUP_INSTANCE_ID(IN);
	// prepare and unpack data
	/*
#ifdef STRAND	
	IN.tSpace0.z = lerp(IN.surfaceNormal.x, facing >= 0 ? IN.tSpace0.z : -IN.tSpace0.z, _NormalSwitch);
	IN.tSpace1.z = lerp(IN.surfaceNormal.y, facing >= 0 ? IN.tSpace1.z : -IN.tSpace1.z, _NormalSwitch);
	IN.tSpace2.z = lerp(IN.surfaceNormal.z, facing >= 0 ? IN.tSpace2.z : -IN.tSpace2.z, _NormalSwitch);
#endif
*/


	//float3 worldPos = IN.worldPos;
	float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
#ifndef USING_DIRECTIONAL_LIGHT
	fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
	fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif



	fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
	


#ifdef UNITY_COMPILER_HLSL
	SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
	SurfaceOutputStandard o;
#endif
	o.Albedo = 0.0;
	o.Emission = 0.0;
	o.Alpha = 0.0;
	o.Occlusion = 1.0;
	fixed3 normalWorldVertex = fixed3(0, 0, 1);
	//o.Normal = IN.worldNormal;
	o.Normal = fixed3(0, 0, 1);	

	//normalWorldVertex = facing > 0 ? normalWorldVertex : -normalWorldVertex;

	// call surface function
	surf(IN, o, facing, worldViewDir);
	fixed3 originalNormal = o.Normal;
	fixed3 worldN;
	worldN.x = dot(IN.tSpace0.xyz, o.Normal);
	worldN.y = dot(IN.tSpace1.xyz, o.Normal);
	worldN.z = dot(IN.tSpace2.xyz, o.Normal);
	worldN = normalize(worldN);
	o.Normal = worldN;


	half atten = 1;

	// Setup lighting environment
	UnityGI gi;
	UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
	gi.indirect.diffuse = 0;
	gi.indirect.specular = 0;
	gi.light.color = 0;
	gi.light.dir = half3(0, 1, 0);
	// Call GI (lightmaps/SH/reflections) lighting function
	UnityGIInput giInput;
	UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
	giInput.light = gi.light;
	giInput.worldPos = worldPos;
	giInput.worldViewDir = worldViewDir;
	giInput.atten = atten;
#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
	giInput.lightmapUV = IN.lmap;
#else
	giInput.lightmapUV = 0.0;
#endif
#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
	giInput.ambient = IN.sh;
#else
	giInput.ambient.rgb = 0.0;
#endif
	giInput.probeHDR[0] = unity_SpecCube0_HDR;
	giInput.probeHDR[1] = unity_SpecCube1_HDR;
#if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
	giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
#endif
#ifdef UNITY_SPECCUBE_BOX_PROJECTION
	giInput.boxMax[0] = unity_SpecCube0_BoxMax;
	giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
	giInput.boxMax[1] = unity_SpecCube1_BoxMax;
	giInput.boxMin[1] = unity_SpecCube1_BoxMin;
	giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif

	gi.light.dir = float3(1, 0, 0);

	LightingStandard_GI(o, giInput, gi);

	
	outEmission = 0;
	// call lighting function to output g-buffer
	outEmission = LightingStandard_Deferred(o, worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
	

#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
	outShadowMask = UnityGetRawBakedOcclusions(IN.lmap.xy, float3(0, 0, 0));
#endif
#ifndef UNITY_HDR_ON
	outEmission.rgb = exp2(-outEmission.rgb);
#endif
	
	
	//outEmission.rgb += Translucency(worldViewDir, worldN, gi);


}


#endif


