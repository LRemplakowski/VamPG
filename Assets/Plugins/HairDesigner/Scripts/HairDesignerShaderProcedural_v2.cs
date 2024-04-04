using UnityEngine;
using System.Collections;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerShaderProcedural_v2 : HairDesignerShader
        {
            public Color m_startColor = Color.black;
            public Color m_endColor = Color.white;
            public float m_colorBias = .5f;
            public float m_colorPower = 1f;
            public float m_stripes = 0f;
            public float m_smoothness = .1f;
            public float m_ao = .5f;
            public float m_hairDensity = 10;
            public float m_hairPerPass = 1;            
            public float m_hairLength = 1f;
            public float m_RndLength = 1;
            public float m_hairWidth = .8f;
            public float m_tipWidth = 1f;
            public int m_UVX = 1;
            public float m_wave1 = 0;
            public float m_wavePower1 = .1f;
            public float m_waveRnd1 = .5f;
            public float m_wave2 = 0;
            public float m_wavePower2 = .1f;
            public float m_waveRnd2 = .5f;
            public float m_waveDir = .5f;
            public float m_chaos = 0f;
            public float m_cut = 0f;
            public float m_RootThickness = .2f;
            public float m_TipThickness = .2f;
            public Color m_rimColor = Color.grey;
            public float m_rimPower = 6f;
            public float m_emission = 0f;
            public float m_alphaCutoff = .5f;
            public float m_alpha = .5f;
            public float m_normalPower = .5f;
            public float m_rndSeed = 0f;
            public float m_windFactor = 0f;

            public override void UpdatePropertyBlock(ref MaterialPropertyBlock pb, HairDesignerBase.eLayerType lt)
            {
                pb.SetColor("_StartColor", m_startColor);
                pb.SetColor("_EndColor", m_endColor);
                pb.SetFloat("_ColorBias", m_colorBias);
                pb.SetFloat("_ColorPower", m_colorPower);
                pb.SetFloat("_Stripes", m_stripes);
                pb.SetColor("_RimColor", m_rimColor);
                pb.SetFloat("_Smoothness", m_smoothness);
                pb.SetFloat("_AO", m_ao);
                pb.SetFloat("_HairDensity", m_hairDensity);
                pb.SetFloat("_HairPerPass", m_hairPerPass);
                pb.SetFloat("_HairLength", m_hairLength);
                pb.SetFloat("_HairWidth", m_hairWidth);
                pb.SetFloat("_TipWidth", m_tipWidth);
                pb.SetFloat("_RndLength", m_RndLength);
                pb.SetFloat("_Wave1", m_wave1);
                pb.SetFloat("_WavePower1", m_wavePower1);
                pb.SetFloat("_WaveRnd1", m_waveRnd1);
                pb.SetFloat("_Wave2", m_wave2);
                pb.SetFloat("_WavePower2", m_wavePower2);
                pb.SetFloat("_WaveRnd2", m_waveRnd2);
                pb.SetFloat("_WaveDir", m_waveDir);
                pb.SetFloat("_Chaos", m_chaos);
                pb.SetFloat("_Cut", m_cut);
                pb.SetFloat("_RootThickness", m_RootThickness);
                pb.SetFloat("_TipThickness", m_TipThickness);
                pb.SetFloat("_RimPower", m_rimPower);
                pb.SetFloat("_UVX", m_UVX);
                pb.SetFloat("_Emission", m_emission);
                pb.SetFloat("_AlphaCutoff", m_alphaCutoff);
                pb.SetFloat("_Alpha", m_alpha);
                pb.SetFloat("_NormalPower", m_normalPower);
                pb.SetFloat("_RndSeed", m_rndSeed);
                pb.SetFloat( "KHD_windFactor", m_windFactor );
                


                switch (lt)
                {
                    
                    case HairDesignerBase.eLayerType.SHORT_HAIR_POLY:
                        pb.SetFloat("_TwoSidedLight", 0);
                        break;                        
                        
                    case HairDesignerBase.eLayerType.LONG_HAIR_POLY:
                        pb.SetFloat("_TwoSidedLight", 1);
                        break;
                }
            }

            /*
            public override void InitPropertyBlock(ref MaterialPropertyBlock pb, HairDesignerBase.eLayerType lt)
            {
                switch(lt)
                {
                    case HairDesignerBase.eLayerType.SHORT_HAIR_POLY:
                        pb.SetFloat("_NormalMode", 0);
                        break;
                    case HairDesignerBase.eLayerType.LONG_HAIR_POLY:
                        pb.SetFloat("_NormalMode", 1);
                        break;
                }
            }*/
        }
    }
}
