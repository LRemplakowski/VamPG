using UnityEngine;
using System.Collections;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerShaderAtlas : HairDesignerShader
        {
            public AtlasParameters m_atlasParameters;
            //public Material m_transparentMaterial;

            public Texture2D m_diffuseTex;
            public Texture2D m_normalTex;
            public Texture2D m_AOTex;
            public Texture2D m_SpecularTex;
            public float m_AOFactor;
            public float m_smoothness = 0f;
            public Color m_rootColor = Color.grey;
            public Color m_tipColor = Color.gray;
            public float m_colorBias = .5f;
            public float m_stripes = 0f;
            public float m_emission = 0f;
            public float m_alphaCutoff = .1f;
            public float m_alphaCutoffTip = -1f;
            public float m_alpha = 1f;
            public float m_rndSeed = 0f;
            public float m_NormalPower = .2f;
            public float m_NormalMode = 0f;
            public float m_alphaAtten = 0f;
            //public Color m_rimColor = Color.grey;
            //public float m_rimPower = 0f;

            public float m_Shift1 = .2f;
            public float m_SpecularExp1 = 1f;
            public Color m_SpecularColor1 = Color.black;

            public float m_Shift2 = .2f;
            public float m_SpecularExp2 = 1f;
            public Color m_SpecularColor2 = Color.black;

            public float m_TransparentShadowReceive = .8f;
            
            public float m_gravity = 0f;
            public float m_cut = 0f;
            public float m_offset = 0f;
            public float m_length = 1f;
            public float m_wind = 1f;
            public float m_windTurbulenceFrequency = 1f;

            public float m_translucencyStrength = 1f;
            public float m_translucencyNormal = .2f;
            public float m_translucencyPower = 2f;
            public float m_translucencyDirect = 1f;
            public float m_translucencyIndirect = .2f;


            


            public override void UpdatePropertyBlock(ref MaterialPropertyBlock pb, HairDesignerBase.eLayerType lt)
            {
                if(!Application.isPlaying)
                    pb.Clear();

                if (m_alphaCutoffTip < 0)
                    m_alphaCutoffTip = m_alphaCutoff;//compatibility with older version

                if (m_diffuseTex != null)
                    pb.SetTexture("_MainTex", m_diffuseTex);
                if (m_normalTex != null)
                    pb.SetTexture("_NormalTex", m_normalTex);
                if (m_SpecularTex != null)
                    pb.SetTexture("_SpecularTex", m_SpecularTex);
                if (m_AOTex != null)
                    pb.SetTexture("_AOTex", m_AOTex);
                pb.SetFloat("_AO", m_AOFactor);
                pb.SetFloat("_Smoothness", m_smoothness);
                if(m_atlasParameters!=null)
                    pb.SetFloat("_AtlasSize", m_atlasParameters.sizeX);

                pb.SetColor("_RootColor", m_rootColor);
                pb.SetColor("_TipColor", m_tipColor);
                pb.SetFloat("_ColorBias", m_colorBias);

                pb.SetFloat("_Stripes", m_stripes);
                pb.SetFloat("_Emission", m_emission);
                pb.SetFloat("_AlphaCutoffRoot", m_alphaCutoff);
                pb.SetFloat("_AlphaCutoffTip", m_alphaCutoffTip);
                pb.SetFloat("_Alpha", m_alpha);
                pb.SetFloat("_RndSeed", m_rndSeed);

                pb.SetFloat("_NormalMode", m_NormalMode);
                pb.SetFloat("_NormalPower", m_NormalPower);
                pb.SetFloat("_AlphaAtten", m_alphaAtten);
                
                //pb.SetColor("_RimColor", m_rimColor);
                //pb.SetFloat("_RimPower", m_rimPower);

                pb.SetColor("_SpecularColor1", m_SpecularColor1);
                pb.SetFloat("_SpecularExp1", m_SpecularExp1);
                pb.SetFloat("_Shift1", m_Shift1);

                pb.SetColor("_SpecularColor2", m_SpecularColor2);
                pb.SetFloat("_SpecularExp2", m_SpecularExp2);
                pb.SetFloat("_Shift2", m_Shift2);

                pb.SetFloat("_TwoSided", lt== HairDesignerBase.eLayerType.LONG_HAIR_POLY ? 1f : 0f);
                pb.SetFloat("_TransparentShadowReceive", m_TransparentShadowReceive);

                
                pb.SetFloat("_Cut", m_cut);
                pb.SetFloat("_Offset", m_offset);
                pb.SetFloat("_Length", m_length);
                pb.SetFloat("KHD_gravityFactor", m_gravity);
                pb.SetFloat("KHD_windFactor", m_wind);
                pb.SetFloat("KHD_windZoneTurbulenceFrequency", m_windTurbulenceFrequency );

                pb.SetFloat("_TranslucencyStrength", m_translucencyStrength);
                pb.SetFloat("_TransluscencyNormal", m_translucencyNormal);
                pb.SetFloat("_TransluscencyPower", m_translucencyPower);
                pb.SetFloat("_TransluscencyDirect", m_translucencyDirect);
                pb.SetFloat("_TransluscencyIndirect", m_translucencyIndirect);
            }


            public override void UpdateMaterialProperty(ref Material mat, HairDesignerBase.eLayerType lt)
            {
#if UNITY_2017_1_OR_NEWER  
                mat.doubleSidedGI = true;
#endif

#if UNITY_5_6_OR_NEWER
                mat.enableInstancing = true;
#endif
            }

        }
    }
}