using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [CustomEditor(typeof(HairDesignerShaderProcedural_v2))]
        public class HairDesignerShaderProcedural_v2Editor : HairDesignerShaderEditor
        {

            public override void OnInspectorGUI()
            {

                base.OnInspectorGUI();
                if (_destroyed) return;

                //GUILayout.Label("HairDesignerShaderProceduralEditor");
                HairDesignerShaderProcedural_v2 s = target as HairDesignerShaderProcedural_v2;

                ShaderGUIBegin<HairDesignerShaderProcedural_v2>(s);

                s.m_startColor = EditorGUILayout.ColorField("Start color", s.m_startColor);
                s.m_endColor = EditorGUILayout.ColorField("End color", s.m_endColor);
                s.m_colorBias = EditorGUILayout.Slider("Color bias", s.m_colorBias, 0, 1);
                s.m_colorPower = EditorGUILayout.Slider("Color power", s.m_colorPower, 0, 10);
                s.m_stripes = EditorGUILayout.Slider("Stripes", s.m_stripes, 0, 50);
                s.m_hairDensity = EditorGUILayout.Slider("Hair density",s.m_hairDensity, 1, 50);
                s.m_hairPerPass = EditorGUILayout.Slider("Hair per pass", s.m_hairPerPass, 1, 50);
                s.m_hairLength = EditorGUILayout.Slider("Hair length", s.m_hairLength, 0f, 1f);
                s.m_RndLength = EditorGUILayout.Slider("Rnd length", s.m_RndLength, 0f, 1f);
                s.m_hairWidth = EditorGUILayout.Slider("Hair Width", s.m_hairWidth, 0f, 50f);
                s.m_tipWidth = EditorGUILayout.Slider("Tip Width", s.m_tipWidth, 0f, 100f);
                s.m_UVX = EditorGUILayout.IntSlider("UVX", s.m_UVX, 1, 10);
                s.m_wave1 = EditorGUILayout.Slider("Wave 1",s.m_wave1, 0, 200);
                s.m_wavePower1 = EditorGUILayout.Slider("Wave Power 1", s.m_wavePower1, .01f, .2f);
                s.m_waveRnd1 = EditorGUILayout.Slider("Wave Rnd 1", s.m_waveRnd1, 0f, 1f);
                s.m_wave2 = EditorGUILayout.Slider("Wave 2", s.m_wave2, 0, 200);
                s.m_wavePower2 = EditorGUILayout.Slider("Wave Power 2", s.m_wavePower2, .01f, .2f);
                s.m_waveRnd2 = EditorGUILayout.Slider("Wave Rnd 2", s.m_waveRnd2, 0f, 1f);
                s.m_waveDir = EditorGUILayout.Slider("Wave Dir", s.m_waveDir, 0f, 1f);
                s.m_chaos = EditorGUILayout.Slider("Chaos",s.m_chaos, 0, 1);
                s.m_cut = EditorGUILayout.Slider("Cut", s.m_cut, 0, 1);
                s.m_RootThickness = EditorGUILayout.Slider("Root thickness", s.m_RootThickness, 0, 1);
                s.m_TipThickness = EditorGUILayout.Slider("Tip thickness", s.m_TipThickness, 0, 1);
                s.m_rimColor = EditorGUILayout.ColorField("Rim color", s.m_rimColor);
                s.m_rimPower = EditorGUILayout.Slider("Rim Power",s.m_rimPower, 0, 8);
                s.m_smoothness = EditorGUILayout.Slider("Smoothness", s.m_smoothness, 0, 1);
                s.m_ao = EditorGUILayout.Slider("AO", s.m_ao, 0, 1);
                s.m_emission = EditorGUILayout.Slider("Emission", s.m_emission, 0, 1);
                s.m_alphaCutoff = EditorGUILayout.Slider("Alpha cutoff", s.m_alphaCutoff, 0, 1);
                s.m_alpha = EditorGUILayout.Slider("Alpha", s.m_alpha, 0, 1);
                s.m_normalPower = EditorGUILayout.Slider("Normal power", s.m_normalPower, 0, 1);
                s.m_rndSeed = EditorGUILayout.Slider("Rnd Seed", s.m_rndSeed, 0, 1);
                //s.m_windFactor = EditorGUILayout.Slider("Wind factor", s.m_windFactor, 0, 10);

                ShaderGUIEnd<HairDesignerShaderProcedural_v2>(s);

            }

           


        }

    }
}
