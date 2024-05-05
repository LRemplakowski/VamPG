#define STRAND
//#define MOTION_DEBUG_MODE



GEOM_Input ApplyGeometry(float id, float3 wpos, half3 wnrm, half4 wtan, half3 surfaceNormal, float2 uv, float2 colorUV
//#ifdef MOTION_DEBUG_MODE
	, float3 motionDir
//#endif
)
{

	//wnrm = -wnrm;

	GEOM_Input o;
	UNITY_INITIALIZE_OUTPUT(GEOM_Input, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	//HS_Input v = IN[j];
	//UNITY_TRANSFER_INSTANCE_ID(v, o);
	

#if defined(UNITY_PASS_SHADOWCASTER)
	// Default shadow caster pass: Apply the shadow bias.
	float scos = dot(wnrm, normalize(UnityWorldSpaceLightDir(wpos)));
	wpos -= wnrm * unity_LightShadowBias.z * sqrt(1 - scos * scos);
	o.pos = UnityApplyLinearShadowBias(UnityWorldToClipPos(float4(wpos, 1)));
	//o.pos = UnityWorldToClipPos(float4(wpos, 1));
	o.texcoord.xy = uv;
	o.surfaceNormal.w = id;
#else

	// GBuffer construction pass
	//half3 bi = cross(wnrm, wtan) * wtan.w * unity_WorldTransformParams.w;
	o.pos = UnityWorldToClipPos(float4(wpos, 1));
	//o.worldPos = wpos;
	//o.worldNormal = wnrm;
	//o.tangent = wtan;
#ifdef MOTION_DEBUG_MODE
	o.texcoord.xyz = motionDir;
#else
	o.texcoord.xy = uv.xy;
	o.texcoord.zw = colorUV;
#endif

	fixed tangentSign = wtan.w * unity_WorldTransformParams.w;
	fixed3 worldBinormal = cross(wnrm, wtan) * tangentSign;

	o.tSpace0 = float4(wtan.x, worldBinormal.x, wnrm.x, wpos.x);
	o.tSpace1 = float4(wtan.y, worldBinormal.y, wnrm.y, wpos.y);
	o.tSpace2 = float4(wtan.z, worldBinormal.z, wnrm.z, wpos.z);

	o.surfaceNormal.xyz = surfaceNormal.xyz;
	o.surfaceNormal.w = id;
#endif //UNITY_PASS_SHADOWCASTER


#if defined(UNITY_PASS_FORWARD)
 		
	// SH / ambient and vertex lights
	#ifndef LIGHTMAP_ON
	#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
		o.sh = 0;
		// Approximated illumination from non-important point lights
	#ifdef VERTEXLIGHT_ON
		o.sh += Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, wpos, wnrm);
	#endif
		o.sh = ShadeSHPerVertex(wnrm, o.sh);
	#endif
	#endif // !LIGHTMAP_ON

			   	
	UNITY_TRANSFER_SHADOW(o, uv); // pass shadow coordinates to pixel shader
	UNITY_TRANSFER_FOG(o, o.pos); // pass fog coordinates to pixel shader

#endif //UNITY_PASS_FORWARD

#if defined(UNITY_PASS_DEFERRED)


	float3 viewDirForLight = UnityWorldSpaceViewDir(wpos);
	#ifndef DIRLIGHTMAP_OFF
		o.viewDir = viewDirForLight;
	#endif
	//#ifdef DYNAMICLIGHTMAP_ON
	//	o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
	//#else
		o.lmap.zw = 0;
	//#endif
	//#ifdef LIGHTMAP_ON
	//	o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	//	#ifdef DIRLIGHTMAP_OFF
	//		o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
	//		o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
	//	#endif
	//#else
		o.lmap.xy = 0;
		#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
			o.sh = 0;
			o.sh = ShadeSHPerVertex(wnrm, o.sh);
		#endif
	//#endif	
#endif //UNITY_PASS_DEFERRED

	return (o);
}



