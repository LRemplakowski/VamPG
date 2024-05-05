using UnityEngine;
using UnityEditor;
using System.Collections;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [CustomEditor(typeof(HairDesignerShaderHDRP_Atlas),true)]
        public class HairDesignerShaderHDRP_AtlasEditor : HairDesignerShaderEditor
        {

            //static HairDesignerShaderHDRP_Atlas m_copy = null;
            Renderer r = null; 

            public override void OnInspectorGUI()
            {
                HairDesignerShaderHDRP_Atlas s = target as HairDesignerShaderHDRP_Atlas;                

                base.OnInspectorGUI();
                if (_destroyed)return;

                if (s.m_atlasParameters == null)
                {
                    s.m_atlasParameters = new AtlasParameters();                    
                }

                if(s.m_atlasParameters.m_shaderParameters.Count == 0 )
                    s.m_atlasParameters.m_shaderParameters.Add(new TextureToolShaderParameters());

                if (s.m_generator != null)
                    s.m_generator.m_shaderAtlasSize = s.m_atlasParameters.sizeX;

                if (s.m_generator.m_meshInstance != null)
                {
                    if (r == null)
                        r = s.m_generator.m_meshInstance.GetComponent<Renderer>();

                    if (r != null)
                    {
                        if (r.sharedMaterials.Length > 1 )
                        {
                            Material[] lst = new Material[1];
                            lst[0] = r.sharedMaterial;
                            r.sharedMaterials = lst;
                            s.m_generator.m_hairMeshMaterial = r.sharedMaterial;
                            s.m_generator.m_hairMeshMaterialTransparent = null;
                        }
                    }
                }


                ShaderGUIBegin<HairDesignerShaderHDRP_Atlas>(s);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                    
                //shader parameters
                s.m_atlasParameters.sizeX = EditorGUILayout.IntSlider("Atlas size", s.m_atlasParameters.sizeX, 1, 8);
                s.m_diffuseTex = EditorGUILayout.ObjectField( "Diffuse Texture", s.m_diffuseTex, typeof(Texture2D), false) as Texture2D;
                s.m_normalTex = EditorGUILayout.ObjectField("Normal Texture", s.m_normalTex, typeof(Texture2D), false) as Texture2D;
                s.m_AOTex = EditorGUILayout.ObjectField("AO Texture", s.m_AOTex, typeof(Texture2D), false) as Texture2D;
                s.m_SpecularTex = EditorGUILayout.ObjectField("Specular Texture", s.m_SpecularTex, typeof(Texture2D), false) as Texture2D;

                GUILayout.BeginHorizontal();
                //GUILayout.FlexibleSpace();
                if (GUILayout.Button("Texture Generator"))
                {
                    HairDesignerTextureGeneratorWindow w = HairDesignerTextureGeneratorWindow.Init();
                    w.m_atlasParameters = s.m_atlasParameters;
                    w.BakeTexture(300);
                    w.Repaint();
                    w.TextureGeneratedCB = TextureGeneratedCB;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Colors", EditorStyles.boldLabel);
                s.m_rootColor = EditorGUILayout.ColorField("Root color", s.m_rootColor);                
                s.m_tipColor = EditorGUILayout.ColorField("Tip color", s.m_tipColor);                
                s.m_colorThreshold = EditorGUILayout.Slider("Root/Tip threshold", s.m_colorThreshold, 0f, 1f);
                

                GUILayout.EndVertical();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Alpha", EditorStyles.boldLabel);
                s.m_alphaClip = EditorGUILayout.Slider("Alpha cutoff", s.m_alphaClip, 0f, 1f);
                s.m_alpha = EditorGUILayout.Slider("Alpha", s.m_alpha, 0f, 1f);
                s.m_rootAlphaThreshold = EditorGUILayout.Slider("Root alpha threshold", s.m_rootAlphaThreshold, 0f, 1f);
                s.m_tipAlphaThreshold = EditorGUILayout.Slider("Tip alpha threshold", s.m_tipAlphaThreshold, 0f, 1f);                               
                s.m_alphaMax = EditorGUILayout.Slider("Alpha Max", s.m_alphaMax, 0f, 1f);
                s.m_alphaStrength = EditorGUILayout.Slider("Alpha Density root", s.m_alphaStrength, 0f, 100f);
                s.m_alphaStrengthTip = EditorGUILayout.Slider("Alpha Density tip", s.m_alphaStrengthTip, 0f, 100f);

                GUILayout.EndVertical();


                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Hair parameters", EditorStyles.boldLabel);
                s.m_atlasOffset = EditorGUILayout.Slider("Random", s.m_atlasOffset, 0f, 1000f);
                s.m_depthOffset = EditorGUILayout.FloatField(new GUIContent( "Depth offset", "default : -0.1"), s.m_depthOffset);
                //s.m_cut = EditorGUILayout.Slider("Cut", s.m_cut, 0f, 1f);
                s.m_length = EditorGUILayout.Slider("Length", s.m_length, 0f, 1f);
                s.m_gravity = EditorGUILayout.FloatField("Gravity", s.m_gravity);
                s.m_wind = EditorGUILayout.FloatField("Wind", s.m_wind);
                s.m_windTurbulenceFrequency = EditorGUILayout.FloatField("Wind turbulence", s.m_windTurbulenceFrequency);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Lighting", EditorStyles.boldLabel);
                s.m_normalStrength = EditorGUILayout.Slider("Normal strength", s.m_normalStrength, 0f, 10f);
                s.m_AOStrength = EditorGUILayout.Slider("AO strength", s.m_AOStrength, 0f, 1f);
                s.m_shadowThreshold = EditorGUILayout.Slider("Shadow threshold", s.m_shadowThreshold, 0f, 1f);
                s.m_transmittance = EditorGUILayout.ColorField("Transmittance", s.m_transmittance);
                s.m_emission = EditorGUILayout.Slider("Emission", s.m_emission, 0f, 1f);
                s.m_emissionPower.x = EditorGUILayout.Slider("Emission Root", s.m_emissionPower.x, 0f, 1f);
                s.m_emissionPower.y = EditorGUILayout.Slider("Emission Tip", s.m_emissionPower.y, 0f, 1f);
                s.m_emissionPower.z = EditorGUILayout.Slider("Emission Root/Tip", s.m_emissionPower.z, 0f, 1f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Specular", EditorStyles.boldLabel);
                s.m_specularColor1 = EditorGUILayout.ColorField("Specular color 1", s.m_specularColor1);
                s.m_smoothness1 = EditorGUILayout.Slider("Smoothness 1", s.m_smoothness1, 0f, 1f);
                s.m_shift1 = EditorGUILayout.Slider("Specular shift 1", s.m_shift1, -2f, 2f);                
                s.m_specularColor2 = EditorGUILayout.ColorField("Specular color 2", s.m_specularColor2);
                s.m_smoothness2 = EditorGUILayout.Slider("Smoothness 2", s.m_smoothness2, 0f, 1f);
                s.m_shift2 = EditorGUILayout.Slider("Specular shift 2", s.m_shift2, -2f, 2f);

                if (s.m_strandDirection.magnitude == 0)
                    GUI.color = Color.red;
                s.m_strandDirection = EditorGUILayout.Vector3Field("Strand direction", s.m_strandDirection);
                GUI.color = Color.white;

                GUILayout.EndVertical();

                

                GUILayout.Space(10);


                /*
                GUILayout.Space(20);                

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy parameters"))
                {
                    m_copy = s.gameObject.AddComponent<HairDesignerShaderHDRP_Atlas>();
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


                ShaderGUIEnd<HairDesignerShaderHDRP_Atlas>(s);

                GUI.enabled = true;
                

            }


            public void TextureGeneratedCB(HairDesignerTextureGeneratorWindow.TextureGeneratedCBData data)
            {
                HairDesignerShaderHDRP_Atlas s = target as HairDesignerShaderHDRP_Atlas;
                s.m_diffuseTex = data.diffuse;
                s.m_normalTex = data.normal;
                s.m_AOTex = data.AO;
                s.m_SpecularTex = data.specular;
            }

        }
    }
}