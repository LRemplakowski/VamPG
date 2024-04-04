using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [CustomEditor(typeof(HairDesignerShaderProcedural))]
        public class HairDesignerShaderProceduralEditor : HairDesignerShaderEditor
        {

            public override void OnInspectorGUI()
            {

                base.OnInspectorGUI();
                if (_destroyed) return;

                //GUILayout.Label("HairDesignerShaderProceduralEditor");
                HairDesignerShaderProcedural s = target as HairDesignerShaderProcedural;

                ShaderGUIBegin<HairDesignerShaderProcedural>(s);

                s.m_startColor = EditorGUILayout.ColorField("Start color", s.m_startColor);
                s.m_endColor = EditorGUILayout.ColorField("End color", s.m_endColor);
                s.m_colorBalance = EditorGUILayout.Slider("Color balance", s.m_colorBalance, 0, 1);
                s.m_stripes = EditorGUILayout.Slider("Stripes", s.m_stripes, 0, 50);
                s.m_hairDensity = EditorGUILayout.Slider("Hair density",s.m_hairDensity, 1, 50);
                s.m_hairSize = EditorGUILayout.Slider("Hair size", s.m_hairSize, 0f, 1f);
                s.m_hairWidth = EditorGUILayout.Slider("Hair Width", s.m_hairWidth, 0f, 50f);
                s.m_UVX = EditorGUILayout.IntSlider("UVX", s.m_UVX, 1, 10);
                s.m_wave = EditorGUILayout.Slider("Wave",s.m_wave, 0, 200);
                s.m_wavePower = EditorGUILayout.Slider("Wave Power",s.m_wavePower, .01f, .2f);
                s.m_chaos = EditorGUILayout.Slider("Chaos",s.m_chaos, 0, 1);
                s.m_cut = EditorGUILayout.Slider("Cut", s.m_cut, 0, 1);
                s.m_rimColor = EditorGUILayout.ColorField("Rim color", s.m_rimColor);
                s.m_rimPower = EditorGUILayout.Slider("Rim Power",s.m_rimPower, 0, 8);
                s.m_smoothness = EditorGUILayout.Slider("Smoothness", s.m_smoothness, 0, 1);
                s.m_ao = EditorGUILayout.Slider("AO", s.m_ao, 0, 1);
                s.m_emission = EditorGUILayout.Slider("Emission", s.m_emission, 0, 1);


                ShaderGUIEnd<HairDesignerShaderProcedural>(s);

            }

           


        }

    }
}
