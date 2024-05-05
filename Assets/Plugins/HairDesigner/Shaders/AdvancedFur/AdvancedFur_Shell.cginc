


//---------------------------------------------------------
//GEOMETRY
//---------------------------------------------------------
[maxvertexcount(27)]
void GS_SHELL(triangle HS_Input IN[3], uint pid : SV_PrimitiveID, inout TriangleStream<GEOM_Input> tristream)
{
	//GEOM_Input o = (GEOM_Input)0;
	GEOM_Input o;
	UNITY_INITIALIZE_OUTPUT(GEOM_Input, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	//if (!_EnableShell)
	//	return;

	float shellCount = clamp(_ShellCount,1,20);
	float length = _Length *_ShellHeight;

	length *= _GlobalScale;

	if (length == 0)
		return;

	if (IN[0].brushDir.w + IN[1].brushDir.w + IN[2].brushDir.w < 0.06)
		return;



	for (float i = 1; i < shellCount; ++i)
	{
		float f = ((float)i) / (float)(shellCount);
		float l = f * length;
		
		for (int j = 0; j < 3; ++j)
		{
			//HS_Input v = IN[j];
			//UNITY_TRANSFER_INSTANCE_ID(v, o);
			//o.worldPos = IN[j].vertex;// -IN[j].normal*_ShellHeight * .5;
			//v.vertex = mul(unity_WorldToObject, v.vertex);

			fixed tangentSign = IN[j].tangent.w * unity_WorldTransformParams.w;
			fixed3 worldBinormal = cross(IN[j].normal, IN[j].tangent) * tangentSign;
			fixed3 worldNormal = IN[j].normal;
			fixed3 worldTangent = IN[j].tangent;
			

			float4 brushDir = float4( (IN[j].brushDir).xyz * _BrushStrength, IN[j].brushDir.w);
			float4 motionDir = (IN[j].motionDir) ;
			//motionDir.xyz *= motionDir.w * _Length * _MotionFactor;
			motionDir.xyz *= motionDir.w * _MotionFactor;
			
			float3 furDir = FurDir(brushDir, motionDir);
			//o.worldPos.xyz += l * IN[j].brushDir.w * normalize(IN[j].normal + IN[j].brushDir * f * _BrushStrength);

			IN[j].vertex.xyz += l * brushDir.w * normalize(IN[j].normal + furDir) / shellCount;

			fixed3 worldPos = IN[j].vertex.xyz;

			o.pos = UnityWorldToClipPos(float4(IN[j].vertex.xyz, 1.0));
			o.texcoord = IN[j].texcoord;
			o.texcoord.z = f;
			//o.position.z += .001;//extra depth

			



#ifdef UNITY_PASS_SHADOWCASTER

			o.pos = UnityApplyLinearShadowBias(o.pos);						
#else
			//o.worldNormal = IN[j].normal;
			//o.worldNormal =  UnityObjectToWorldNormal(IN[j].normal);
			//o.tangent = IN[j].tangent;
			o.tSpace0 = float4(IN[j].tangent.x, worldBinormal.x, IN[j].normal.x, IN[j].vertex.x);
			o.tSpace1 = float4(IN[j].tangent.y, worldBinormal.y, IN[j].normal.y, IN[j].vertex.y);
			o.tSpace2 = float4(IN[j].tangent.z, worldBinormal.z, IN[j].normal.z, IN[j].vertex.z);

#endif

			
#ifdef UNITY_PASS_FORWARD
	// SH / ambient and vertex lights
	#ifndef LIGHTMAP_ON
	#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
				o.sh = 0;
				// Approximated illumination from non-important point lights
	#ifdef VERTEXLIGHT_ON
				o.sh += Shade4PointLights(
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, worldPos, worldNormal);
	#endif
				o.sh.xyz = ShadeSHPerVertex(worldNormal, o.sh);
	#endif
	#endif // !LIGHTMAP_ON



			//UNITY_TRANSFER_SHADOW(o, IN[j].texcoord1.xy); // pass shadow coordinates to pixel shader
			UNITY_TRANSFER_SHADOW(o, IN[j].texcoord.xy); // pass shadow coordinates to pixel shader
			UNITY_TRANSFER_FOG(o, o.pos); // pass fog coordinates to pixel shader
#endif



#if defined(UNITY_PASS_DEFERRED)


			float3 viewDirForLight = UnityWorldSpaceViewDir(IN[j].vertex);
			//float3 viewDirForLight = UnityWorldSpaceViewDir(o.worldPos);
#ifndef DIRLIGHTMAP_OFF
			//o.viewDir = viewDirForLight;
			o.viewDir.x = dot(viewDirForLight, worldTangent);
			o.viewDir.y = dot(viewDirForLight, worldBinormal);
			o.viewDir.z = dot(viewDirForLight, worldNormal);
#endif
//#ifdef DYNAMICLIGHTMAP_ON
//			o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
//#else
			o.lmap.zw = 0;
//#endif
//#ifdef LIGHTMAP_ON
//			o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
//#ifdef DIRLIGHTMAP_OFF
//			o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
//			o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
//#endif
//#else
			o.lmap.xy = 0;
#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
			o.sh = 0;
			o.sh = ShadeSHPerVertex(worldNormal, o.sh);
#endif
//#endif	
#endif //UNITY_PASS_DEFERRED



			tristream.Append(o);

		}
		tristream.RestartStrip();
	}

	return;

}


void surf(GEOM_Input input, inout SurfaceOutputStandard o, fixed facing, float3 viewDir)
{	
	float4 maskCol = tex2D(_MaskTex, input.texcoord*_MaskTex_ST.xy);
	float mask = maskCol.a;
	float f = input.texcoord.z;
	float lod = _MaskLOD;

	fixed4 density = tex2D(_DensityTex, input.texcoord*_DensityTex_ST.xy)*mask;
	//density += tex2D(_DensityTex, input.texcoord*_DensityTex_ST.xy*2.0)*mask;
	//density += tex2D(_DensityTex, input.texcoord*_MaskTex_ST.xy*10)*mask;

	float clipFactor = sqrt(length(density.xyz)) - f * (1 + _ShellThickness * (1 - f)) + step(.0001, mask)*(lod)*(1 - f);

	



	//half4 albedo = tex2D(_FurTex, input.texcoord).rgba * lerp(_RootColor, _TipColor, pow(input.texcoord.y, _ColorThreshold)).rgba;
	//albedo.rgb *= tex2D(_MainTex, input.texcoord.zw).rgb;
	float colorStrength = clamp((f + (1 - mask))*_ShellColorThreshold, 0.0, 1.0);
	float4 rootTipColor = lerp(_ShellRootColor, _ShellTipColor, colorStrength);
	rootTipColor *= lerp(1, clamp(1 - ((1 - f) / (.9 - mask)), 0, 1), _ShellAO);

	clip(clipFactor-(1-rootTipColor.a));

	float4 albedo = tex2D(_MainTex, input.texcoord*_MainTex_ST.xy);
	
	 
	half4 normal = tex2D(_DensityNormalTex, input.texcoord*_DensityTex_ST.xy);
	o.Normal = lerp(o.Normal, UnpackScaleNormal(normal, _NormalStrength).rgb,f);

	float4 color = tex2D(_ColorTex, input.texcoord); 
	color.rgb = (albedo.rgb * color.rgb * color.a) + color.rgb * (1 - color.a);
	albedo.rgb = lerp(albedo.rgb, color.rgb, _ColorTexIntensity);


	albedo.rgba  *= rootTipColor.rgba;


	o.Albedo = albedo.rgb;
	o.Metallic = _Metallic;
	o.Smoothness = _Smoothness*f*f*.4;
	o.Alpha = albedo.a;
	o.Occlusion = 1-_StrandAO * f*f;

	o.Emission = _Emission * o.Albedo * lerp(_RootEmissionColor, _TipEmissionColor, clamp(pow(f, _EmissionThreshold), 0, 1)).rgb;
	//o.Albedo.rgb = float3(1, 0, 0);
}




