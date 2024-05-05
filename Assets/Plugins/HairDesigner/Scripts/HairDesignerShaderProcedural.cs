using UnityEngine;
using System.Collections;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerShaderProcedural : HairDesignerShader
        {
            public Color m_startColor = Color.black;
            public Color m_endColor = Color.white;
            public float m_colorBalance = .5f;
            public float m_stripes = 0f;
            public float m_smoothness = .1f;
            public float m_ao = .5f;
            public float m_hairDensity = 20;
            public float m_hairSize = 1f;
            public float m_hairWidth = 1.2f;
            public int m_UVX = 1;
            public float m_wave = 0;
            public float m_wavePower = .1f;
            public float m_chaos = 0f;
            public float m_cut = 0f;
            public Color m_rimColor = Color.grey;
            public float m_rimPower = 6f;
            public float m_emission = 0f;

            public override void UpdatePropertyBlock(ref MaterialPropertyBlock pb, HairDesignerBase.eLayerType lt)
            {
                pb.SetColor("_StartColor", m_startColor);
                pb.SetColor("_EndColor", m_endColor);
                pb.SetFloat("_ColorBalance", m_colorBalance);
                pb.SetFloat("_Stripes", m_stripes);
                pb.SetColor("_RimColor", m_rimColor);
                pb.SetFloat("_Smoothness", m_smoothness);
                pb.SetFloat("_AO", m_ao);
                pb.SetFloat("_HairDensity", m_hairDensity);
                pb.SetFloat("_HairSize", m_hairSize);
                pb.SetFloat("_HairWidth", m_hairWidth);
                pb.SetFloat("_Wave", m_wave);
                pb.SetFloat("_WavePower", m_wavePower);
                pb.SetFloat("_Chaos", m_chaos);
                pb.SetFloat("_Cut", m_cut);
                pb.SetFloat("_RimPower", m_rimPower);
                pb.SetFloat("_UVX", m_UVX);
                pb.SetFloat("_Emission", m_emission);

                switch (lt)
                {
                    case HairDesignerBase.eLayerType.SHORT_HAIR_POLY:
                        pb.SetFloat("_NormalMode", 0);
                        break;
                        /*
                    case HairDesignerBase.eLayerType.LONG_HAIR_POLY:
                        pb.SetFloat("_NormalMode", 1);
                        break;*/
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
