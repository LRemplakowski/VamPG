using UnityEngine;
using System.Collections;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerShaderHDRP_Atlas : HairDesignerShader
        {
            public AtlasParameters m_atlasParameters;
            //public Material m_transparentMaterial;

            public Texture2D m_diffuseTex;            
            public Texture2D m_normalTex;
            public float m_normalStrength = .2f;
            public Texture2D m_AOTex;            
            public float m_AOStrength;
            public Texture2D m_SpecularTex;

            public Color m_rootColor = Color.grey;
            public float m_rootAlphaThreshold = .5f;
            public Color m_tipColor = Color.white;
            public float m_tipAlphaThreshold = .5f;
            public float m_colorThreshold = .5f;
            public float m_alphaClip = .1f;
            public float m_alpha = 1f;
            public float m_alphaStrength = 3f;
            public float m_alphaStrengthTip = 3f;            
            public float m_alphaMax = 1f;

            public float m_atlasOffset = 1f;            
            public float m_depthOffset = 0f;

            public float m_shift1 = .2f;
            public float m_smoothness1 = .5f;
            public Color m_specularColor1 = Color.black;

            public float m_shift2 = .2f;
            public float m_smoothness2 = .5f;
            public Color m_specularColor2 = Color.black;

            public Color m_transmittance = Color.black;
            public float m_shadowThreshold = .8f;

            public Vector3 m_strandDirection = Vector3.up;

            public float m_gravity = 0f;
            //public float m_cut = 0f;
            public float m_length = 1f;
            public float m_wind = 1f;
            public float m_windTurbulenceFrequency = 1f;

            public float m_emission = 0f;
            public Vector3 m_emissionPower = new Vector3(.1f,.1f,.5f);
            


            public override void UpdatePropertyBlock(ref MaterialPropertyBlock pb, HairDesignerBase.eLayerType lt)
            {
                if(!Application.isPlaying)
                    pb.Clear();
                                
                
                if (m_diffuseTex != null)
                    pb.SetTexture("_DiffuseTex", m_diffuseTex);
                if (m_normalTex != null)
                    pb.SetTexture("_NormalTex", m_normalTex);
                if (m_SpecularTex != null)
                    pb.SetTexture("_SpecularTex", m_SpecularTex);

                if (m_AOTex != null)
                    pb.SetTexture("_AOTex", m_AOTex);
                pb.SetFloat("_AOStrength", m_AOStrength);
                if(m_atlasParameters!=null)
                    pb.SetFloat("_AtlasSize", m_atlasParameters.sizeX);

                pb.SetColor("_RootColor", m_rootColor);
                pb.SetColor("_TipColor", m_tipColor);
                pb.SetFloat("_ColorThreshold", m_colorThreshold);
                pb.SetFloat("_RootAlphaThreshold", m_rootAlphaThreshold);
                pb.SetFloat("_TipAlphaThreshold", m_tipAlphaThreshold);

                pb.SetFloat("_AlphaClip", m_alphaClip);
                pb.SetFloat("_Alpha", m_alpha);
                pb.SetFloat("_AlphaStrength", m_alphaStrength);
                pb.SetFloat("_AlphaStrengthTip", m_alphaStrengthTip);
                
                pb.SetFloat("_AlphaMax", m_alphaMax);

                pb.SetFloat("_AtlasOffset", m_atlasOffset);

                pb.SetFloat("_DepthOffset", m_depthOffset);


                pb.SetFloat("_NormalStrength", m_normalStrength);
                pb.SetFloat("_ShadowThreshold", m_shadowThreshold);
                pb.SetColor("_Transmittance", m_transmittance);
                

                pb.SetColor("_SpecColor1", m_specularColor1);
                pb.SetFloat("_Smoothness1", m_smoothness1);
                pb.SetFloat("_SpecShift1", m_shift1);

                pb.SetColor("_SpecColor2", m_specularColor2);
                pb.SetFloat("_Smoothness2", m_smoothness2);
                pb.SetFloat("_SpecShift2", m_shift2);
                
                
                //pb.SetFloat("_Cut", m_cut);
                pb.SetFloat("_Length", m_length);

                pb.SetFloat("KHD_gravityFactor", m_gravity);
                pb.SetFloat("KHD_windFactor", m_wind);
                pb.SetFloat("KHD_windZoneTurbulenceFrequency", m_windTurbulenceFrequency );

                pb.SetVector("_StrandDirection", m_strandDirection);


                pb.SetFloat("_Emission", m_emission);
                pb.SetVector("_EmissionPower", m_emissionPower);
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