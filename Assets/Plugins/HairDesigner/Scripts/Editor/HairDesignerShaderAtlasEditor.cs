using UnityEngine;
using UnityEditor;
using System.Collections;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [CustomEditor(typeof(HairDesignerShaderAtlas))]
        public class HairDesignerShaderAtlasEditor : HairDesignerShaderEditor
        {

            Renderer r = null; 

            public override void OnInspectorGUI()
            {
                HairDesignerShaderAtlas s = target as HairDesignerShaderAtlas;                

                base.OnInspectorGUI();
                if (_destroyed)return;

                if (s.m_atlasParameters == null)
                {
                    s.m_atlasParameters = new AtlasParameters();                    
                }

                if(s.m_atlasParameters.m_shaderParameters.Count == 0 )
                    s.m_atlasParameters.m_shaderParameters.Add(new TextureToolShaderParameters());



                ShaderGUIBegin<HairDesignerShaderAtlas>(s);


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
                s.m_colorBias = EditorGUILayout.Slider("Color bias", s.m_colorBias, 0f, 1f);
                s.m_stripes = EditorGUILayout.Slider("Stripes", s.m_stripes, 0f, 100f);
                s.m_alphaCutoff = EditorGUILayout.Slider("Alpha cutoff Root", s.m_alphaCutoff, 0f, 1f);
                s.m_alphaCutoffTip = EditorGUILayout.Slider("Alpha cutoff Tip", s.m_alphaCutoffTip, 0f, 1f);
                s.m_alpha = EditorGUILayout.Slider("Alpha", s.m_alpha, 0f, 1f);
                GUILayout.EndVertical();


                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Hair parameters", EditorStyles.boldLabel);
                s.m_rndSeed = EditorGUILayout.Slider("Random", s.m_rndSeed, 0f, 1000f);
                s.m_cut = EditorGUILayout.Slider("Cut", s.m_cut, 0f, 1f);
                s.m_offset = EditorGUILayout.Slider("Offset", s.m_offset, 0f, 1f);
                s.m_length = EditorGUILayout.Slider("Length", s.m_length, 0f, 1f);
                s.m_gravity = EditorGUILayout.FloatField("Gravity", s.m_gravity);
                s.m_wind = EditorGUILayout.FloatField("Wind", s.m_wind);
                s.m_windTurbulenceFrequency = EditorGUILayout.FloatField("Wind turbulence", s.m_windTurbulenceFrequency);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Lighting", EditorStyles.boldLabel);
                s.m_NormalPower = EditorGUILayout.Slider("Normal power", s.m_NormalPower, 0f, 1f);
                s.m_NormalMode = EditorGUILayout.Slider("Normal mode", s.m_NormalMode, 0f, 1f);
                s.m_alphaAtten = EditorGUILayout.Slider("Alpha atten", s.m_alphaAtten, 0f, 1f);
                s.m_AOFactor = EditorGUILayout.Slider("AO", s.m_AOFactor, 0f, 1f);                
                s.m_smoothness = EditorGUILayout.Slider("Smoothness", s.m_smoothness, 0f, 1f);
                s.m_emission = EditorGUILayout.Slider("Emission", s.m_emission, 0f, 1f);                                
                GUI.enabled = s.m_generator.m_hairMeshMaterialTransparent != null;
                s.m_TransparentShadowReceive = EditorGUILayout.Slider("Transparency shadows", s.m_TransparentShadowReceive, 0f, 1f);
                GUI.enabled = true;
                GUILayout.EndVertical();

                //s.m_rimColor = EditorGUILayout.ColorField("Rim color", s.m_rimColor);
                //s.m_rimPower = EditorGUILayout.Slider("Rim power", s.m_rimPower, 0f, 1f);

                float specularRange = 3000f;

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Specular", EditorStyles.boldLabel);
                s.m_SpecularColor1 = EditorGUILayout.ColorField("Specular color 1", s.m_SpecularColor1);
                s.m_SpecularExp1 = EditorGUILayout.Slider("Specular power 1", s.m_SpecularExp1/ specularRange, 0f, 1f)* specularRange;
                s.m_Shift1 = EditorGUILayout.Slider("Specular shift 1", s.m_Shift1, -2f, 2f);                
                s.m_SpecularColor2 = EditorGUILayout.ColorField("Specular color 2", s.m_SpecularColor2);
                s.m_SpecularExp2 = EditorGUILayout.Slider("Specular power 2", s.m_SpecularExp2/ specularRange, 0f, 1f) * specularRange;
                s.m_Shift2 = EditorGUILayout.Slider("Specular shift 2", s.m_Shift2, -2f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Translucency", EditorStyles.boldLabel);
                s.m_translucencyStrength = EditorGUILayout.Slider("Strength", s.m_translucencyStrength, 0f, 100f);
                s.m_translucencyNormal = EditorGUILayout.Slider("Normal direction", s.m_translucencyNormal, 0f, 1f);
                s.m_translucencyPower = EditorGUILayout.Slider("Power", s.m_translucencyPower, 0f, 100f);
                s.m_translucencyDirect = EditorGUILayout.Slider("Direct light", s.m_translucencyDirect, 0f, 1f);
                s.m_translucencyIndirect = EditorGUILayout.Slider("Indirect light", s.m_translucencyIndirect, 0f, 1f);
                GUILayout.EndVertical();

                GUILayout.Space(10);

                
                if (s.m_defaultTransparent == null)
                {
                    if(s.m_generator.m_hairMeshMaterialTransparent==null || s.m_generator.m_hairMeshMaterialTransparent.shader.name != "HairDesigner/Atlas_transparent")
                        EditorGUILayout.HelpBox("The shader should be : 'HairDesigner/Atlas_transparent'\nPlease Drag&Drop the material 'HD_TextureAtlas_transparent'", MessageType.Warning);
                    s.m_generator.m_hairMeshMaterialTransparent = EditorGUILayout.ObjectField("Transparent material", s.m_generator.m_hairMeshMaterialTransparent, typeof(Material), true) as Material;
                }
                else
                    s.m_generator.m_hairMeshMaterialTransparent = s.m_defaultTransparent;

                if (s.m_generator.m_meshInstance != null )
                {
                    if(r==null)
                        r = s.m_generator.m_meshInstance.GetComponent<Renderer>();

                    if (r != null)
                    {
                        if (r.sharedMaterials[0] != s.m_generator.m_hairMeshMaterial)
                        {
                            Material[] lst = new Material[1];
                            lst[0] = s.m_generator.m_hairMeshMaterial;
                            r.sharedMaterials = lst;
                            s.m_generator.m_hairMeshMaterialTransparent = null;
                        }

                        if (r.sharedMaterials.Length == 1 && s.m_generator.m_hairMeshMaterialTransparent != null)
                        {
                            Material[] lst = new Material[2];
                            lst[0] = r.sharedMaterials[0];
                            lst[1] = s.m_generator.m_hairMeshMaterialTransparent;
                            r.sharedMaterials = lst;                            
                        }

                        if (r.sharedMaterials.Length == 2  )
                        {
                            if (s.m_generator.m_hairMeshMaterialTransparent == null)
                            {
                                Material[] lst = new Material[1];
                                lst[0] = r.sharedMaterials[0];
                                r.sharedMaterials = lst;
                                s.m_generator.m_hairMeshMaterialTransparent = null;
                            }
                            else
                            {
                                if (s.m_generator.m_hairMeshMaterialTransparent != r.sharedMaterials[1])
                                {
                                    Material[] lst = new Material[2];
                                    lst[0] = r.sharedMaterials[0];
                                    lst[1] = s.m_generator.m_hairMeshMaterialTransparent;
                                    r.sharedMaterials = lst;
                                }
                            }

                        }

                    }
                }

                

                GUILayout.Space(20);

                ShaderGUIEnd<HairDesignerShaderAtlas>(s);


                

            }


            public void TextureGeneratedCB(HairDesignerTextureGeneratorWindow.TextureGeneratedCBData data)
            {
                HairDesignerShaderAtlas s = target as HairDesignerShaderAtlas;
                s.m_diffuseTex = data.diffuse;
                s.m_normalTex = data.normal;
                s.m_AOTex = data.AO;
                s.m_SpecularTex = data.specular;
            }

        }
    }
}