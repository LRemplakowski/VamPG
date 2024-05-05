using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [CustomEditor(typeof(HairDesignerShaderAdvancedFur),true)]
        public class HairDesignerShaderAdvancedFurEditor : HairDesignerShaderEditor
        {
            

            bool LabelToggle( HairDesignerShaderAdvancedFur s, int toggleId, string name)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                s.m_editorSettings.toggleGroups[toggleId] = GUILayout.Toggle(s.m_editorSettings.toggleGroups[toggleId],"", EditorStyles.foldout, GUILayout.MaxWidth(15));
                s.m_editorSettings.toggleGroups[toggleId] = GUILayout.Toggle(s.m_editorSettings.toggleGroups[toggleId],name, EditorStyles.toolbarDropDown);
                GUILayout.EndHorizontal();
                return s.m_editorSettings.toggleGroups[toggleId];
            }

            void BeginToggle()
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical(EditorStyles.textArea);
            }


            void EndToggle()
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }


            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (_destroyed) return;

                HairDesignerShaderAdvancedFur s = target as HairDesignerShaderAdvancedFur;

                if (s.m_editorSettings == null)
                    s.m_editorSettings = new HairDesignerShaderAdvancedFur.EditorSettings();

                while (s.m_editorSettings.toggleGroups.Count < 5)
                    s.m_editorSettings.toggleGroups.Add(true);

                /*
                while (m_textureGroups.Count < s.m_editorSettings.toggleGroups.Count)
                {                    
                    m_textureGroups.Add(new AnimBool(true));
                    int id = m_textureGroups.Count - 1;
                    m_textureGroups[id].value = s.m_editorSettings.toggleGroups[id];
                    m_textureGroups[id].valueChanged.AddListener(Repaint);                    
                }
                */

                
                if (s.m_atlasParameters == null)
                {
                    s.m_atlasParameters = new AtlasParameters();
                }

                if (s.m_atlasParameters.m_shaderParameters.Count == 0)
                    s.m_atlasParameters.m_shaderParameters.Add(new TextureToolShaderParameters());



                ShaderGUIBegin<HairDesignerShaderAdvancedFur>(s);

                int toggleId = 0;
                //s.m_editorSettings.toggleGroups[toggleId] = EditorGUILayout.ToggleLeft("Textures", s.m_editorSettings.toggleGroups[toggleId], EditorStyles.toolbarDropDown);
                //if(s.m_editorSettings.toggleGroups[toggleId])
                if(LabelToggle(s,toggleId,"Textures"))
                {
                    BeginToggle();

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label("Main color", EditorStyles.boldLabel);
                    GUILayoutTextureSlot("Main texture", ref s.m_mainTex, ref s.m_mainTexScale, ref s.m_mainTexOffset);
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label("Shell density", EditorStyles.boldLabel);
                    GUILayoutTextureSlot("Density texture", ref s.m_densityTex, ref s.m_densityTexScale, ref s.m_densityTexOffset);                    
                    GUILayoutTextureSlot("Density normal", ref s.m_densityNormalTex, ref s.m_densityTexScale, ref s.m_densityTexOffset);
                    GUILayout.EndVertical();


                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Strand textures", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Texture Generator"))
                    {
                        HairDesignerTextureGeneratorWindow w = HairDesignerTextureGeneratorWindow.Init();
                        w.m_atlasParameters = s.m_atlasParameters;
                        w.BakeTexture(300);
                        w.Repaint();
                        w.TextureGeneratedCB = TextureGeneratedCB;
                    }
                    GUILayout.EndHorizontal();


                    GUILayoutTextureSlot("Fur texture", ref s.m_furTex, ref s.m_furTexScale, ref s.m_furTexOffset);

                    s.m_atlasParameters.sizeX = EditorGUILayout.IntSlider("Atlas size", s.m_atlasParameters.sizeX, 1, 8);
                    s.m_cutout = EditorGUILayout.Slider("Cutout", s.m_cutout, 0f, 1f);
                    //s.m_cutoutAlpha = EditorGUILayout.Slider("Cutout alpha", s.m_cutoutAlpha, 0f, 1f);



                    GUILayoutTextureSlot("Fur normal texture", ref s.m_normalTex, ref s.m_normalTexScale, ref s.m_normalTexOffset);


                    s.m_normalStrength = EditorGUILayout.Slider("Normal strength", s.m_normalStrength, 0f, 5f);
                    

                    //GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();






                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label("Design textures", EditorStyles.boldLabel);
                    GUILayoutTextureSlot("Mask texture", ref s.m_maskTex, ref s.m_maskTexScale, ref s.m_maskTexOffset);
                    GUILayoutTextureSlot("Color texture", ref s.m_colorTex, ref s.m_colorTexScale, ref s.m_colorTexOffset);
                    GUI.enabled = s.m_colorTex != null;
                    s.m_colorTexIntensity = EditorGUILayout.Slider("Color intensity", s.m_colorTexIntensity, 0f, 1f);
                    GUI.enabled = true;


                    s.m_brushTex = EditorGUILayout.ObjectField("Brush texture", s.m_brushTex, typeof(Texture2D), false) as Texture2D;
                    GUI.enabled = s.m_brushTex != null;
                    s.m_brushStrength = EditorGUILayout.Slider("Brush strength", s.m_brushStrength, 0, 1);
                    GUI.enabled = true;

                    GUILayout.EndVertical();

                    EndToggle();
                }
                //EditorGUILayout.EndFadeGroup();

                toggleId++;
                if (LabelToggle(s, toggleId, "General"))
                {
                    BeginToggle();
                    s.m_length = Mathf.Clamp(EditorGUILayout.FloatField("Fur length", s.m_length), 0, float.MaxValue);
                    GUILayout.Space(5);
                    s.m_gravity = EditorGUILayout.Slider("Gravity", s.m_gravity, 0, 10);
                    GUILayout.Space(5);
                    s.m_wind = EditorGUILayout.FloatField("Wind", s.m_wind);
                    s.m_windTurbulenceFrequency = EditorGUILayout.FloatField("Wind turbulence", s.m_windTurbulenceFrequency);
                    GUILayout.Space(5);
                    s.m_motionFactor = EditorGUILayout.FloatField("Motion factor", s.m_motionFactor);
                    GUILayout.Space(5);
                    s.m_smoothness = EditorGUILayout.Slider("Smoothness", s.m_smoothness, 0, 1);
                    s.m_metallic = EditorGUILayout.Slider("Metallic", s.m_metallic, 0, 1);
                    s.m_AO = EditorGUILayout.Slider("AO", s.m_AO, 0, 1);


                    GUILayout.Space(5);
#if UNITY_2018_3_OR_NEWER                    
                    s.m_rootEmissionColor = EditorGUILayout.ColorField(new GUIContent("Root emission"), s.m_rootEmissionColor, true, true, true);
                    s.m_tipEmissionColor = EditorGUILayout.ColorField(new GUIContent("Tip emission"), s.m_tipEmissionColor, true, true, true);

#else
                    ColorPickerHDRConfig hdrConfig = new ColorPickerHDRConfig(-10, 10, -10, 10);
                    s.m_rootEmissionColor = EditorGUILayout.ColorField(new GUIContent("Root emission"), s.m_rootEmissionColor,true,true,true, hdrConfig);
                    s.m_tipEmissionColor = EditorGUILayout.ColorField(new GUIContent("Tip emission"), s.m_tipEmissionColor, true, true, true, hdrConfig);
#endif

                    s.m_emissionThreshold = EditorGUILayout.Slider("Emission threshhold", s.m_emissionThreshold, 0f, 10f);
                    s.m_emission = EditorGUILayout.Slider("Emission", s.m_emission, 0f, 1f);
                    GUILayout.Space(5);

                                        
                    s.m_rimColor = EditorGUILayout.ColorField("Rim color", s.m_rimColor);
                    s.m_rimPower = EditorGUILayout.Slider("Rim power", s.m_rimPower, 0, 10);

                    //s.m_translucency = EditorGUILayout.Slider("Translucency", s.m_translucency, 0, 1);

                    s.m_randomSeed = EditorGUILayout.FloatField("Random seed", s.m_randomSeed);

                    EndToggle();
                }



                    toggleId++;
                if (LabelToggle(s, toggleId, "Strands"))
                {
                    BeginToggle();
                    s.m_strandRootColor = EditorGUILayout.ColorField("Strand Root Color", s.m_strandRootColor);
                    s.m_strandTipColor = EditorGUILayout.ColorField("Strand Tip Color", s.m_strandTipColor);
                    s.m_strandColorThreshold = EditorGUILayout.Slider("Strand Color threshold", s.m_strandColorThreshold, 0f, 5f);

                    //s.m_TessEdge = Mathf.Clamp(EditorGUILayout.IntField("Density", (int)s.m_TessEdge), 1, 20);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Density");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("-")) s.m_strandDensity--;
                    GUILayout.Label("" + s.m_strandDensity);
                    if (GUILayout.Button("+")) s.m_strandDensity++;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    s.m_strandDensity = Mathf.Clamp(s.m_strandDensity, 1, 20);

                    if( s.m_strandDensity>5 )                    
                        EditorGUILayout.HelpBox("High density will impact performances according to the model polycount.\nTry to increase the 'Width' parameter to increase coverage.", MessageType.Warning);
                    

                    s.m_strandWidth = Mathf.Clamp(EditorGUILayout.FloatField("Width", s.m_strandWidth), 0, float.MaxValue);
                    s.m_strandWidthConstantOrPerFaceFactor = EditorGUILayout.Slider("Contant/per face", s.m_strandWidthConstantOrPerFaceFactor, 0, 1);

                    s.m_strandRandomLength = EditorGUILayout.Slider("Random length", s.m_strandRandomLength, 0, 1);

                    s.m_strandSubdivision = EditorGUILayout.IntSlider("Strand subdivisions", s.m_strandSubdivision, 1, 4);
                    s.m_strandFlexibility = EditorGUILayout.Slider("Strand flexibility", s.m_strandFlexibility, 0, 1);


                    s.m_strandOrientation = EditorGUILayout.Slider("Orientation", s.m_strandOrientation, 0f, 1f);
                    s.m_normalSwitch = EditorGUILayout.Slider("Normal switch", s.m_normalSwitch, 0f, 1f);


                    s.m_strandWaveFrequency = EditorGUILayout.Slider("Wave Frequency", s.m_strandWaveFrequency, 0, 10);
                    s.m_strandWaveAmplitude = EditorGUILayout.Slider("Wave Amplitude", s.m_strandWaveAmplitude, 0, 1);

                    s.m_strandPinch = EditorGUILayout.Slider("Pinch", s.m_strandPinch, -1, 1);




                    EndToggle();

                }

                toggleId++;
                if (LabelToggle(s, toggleId, "Shells"))
                {
                    BeginToggle();
                    s.m_shellRootColor = EditorGUILayout.ColorField("Shell Root Color", s.m_shellRootColor);
                    s.m_shellTipColor = EditorGUILayout.ColorField("Shell Tip Color", s.m_shellTipColor);
                    s.m_shellColorThreshold = EditorGUILayout.Slider("Shell Color threshold", s.m_shellColorThreshold, 0f, 5f);


                    s.m_shellCount = EditorGUILayout.IntSlider("Shell count", s.m_shellCount, 1, 9);
                    s.m_shellHeight = EditorGUILayout.Slider("Shell height", s.m_shellHeight, 0, 1);
                    s.m_ShellCoverage = Mathf.Clamp(EditorGUILayout.FloatField("Coverage", s.m_ShellCoverage), 0, float.MaxValue);

                    s.m_ShellThickness = EditorGUILayout.Slider("Thickness", s.m_ShellThickness, 0, 1);

                    EndToggle();
                }


                toggleId++;
                if (LabelToggle(s, toggleId, "LOD"))
                {
                    BeginToggle();

                    s.m_useLOD = GUILayout.Toggle(s.m_useLOD, "Use LOD");                   

                    GUI.enabled = s.m_useLOD;

                    if (SceneView.lastActiveSceneView != null)
                        GUILayout.Label("Camera distance : " + (s.m_useLOD ? Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, s.transform.position):0));

                    for (int i = 0; i < s.m_LODSettings.Count; ++i)
                    {
                        if (i > 0 && (s.m_LODSettings[i].m_range.x != s.m_LODSettings[i - 1].m_range.y))
                            s.m_LODSettings[i].m_range.x = s.m_LODSettings[i - 1].m_range.y;

                        GUI.backgroundColor = (s.m_currentLOD == i && s.m_useLOD) ? Color.yellow : Color.white;

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        s.m_LODSettings[i].m_range = EditorGUILayout.Vector2Field("Camera range", s.m_LODSettings[i].m_range);
                        s.m_LODSettings[i].m_strandDensity = EditorGUILayout.IntField("Strand density", s.m_LODSettings[i].m_strandDensity);
                        s.m_LODSettings[i].m_strandDensity = Mathf.Clamp(s.m_LODSettings[i].m_strandDensity, 1, 20);

                        s.m_LODSettings[i].m_strandSubdivision = EditorGUILayout.IntSlider("Strand subdivisions", s.m_LODSettings[i].m_strandSubdivision, 0, 4);

                        s.m_LODSettings[i].m_shellCount = EditorGUILayout.IntSlider("Shell count", s.m_LODSettings[i].m_shellCount, 0, 9);
                        s.m_LODSettings[i].m_shellCoverage = Mathf.Clamp(EditorGUILayout.FloatField("Coverage", s.m_LODSettings[i].m_shellCoverage), 0, float.MaxValue);


                        s.m_LODSettings[i].m_strandsShadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup("Strands shadow mode", s.m_LODSettings[i].m_strandsShadowCastingMode);
                        s.m_LODSettings[i].m_shellsShadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup("Shells shadow mode", s.m_LODSettings[i].m_shellsShadowCastingMode);

                        s.m_LODSettings[i].m_updateMotion = EditorGUILayout.Toggle("Update Motion",s.m_LODSettings[i].m_updateMotion);

                        GUILayout.EndVertical();

                    }

                    GUI.enabled = true;

                    GUI.backgroundColor = Color.white;




                    if ( GUILayout.Button("+") )
                    {
                        s.m_LODSettings.Add(new HairDesignerShaderAdvancedFur.LODData());
                    }


                    if( s.m_useLOD )
                    {
                        SceneView.lastActiveSceneView.Repaint();
                    }

                    EndToggle();
                }

                ShaderGUIEnd<HairDesignerShaderAdvancedFur>(s);

                

            }

            public void TextureGeneratedCB(HairDesignerTextureGeneratorWindow.TextureGeneratedCBData data)
            {
                HairDesignerShaderAdvancedFur s = target as HairDesignerShaderAdvancedFur;
                s.m_furTex = data.diffuse;
                s.m_normalTex = data.normal;                
            }



            
            private void OnEnable()
            {               
                HairDesignerShaderAdvancedFur s = target as HairDesignerShaderAdvancedFur;
                if (s != null)
                    s.OnLODCallback += RefreshInspector;
            }

            private void OnDisable()
            {                
                HairDesignerShaderAdvancedFur s = target as HairDesignerShaderAdvancedFur;

                if(s!=null)
                    s.OnLODCallback -= RefreshInspector;
            }



            void RefreshInspector()
            {
                HairDesignerShaderAdvancedFur s = target as HairDesignerShaderAdvancedFur;

                if (s != null) 
                    Repaint();
            }
        }
    }
}