//---------------------------------------------------------
//GEOMETRY
//---------------------------------------------------------
[maxvertexcount(24)]
void GS_STRAND(triangle HS_Input IN[3], uint pid : SV_PrimitiveID, inout TriangleStream<GEOM_Input> tristream)
{
	GEOM_Input o = (GEOM_Input)0;


	//if (!_EnableStrands)
	//	return;
	
	
	_localRnd = 0;

	if (_Length <= 0)
		return;

	float3 edgeA = IN[1].vertex - IN[0].vertex;
	float3 edgeB = IN[2].vertex - IN[0].vertex;
	float3 normalFace = normalize(cross(edgeA, edgeB));
	float3 center = (IN[0].vertex + IN[1].vertex + IN[2].vertex) / 3.0;
	float4 texcoord = (IN[0].texcoord + IN[1].texcoord + IN[2].texcoord) / 3.0;
	float3 normal = (IN[0].normal + IN[1].normal + IN[2].normal) / 3.0;
	//float size = _Size * min(edgeA, edgeB);
	float subdivision = (int)_Subdivision;
	  	
	


	float edgeMin = min(edgeA, edgeB);
	int loops = 1;

	//for (int i = 0; i < loops; i++)
	int i = 0;
	{
		//for each strand

		//float3 normal = normalize(IN[0].normal + IN[1].normal + IN[2].normal);

		float3 wind = (sin(_Time.z* Random(_RandomSeed + i)) * float3(0, 1, 0) + float3(1, 0, 0)) * _Length * (abs(sin(_Time.y* Random(_RandomSeed + i)))* Random(_RandomSeed + i));
		wind *= _Wind;


		float4 brushDir = (IN[0].brushDir + IN[1].brushDir + IN[2].brushDir) / 3.0;
		float4 motionDir = (IN[0].motionDir + IN[1].motionDir + IN[2].motionDir) / 3.0;
		float rnd = (IN[0].rnd + IN[1].rnd + IN[2].rnd) / 3.0;
		
		


		//motionDir.xyz *= motionDir.w * _Length * _MotionFactor;
		motionDir.xyz *= motionDir.w * _MotionFactor;
		
		/*
		if (dot(normalize(motionDir), normal) < 0)
		{
			//motionDir.xyz -= normal * dot(motionDir, normal);
		}
		*/
		
		float3 furDir = FurDir(brushDir, motionDir);
		//furDir = motionDir;


		//brushDir = IN[0].brushDir;
		//if(length(brushDir)>0)
		//	brushDir = float4(0, -10, 0,1);
		//brushDir = IN[0].brushDir;


		

		//float3 dir = normalize(normal*.001 + float3(0, -_Gravity , 0) + brushDir.xyz );// +wind * _Wind;
		float3 dir = normalize(float3(0, -_Gravity, 0)*.1 + furDir.xyz);// +wind * _Wind;
		//float3 dir = normalize( brushDir.xyz);// +wind * _Wind;
		//float3 dir = normalize(normal*.001 + brushDir.xyz);
        float dotNDir = dot(normalize(normalFace), normalize(dir));

		/*
        if (dotNDir<0)
        {
            dir -= dotNDir * normalize(normalFace);
        }
		*/
		//chaos
		//normalFace = lerp(normalFace, normalize( normalFace + cross(normalFace,edgeB)), 1.0);




		//DEBUG
		//dir = brushDir;
		//motionDir = float4(0, 1, 0, 1);
		//dir = furDir;
		
		//dir *= clamp(dot(normalFace, dir), 0.5, 1.0);
		/*
		float dorNDir = dot(normalFace, dir);
		if (dorNDir < 0)
		{
			dir = normalize(dir - dorNDir * normalFace) * length(dir);
		}
		*/
		


		//float3 side = cross(normalize(dir), normalFace);


            float w0 = Random((_RandomSeed + IN[0].texcoord.z) * i + pid);
		float w1 = Random(w0 + +IN[1].texcoord.z + _RandomSeed * i);
		float w2 = Random(w1 + +IN[2].texcoord.z + _RandomSeed + _RandomSeed * i);
		float wT = w0 + w1 + w2;


		w0 = w0 / wT;
		w1 = w1 / wT;
		w2 = w2 / wT;


		float3 pos = IN[0].vertex *w0 + IN[1].vertex *w1 + IN[2].vertex *w2;

		float4 mask = tex2Dlod(_MaskTex, texcoord);

		//brushDir.w = .1;
		float l = _Length * (brushDir.w);
		l *= lerp(1.0, 0.5, frac(abs(Random(texcoord.z)))*_StrandRandomLength);


		//l = mask.a >0 ? l : 0;
		l *= mask.a;

		if (l < _Length * _StrandHeightMin)
			return;


		l *= _GlobalScale;
		
        if (dotNDir < 0)
            l *= 1.0+dotNDir*.1;

		float3 globalDir = dir;

		for (float j = 0; j < subdivision; j++)
		{
			//if(dot(globalDir, normalFace)<0)
			//	dir -= dot(globalDir, normalFace)*normalFace;

			//o.ambient = float3(1, 1, 1);		

			float f0 = ((j+0) / (subdivision));
			float f1 = ((j+1) / (subdivision));

			if(f0 == 0)
				f0 = 0.000001;//Bug fix on 1st subdiv

			float omf0 = 1 - f0;
			float omf1 = 1 - f1;
			float p2f0 = f0 * f0;
			float p2f1 = f1 * f1;

			float3 dir0 = dir;
			float3 dir1 = dir;
			
			
			//avoid mesh collision
						
            
            dir0 = normalize(lerp(lerp(normalFace + dir, normalFace + dir*.5, abs(dotNDir)), dir, f0)) * length(dir);
            dir1 = normalize(lerp(lerp(normalFace + dir, normalFace + dir*.5, abs(dotNDir)), dir, f1)) * length(dir);
			
			
			
            if (dotNDir<0)
            {
                dir0 = normalize(dir0 - dotNDir * normalize(normalFace));
                dir1 = normalize(dir1 - dotNDir * normalize(normalFace));
            }
			
			
			//dir = lerp(normalize(normalFace + globalDir), globalDir, 1-abs(dot(globalDir, normalFace))*f0);
			//float3 dir0 = lerp(normalize(normalFace + globalDir), globalDir, p2f0);
			//float3 dir1 = lerp(normalize(normalFace + globalDir), globalDir, p2f1);

			//wind = lerp(float3(0, 0, 0), wind, f1);
			//dir = dir + wind;

			//float3 side = cross(normalize(lerp(edgeA, edgeB, f0) + dir), normalFace);
                float3 side = cross(normalize(dir * .1 + edgeA * 10.0 + edgeB * 10.0), normalFace);
			
			side = lerp(side, cross(normalize(dir), normalFace), _StrandOrientation );//for feather or dragon scale

			//float4 c0 = lerp(fixed4(0., 0., 0., 1.), fixed4(1., 1., 1., 1.), f0);
			//float4 c1 = lerp(fixed4(0., 0., 0., 1.), fixed4(1., 1., 1., 1.), f1);

			float size = _Size * (l / _Length);

			float s0 = lerp(size, size * omf0, _Pinch) * lerp(.01, min(length(edgeA), length(edgeB)), _SizeFaceFactor);
			float s1 = lerp(size, size * omf1, _Pinch)* lerp(.01, min(length(edgeA), length(edgeB)), _SizeFaceFactor);

			float3 wpos;
			float3 wnrm;
			half4 wtan;
			float2 uv;

			/*
			float3 extr0 = lerp(normalize(normal+ _StrandFlexibility* dir0), dir0,  p2f0) *l *f0;
			float3 extr1 = lerp(normalize(normal + _StrandFlexibility * dir1), dir1,  p2f1) *l *f1;
			*/
			
            float3 extr0 = lerp(normalize(normal * (1.0 - _StrandFlexibility) + p2f0 * dir0), dir0, f0 * (_StrandFlexibility)) * l * f0;
            float3 extr1 = lerp(normalize(normal * (1.0 - _StrandFlexibility) + p2f1 * dir1), dir1, f1 * (_StrandFlexibility)) * l * f1;

           
			
			wpos = pos + side * s0 + extr0;
			wnrm = lerp(normalize(extr0), normalize(cross(side, extr0)), f0);
			wtan = IN[0].tangent;
			uv.xy = float2(.98, f0);
			o = ApplyGeometry(rnd, wpos, wnrm, wtan, normal, uv, texcoord.xy, motionDir);
			tristream.Append(o);

			wpos = (pos + side * s1 + extr1);
			uv.xy = float2(.98, f1);
			wnrm = lerp(normalize(extr1), normalize(cross(side, extr1)), f1);
			wtan = IN[0].tangent;
			o = ApplyGeometry(rnd, wpos, wnrm, wtan, normal, uv, texcoord.xy, motionDir);
			tristream.Append(o);

			wpos = (pos - side * s0 + extr0);
			uv.xy = float2(0.02, f0);
			wnrm = lerp(normalize(extr0), normalize(cross(side, extr0)), f0);
			wtan = IN[0].tangent;
			o = ApplyGeometry(rnd, wpos, wnrm, wtan, normal, uv, texcoord.xy, motionDir);
			tristream.Append(o);

			tristream.RestartStrip();


			wpos = (pos - side * s0 + extr0);
			uv.xy = float2(0.02, f0);
			wnrm = lerp(normalize(extr0), normalize(cross(side, extr0)), f0);
			wtan = IN[0].tangent;
			o = ApplyGeometry(rnd, wpos, wnrm, wtan, normal, uv, texcoord.xy, motionDir);
			tristream.Append(o);

			wpos = (pos + side * s1 + extr1);
			uv.xy = float2(.98, f1);
			wnrm = lerp(normalize(extr1), normalize(cross(side, extr1)), f1);
			wtan = IN[0].tangent;
			o = ApplyGeometry(rnd, wpos, wnrm, wtan, normal, uv, texcoord.xy, motionDir);
			tristream.Append(o);

			wpos = (pos - side * s1 + extr1);
			uv.xy = float2(0.02, f1);
			wnrm = lerp(normalize(extr1), normalize(cross(side, extr0)), f1);
			wtan = IN[0].tangent;
			o = ApplyGeometry(rnd, wpos, wnrm, wtan,normal, uv, texcoord.xy, motionDir);

			tristream.Append(o);


			tristream.RestartStrip();

		}
	}


}





