


// Default shadow caster pass
half4 PS_SHELL_SHADOWS(GEOM_Input input) : SV_Target{



	float4 maskCol = tex2D(_MaskTex, input.texcoord);
	float mask = maskCol.a;
	float f = input.texcoord.z;
	float lod = _MaskLOD;

	fixed4 density = tex2D(_DensityTex, input.texcoord*_DensityTex_ST.xy)*mask;

	float clipFactor = sqrt(length(density.xyz)) - f * (1 + _ShellThickness * (1 - f)) + step(.0001, mask)*(lod)*(1 - f);
	//clipFactor = clipFactor + f * _Test;
	clip(clipFactor);


	return 1;
}






half4 PS_STRAND_SHADOWS(GEOM_Input input) : SV_Target{


	//clip(tex2D(_FurTex, input.texcoord).a*_ShadowsStrength - _Cutout);

#ifndef MOTION_DEBUG_MODE
	//Fix some clamp issues...
    clip(input.texcoord.y - .02);
    clip((1 - input.texcoord.y) - .02);
#endif
	
	float2 uvTex = input.texcoord;    
    uvTex = AtlasUV(input.surfaceNormal.w, _AtlasSize, uvTex);
	
	

	float2 uv = uvTex.xy;
	uv.x += sin(3.1416*2.0*uv.y*_StrandWaveFrequency)*_StrandWaveAmplitude*(1.0 - uv.y*.2)*.1;

	float alpha = tex2D(_FurTex, uv).a;
	alpha *= lerp(_StrandRootColor.a, _StrandTipColor.a, clamp(pow(input.texcoord.y, _StrandColorThreshold), 0, 1));


	clip(alpha  - _Cutout);
	return 1;
}



