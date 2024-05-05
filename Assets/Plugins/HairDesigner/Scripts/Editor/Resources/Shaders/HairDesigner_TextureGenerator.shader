// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/HairDesigner_TextureGenerator"
	{
		Properties
		{
			/*
			[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
			_Color ("Tint", Color) = (1,1,1,1)
			[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
			*/

			_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)



		}

			SubShader
		{
			Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha
			//lend SrcAlpha OneMinusSrcAlpha
			Pass
		{

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
			//#include "UnityCG.cginc"
#define ALPHA_ON 1
#include "HairDesigner_TextureGenerator.cginc"

		struct appdata_t
		{
			float4 vertex   : POSITION;
			float4 color    : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex   : SV_POSITION;
			fixed4 color : COLOR;
			float2 texcoord  : TEXCOORD0;
		};


		v2f vert(appdata_t IN)
		{
			v2f OUT;
			UNITY_INITIALIZE_OUTPUT(v2f, OUT);
#if UNITY_VERSION <= 559
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
#else
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
#endif
			
			OUT.texcoord = IN.texcoord;			
			return OUT;
		}

		sampler2D _AlphaTex;
		float _AlphaSplitEnabled;
			


		fixed4 frag(v2f IN) : SV_Target
		{	
			fixed4 c = Diffuse(IN.texcoord.xy);
			return c;
		}
			ENDCG
		}
		}
	}
