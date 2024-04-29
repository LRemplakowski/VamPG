using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        public class HairDesignerTextureGeneratorWindow : EditorWindow
        {
            public static Texture2D m_icon;

            public static Texture2D m_diffuse;
            public static Texture2D m_normal;
            public static Texture2D m_AO;
            public static Texture2D m_Specular;
            public static Texture2D m_alpha;

            //public static MaterialPropertyBlock m_pb;
            public bool m_autoRefresh = true;
            public bool m_needRefresh = false;
            public bool m_bakeAlphaMap = false;

            public TextureToolShaderParameters m_parameters;
            public AtlasParameters m_atlasParameters = new AtlasParameters();
            public int m_currentTextureId = 0;
            public eTextureID m_display = eTextureID.ALL;

            public class TextureGeneratedCBData
            {
                public int atlasSize = 1;
                public Texture2D diffuse;
                public Texture2D normal;
                public Texture2D AO;
                public Texture2D specular;

            }
            public System.Action<TextureGeneratedCBData> TextureGeneratedCB;


            public enum eTextureID
            {
                DIFFUSE = 0,
                NORMAL,
                AO,
                SPECULAR_ANISO,
                ALL
            }

            public enum eSpecularMode
            {
                COMBINED = 0,
                SPECULAR,
                ANISO_SHIFT,
                ANISO_MASK 
            }

            eSpecularMode m_specularMode = eSpecularMode.COMBINED;





            [MenuItem("Window/HairDesigner/Texture Generator")]
            static public HairDesignerTextureGeneratorWindow Init()
            {
                // Get existing open window or if none, make a new one:
                HairDesignerTextureGeneratorWindow window = (HairDesignerTextureGeneratorWindow)EditorWindow.GetWindow(typeof(HairDesignerTextureGeneratorWindow));
                m_icon = Resources.Load<Texture2D>("Icons/BULLET");
                window.titleContent = new GUIContent("Texture Gen", m_icon);
                window.Show();

                window.m_atlasParameters = new AtlasParameters();

                //window.BakeTexture(512);
                return window;
            }








            Material mat;
            MaterialEditor matEditor;
            //MaterialProperty matProp;
            Object[] m_obj = null;


            void UpdateMatTextures()
            {
                if (mat == null)
                {
                    mat = new Material(Shader.Find("HairDesigner/Atlas_transparent"));
                }
                mat.SetTexture("_MainTex", m_diffuse);
                mat.SetTexture("_NormalTex", m_normal);
                mat.SetTexture("_AOTex", m_AO);
                mat.SetTexture("_SpecularTex", m_Specular);
            }

            void DrawMaterial()
            {
                if (mat == null)
                {
                    mat = new Material(Shader.Find("HairDesigner/Atlas_transparent"));
                }

                if (mat != null)
                {
                    if (matEditor == null)
                        matEditor = Editor.CreateEditor(mat) as MaterialEditor;

                    UpdateMatTextures();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    matEditor.DefaultPreviewSettingsGUI();
                    //matEditor.DoubleSidedGIField();
                    //matEditor.RenderStaticPreview("")
                    if (m_obj == null)
                    {
                        //matProp = new MaterialProperty();
                        m_obj = new Object[1];
                        m_obj[0] = mat;
                    }
                    //matEditor.DefaultShaderProperty(matProp, "test");
                    

                    GUILayout.EndHorizontal();
                    matEditor.DrawPreview(GUILayoutUtility.GetRect(256, 256));

                    //matEditor.BeginAnimatedCheck(new Rect(0, 0, 100, 100), matProp);
                    //matEditor.DrawDefaultInspector();
                                       
                    /*
                    matEditor.FloatProperty(MP("_Smoothness"), "Smoothness");
                    matEditor.ColorProperty(MP("_RootColor"), "Root Color");
                    matEditor.ColorProperty(MP("_TipColor"), "Tip Color");
                    matEditor.RangeProperty(MP("_ColorBias"), "Color bias");


                    matEditor.RangeProperty(MP("_Shift1"), "Color bias");
                    matEditor.RangeProperty(MP("_SpecularExp1"), "Color bias");
                    matEditor.ColorProperty(MP("_SpecularColor1"), "Color bias");
                    */
                    //GUILayout.HorizontalScrollbar()
                    //matEditor.PropertiesGUI();
                }
                                
            }



            MaterialProperty MP(string propName )
            {
                return MaterialEditor.GetMaterialProperty(m_obj, propName);
            }


            Vector2 scroll = new Vector2(0, 0);
            static int tabID = 0;
            int tabParametersID = 0;
            void OnGUI()
            {
                //GUILayout.Label("Diffuse Texture", EditorStyles.boldLabel);


                if (m_atlasParameters == null)
                {
                    EditorGUILayout.HelpBox("No atlas", MessageType.Error);
                    return;
                }
                scroll = GUILayout.BeginScrollView(scroll);
                GUILayout.BeginVertical(EditorStyles.helpBox);                

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Atlas parameters", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Texture grid", GUILayout.MaxWidth(100));
                m_atlasParameters.sizeX = EditorGUILayout.IntSlider(m_atlasParameters.sizeX, 1, 8, GUILayout.MaxWidth(200));
                m_atlasParameters.sizeY = m_atlasParameters.sizeX;
                GUILayout.EndHorizontal();



                int id = 0;
                switch (m_atlasParameters.textureSize)
                {
                    case 128: id = 0; break;
                    case 256: id = 1; break;
                    case 512: id = 2; break;
                    case 1024: id = 3; break;
                    case 2048: id = 4; break;
                    case 4096: id = 5; break;
                }

                string[] sizeLbl = { "128", "256", "512", "1024", "2048", "4096" };
                id = EditorGUILayout.Popup("Atlas size", id, sizeLbl, GUILayout.MaxWidth(250));

                m_atlasParameters.textureSize = (int)Mathf.Pow(2, id + 7);
                int textureSize = Mathf.RoundToInt((float)m_atlasParameters.textureSize / (float)m_atlasParameters.sizeX);
                GUILayout.Label("Textures size : " + textureSize + "x" + textureSize, EditorStyles.miniLabel);
                GUILayout.Label("");



                GUILayout.BeginVertical();
                m_atlasParameters.m_enableDiffuse = GUILayout.Toggle(m_atlasParameters.m_enableDiffuse, "Diffuse");
                m_atlasParameters.m_enableNormal = GUILayout.Toggle(m_atlasParameters.m_enableNormal, "Normal");
                m_atlasParameters.m_enableAO = GUILayout.Toggle(m_atlasParameters.m_enableAO, "AO");
                m_atlasParameters.m_enableSpecular = GUILayout.Toggle(m_atlasParameters.m_enableSpecular, "Specular/Aniso");
                GUILayout.EndVertical();


                GUILayout.EndVertical();
                GUILayout.Space(10);

                // GUILayout.FlexibleSpace();
                GUILayout.BeginVertical( EditorStyles.helpBox );
                for (int i = 0; i < m_atlasParameters.sizeX; ++i)
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < m_atlasParameters.sizeY; ++j)
                    {
                        int index = i * m_atlasParameters.sizeY + j;
                        GUI.color = m_currentTextureId == index ? Color.white : Color.grey;
                        if (GUILayout.Button("" + ((index + 1)), GUILayout.Width(25)))
                        {
                            m_currentTextureId = index;
                            BakeTexture(512);
                        }
                    }
                    GUI.color = Color.white;
                    //GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();

                //GUILayout.Space(10);

                while (m_atlasParameters.m_shaderParameters.Contains(null))
                    m_atlasParameters.m_shaderParameters.Remove(null);

                while (m_atlasParameters.m_shaderParameters.Count < m_atlasParameters.sizeX * m_atlasParameters.sizeY)
                    m_atlasParameters.m_shaderParameters.Add(new TextureToolShaderParameters());

                //GUILayout.BeginHorizontal();

                


                GUILayout.EndVertical();



                if (m_currentTextureId >= m_atlasParameters.m_shaderParameters.Count)
                    m_currentTextureId = 0;

                if (m_currentTextureId == m_atlasParameters.m_shaderParameters.Count)
                    return;

                m_parameters = m_atlasParameters.m_shaderParameters[m_currentTextureId];

                //GUILayout.Label("Texture " + (m_currentTextureId + 1) + " parameters", EditorStyles.helpBox);

                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                
                GUILayout.BeginVertical(EditorStyles.helpBox);
                
                
               /*
                GUIContent[] toolbar = new GUIContent[] {                    
                    new GUIContent("Textures"),
                    new GUIContent("Material")                    
                };
                */
                //tabID = GUILayout.Toolbar(tabID, toolbar);


                if (tabID == 1)
                {
                    DrawMaterial();
                }
                else
                {
                    //GUILayout.Space(20);
                    

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();

                    /*
                    bool oldHDRPButton = m_bakeAlphaMap;
                    m_bakeAlphaMap = EditorGUILayout.Toggle("HDRP", m_bakeAlphaMap);
                    if (oldHDRPButton != m_bakeAlphaMap)
                        m_needRefresh = true;
                    */

                    m_bakeAlphaMap = HairDesignerEditor.GetRenderPipeline() == HairDesignerEditor.eRP.HDRP;


                    m_display = (eTextureID)EditorGUILayout.EnumPopup("Display", m_display);
                    m_atlasParameters.editorBgLight = EditorGUILayout.Slider("Background color", m_atlasParameters.editorBgLight, 0f, 1f);
                    GUILayout.EndVertical();
                    GUILayout.Space(20);
                    GUILayout.BeginVertical();
                    m_autoRefresh = GUILayout.Toggle(m_autoRefresh, "Auto Refresh");
                    //if (!m_autoRefresh)
                    {
                        GUI.backgroundColor = m_needRefresh?Color.yellow:Color.white;
                        if (GUILayout.Button("Refresh"))
                        {
                            m_needRefresh = false;
                            
                            BakeTexture(textureSize < 512 ? textureSize : 512);
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();

                    GUILayout.Space(20);

                    GUI.backgroundColor = Color.Lerp(Color.black, Color.white, m_atlasParameters.editorBgLight);

                    if (m_display == eTextureID.ALL)
                    {
                        if (m_specularMode != eSpecularMode.COMBINED && m_autoRefresh )
                        {
                            m_specularMode = eSpecularMode.COMBINED;
                            BakeTexture(textureSize < 512 ? textureSize : 512);
                        }
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (m_atlasParameters.m_enableDiffuse)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Label(new GUIContent(m_diffuse), GUILayout.Width(textureSize < 256 ? textureSize : 256), GUILayout.Height(textureSize < 256 ? textureSize : 256));
                            GUILayout.EndVertical();
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (m_atlasParameters.m_enableNormal)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Label(new GUIContent(m_normal), GUILayout.Width(textureSize < 120 ? textureSize : 120), GUILayout.Height(120));
                            GUILayout.EndVertical();
                        }

                        if (m_atlasParameters.m_enableAO)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Label(new GUIContent(m_AO), GUILayout.Width(textureSize < 120 ? textureSize : 120), GUILayout.Height(textureSize < 120 ? textureSize : 120));
                            GUILayout.EndVertical();
                        }

                        if (m_atlasParameters.m_enableSpecular)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Label(new GUIContent(m_Specular), GUILayout.Width(textureSize < 120 ? textureSize : 120), GUILayout.Height(textureSize < 120 ? textureSize : 120));
                            GUILayout.EndVertical();
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        int size = 350;
                        switch (m_display)
                        {                            
                            case eTextureID.DIFFUSE:
                                GUILayout.Label(new GUIContent(m_diffuse), GUILayout.Width(textureSize < size ? textureSize : size), GUILayout.Height(textureSize < size ? textureSize : size));
                                break;
                            case eTextureID.NORMAL:
                                GUILayout.Label(new GUIContent(m_normal), GUILayout.Width(textureSize < size ? textureSize : size), GUILayout.Height(textureSize < size ? textureSize : size));
                                break;
                            case eTextureID.AO:
                                GUILayout.Label(new GUIContent(m_AO), GUILayout.Width(textureSize < size ? textureSize : size), GUILayout.Height(textureSize < size ? textureSize : size));
                                break;
                            case eTextureID.SPECULAR_ANISO:
                                GUILayout.Label(new GUIContent(m_Specular), GUILayout.Width(textureSize < size ? textureSize : size), GUILayout.Height(textureSize < size ? textureSize : size));
                                GUI.backgroundColor = Color.white;
                                EditorGUI.BeginChangeCheck();
                                m_specularMode = (eSpecularMode)EditorGUILayout.EnumPopup("Specular mode", m_specularMode);
                                
                                if (EditorGUI.EndChangeCheck())
                                    BakeTexture(textureSize < 512 ? textureSize : 512);
                                   
                                break;

                                
                        }
                        GUILayout.EndVertical();                        
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        
                    }

                    
                    GUI.backgroundColor = Color.white;

                    
                }


                

                GUILayout.EndVertical();


                GUIContent[] toolbarLabels = new GUIContent[] {
                    new GUIContent("Global"),
                    new GUIContent("Shape"),
                    new GUIContent("Diffuse / AO"),
                    new GUIContent("Specular/Aniso")
                };


                
                GUILayout.BeginVertical( EditorStyles.helpBox );
                tabParametersID = GUILayout.Toolbar(tabParametersID, toolbarLabels);
                EditorGUI.BeginChangeCheck();
                switch (tabParametersID)
                {
                    case 0:
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Strand parameters", EditorStyles.boldLabel);
                        m_parameters.m_hairCount = EditorGUILayout.IntSlider("Hair count", m_parameters.m_hairCount, 1, 2000);
                        m_parameters.m_rnd = EditorGUILayout.Slider("Random", m_parameters.m_rnd, 0f, 1f);
                        EditorGUILayout.MinMaxSlider(new GUIContent("root Offset"), ref m_parameters.m_rootOffset.x, ref m_parameters.m_rootOffset.y, 0f, 1f);
                        EditorGUILayout.MinMaxSlider(new GUIContent("Random length"), ref m_parameters.m_length.x, ref m_parameters.m_length.y, 0f, 1f);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Scale", EditorStyles.boldLabel);
                        m_parameters.m_scale.x = EditorGUILayout.Slider("Scale Y", m_parameters.m_scale.x, 0f, 1f);
                        m_parameters.m_scale.y = EditorGUILayout.Slider("Scale X root", m_parameters.m_scale.y, 0f, 1f);
                        m_parameters.m_scale.z = EditorGUILayout.Slider("Scale X tip", m_parameters.m_scale.z, 0f, 1f);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Hair width", EditorStyles.boldLabel);                        
                        m_parameters.m_width.x = EditorGUILayout.Slider("Width tip", m_parameters.m_width.x, 0f, 1f);
                        m_parameters.m_width.y = EditorGUILayout.Slider("Width root", m_parameters.m_width.y, 0f, 1f);
                        m_parameters.m_rootThickness = EditorGUILayout.Slider("root Thickness", m_parameters.m_rootThickness, 0, 1);
                        GUILayout.EndVertical();

                        break;

                    case 1:
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Waves", EditorStyles.boldLabel);

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        m_parameters.m_wave0.y = EditorGUILayout.Slider("Strand amplitude", m_parameters.m_wave0.y*10f, 0f, 1f)/10f;
                        m_parameters.m_wave0.x = EditorGUILayout.Slider("Strand period", m_parameters.m_wave0.x, 0f, 20f);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(EditorStyles.helpBox);                        
                        m_parameters.m_wave1.z = EditorGUILayout.Slider("Hair 1 amplitude", m_parameters.m_wave1.z, 0f, 1f);
                        m_parameters.m_wave1.w = EditorGUILayout.Slider("Hair 1 period", m_parameters.m_wave1.w, 0f, 100f);
                        EditorGUILayout.MinMaxSlider(new GUIContent("Hair 1 min max"), ref m_parameters.m_wave1.x, ref m_parameters.m_wave1.y, 0f, 1f);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        m_parameters.m_wave2.z = EditorGUILayout.Slider("Hair 2 amplitude", m_parameters.m_wave2.z, 0f, 1f);
                        m_parameters.m_wave2.w = EditorGUILayout.Slider("Hair 2 period", m_parameters.m_wave2.w, 0f, 100f);
                        EditorGUILayout.MinMaxSlider(new GUIContent("Hair 2 min max"), ref m_parameters.m_wave2.x, ref m_parameters.m_wave2.y, 0f, 1f);
                        GUILayout.EndVertical();

                        GUILayout.EndVertical();
                        //m_parameters.m_test = EditorGUILayout.Slider("test", m_parameters.m_test, 0f, 1f);

                        break;

                    case 2:
                        EditorGUI.BeginChangeCheck();

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Noise", EditorStyles.boldLabel);
                        m_parameters.m_noise.x = EditorGUILayout.Slider("Noise scale X", m_parameters.m_noise.x / 1000f, 0, 1f) * 1000f;
                        m_parameters.m_noise.y = EditorGUILayout.Slider("Noise scale Y", m_parameters.m_noise.y / 10f, 0, 1f) * 10f;
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Color", EditorStyles.boldLabel);
                                               
                        m_parameters.m_nbColors = EditorGUILayout.IntSlider("Color count", m_parameters.m_nbColors, 1, 3);

                        if (m_parameters.m_colors == null)
                            m_parameters.m_colors = new List<TextureToolColor>();

                        while (m_parameters.m_colors.Count < m_parameters.m_nbColors)
                            m_parameters.m_colors.Add(new TextureToolColor() );

                        for (int i = 0; i < m_parameters.m_nbColors; ++i)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Root / Tip");                         
                            m_parameters.m_colors[i].root = EditorGUILayout.ColorField( "", m_parameters.m_colors[i].root, GUILayout.MaxWidth(100f));
                            m_parameters.m_colors[i].tip = EditorGUILayout.ColorField(  "", m_parameters.m_colors[i].tip, GUILayout.MaxWidth(100f));
                            GUILayout.EndHorizontal();
                            m_parameters.m_colors[i].bias = EditorGUILayout.Slider("Bias", m_parameters.m_colors[i].bias, 0f, 1f);
                            GUILayout.EndVertical();
                            
                        }

                        m_parameters.m_noise.z = EditorGUILayout.Slider("Noise diffuse", m_parameters.m_noise.z, 0, 1);
                        m_parameters.m_depth.x = EditorGUILayout.Slider("Depth diffuse", m_parameters.m_depth.x, 0, 1);
                        GUILayout.EndVertical();

                        if (EditorGUI.EndChangeCheck())
                            m_display = m_display!= eTextureID.ALL ? eTextureID.DIFFUSE : eTextureID.ALL;



                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("AO", EditorStyles.boldLabel);
                        m_parameters.m_ao = EditorGUILayout.Slider("AO", m_parameters.m_ao, 0, 1);
                        m_parameters.m_noise.w = EditorGUILayout.Slider("Noise AO", m_parameters.m_noise.w, 0, 1);
                        m_parameters.m_depth.y = EditorGUILayout.Slider("Depth AO", m_parameters.m_depth.y, 0, 1);
                        if (EditorGUI.EndChangeCheck())
                            m_display = m_display != eTextureID.ALL ? eTextureID.AO : eTextureID.ALL;
                        GUILayout.EndVertical();
                        break;


                    case 3:
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Specular", EditorStyles.boldLabel);
                        m_parameters.m_specularRoot = EditorGUILayout.Slider("Specular root", m_parameters.m_specularRoot, 0, 1);
                        m_parameters.m_specularTip = EditorGUILayout.Slider("Specular tip", m_parameters.m_specularTip, 0, 1);
                        GUILayout.EndVertical();
                        if (EditorGUI.EndChangeCheck())
                        {
                            m_display = m_display != eTextureID.ALL ? eTextureID.SPECULAR_ANISO : eTextureID.ALL;                            
                            m_specularMode = eSpecularMode.SPECULAR;
                        }

                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Aniso shift (Specular 1 & 2)", EditorStyles.boldLabel);
                        m_parameters.m_specularShift.x = EditorGUILayout.Slider("Noise X", m_parameters.m_specularShift.x/ 100f, 0, 1f) * 100f;
                        m_parameters.m_specularShift.y = EditorGUILayout.Slider("Noise Y", m_parameters.m_specularShift.y/10f, 0, 1f)*10f;
                        m_parameters.m_specularShift.z = EditorGUILayout.Slider("Contrast", m_parameters.m_specularShift.z, 0, 1f);
                        GUILayout.EndVertical();
                        if (EditorGUI.EndChangeCheck())
                        {
                            m_display = m_display != eTextureID.ALL ? eTextureID.SPECULAR_ANISO : eTextureID.ALL;
                            m_specularMode = eSpecularMode.ANISO_SHIFT;
                        }
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Aniso mask (Specular 2)", EditorStyles.boldLabel);
                        m_parameters.m_specularMask.x = EditorGUILayout.Slider("Noise X", m_parameters.m_specularMask.x / 400f, 0, 1f) * 400f;
                        m_parameters.m_specularMask.y = EditorGUILayout.Slider("Noise Y", m_parameters.m_specularMask.y / 10f, 0, 1f) * 10f;
                        m_parameters.m_specularMask.z = EditorGUILayout.Slider("Contrast", m_parameters.m_specularMask.z , 0, 1f);
                        GUILayout.EndVertical();
                        if (EditorGUI.EndChangeCheck())
                        {
                            m_display = m_display != eTextureID.ALL ? eTextureID.SPECULAR_ANISO : eTextureID.ALL;
                            m_specularMode = eSpecularMode.ANISO_MASK;
                        }
                        break;


                }


                if( EditorGUI.EndChangeCheck())
                {
                    m_needRefresh = true;
                }







                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy texture " + (m_currentTextureId + 1) + " parameters to atlas", GUILayout.Height(30)))
                {
                    int choice = EditorUtility.DisplayDialogComplex("Copy texture parameters", "all the other texture parameters will be overwrite", "Copy", "Copy and randomize", "Cancel");

                    if (choice != 2)
                    {

                        for (int i = 0; i < m_atlasParameters.m_shaderParameters.Count; ++i)
                        {
                            if (i != m_currentTextureId)
                            {
                                m_atlasParameters.m_shaderParameters[i] = m_atlasParameters.m_shaderParameters[m_currentTextureId].Copy();
                                if (choice == 1)
                                    m_atlasParameters.m_shaderParameters[i].m_rnd = Random.value * 100f;
                            }

                        }
                    }
                }


                GUILayout.EndVertical();

                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                /*
                if (GUILayout.Button("Bake", GUILayout.Height(30)))
                    BakeTexture(textureSize < 512 ? textureSize : 512);
                */



                if (m_needRefresh)
                {
                    if (m_autoRefresh)
                    {
                        BakeTexture(textureSize < 512 ? textureSize : 512);
                        m_needRefresh = false;
                    }
                }


                GUILayout.FlexibleSpace();
                if (GUILayout.Button("  Save & apply  ", GUILayout.Height(30)))
                {
                    m_saveBug = false;
                    m_saveBugCount = 0;
                    SaveTexture();                    
                }


                if (m_saveBug && m_saveBugCount < 10 )
                    SaveTexture();

                GUILayout.EndHorizontal();

                GUILayout.EndScrollView();

            }



            void UpdateShaderParams()
            {
                m_parameters = m_atlasParameters.m_shaderParameters[m_currentTextureId];

                /*
                if (m_pb == null)
                    m_pb = new MaterialPropertyBlock();

                
                m_pb.SetVector("_scale", m_parameters.m_scale);
                m_pb.SetFloat("_hairCount", m_parameters.m_hairCount);

                m_pb.SetVectorArray("_colorsRoot", m_parameters.GetColorArray(true));
                m_pb.SetVectorArray("_colorsTip", m_parameters.GetColorArray(false));
                m_pb.SetFloatArray("_colorsBias", m_parameters.GetColorBias());
                m_pb.SetFloat("_nbColors", m_parameters.m_nbColors);

                m_pb.SetFloat("_rnd", m_parameters.m_rnd);
                m_pb.SetVector("_width", m_parameters.m_width);
                m_pb.SetVector("_length", m_parameters.m_length);
                m_pb.SetVector("_wave0", m_parameters.m_wave0);
                m_pb.SetVector("_wave1", m_parameters.m_wave1);
                m_pb.SetVector("_wave2", m_parameters.m_wave2);
                m_pb.SetVector("_rootOffset", m_parameters.m_rootOffset);
                m_pb.SetFloat("_rootThickness", m_parameters.m_rootThickness);
                m_pb.SetVector("_noise", m_parameters.m_noise);
                m_pb.SetVector("_depth", m_parameters.m_depth);
                m_pb.SetFloat("_ao", m_parameters.m_ao);
                m_pb.SetFloat("_specularRoot", m_parameters.m_specularRoot);
                m_pb.SetFloat("_specularTip", m_parameters.m_specularTip);
                m_pb.SetVector("_specularShift", m_parameters.m_specularShift);
                m_pb.SetVector("_specularMask", m_parameters.m_specularMask);

                m_pb.SetFloat("_test", m_parameters.m_test);
                */


                mat.SetVector("_scale", m_parameters.m_scale);
                mat.SetFloat("_hairCount", m_parameters.m_hairCount);

                mat.SetVectorArray("_colorsRoot", m_parameters.GetColorArray(true));
                mat.SetVectorArray("_colorsTip", m_parameters.GetColorArray(false));
                mat.SetFloatArray("_colorsBias", m_parameters.GetColorBias());
                mat.SetFloat("_nbColors", m_parameters.m_nbColors);

                mat.SetFloat("_rnd", m_parameters.m_rnd);
                mat.SetVector("_width", m_parameters.m_width);
                mat.SetVector("_length", m_parameters.m_length);
                mat.SetVector("_wave0", m_parameters.m_wave0);
                mat.SetVector("_wave1", m_parameters.m_wave1);
                mat.SetVector("_wave2", m_parameters.m_wave2);
                mat.SetVector("_rootOffset", m_parameters.m_rootOffset);
                mat.SetFloat("_rootThickness", m_parameters.m_rootThickness);
                mat.SetVector("_noise", m_parameters.m_noise);
                mat.SetVector("_depth", m_parameters.m_depth);
                mat.SetFloat("_ao", m_parameters.m_ao);
                mat.SetFloat("_specularRoot", m_parameters.m_specularRoot);
                mat.SetFloat("_specularTip", m_parameters.m_specularTip);
                mat.SetVector("_specularShift", m_parameters.m_specularShift);
                mat.SetVector("_specularMask", m_parameters.m_specularMask);

                mat.SetFloat("_test", m_parameters.m_test);
            }



            public void ApplyAlphaMap( ref Texture2D tex, ref Texture2D alpha )
            {
                for (int x = 0; x < alpha.width; ++x)
                {
                    for (int y = 0; y < alpha.height; ++y)
                    {
                        Color a = alpha.GetPixel(x, y);
                        Color d = tex.GetPixel(x, y);
                        d.a = a.r;
                        tex.SetPixel(x, y, d);
                    }
                }
            }


            public void ApplyAlphaContrast( ref Texture2D alpha)
            {
                float max = 0;
                float min = 1;
                for (int x = 0; x < alpha.width; ++x)
                {
                    for (int y = 0; y < alpha.height; ++y)
                    {
                        Color a = alpha.GetPixel(x, y);
                        if( a.r> max )
                            max = a.r;
                        if (a.r < min)
                            min = a.r;
                    }
                }

                for (int x = 0; x < alpha.width; ++x)
                {
                    for (int y = 0; y < alpha.height; ++y)
                    {
                        Color a = alpha.GetPixel(x, y);
                        float f = 1f / (max-min);
                        a.r = (a.r - min) * f;
                        a.g = (a.g - min) * f;
                        a.b = (a.b - min) * f;
                        a.a = (a.a - min) * f;
                        alpha.SetPixel(x, y, a );
                    }
                }
            }



            public void BakeTexture(int size)
            {

                

                //PreviewRenderUtility preview = new PreviewRenderUtility();
                //Camera renderCam = preview.camera;

                
                
                
                //GameObject go = new GameObject("tmp");
                //Camera renderCam = go.AddComponent<Camera>();
                
                RenderTexture oldActive = RenderTexture.active;



                
                RenderTexture rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);             
                RenderTexture.active = rt;

                /*
                renderCam.orthographic = true;
                renderCam.orthographicSize = 5;
                renderCam.targetTexture = rt;
                renderCam.cullingMask = 1 << 30;
                renderCam.targetTexture = rt;
                renderCam.depthTextureMode = DepthTextureMode.None;

                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.hideFlags = HideFlags.HideAndDontSave;
                plane.transform.parent = renderCam.transform;
                plane.transform.position = Vector3.forward * 10f;
                plane.transform.rotation = Quaternion.Euler(0, 0, 180) * Quaternion.Euler(-90, 0, 0);
                plane.layer = 30;
                //MaterialPropertyBlock pb = new MaterialPropertyBlock();        

                MeshRenderer mr = plane.GetComponent<MeshRenderer>();
                */


                //Material mat;
                /*
                if(HairDesignerEditor.GetRenderPipeline() == HairDesignerEditor.eRP.HDRP)
                    mat = new Material(Shader.Find("Hidden/HairDesigner_TextureGenerator_HDRP"));
                else
                */
                    mat = new Material(Shader.Find("Hidden/HairDesigner_TextureGenerator"));


                UpdateShaderParams();

                //mat = new Material(Shader.Find("Test/TestUnlit"));
                //mr.sharedMaterial = mat;

                /*
                CustomRenderTexture cRT = m_cRT;

                if(cRT == null)
                    cRT = new CustomRenderTexture(size, size, RenderTextureFormat.ARGB32);
                //cRT.material = mat;
                */

                /*
                if (!m_bakeAlphaMap)
                {
                    renderCam.backgroundColor = new Color(0, 0, 0, 0);
                    renderCam.clearFlags = CameraClearFlags.Color;
                }
                else
                {
                    renderCam.backgroundColor = new Color(0, 0, 0, 0);
                    renderCam.clearFlags = CameraClearFlags.SolidColor;
                }
                */

                //Bake ALPHA MAP
                if(m_bakeAlphaMap)
                {
                    //m_pb.SetFloat("_textureID", -1f);
                    mat.SetFloat("_textureID", -1f);

                    //mr.SetPropertyBlock(m_pb);
                    //renderCam.Render();
                    //preview.Render(true);                    
                    //cRT.Update();

                    

                    if (m_alpha == null || m_alpha.width != rt.width)
                        m_alpha = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, true);

                    //RenderQuad(ref mat);
                    Graphics.Blit(null, rt, mat);
                    m_alpha.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

                    //ApplyAlphaContrast(ref m_alpha);

                    m_alpha.Apply();
                }

                if (m_atlasParameters.m_enableDiffuse)
                {
                    //m_pb.SetFloat("_textureID", (float)eTextureID.DIFFUSE);
                    mat.SetFloat("_textureID", (float)eTextureID.DIFFUSE);

                    //m_pb.SetFloat("_textureID", -1);
                    //mat.SetFloat("_textureID", -1);
                    //mr.SetPropertyBlock(m_pb);

                    

                    //renderCam.Render();

                    /*
                    cRT.updateMode = CustomRenderTextureUpdateMode.OnDemand;
                    cRT.initializationMode = CustomRenderTextureUpdateMode.OnDemand;
                    cRT.initializationColor = Color.blue;
                    cRT.initializationSource = CustomRenderTextureInitializationSource.TextureAndColor;
                    cRT.Initialize();
                    //cRT.Update();
                    */

                    if (m_diffuse == null || m_diffuse.width != rt.width)
                        m_diffuse = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true);

                    //RenderTexture renderTexture = RenderTexture.GetTemporary(m_diffuse.width, m_diffuse.width);
                    
                    Graphics.Blit(null, rt, mat);


                    /*
                    RenderQuad(ref mat);
                    renderCam.Render();
                    RenderQuad(ref mat);
                    */
                    //RenderTexture.active = cRT;
                    m_diffuse.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                    
                        /*
                    if (m_bakeAlphaMap)
                        ApplyAlphaMap(ref m_diffuse, ref m_alpha);
                        */
                    
                    m_diffuse.Apply();
                }

                if (m_atlasParameters.m_enableNormal)
                {
                    mat.SetFloat("_textureID", (float)eTextureID.NORMAL);
                    //mr.SetPropertyBlock(m_pb);
                    //renderCam.Render();

                    if (m_normal == null || m_normal.width != rt.width)
                        m_normal = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true);

                    Graphics.Blit(null, rt, mat);
                    m_normal.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                    m_normal.Apply();
                }

                if (m_atlasParameters.m_enableAO)
                {
                    mat.SetFloat("_textureID", (float)eTextureID.AO);
                    //mr.SetPropertyBlock(m_pb);
                    //renderCam.Render();

                    if (m_AO == null || m_AO.width != rt.width)
                        m_AO = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true);

                    Graphics.Blit(null, rt, mat);
                    m_AO.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

                    if (m_bakeAlphaMap)
                        ApplyAlphaMap(ref m_AO, ref m_alpha);
                    m_AO.Apply();
                }


                if (m_atlasParameters.m_enableSpecular)
                {
                    mat.SetFloat("_textureID", (float)eTextureID.SPECULAR_ANISO);
                    mat.SetFloat("_specularMode", (float)m_specularMode);
                    //mr.SetPropertyBlock(m_pb);
                    //renderCam.Render();
                    if (m_Specular == null || m_Specular.width != rt.width)
                        m_Specular = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true);

                    Graphics.Blit(null, rt, mat);
                    m_Specular.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

                    /*
                    if (m_bakeAlphaMap)
                        ApplyAlphaMap(ref m_Specular, ref m_alpha);
                        */
                    m_Specular.Apply();
                }

                
                RenderTexture.active = oldActive;
                //DestroyImmediate(plane);
                //DestroyImmediate(go);


                /*
                if(m_cRT == null)
                    cRT.Release();
                    */
            }





            bool m_saveBug = false;
            int m_saveBugCount = 0;
            string m_saveBugPath = "";

            public void SaveTexture()
            {
                if (m_diffuse == null)
                    return;

                m_specularMode = eSpecularMode.COMBINED;

                if (m_atlasParameters.filename == "")
                    m_atlasParameters.filename = "HairTexture";

                //string filename = "TextureGenerated";
                /*
                string path = EditorUtility.SaveFilePanel(                
                "Save texture as PNG",
                Application.dataPath + m_atlasParameters.path,
                m_atlasParameters.filename + ".png",
                "png");
                */
                if (m_atlasParameters.path == null)
                    m_atlasParameters.path = "";


                string[] pathSplit = m_atlasParameters.path.Split('/');
                string path = "";
                for (int i = 1; i < pathSplit.Length - 1; ++i)
                    path += "/" + pathSplit[i];
                path = Application.dataPath + path;

                if (!m_saveBug)
                {
                    path = EditorUtility.SaveFilePanelInProject(
                        "Save texture as PNG",
                        m_atlasParameters.filename + ".png",
                        "png", "msg", path);

                    m_saveBugPath = path;
                }
                else
                {
                    path = m_saveBugPath;
                }


                if (path.Length == 0)
                {
                    m_saveBug = false;
                    m_saveBugCount = 0;
                    return;
                }


                m_atlasParameters.filename = path.Split('/')[path.Split('/').Length-1]
                    .Replace(".png", "")
                    .Replace("_diffuse", "")
                    .Replace("_normal", "")
                    .Replace("_AO", "")
                    + "_diffuse";
                m_atlasParameters.path = path.Replace(Application.dataPath, "");

                Texture2D diffuse = new Texture2D(m_atlasParameters.textureSize, m_atlasParameters.textureSize);
                Texture2D normal = new Texture2D(m_atlasParameters.textureSize, m_atlasParameters.textureSize);
                Texture2D AO = new Texture2D(m_atlasParameters.textureSize, m_atlasParameters.textureSize);
                Texture2D Specular = new Texture2D(m_atlasParameters.textureSize, m_atlasParameters.textureSize);
                UpdateMatTextures();//assign textures -> avoid delete

                int textureSize = Mathf.FloorToInt((float)m_atlasParameters.textureSize / (float)m_atlasParameters.sizeX);

                for (int i = 0; i < m_atlasParameters.sizeX; ++i)
                {
                    for (int j = 0; j < m_atlasParameters.sizeY; ++j)
                    {
                        EditorUtility.DisplayProgressBar("Bake textures", "diffuse / normal / AO", (float)(i * m_atlasParameters.sizeY + j) / (float)(m_atlasParameters.sizeX * m_atlasParameters.sizeY));

                        m_currentTextureId = i * m_atlasParameters.sizeY + j;

                        BakeTexture(textureSize);

                        if (m_atlasParameters.m_enableDiffuse)
                            diffuse.SetPixels32(j * textureSize, (m_atlasParameters.sizeX - 1 - i) * textureSize, textureSize, textureSize, m_diffuse.GetPixels32());

                        if (m_atlasParameters.m_enableNormal)
                            normal.SetPixels32(j * textureSize, (m_atlasParameters.sizeX - 1 - i) * textureSize, textureSize, textureSize, m_normal.GetPixels32());

                        if (m_atlasParameters.m_enableAO)
                            AO.SetPixels32(j * textureSize, (m_atlasParameters.sizeX - 1 - i) * textureSize, textureSize, textureSize, m_AO.GetPixels32());

                        if (m_atlasParameters.m_enableSpecular)
                            Specular.SetPixels32(j * textureSize, (m_atlasParameters.sizeX - 1 - i) * textureSize, textureSize, textureSize, m_Specular.GetPixels32());

                    }
                }


                //Fix transparency        
                //if((float)textureSize < (float)m_atlasParameters.textureSize/ (float)m_atlasParameters.sizeX )
                {
                    Color32[] t = new Color32[1];
                    t[0] = new Color(0, 0, 0, 0);

                    int pixelOffset = m_atlasParameters.textureSize - textureSize * m_atlasParameters.sizeX;

                    for (int j = 0; j < pixelOffset; ++j)
                    {
                        for (int i = 0; i < m_atlasParameters.textureSize; ++i)
                        {
                            diffuse.SetPixels32(i, m_atlasParameters.textureSize - 1 - j, 1, 1, t);
                            diffuse.SetPixels32(m_atlasParameters.textureSize - 1 - j, i, 1, 1, t);
                        }
                    }

                }
                diffuse.Apply();
                normal.Apply();
                AO.Apply();
                Specular.Apply();

                // save to file

                path = path.Replace("_diffuse","").Replace("_normal", "").Replace("_AO", "");

                TextureImporter importer_diffuse = null;
                string diffusePath = path.Replace(".png", "_diffuse.png").Replace(Application.dataPath, "");
                if (m_atlasParameters.m_enableDiffuse)
                {
                    EditorUtility.DisplayProgressBar("Save textures", "diffuse", 0f);
                    byte[] bytes = diffuse.EncodeToPNG();
                    System.IO.File.WriteAllBytes(path.Replace(".png", "_diffuse.png"), bytes);                    
                    AssetDatabase.Refresh();
                    importer_diffuse = AssetImporter.GetAtPath(diffusePath) as TextureImporter;
                    importer_diffuse.wrapMode = TextureWrapMode.Clamp;
                    
                }


                TextureImporter importer_normal = null;
                string normalPath = path.Replace(".png", "_normal.png").Replace(Application.dataPath, "");
                if (m_atlasParameters.m_enableNormal)
                {
                    if (normal == null)
                    {
                        //Debug.Log("Error");
                        m_saveBug = true;
                        m_saveBugCount++;
                        return;
                    }
                    EditorUtility.DisplayProgressBar("Save textures", "normal", .25f);
                    byte[] bytes = normal.EncodeToPNG();
                    System.IO.File.WriteAllBytes(path.Replace(".png", "_normal.png"), bytes);                    
                    AssetDatabase.Refresh();
                    importer_normal = AssetImporter.GetAtPath(normalPath) as TextureImporter;
#if UNITY_5_5_OR_NEWER
                    importer_normal.textureType = TextureImporterType.NormalMap;
#else
                    importer_normal.normalmap = true;
#endif
                    importer_normal.wrapMode = TextureWrapMode.Clamp;
                    
                }

                TextureImporter importer_ao = null;
                string AOPath = path.Replace(".png", "_AO.png").Replace(Application.dataPath, "");
                if (m_atlasParameters.m_enableAO)
                {
                    EditorUtility.DisplayProgressBar("Save textures", "AO", 0.5f);
                    byte[] bytes = AO.EncodeToPNG();
                    System.IO.File.WriteAllBytes(path.Replace(".png", "_AO.png"), bytes);                    
                    AssetDatabase.Refresh();
                    importer_ao = AssetImporter.GetAtPath(AOPath) as TextureImporter;
                    importer_ao.wrapMode = TextureWrapMode.Clamp;                    
                }


                TextureImporter importer_specular = null;
                string specularPath = path.Replace(".png", "_Specular.png").Replace(Application.dataPath, "");
                if (m_atlasParameters.m_enableSpecular)
                {
                    EditorUtility.DisplayProgressBar("Save textures", "Specular", 0.75f);
                    byte[] bytes = Specular.EncodeToPNG();
                    System.IO.File.WriteAllBytes(path.Replace(".png", "_Specular.png"), bytes);
                    AssetDatabase.Refresh();
                    importer_specular = AssetImporter.GetAtPath(specularPath) as TextureImporter;
                    importer_specular.wrapMode = TextureWrapMode.Clamp;                    
                }


                EditorUtility.DisplayProgressBar("Save textures", "Reimport", 1f);

                if (importer_diffuse!= null) importer_diffuse.SaveAndReimport();
                if (importer_normal != null) importer_normal.SaveAndReimport();
                if (importer_ao != null) importer_ao.SaveAndReimport();
                if (importer_specular != null) importer_specular.SaveAndReimport();

                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                
                
                
                              

                if (TextureGeneratedCB != null)
                {
                    TextureGeneratedCBData d = new TextureGeneratedCBData();
                    
                    d.diffuse = AssetDatabase.LoadAssetAtPath(diffusePath, typeof(Texture2D)) as Texture2D;
                    d.normal = AssetDatabase.LoadAssetAtPath(normalPath, typeof(Texture2D)) as Texture2D;
                    d.AO = AssetDatabase.LoadAssetAtPath(AOPath, typeof(Texture2D)) as Texture2D;
                    d.specular = AssetDatabase.LoadAssetAtPath(specularPath, typeof(Texture2D)) as Texture2D;

                    TextureGeneratedCB(d);
                }

                System.GC.Collect();//clean memory
                m_saveBug = false;

            }


        }
    }
}
