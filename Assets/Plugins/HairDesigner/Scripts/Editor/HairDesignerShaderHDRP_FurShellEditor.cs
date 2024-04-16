using UnityEngine;
using UnityEditor;
using System.Collections;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [CustomEditor(typeof(HairDesignerShaderHDRP_FurShell))]
        public class HairDesignerShaderHDRP_FurShellEditor : HairDesignerShaderEditor
        {
            public static HairDesignerShaderHDRP_FurShell m_copy;
            

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (_destroyed) return;

                HairDesignerShaderHDRP_FurShell s = target as HairDesignerShaderHDRP_FurShell;


                if (s == null)
                    return;


                //Fix the instancing mode for HDRP
                HairDesignerGeneratorFurShellBase g = s.m_generator as HairDesignerGeneratorFurShellBase;

                ShaderGUIBegin<HairDesignerShaderHDRP_FurShell>(s);

                s.m_sortingPriority = EditorGUILayout.IntField("Sorting order", s.m_sortingPriority);

                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Label("Textures", EditorStyles.miniBoldLabel);
                GUILayoutTextureSlot("Main texture", ref s.m_mainTex, ref s.m_mainTexScale, ref s.m_mainTexOffset);
                GUILayoutTextureSlot("Density texture", ref s.m_densityTex, ref s.m_densityTexScale, ref s.m_densityTexOffset);
                GUILayoutTextureSlot("Normal texture", ref s.m_normalTex, ref s.m_densityTexScale, ref s.m_densityTexOffset);
                GUILayoutTextureSlot("Mask texture", ref s.m_maskTex, ref s.m_maskTexScale, ref s.m_maskTexOffset);
                GUILayoutTextureSlot("Color texture", ref s.m_colorTex, ref s.m_colorTexScale, ref s.m_colorTexOffset);
                s.m_brushTex = EditorGUILayout.ObjectField( "Brush texture", s.m_brushTex, typeof(Texture2D), false) as Texture2D;
                GUILayout.EndVertical();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Parameters", EditorStyles.miniBoldLabel);
                s.m_furLength = Mathf.Clamp(EditorGUILayout.FloatField("Fur length", s.m_furLength), 0, float.MaxValue);

                s.m_furNoise.x = EditorGUILayout.Slider("Noise amplitude", s.m_furNoise.x * 10f, 0, 1) * .1f;
                s.m_furNoise.y = EditorGUILayout.Slider("Noise random", s.m_furNoise.y, 0, 10);

                s.m_transparencyThreshold = EditorGUILayout.Slider("Transparency threshold", s.m_transparencyThreshold, 0, 1f);
                s.m_furDensity = EditorGUILayout.Slider("Density", s.m_furDensity, 0, 5);
                s.m_thickness = EditorGUILayout.Slider("Thickness", s.m_thickness, 0, 1);
                s.m_curlNumber = EditorGUILayout.Slider("Curl number", s.m_curlNumber, 0, 50);
                s.m_curlRadius = EditorGUILayout.Slider("Curl radius", s.m_curlRadius, 0, 10);
                //s.m_curlRnd = EditorGUILayout.Slider("Curl random", s.m_curlRnd, 0, 1);
                GUILayout.EndVertical();


                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Brush texture parameters", EditorStyles.miniBoldLabel);

                GUI.enabled = s.m_brushTex != null;
                s.m_brushFactor = EditorGUILayout.Slider("Brush Factor", s.m_brushFactor, 0, 1);
                s.m_lengthFactor = EditorGUILayout.Slider("Length Factor", s.m_lengthFactor, 0, 1);
                s.m_BrushBend.x = EditorGUILayout.Slider("Brush bend (short)", s.m_BrushBend.x, 0, 2);
                s.m_BrushBend.y = EditorGUILayout.Slider("Brush bend (long)", s.m_BrushBend.y, 0, 2);
                GUI.enabled = true;

                GUILayout.EndVertical();

               

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Colors", EditorStyles.miniBoldLabel);

                s.m_RootColor = EditorGUILayout.ColorField("Root Color", s.m_RootColor);
                s.m_TipColor = EditorGUILayout.ColorField("Tip Color", s.m_TipColor);
                s.m_ColorBias = EditorGUILayout.Slider("Color bias 1", s.m_ColorBias, 0f, 1f);
                //s.m_ColorBiasLength = EditorGUILayout.Slider("Color bias 2", s.m_ColorBiasLength, 0f, 1f);

                GUILayout.Label("Specular 1", EditorStyles.miniLabel);
                s.m_specular1 = EditorGUILayout.ColorField("Color", s.m_specular1);
                s.m_smoothness1 = EditorGUILayout.Slider("Smoothness", s.m_smoothness1, 0, 1);
                s.m_specularShift1 = EditorGUILayout.Slider("Shift", s.m_specularShift1, -1f, 1f);

                GUILayout.Space(5);
                GUILayout.Label("Specular 2", EditorStyles.miniLabel);
                s.m_specular2 = EditorGUILayout.ColorField("Color", s.m_specular2);
                s.m_smoothness2 = EditorGUILayout.Slider("Smoothness", s.m_smoothness2, 0, 1);
                s.m_specularShift2 = EditorGUILayout.Slider("Shift", s.m_specularShift2, -1f, 1f);

                GUILayout.EndVertical();



                GUILayout.BeginVertical(EditorStyles.helpBox);
                



                GUILayout.Label("Lighting", EditorStyles.miniBoldLabel);
                //s.m_metallic = EditorGUILayout.Slider("Metallic", s.m_metallic, 0, 1);
                s.m_normalStrength.x = EditorGUILayout.Slider("Normal strength Root", s.m_normalStrength.x, 0, 10);
                s.m_normalStrength.y = EditorGUILayout.Slider("Normal strength Tip", s.m_normalStrength.y, 0, 10);


                s.m_emission = EditorGUILayout.Slider("Emission", s.m_emission, 0, 10);
                s.m_emissionPower.x = EditorGUILayout.Slider("Emission power Root", s.m_emissionPower.x, 0, 1f) ;
                s.m_emissionPower.y = EditorGUILayout.Slider("Emission power Tip", s.m_emissionPower.y, 0, 1f) ;
                s.m_AO = EditorGUILayout.Slider("AO", s.m_AO, 0, 1);

                s.m_RimColor = EditorGUILayout.ColorField("Rim color", s.m_RimColor);
                s.m_RimPower = EditorGUILayout.Slider("Rim power", s.m_RimPower, 0, 1);

                GUILayout.EndVertical();


                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Forces", EditorStyles.miniBoldLabel);
                s.m_gravity = EditorGUILayout.Slider("Gravity", s.m_gravity, 0, 1);
                s.m_wind = EditorGUILayout.FloatField("Wind", s.m_wind);
                s.m_windTurbulenceFrequency = EditorGUILayout.FloatField("Wind turbulence", s.m_windTurbulenceFrequency);

                GUILayout.EndVertical();

                ShaderGUIEnd<HairDesignerShaderHDRP_FurShell>(s);


                /*
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy parameters"))
                {
                    m_copy = s.gameObject.AddComponent<HairDesignerShaderHDRP_FurShell>();
                    m_copy.hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
                    EditorUtility.CopySerialized(s, m_copy);
                }

                GUI.enabled = m_copy != null;
                if (GUILayout.Button("Paste parameters"))
                {
                    Undo.RecordObject(s, "Shader parameters paste");
                    EditorUtility.CopySerialized(m_copy, s);
                }
                GUILayout.EndHorizontal();
                */
            }



        }
    }
}