void surf(inout GEOM_Input input, inout SurfaceOutputStandard o, fixed facing, float3 viewDir)
{



#ifndef MOTION_DEBUG_MODE
	//Fix some clamp issues...
	clip(input.texcoord.y - .02);
	clip((1 - input.texcoord.y) - .02);
#endif

	float2 uvTex = input.texcoord;
	
    uvTex = AtlasUV(input.surfaceNormal.w, _AtlasSize, uvTex);
	

#if !defined(UNITY_PASS_SHADOWCASTER)	
	
	float3 finalNormal = float3(input.tSpace0.z, input.tSpace1.z, input.tSpace2.z);	
	finalNormal *= facing > 0 ? 1 : -1;

	input.tSpace0.z = lerp(input.surfaceNormal.x, finalNormal.x , _NormalSwitch*.5);
	input.tSpace1.z = lerp(input.surfaceNormal.y, finalNormal.y , _NormalSwitch*.5);
	input.tSpace2.z = lerp(input.surfaceNormal.z, finalNormal.z , _NormalSwitch*.5);
	
#endif

	float2 uv = uvTex.xy;
	uv.x += sin(3.1416*2.0*uv.y*_StrandWaveFrequency)*_StrandWaveAmplitude*(1.0-uv.y*.2)*.1;
	half4 albedo = tex2D(_FurTex, uv.xy ).rgba;


	//half4 albedo = tex2D(_FurTex, input.texcoord).rgba;
	albedo.rgb *= tex2D(_MainTex, input.texcoord.zw*_MainTex_ST.xy).rgb;
	
	float4 color = tex2D(_ColorTex, input.texcoord.zw);
	color.rgb = (albedo.rgb * color.rgb * color.a) + color.rgb * (1 - color.a);
	albedo.rgb = lerp(albedo.rgb, color.rgb, _ColorTexIntensity);


	albedo.rgba *= lerp(_StrandRootColor, _StrandTipColor, clamp(pow(input.texcoord.y, _StrandColorThreshold), 0, 1));

	half4 normal = tex2D(_NormalMap, uv.xy);
	normal.xyz = UnpackScaleNormal(normal, _NormalStrength);
	//if (facing < 0)normal = -normal;
	
	
	
#ifndef MOTION_DEBUG_MODE

	#ifndef TRANSPARENCY
		clip(albedo.a - _Cutout);
	#else
		clip(albedo.a - _CutoutAlpha);
	#endif

#endif

	o.Albedo = albedo.rgb;

	//if (facing > 0)o.Albedo = float3(1,0,0);

	o.Normal = normal;	
	o.Metallic = _Metallic;
	o.Smoothness = _Smoothness * input.texcoord.y;
	o.Alpha = 1;
	o.Emission = _Emission * o.Albedo * lerp(_RootEmissionColor, _TipEmissionColor, clamp(pow(input.texcoord.y, _EmissionThreshold), 0, 1)).rgb ;
	//o.Emission = float3(1,0,0);
	o.Occlusion = lerp(1,input.texcoord.y*2-1, _StrandAO);

#if !defined(UNITY_PASS_SHADOWCASTER)	
	fixed3 view = normalize(viewDir);
	fixed3 nml = input.surfaceNormal;
	fixed VdN = dot(view, nml);
	fixed rim = 1.0 - saturate(VdN);
	o.Emission += _RimColor.rgb * _RimColor.a * pow(rim, _RimPower) * input.texcoord.y* input.texcoord.y;
#endif

#ifdef MOTION_DEBUG_MODE
	o.Albedo = input.texcoord.rgb;
	o.Emission = input.texcoord.rgb;
#endif
}




