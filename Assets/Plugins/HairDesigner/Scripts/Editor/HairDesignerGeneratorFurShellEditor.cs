using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [CustomEditor(typeof(HairDesignerGeneratorFurShellBase), true)]
        public class HairDesignerGeneratorFurShellBaseEditor : HairGeneratorEditor
        {
            public enum eTab
            {
                //INFO,
                DESIGN,
                MATERIAL,
                MOTION,
                ADVANCED,
                DEBUG = 100
            }

            public enum eTextureSlot
            {
                MAIN,
                DENSITY,
                MASK,
                BRUSH,
                COLOR
            }

            public static eTab m_tab = eTab.MATERIAL;
            ePaintingTool m_currentTool = ePaintingTool.ADD;
            ePaintingTool m_secondaryTool = ePaintingTool.BRUSH;
            //Vector3 m_lastBrushPos = Vector3.zero;
            Texture2D m_maskFill;
            Texture2D m_colorFill;
            bool m_invertTextureMaskref = false;
            bool m_hideShell = false;
            public bool m_initialized = false;

            static Camera m_lastSceneviewCam;


            //static bool m_maskReady = false;
            //static bool m_colorReady = false;            
            static bool m_lockMask = false;
            static Color m_toolColor = Color.black;
            static float m_colorReplaceMix = 1f;

            bool m_startPaint = false;
            bool m_showBrush = true;

            static Dictionary<Vector3, List<int>> m_verticesLink;

            static Vector3[] m_vertices;
            static Vector3[] m_normals;
            static Vector4[] m_tangents;
            static int[] m_triangles;
            static Vector2[] m_uvs;

            void InitLinkedVertices()
            {
                if (m_verticesLink != null || HairDesignerEditor.m_meshCollider == null )
                    return;

                m_verticesLink = new Dictionary<Vector3, List<int>>();
                //HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                m_vertices = HairDesignerEditor.m_meshCollider.sharedMesh.vertices;
                Vector3[] m_normals = HairDesignerEditor.m_meshCollider.sharedMesh.normals;
                Vector4[] m_tangents = HairDesignerEditor.m_meshCollider.sharedMesh.tangents;
                int[] m_triangles = HairDesignerEditor.m_meshCollider.sharedMesh.triangles;
                Vector2[] m_uvs = HairDesignerEditor.m_meshCollider.sharedMesh.uv;


                for (int i = 0; i < m_vertices.Length; ++i)
                {
                    if (!m_verticesLink.ContainsKey(m_vertices[i]))                    
                        m_verticesLink[m_vertices[i]] = new List<int>();

                    m_verticesLink[m_vertices[i]].Add(i);                                    
                }
                /*
                string title = "Vertices check";
                string info = "please wait";
                float progress = 0f;
                if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
                for (int i = 0; i < vertices.Length; ++i)
                {                    
                    for (int j = i + 1; j < vertices.Length; ++j)
                    {
                        if( Vector3.Distance( vertices[i], vertices[j])<.0001f  )
                        {
                            m_verticesLink[i].Add(j);
                            m_verticesLink[j].Add(i);
                        }
                    }
                    progress = (float)i / (float)vertices.Length;
                    if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                }
                EditorUtility.ClearProgressBar();

                Debug.Log("vertices ok");
                */
            }

            static HairDesignerGeneratorFurShellBaseEditor()
            {
                //Static function called on start
                AssignDelegate();                
            }

            static void AssignDelegate()
            {
//#if UNITY_2017_3_OR_NEWER
                EditorApplication.playModeStateChanged += HideAllShellsWireframe;
/*
#else
                EditorApplication.playmodeStateChanged += HideAllShellsWireframe;
#endif
*/
            }

//#if UNITY_2018_3_OR_NEWER
            static void HideAllShellsWireframe(PlayModeStateChange state)
            {
                HideAllShellsWireframe();
            }
//#endif


            static void HideAllShellsWireframe()

            {
                HairDesignerGeneratorFurShellBase[] furShellGenerators = FindObjectsOfType<HairDesignerGeneratorFurShellBase>();
                for(int i=0; i< furShellGenerators.Length;++i)
                {                    
                    HideShellsWireframe(furShellGenerators[i]);
                }
            }


            public static int m_selectedLOD = -1;



            public override void OnInspectorGUI()
            {
                hideWireframe = false;

                eTab lastTab = m_tab;
                base.OnInspectorGUI();

                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;

                if (g == null)
                    return;


#if UNITY_2018_3_OR_NEWER
                if (PrefabUtility.IsPartOfPrefabAsset(g))
#else
                if (PrefabUtility.GetPrefabParent(g.gameObject) == null && PrefabUtility.GetPrefabObject(g.gameObject) != null)
#endif
                {
                    //GUILayout.Label("Instantiate the prefab for modifications", EditorStyles.helpBox);
                    return;
                }

                if (g.m_hd == null)
                    return;

                if (!m_initialized)
                {
                    /*
                    if (m_verticesLink!=null)
                        m_verticesLink.Clear();
                    m_verticesLink = null;
                    */
                    //Debug.Log("initialize");

                    g.UpdateShells();

                    m_initialized = true;
                }
                

                GUIContent[] toolbar = new GUIContent[] {
                    //new GUIContent("Info",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.INFO))),
                    new GUIContent("Design",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.DESIGN))),
                    new GUIContent("Material",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.MATERIAL))),
                    new GUIContent("Motion",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.MOTION)))
                };

                m_tab = (eTab)Toolbar((int)m_tab, toolbar);

                if (m_tab != lastTab)
                {
                    GUI.FocusControl("TAB");

                }

                /*
                GUILayout.Label( g._vertexTex );
                GUILayout.Label(g._normalTex);
                EditorGUILayout.ObjectField(g._vertexTex, typeof(Texture2D), false);
                */

                //g.m_meshLocked = !g.m_editorData.m_unsavedColorTexFile && !g.m_editorData.m_unsavedMaskTexFile;
                g.m_meshLocked = true;

                if (m_tab == eTab.DESIGN)
                {
                    DisableSceneTool();
                    

                    if (m_hairDesignerEditor != null)                    
                        m_hairDesignerEditor.CreateColliderAndTpose(g.m_skinningVersion160);


                    //TPoseModeChoose(g.m_hd);

                    InitLinkedVertices();

                    g.m_requirePainting = true;
                    HairDesignerEditor.m_showSceneMenu = true;
                    hideWireframe = true;

                    GUI.enabled = !g.m_useLOD;
                    g.m_shellCount = EditorGUILayout.IntField("Shells count", g.m_shellCount);
                    g.m_shellCount = Mathf.Clamp(g.m_shellCount, 1, 500);
                    g.m_shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup("Casting mode", g.m_shadowCastingMode);
                    g.m_furWidthUpscale = EditorGUILayout.Slider("Fur width upscale", g.m_furWidthUpscale,0f,1f);


                    GUI.enabled = true;





                    //if (HairDesignerEditor.GetRenderPipeline() == HairDesignerEditor.eRP.STANDARD)
                    {
                        GUILayout.BeginHorizontal();
                        g.m_useLOD = EditorGUILayout.Toggle("LOD", g.m_useLOD);
                        if (g.m_useLOD)
                            g.m_selectCurrentLOD = EditorGUILayout.Toggle("Select current", g.m_selectCurrentLOD);
                        GUILayout.EndHorizontal();
                    }
                    /*
                    else
                    {
                        g.m_useLOD = false;
                    }*/



                    if (g.m_useLOD)
                    {
                        int currentLODCam = -1;
                        if(m_lastSceneviewCam!=null)
                            currentLODCam = g.GetLOD(m_lastSceneviewCam);


                        //GUILayout.Label("LOD groups", EditorStyles.miniBoldLabel);
                        //g.m_LODGroups = g.m_LODGroups.OrderBy(grp => grp.m_range.x).ToList<HairDesignerGeneratorFurShellBase.LODGroup>();

                        
                        
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);
                        for (int i = 0; i < g.m_LODGroups.Count; ++i)
                        {
                            if(g.m_selectCurrentLOD)
                                m_selectedLOD = currentLODCam;
                            //if( i== currentLODCam)
                            //    GUI.color = m_selectedLOD == i ? HairDesignerEditor.mainColor*.5f : HairDesignerEditor.mainColor;
                            //else
                                GUI.color = m_selectedLOD == i ? HairDesignerEditor.unselectionColor : Color.white;

                            if (GUILayout.Button( (i == currentLODCam ?"[":"") + 
                                g.m_LODGroups[i].m_range.x + " - " +
                                g.m_LODGroups[i].m_range.y + (i == currentLODCam ? "]" : "")
                                ,  EditorStyles.helpBox))
                            {
                                m_selectedLOD = m_selectedLOD != i ? i : -1;
                                g.m_selectCurrentLOD = false;
                                GUI.FocusControl("");
                            }
                        }
                        GUI.color = Color.white;
                        if (GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            HairDesignerGeneratorFurShellBase.LODGroup lodg = new HairDesignerGeneratorFurShellBase.LODGroup();
                            if (g.m_LODGroups.Count > 0)
                            {
                                lodg.m_range.x = g.m_LODGroups[g.m_LODGroups.Count - 1].m_range.y;
                                lodg.m_range.y = lodg.m_range.x + 100;
                                lodg.m_shadowCastingMode = g.m_LODGroups[g.m_LODGroups.Count - 1].m_shadowCastingMode;
                            }
                            else
                            {
                                lodg.m_shadowCastingMode = g.m_shadowCastingMode;
                            }
                            g.m_LODGroups.Add(lodg);
                            m_selectedLOD = g.m_LODGroups.Count - 1;
                            GUI.FocusControl("");
                        }

                        GUILayout.EndHorizontal();
                        GUI.color = Color.white;

                        if (m_selectedLOD != -1 && g.m_LODGroups.Count > m_selectedLOD)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            g.m_LODGroups[m_selectedLOD].m_range = EditorGUILayout.Vector2Field("Camera distance", g.m_LODGroups[m_selectedLOD].m_range);
                            g.m_LODGroups[m_selectedLOD].m_shellCount = EditorGUILayout.IntField("Shell count", g.m_LODGroups[m_selectedLOD].m_shellCount);
                            g.m_LODGroups[m_selectedLOD].m_shellCount = g.m_LODGroups[m_selectedLOD].m_shellCount < 0 ? 0 : g.m_LODGroups[m_selectedLOD].m_shellCount;
                            g.m_LODGroups[m_selectedLOD].m_shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup("Casting mode", g.m_LODGroups[m_selectedLOD].m_shadowCastingMode);
                            g.m_LODGroups[m_selectedLOD].m_maskLOD = EditorGUILayout.Slider("Fur width upscale", g.m_LODGroups[m_selectedLOD].m_maskLOD, 0f, 1f);
                            if (m_selectedLOD > 0)
                                g.m_LODGroups[m_selectedLOD - 1].m_range.y = g.m_LODGroups[m_selectedLOD].m_range.x;

                            if (m_selectedLOD < g.m_LODGroups.Count-1)
                                g.m_LODGroups[m_selectedLOD + 1].m_range.x = g.m_LODGroups[m_selectedLOD].m_range.y;

                            if (g.m_LODGroups[m_selectedLOD].m_range.y < g.m_LODGroups[m_selectedLOD].m_range.x)
                                g.m_LODGroups[m_selectedLOD].m_range.y = g.m_LODGroups[m_selectedLOD].m_range.x;

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Delete"))
                            {
                                g.m_LODGroups.RemoveAt(m_selectedLOD);
                                m_selectedLOD = -1;
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.EndVertical();
                        }


                        

                        /*
                        for (int i = 0; i < g.m_LODGroups.Count; ++i)
                        {                                                                                   
                            g.m_LODGroups[i].m_range = EditorGUILayout.Vector2Field("Camera distance", g.m_LODGroups[i].m_range);
                            g.m_LODGroups[i].m_shellCount = EditorGUILayout.IntField("Shell count", g.m_LODGroups[i].m_shellCount);
                            g.m_LODGroups[i].m_shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup("Casting mode", g.m_LODGroups[i].m_shadowCastingMode);
                            g.m_LODGroups[i].m_maskLOD = EditorGUILayout.Slider("Mask LOD", g.m_LODGroups[i].m_maskLOD,0f,1f);
                            if (i > 0)
                                g.m_LODGroups[i - 1].m_range.y = g.m_LODGroups[i].m_range.x;

                            if (g.m_LODGroups[i].m_range.y < g.m_LODGroups[i].m_range.x)
                                g.m_LODGroups[i].m_range.y = g.m_LODGroups[i].m_range.x;
                            
                        }
                        */
                        GUILayout.Space(5);                        
                    }



                   

                    eInstancingMode old = g.RuntimeInstancingMode;

                    g.RuntimeInstancingMode = (eInstancingMode)EditorGUILayout.EnumPopup("Instancing mode", g.RuntimeInstancingMode);
                    if (g.RuntimeInstancingMode != old)
                    {
                        g.ClearShells();
                        g.UpdateShells();
                    }
                    if (!g.GetShaderParams().InstancingModeCompatibility(g.RuntimeInstancingMode))
                        EditorGUILayout.HelpBox("Shader not compatible with " + g.RuntimeInstancingMode, MessageType.Error);

                    EditorGUILayout.HelpBox("Instancing mode :\nChoose 'undefine' to let the system choose the better option.\nMesh instancing options are faster", MessageType.Info);

                    if (g.RuntimeInstancingMode == eInstancingMode.MESH_INSTANCING_INDIRECT)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Label("Mesh Instancing Indirect options", EditorStyles.boldLabel);
                        g.m_MII_useRendererBoundingBox = EditorGUILayout.Toggle("Use character bounding box", g.m_MII_useRendererBoundingBox);

                        if (!g.m_MII_useRendererBoundingBox)
                        {
                            g.m_MII_boundingBoxCenter = EditorGUILayout.Vector3Field("Bounding box center", g.m_MII_boundingBoxCenter);
                            g.m_MII_boundingBoxExtents = EditorGUILayout.Vector3Field("Bounding box size", g.m_MII_boundingBoxExtents);
                        }
                        else
                        {
                            g.m_MII_boundingBoxScale = EditorGUILayout.FloatField("Bounding box scale", g.m_MII_boundingBoxScale);
                        }

                        g.m_MII_drawBoundingBox = EditorGUILayout.Toggle("Draw bounding box", g.m_MII_drawBoundingBox);
                        GUILayout.EndVertical();
                    }



                    /*
                    g.m_recalculateNormals = EditorGUILayout.Toggle("Recalculate normals", g.m_recalculateNormals);
                    g.m_recalculateBounds = EditorGUILayout.Toggle("Recalculate bounds", g.m_recalculateBounds);
                    */
                    if (g.m_hd.m_smr != null && HairDesignerEditor.GetRenderPipeline() == HairDesignerEditor.eRP.STANDARD )
                        g.m_gpuSkinning = EditorGUILayout.Toggle("GPU skinning (beta)", g.m_gpuSkinning);
                    else
                        g.m_gpuSkinning = false;

                    //g.m_enableDrawMeshInstanced = EditorGUILayout.Toggle("Draw mesh instancing", g.m_enableDrawMeshInstanced);
                    //HairDesignerShader s = g.GetShaderParams() as HairDesignerShader;

                    GUILayout.Space(5);
                    
                    if (GUI.changed)
                    {
                        g.UpdateShells();
                        HideShellsWireframe(g);
                        SceneView.RepaintAll();
                    }

                    
                }
                else
                {
                    m_selectedLOD = -1;
                }


                if (m_tab == eTab.MATERIAL)
                {
                    EnableSceneTool();

                    if(GUILayout.Button("Refresh Material"))
                    {
                        g.m_furMaterial = null;
                        g.m_furMaterialOpaque = null;
                        g.m_materialInitialized = false;
                        g.ClearShells();
                        g.UpdateShells();
                    }


                    if (m_hairDesignerEditor != null)
                        m_hairDesignerEditor.RestorePose();
                    g.m_requirePainting = false;
                    HairDesignerEditor.m_showSceneMenu = false;
                    hideWireframe = true;
                    m_verticesLink = null;

                    GUILayout.Label(new GUIContent("Materials slots", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);
                    for (int i = 0; i < g.m_materialsEnabled.Count; ++i)
                        g.m_materialsEnabled[i] = EditorGUILayout.Toggle("Material slot " + (i + 1), g.m_materialsEnabled[i]);

                    DrawMaterialSettings();

                    if (g.m_hairMeshMaterial != null)
                    {
                        //g.UpdatePropertyBlock(ref g.m_matPropBlkHair);
                        //g.m_shaderParams[0].UpdatePropertyBlock(ref g.m_matPropBlkHair, HairDesignerBase.eLayerType.FUR_SHELL);
                        HairDesignerShader sp = g.GetShaderParams();
                        if(sp!=null)
                            sp.UpdateMaterialProperty(ref g.m_hairMeshMaterial, HairDesignerBase.eLayerType.FUR_SHELL);
                    }
                    if (GUI.changed)
                    {
                        g.UpdateShells();
                        HideShellsWireframe(g);
                    }
                    
                    SceneView.RepaintAll();
                }





                if (m_tab == eTab.MOTION)
                {
                    EnableSceneTool();
                    HairDesignerEditor.m_showSceneMenu = false;
                    MotionZoneInspectorGUI();
                    m_verticesLink = null;
                }



                if (!m_hideShell)
                {
                    m_hideShell = true;
                    HideShellsWireframe(g);
                }



                

            }






            public static void HideShellsWireframe(HairDesignerGeneratorFurShellBase g)
            {
                for (int i = 0; i < g.m_shellCount; ++i)
                {
                    if (g.m_shells.Count > i)
#if UNITY_5_5_OR_NEWER
                        EditorUtility.SetSelectedRenderState(g.m_shells[i], EditorSelectedRenderState.Hidden);
#else
                        EditorUtility.SetSelectedWireframeHidden(g.m_shells[i], true);
#endif
                }
            }







            void MotionZoneInspectorGUI()
            {
                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;


                    for (int i = 0; i < g.m_motionZones.Count; ++i)
                    {
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(new GUIContent("Motion zones " + (i + 1), (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);
                        if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH), GUILayout.MaxWidth(25)))
                        {
                            if (EditorUtility.DisplayDialog("Motion zone", "Delete motion zone", "Ok", "Cancel"))
                            {
                                Undo.RecordObject(g, "Delete motion zone");
                                g.m_motionZones.RemoveAt(i--);
                                break;
                            }
                        }
                        GUILayout.EndHorizontal();

                        

                        GUILayout.Label("Position & hierachy", EditorStyles.boldLabel);
                        g.m_motionZones[i].parent = EditorGUILayout.ObjectField("Parent", g.m_motionZones[i].parent, typeof(Transform), true) as Transform;


                        Transform bone = SelectBone(g.m_motionZones[i].parent);
                        g.m_motionZones[i].parentname = bone == null ? "" : bone.name;

                        if (bone != null)
                        {
                            g.m_motionZones[i].parent = bone;
                            g.m_motionZones[i].localPosition = Vector3.zero;
                        }

                        g.m_motionZones[i].localPosition = EditorGUILayout.Vector3Field("Offset", g.m_motionZones[i].localPosition);
                        g.m_motionZones[i].radius = EditorGUILayout.FloatField("Radius", g.m_motionZones[i].radius);

                        GUILayout.Label("Motion parameters", EditorStyles.boldLabel);

                        g.m_motionZones[i].pid.m_params.kp = EditorGUILayout.FloatField("Damping", g.m_motionZones[i].pid.m_params.kp);
                        g.m_motionZones[i].pid.m_params.ki = EditorGUILayout.FloatField("Bouncing", g.m_motionZones[i].pid.m_params.ki);
                        g.m_motionZones[i].motionLimit.y = EditorGUILayout.FloatField("Limit", g.m_motionZones[i].motionLimit.y);
                        g.m_motionZones[i].motionLimit.y = Mathf.Clamp(g.m_motionZones[i].motionLimit.y, 0, g.m_motionZones[i].motionLimit.y);
                        g.m_motionZones[i].motionLimit.x = -g.m_motionZones[i].motionLimit.y;

                        g.m_motionZones[i].motionFactor = EditorGUILayout.FloatField("Motion factor", g.m_motionZones[i].motionFactor);

                        g.m_motionZones[i].pid.m_params.kd = 0;
                        GUILayout.Box(" ", GUILayout.Height(50f), GUILayout.ExpandWidth(true));
                        g.m_motionZones[i].pid.m_pidX.m_params = g.m_motionZones[i].pid.m_params;
                        HairDesignerPID pid = new HairDesignerPID(g.m_motionZones[i].pid.m_params.kp, g.m_motionZones[i].pid.m_params.ki, 0);
                        //pid.m_params = g.m_motionZones[i].pid.m_params;
                        //pid.m_params.limits = new Vector2(-10, 10);
                        HairDesignerEditor.GUIDrawPidResponse(pid, GUILayoutUtility.GetLastRect(), 5f);
                    }

                    if (GUILayout.Button("+ New motion Zone +", GUILayout.Height(40)))
                    {
                        Undo.RecordObject(g, "New motion zone");
                        MotionZone mz = new MotionZone();
                        mz.parent = g.m_hd.transform;
                        g.m_motionZones.Add(mz);
                    }

                    EditorGUILayout.HelpBox("The motion zone must encapsulate the hair strands to enable dynamic animation.\nLock the motion zone to the nearest bone for skeleton animation.", MessageType.Info);


                }






            bool TextureReady( HairDesignerShader s, ePaintingTool tool )
            {
                eTextureSlot slot = eTextureSlot.MAIN;
                Texture2D tex = null;

                switch(tool)
                {
                    case ePaintingTool.ADD: slot = eTextureSlot.MASK; break;
                    case ePaintingTool.BRUSH: slot = eTextureSlot.BRUSH; break;
                    case ePaintingTool.COLOR: slot = eTextureSlot.COLOR; break;
                }
                bool ready = true;

                if (m_currentTool == tool)
                {
                    tex = s.GetTexture((int)slot);

                    if (s != null && tex != null)
                    {
                        try
                        {
                            
                            tex.GetPixel(0, 0);
                            ready = true;
                        }
                        catch
                        {
                            EditorGUILayout.HelpBox("Texture flag Read/Write is not enabled\nplease create a new one", MessageType.Error);
                            ready = false;
                            if (GUILayout.Button("Fix"))
                            {
                                TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex)) as TextureImporter;
                                ImporterSettings(ref importer);
                                importer.SaveAndReimport();
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No texture assigned\nplease create a new one", MessageType.Error);
                        ready = false;

                    }                    


                    if (!ready)
                    {
                        //EditorGUILayout.HelpBox("Color texture is not ready for painting\nplease create a new one", MessageType.Error);
                        GUILayout.Space(5);
                        if (GUILayout.Button("Create new texture"))
                        {
                            CreateTexture(slot);
                        }
                        return false;
                    }


                    bool textureFormatReady = false;
                    switch (tex.format)
                    {
                        case TextureFormat.ARGB32:
                        case TextureFormat.RGBA32:
                            textureFormatReady = true;
                            break;
                    }

                    if(!textureFormatReady)
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Texture format must be\nARGB32 or RGBA32");
                        if (GUILayout.Button("Fix texture format"))
                        {
                            TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex)) as TextureImporter;
                            ImporterSettings(ref importer);
                            importer.SaveAndReimport();
                        }
                        if (GUILayout.Button("Select texture in project"))
                        {
                            Selection.activeObject = tex;
                        }
                        return false;
                    }



                }

                return true;
            }



            public override void DrawBrush()
            {
                if ( m_currentTool!= ePaintingTool.COLOR || m_showBrush)
                {
                    base.DrawBrush();
                }               
            }



            /// <summary>
            /// DrawSceneMenu
            /// </summary>
            /// <param name="width"></param>
            public override void DrawSceneMenu(float width)
            {

                m_lastSceneviewCam = SceneView.currentDrawingSceneView.camera;

                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;

                if (g == null)
                    return;

                HairDesignerShader s = g.GetShaderParams();
                if(s!=null)
                    s.UpdateMaterialProperty(ref g.m_furMaterial, HairDesignerBase.eLayerType.FUR_SHELL);

                
                

                //g.UpdateShells();

                GUILayout.BeginHorizontal();
                GUI.color = m_currentTool == ePaintingTool.ADD ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Mask", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.PAINT)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.ADD;
                }

                GUI.color = m_currentTool == ePaintingTool.BRUSH ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Brush", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BRUSH)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.BRUSH;
                }

                GUI.color = m_currentTool == ePaintingTool.COLOR ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Color", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.COLOR)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.COLOR;
                }

                GUI.color = Color.white;

                GUILayout.EndHorizontal();

                /*
                if (m_currentTool == ePaintingTool.ADD)
                {
                    if (!m_maskReady)
                    {
                        EditorGUILayout.HelpBox("The mask is not ready for painting\nplease create a new one", MessageType.Error);
                        GUILayout.Space(5);
                        if (GUILayout.Button("Create a new mask"))
                        {
                            CreateMaskTexture(eTextureSlot.MASK);
                        }

                        return;
                    }
                }*/



                if (!TextureReady( s, ePaintingTool.ADD ))return;
                if (!TextureReady(s, ePaintingTool.BRUSH)) return;
                if (!TextureReady(s, ePaintingTool.COLOR)) return;



                /*
                GUILayout.BeginHorizontal();

                GUI.color = m_currentTool == ePaintingTool.SCALE ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Scale", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.SCALE)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.SCALE;
                }
                GUILayout.Space(100);

                GUILayout.EndHorizontal();
                */

                //if (GUILayout.Button("Hair Strand Editor"))
                //{
                //    HairStrandPreview.Init();
                //    HairStrandPreview.m_hd = hd;
                //}



                GUI.color = Color.black;
                GUILayout.Label("Brush settings", EditorStyles.centeredGreyMiniLabel);
                GUI.color = Color.white;

                GUILayout.BeginHorizontal();
                GUILayout.Label("Size", GUILayout.MinWidth(width * .3f));
                m_brushRadius = (EditorGUILayout.Slider((m_brushRadius) / .3f, 0.05f, 1f, GUILayout.MaxWidth(width * .7f)) * .3f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Intensity", GUILayout.MinWidth(width * .3f));
                m_brushIntensity = EditorGUILayout.Slider(m_brushIntensity, 0f, 1f, GUILayout.MaxWidth(width * .7f));
                GUILayout.EndHorizontal();

                if (m_currentTool != ePaintingTool.BRUSH)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Brush Density", GUILayout.MinWidth(width * .3f));
                    m_brushDensity = (EditorGUILayout.Slider((m_brushDensity) / .3f, 0.05f, 1f, GUILayout.MaxWidth(width * .7f)) * .3f);
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Pixel Density", GUILayout.MinWidth(width * .3f));
                    m_pixelRange = (EditorGUILayout.Slider((m_pixelRange), 0f, 1f, GUILayout.MaxWidth(width * .7f)));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    m_brushDensity = 1f;

                    
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Falloff", GUILayout.MinWidth(width * .3f));
                m_brushFalloff = EditorGUILayout.Slider(m_brushFalloff, 0f, 1f, GUILayout.MaxWidth(width * .7f));
                GUILayout.EndHorizontal();

                if (m_currentTool == ePaintingTool.ADD)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Min max length", GUILayout.MinWidth(width * .3f));

                    EditorGUILayout.MinMaxSlider(ref g.m_editorData.m_furMaskMin, ref g.m_editorData.m_furMaskMax, 0f, 1f, GUILayout.MaxWidth(width * .7f));
                    GUILayout.EndHorizontal();

                    m_lockMask = GUILayout.Toggle(m_lockMask, "lock no mask");



                    GUILayout.Space(10);

                    if (GUILayout.Button("Fill mask with min length ("+ Mathf.Round(g.m_editorData.m_furMaskMin *100f)*.01f + ")"))
                    {
                        if (EditorUtility.DisplayDialog("Fill mask", "Fill all with min length : "+ g.m_editorData.m_furMaskMin, "ok", "cancel"))                        
                            FillMask(g.m_editorData.m_furMaskMin);
                        
                    }

                    if (GUILayout.Button("Fill mask with max length (" + Mathf.Round(g.m_editorData.m_furMaskMax * 100f) * .01f + ")"))
                    {
                        if (EditorUtility.DisplayDialog("Fill mask", "Fill all with min length : " + g.m_editorData.m_furMaskMax, "ok", "cancel"))
                            FillMask(g.m_editorData.m_furMaskMax);

                    }


                    GUILayout.Space(10);
                    m_maskFill = EditorGUILayout.ObjectField(m_maskFill, typeof(Texture2D), false) as Texture2D;
                    GUI.enabled = m_maskFill != null;
                    GUILayout.BeginHorizontal();
                    m_invertTextureMaskref = GUILayout.Toggle(m_invertTextureMaskref, "invert");                    
                    if (GUILayout.Button("Fill mask with texture"))                                            
                        if (EditorUtility.DisplayDialog("Fill mask", "Fill all with the texture", "ok", "cancel"))
                            FillMask(m_maskFill, m_invertTextureMaskref);                                            
                    GUILayout.EndHorizontal();
                    GUI.enabled = true;

                    


                }

                //m_strandSpacing = EditorGUILayout.FloatField("Hair spacing", m_strandSpacing);


                if (m_currentTool == ePaintingTool.BRUSH)
                {
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUI.color = m_secondaryTool == ePaintingTool.BRUSH ? Color.white : Color.grey;
                    if (GUILayout.Button("Direction", EditorStyles.miniButtonLeft))
                    {
                        m_secondaryTool = ePaintingTool.BRUSH;
                    }
                    GUI.color = m_secondaryTool == ePaintingTool.HEIGHT ? Color.white : Color.grey;
                    if (GUILayout.Button("Height", EditorStyles.miniButtonRight))
                    {
                        m_secondaryTool = ePaintingTool.HEIGHT;
                    }
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();

                    if (m_secondaryTool == ePaintingTool.HEIGHT)
                    {
                        GUILayout.Label("Min/Max height", EditorStyles.miniLabel);
                        EditorGUILayout.MinMaxSlider( ref g.m_editorData.m_furHeightMin, ref g.m_editorData.m_furHeightMax, 0f, 1f);
                    }

                    GUILayout.Space(20);

                    if (GUILayout.Button("Erase brush data"))
                    {
                        if (EditorUtility.DisplayDialog("Erase brush data", "All brush data will be removed", "ok", "cancel"))
                        {
                            Texture2D tex = (g.GetShaderParams() as HairDesignerShader).GetTexture(3);
                            for (int x = 0; x < tex.width; ++x)
                            {
                                for (int y = 0; y < tex.height; ++y)
                                {
                                    Color c = tex.GetPixel(x, y);
                                    c.r = 1f;
                                    c.g = .5f;
                                    c.b = .5f;
                                    tex.SetPixel(x, y, c);
                                }
                            }
                            tex.Apply();
                        }

                    }
                }




                if(m_currentTool == ePaintingTool.COLOR)
                {                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Replace/Mix", GUILayout.MinWidth(width * .3f));
                    m_colorReplaceMix = EditorGUILayout.Slider(m_colorReplaceMix, 0f, 1f, GUILayout.MaxWidth(width * .7f));
                    GUILayout.EndHorizontal();

                    
                    m_toolColor = EditorGUILayout.ColorField("Color", m_toolColor);
                    m_showBrush = EditorGUILayout.ToggleLeft("Show brush", m_showBrush);
                        

                    if (GUILayout.Button("Fill color"))
                    {
                        if (EditorUtility.DisplayDialog("Fill color", "Fill all with the color", "ok", "cancel"))
                            FillColor(m_toolColor);

                    }

                    GUILayout.Space(10);
                    m_colorFill = EditorGUILayout.ObjectField(m_colorFill, typeof(Texture2D), false) as Texture2D;
                    GUI.enabled = m_colorFill != null;
                    GUILayout.BeginHorizontal();                    
                    if (GUILayout.Button("Fill color with texture"))
                        if (EditorUtility.DisplayDialog("Fill color", "Fill all with the texture", "ok", "cancel"))
                            FillColor(m_colorFill);
                    GUILayout.EndHorizontal();
                    GUI.enabled = true;

                }

                GUILayout.Space(20);

                switch (m_currentTool)
                {
                    case ePaintingTool.ADD:
                        EditorGUILayout.HelpBox(
                            "Ctrl  : remove mode\n"+
                            "Shift : smooth mode\n"
                            , MessageType.Info);                        

                        break;

                    case ePaintingTool.BRUSH:
                        if(m_secondaryTool == ePaintingTool.BRUSH)
                            EditorGUILayout.HelpBox("Ctrl  : raise the fur\nShift : smooth direction", MessageType.Info);
                        else
                            EditorGUILayout.HelpBox("Ctrl  : increase height\nShift : smooth height", MessageType.Info);
                        break;

                    
                }


                
                

                if (m_currentTool == ePaintingTool.COLOR)
                {
                    Texture2D tex = s.GetTexture(4);
                    SaveOrRevertTextures(ref tex, ref g.m_editorData.m_unsavedColorTexFile);
                    if (g.m_editorData.m_unsavedColorTexFile)
                        EditorGUILayout.HelpBox("Color texture need to be saved to the project.", MessageType.Warning);
                }


                if (m_currentTool == ePaintingTool.ADD)
                {
                    Texture2D tex = s.GetTexture(2);
                    SaveOrRevertTextures(ref tex, ref g.m_editorData.m_unsavedMaskTexFile);
                    if (g.m_editorData.m_unsavedMaskTexFile)
                        EditorGUILayout.HelpBox("Mask texture need to be saved to the project.", MessageType.Warning);
                }

                if ( m_currentTool == ePaintingTool.BRUSH )
                {
                    Texture2D tex = s.GetTexture(3);
                    SaveOrRevertTextures(ref tex, ref g.m_editorData.m_unsavedBrushTexFile);
                    if (g.m_editorData.m_unsavedBrushTexFile)
                        EditorGUILayout.HelpBox("Brush texture need to be saved to the project.", MessageType.Warning);
                }




            }

            void SaveOrRevertTextures( ref Texture2D tex, ref bool flag )
            {
                bool refreshImporter = false;

                if (GUILayout.Button("Save texture"))
                {
                    flag = false;
                    byte[] bytes = tex.EncodeToPNG();
                    System.IO.File.WriteAllBytes(AssetDatabase.GetAssetPath(tex), bytes);
                    AssetDatabase.Refresh();
                    refreshImporter = true;
                }

                if (GUILayout.Button("Revert texture"))
                {
                    if (EditorUtility.DisplayDialog("Revert texture", "Revert the texture to the previous version", "Ok", "Cancel"))
                    {
                        flag = false;
                        refreshImporter = true;
                    }
                }

                if (refreshImporter)
                {
                    TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex)) as TextureImporter;
                    ImporterSettings(ref importer);
                    importer.SaveAndReimport();

                    HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                    g.m_needMeshRebuild = true;
                    //g.UpdateInstance();
                    g.UpdateShells();
                }
            }


            public void ImporterSettings( ref TextureImporter importer )
            {
//#if UNITY_5_5_OR_NEWER
                if (importer == null)
                    return;

                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.sRGBTexture = false;
                importer.alphaIsTransparency = true;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.isReadable = true;
                //importer.textureFormat = TextureImporterFormat.RGBA32;
                /*
                #else
                                importer.linearTexture = true;
                                importer.textureFormat = TextureImporterFormat.ARGB32;
                #endif
                */
            }


            /// <summary>
            /// Generate texture mask, brush & color
            /// </summary>
            /// <param name="textureSlot"></param>
            public void CreateTexture(eTextureSlot textureSlot)
            {
                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                HairDesignerShader s = g.GetShaderParams() as HairDesignerShader;

                if (s == null)
                    return;

                int width = 2048;
                int height = 2048;

                string filename = "";
                string texturePath = "";
                Color c = Color.white;
                switch (textureSlot)
                {
                    case eTextureSlot.MASK:
                        filename = s.GetTexture(2) != null ? s.GetTexture(2).name : "FurMask_" + g.gameObject.name + "_" + g.m_name;
                        texturePath = s.GetTexture(2) == null ? "Assets/" : AssetDatabase.GetAssetPath(s.GetTexture(2));
                        c = new Color(.5f, .5f, .5f, 0f);
                        break;

                    case eTextureSlot.BRUSH:
                        filename = s.GetTexture(3) != null ? s.GetTexture(3).name : "FurBrush_" + g.gameObject.name + "_" + g.m_name;
                        texturePath = s.GetTexture(3) == null ? "Assets/" : AssetDatabase.GetAssetPath(s.GetTexture(3));
                        c = new Color(1f, .5f, .5f, .99f);
                        width = height = 1024;

                        int vCount = 0;
                        if (g.m_hd.m_smr != null)
                            vCount = g.m_hd.m_smr.sharedMesh.vertexCount;
                        if (g.m_hd.GetComponent<MeshFilter>() != null)
                            vCount = g.GetComponent<MeshFilter>().sharedMesh.vertexCount;

                        if(vCount >0)
                        {
                            //set the minimum size for the texture
                            if (vCount <= 1024*1024) width = height = 1024;
                            if (vCount <= 512*512) width = height = 512;
                            if (vCount <= 256*256 ) width = height = 256;
                            if (vCount <= 128*128 ) width = height = 128;
                            if (vCount <= 64*64) width = height = 64;
                            if (vCount <= 32*32) width = height = 32;
                            if (vCount <= 16*16) width = height = 16;
                            if (vCount <= 8*8) width = height = 8;
                        }

                        break;

                    case eTextureSlot.COLOR:
                        filename = s.GetTexture(4) != null ? s.GetTexture(4).name : "FurColor_" + g.gameObject.name + "_" + g.m_name;
                        texturePath = s.GetTexture(4) == null ? "Assets/" : AssetDatabase.GetAssetPath(s.GetTexture(4));

                        c = new Color(1f, 1f, 1f, .99f);
                        break;
                }

                string path = EditorUtility.SaveFilePanel(
                "Save texture as PNG",
                texturePath,
                filename + ".png",
                "png");

                if (path.Length == 0)                
                    return;
                

                Texture2D image = new Texture2D(width, height, TextureFormat.ARGB32, true, true);                
                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < width; ++y)
                        image.SetPixel(x,y,c);
                image.Apply();

                byte[] bytes = image.EncodeToPNG();
                System.IO.File.WriteAllBytes(path, bytes);
                AssetDatabase.Refresh();

                
                path = path.Replace(Application.dataPath, "Assets");


                switch (textureSlot)
                {
                    case eTextureSlot.MASK:
                        s.SetTexture(2, AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D);
                        break;

                    case eTextureSlot.BRUSH:
                        s.SetTexture(3, AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D);
                        break;

                    case eTextureSlot.COLOR:
                        s.SetTexture(4, AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D);
                        break;
                }


                s.UpdateMaterialProperty( ref g.m_furMaterial, HairDesignerBase.eLayerType.FUR_SHELL );

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                ImporterSettings(ref importer);


#if UNITY_5_5_OR_NEWER
                importer.textureType = TextureImporterType.Default;
#else
                importer.textureType = TextureImporterType.Advanced;
#endif
                importer.wrapMode = TextureWrapMode.Clamp;
                //importer.textureFormat = TextureImporterFormat.ARGB32;
                importer.mipmapEnabled = false;
                importer.compressionQuality = 0;
                
                importer.isReadable = true;
                importer.SaveAndReimport();
            }



            Color m_transparentWhite = new Color(1f, 1f, 1f, .1f);

            /// <summary>
            /// 
            /// </summary>
            public override void DrawSceneTools()
            {
                if (m_tab == eTab.MOTION)
                    MotionZoneSceneGUI();

                if ( m_tab == eTab.DESIGN )
                {
                    HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                    if (g.RuntimeInstancingMode == eInstancingMode.MESH_INSTANCING_INDIRECT &&
                        g.m_MII_drawBoundingBox)
                    {
                        Handles.color = m_transparentWhite;                        
                        Handles.DrawWireCube(g.m_r.bounds.center, g.m_r.bounds.size);
                        Handles.color = Color.blue;
                        Handles.DrawWireCube(g.m_MII_Bounbs.center, g.m_MII_Bounbs.size);
                    }
                }
            }




            /// <summary>
            /// Fill the texture with color
            /// </summary>
            /// <param name="l"></param>
            void FillColor(Color color)
            {
                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                g.m_editorData.m_unsavedColorTexFile = true;
                color.a = m_colorReplaceMix;
                
                Texture2D tex = (g.GetShaderParams() as HairDesignerShader).GetTexture(4);
                Undo.RegisterCompleteObjectUndo(tex, "HairDesigner fill color");

                for (int x = 0; x < tex.width; ++x)
                {
                    for (int y = 0; y < tex.height; ++y)
                    {                        
                        tex.SetPixel(x, y, color);
                    }
                }
                tex.Apply();
            }


            bool CheckTextureFormat(string path)
            {
                bool extOk = true;
                string[] split = path.Split('.');
                if (split.Length == 0)
                {
                    extOk = false;
                }
                else
                {
                    string ext = split[split.Length - 1].ToLower();
                    extOk = ext == "png" || ext == "jpg" || ext == "jpeg";
                        
                }

                if (!extOk)                
                    EditorUtility.DisplayDialog("Import error", "the texture format must be PNG or JPG", "OK");                    
                

                return extOk;
            }



            /// <summary>
            /// Fill color with a texture
            /// </summary>
            /// <param name="texOriginal"></param>
            void FillColor(Texture2D texOriginal)
            {                
                if (texOriginal == null)
                    return;

                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                g.m_editorData.m_unsavedColorTexFile = true;
                Texture2D texRef = new Texture2D(texOriginal.width, texOriginal.height, TextureFormat.ARGB32, true, true);

                string path = AssetDatabase.GetAssetPath(texOriginal);

                if (!CheckTextureFormat(path))
                    return;


                path = Application.dataPath + path.Remove(0, 6);
                //Debug.Log(path);
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                //byte[] bytes = texOriginal.EncodeToPNG();
                texRef.LoadImage(bytes, false);
                texRef.Apply();

                
                Texture2D tex = (g.GetShaderParams() as HairDesignerShader).GetTexture(4);
                Undo.RegisterCompleteObjectUndo(tex, "HairDesigner fill color");

                for (int x = 0; x < tex.width; ++x)
                {
                    for (int y = 0; y < tex.height; ++y)
                    {                        
                        Color c = texRef.GetPixel((int)((float)x * (float)texRef.width / (float)tex.width),
                                         (int)((float)y * (float)texRef.height / (float)tex.height));

                        c.a = m_colorReplaceMix;
                        tex.SetPixel(x, y, c);
                    }
                }
                tex.Apply();
                AssetDatabase.Refresh();
                DestroyImmediate(texRef);
            }




            void FillMask(float l)
            {
                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                g.m_editorData.m_unsavedMaskTexFile = true;
                
                Texture2D tex = (g.GetShaderParams() as HairDesignerShader).GetTexture(2);
                Undo.RegisterCompleteObjectUndo(tex, "HairDesigner fill mask");

                for (int x = 0; x < tex.width; ++x)
                {
                    for (int y = 0; y < tex.height; ++y)
                    {
                        Color c = tex.GetPixel(x, y);
                        c.a = !m_lockMask ? l : (c.a==0?0:l);
                        tex.SetPixel(x, y, c);
                    }
                }
                tex.Apply();
               
            }



            /// <summary>
            /// Fill mask with texture
            /// </summary>
            /// <param name="texOriginal"></param>
            /// <param name="invert"></param>
            void FillMask(Texture2D texOriginal, bool invert )
            {
                if (texOriginal == null)
                    return;

                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                g.m_editorData.m_unsavedMaskTexFile = true;
                Texture2D texRef = new Texture2D(texOriginal.width, texOriginal.height, TextureFormat.ARGB32, true, true);
                string path = AssetDatabase.GetAssetPath(texOriginal);
                if (!CheckTextureFormat(path))
                    return;
                
                path = Application.dataPath + path.Remove(0, 6);
                //Debug.Log(path);
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                //byte[] bytes = texOriginal.EncodeToPNG();
                texRef.LoadImage(bytes, false);
                texRef.Apply();

                Texture2D tex = (g.GetShaderParams() as HairDesignerShader).GetTexture(2);
                Undo.RegisterCompleteObjectUndo(tex, "HairDesigner fill mask");

                for (int x = 0; x < tex.width; ++x)
                {
                    for (int y = 0; y < tex.height; ++y)
                    {
                        Color c = tex.GetPixel(x, y);
                        Color c2 = texRef.GetPixel( (int)((float)x*(float)texRef.width/(float)tex.width),
                                         (int)((float)y * (float)texRef.height / (float)tex.height));

                        float a = invert ? 1f - (c2.r+c2.g+c2.b)/3f : (c2.r + c2.g + c2.b) / 3f;
                        c.a = !m_lockMask ? a : (c.a == 0 ? 0 : a);

                        tex.SetPixel(x, y, c);
                    }
                }
                tex.Apply();

                DestroyImmediate(texRef);
            }



            public override void OnDuplicate(HairDesignerGenerator copy)
            {
                HairDesignerGeneratorFurShellBase cp = copy as HairDesignerGeneratorFurShellBase;
                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                g.Init();
                cp.m_furMaterial = new Material(g.m_furMaterial);
                cp.Init();
                HideAllShellsWireframe();

            }



            public void OnEnable()
            {
                m_verticesLink = null;
                m_normals = null;                
                m_tangents = null;                
                m_triangles = null;                
                m_uvs = null;

                
            }


            public void OnDestroy()
            {

                HairDesignerEditor.DestroyCollider();

                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                if (g == null) return;



#if UNITY_2018_3_OR_NEWER
                if (PrefabUtility.IsPartOfPrefabAsset(g))
#else
                if (PrefabUtility.GetPrefabParent(g.gameObject) == null && PrefabUtility.GetPrefabObject(g.gameObject) != null)   
#endif
                {
                    return;
                }

                g.UpdateShells();
                HideShellsWireframe(g);
                if (m_verticesLink != null)
                {
                    m_verticesLink.Clear();
                    m_verticesLink = null;
                }
            }



            /// <summary>
            /// PaintToolAction
            /// </summary>
            /// <param name="svCam"></param>

            public override void PaintToolAction()
            {
                HairDesignerGeneratorFurShellBase g = target as HairDesignerGeneratorFurShellBase;
                HairDesignerShader s = g.GetShaderParams() as HairDesignerShader;
                Texture2D maskTex = s.GetTexture(2);
                Texture2D brushTex = s.GetTexture(3);
                Texture2D colorTex = s.GetTexture(4);

                switch (m_currentTool)//check textures
                {
                    case ePaintingTool.ADD: if (maskTex == null) return; break;
                    case ePaintingTool.BRUSH: if (brushTex == null) return; break;
                    case ePaintingTool.COLOR: if (colorTex == null) return; break;
                }

                Camera svCam = SceneView.currentDrawingSceneView.camera;
                if (Event.current.alt || HairDesignerEditor.m_meshCollider == null)
                    return;

                m_CtrlMode = Event.current.control;
                m_ShiftMode = Event.current.shift;
                m_AltMode = Event.current.alt;

                HairDesignerBase hd = (target as HairDesignerGenerator).m_hd;

                Vector3 mp = Event.current.mousePosition* EditorGUIUtility.pixelsPerPoint;
                mp.y = svCam.pixelHeight - mp.y;

                //if (m_currentTool == ePaintingTool.ADD)
                {
                    //float pixelSize = Vector3.Distance(svCam.WorldToScreenPoint(svCam.transform.position + svCam.transform.forward), svCam.WorldToScreenPoint(svCam.transform.position + svCam.transform.forward + svCam.transform.right * m_brushSize));
                    //Vector2 rnd = Random.insideUnitCircle * pixelSize;

                    //mp.x += rnd.x;
                    //mp.y += rnd.y;
                }






                //Setup painting info
                //StrandData dt = new StrandData();
                //Vector2 mp2d = Event.current.mousePosition;
                //mp2d.y = svCam.pixelHeight - mp2d.y;

                BrushToolData bt = new BrushToolData();
                bt.mousePos = mp;
                bt.transform = hd.transform;
                bt.tool = m_currentTool;
                bt.cam = svCam;
                bt.CtrlMode = m_CtrlMode;
                bt.ShiftMode = m_ShiftMode;
                bt.brushSize = m_brushSize;
                bt.brushScreenDir = (svCam.transform.right * Event.current.delta.x - svCam.transform.up * Event.current.delta.y).normalized;
                bt.brushFalloff = m_brushFalloff;
                bt.brushIntensity = m_brushIntensity;
                //if (m_currentTool == ePaintingTool.ADD)

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    m_startPaint = true;
                }

                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    m_startPaint = false;
                }

                if (Event.current.type == EventType.MouseDrag)
                {


                    Mesh mesh = HairDesignerEditor.m_meshCollider.sharedMesh;

                    if(m_normals == null)
                        m_normals = mesh.normals;
                    if (m_tangents == null)
                        m_tangents = mesh.tangents;
                    if (m_triangles == null)
                        m_triangles = mesh.triangles;
                    if (m_uvs == null)
                        m_uvs = mesh.uv;
                    
                    //UnwrapParam settings = new UnwrapParam();                    
                    //m_uvs = Unwrapping.GeneratePerTriangleUV(mesh, settings);
                    mesh.uv = m_uvs;


                    List<int> brushVerticesApplied = new List<int>();


                    float meanMaskColor = 0;

                    int maxRay = Mathf.FloorToInt(5000f * m_brushDensity * m_brushDensity);

                    if (m_currentTool == ePaintingTool.BRUSH)
                    {
                        //maxRay = (int)( (float)maxRay * .2f );//brush is per vertex, not per pixel -> required less raycast
                        maxRay = Mathf.Clamp((int)(m_brushIntensity * 10f), 1, 10);
                    }

                    for (int rc = 0; rc < maxRay; ++rc)
                    {

                        //Vector3 worldPos = Vector3.zero;
                        Vector2 uv = Vector2.zero;
                        Vector3 normal = Vector3.zero;
                        Vector3 tangent = Vector3.zero;
                        Vector3 averageDir = Vector3.zero;
                        int vertexId = -1;
                        /*
                        if (svCam.orthographic)
                        {
                            mp = svCam.ScreenToWorldPoint(mp);
                            mp += svCam.transform.forward;
                            m_constUnit = (svCam.ViewportToWorldPoint(Vector3.zero) - svCam.ViewportToWorldPoint(Vector3.one)).magnitude;
                            m_constUnit = HandleUtility.GetHandleSize(hd.transform.position) * 10f;
                            m_brushSize = m_brushRadius * m_constUnit;
                        }
                        else
                        {
                            Ray rDir = svCam.ScreenPointToRay(mp);
                            mp = svCam.transform.position + rDir.direction.normalized;
                            m_brushSize = m_brushRadius;
                        }
                        */
                        float f = ((float)rc / (float)maxRay);

                        //float max = 30f;
                        Vector2 offset;

                        if (m_currentTool == ePaintingTool.ADD && m_brushIntensity < 1f)
                        {
                            offset = Random.insideUnitCircle;
                        }
                        else
                        {
                            offset.x = (Mathf.Cos(Mathf.PI * 20f * f + System.DateTime.Now.Millisecond));
                            offset.y = (Mathf.Sin(Mathf.PI * 20f * f + System.DateTime.Now.Millisecond));
                        }

                        offset *= m_brushRadius * 500f * f;




                        Vector2 mp2 = mp;
                        mp2.x += offset.x;
                        mp2.y += offset.y;

                        //float falloff = Mathf.Lerp(1f, 1f - offset.magnitude / (float)max, m_brushFalloff);
                        float falloff = Mathf.Lerp(1f, 1f - f, m_brushFalloff);

                        Ray r = svCam.ScreenPointToRay(mp2);

                        RaycastHit[] hitInfos = Physics.RaycastAll(r);
                        bool hitCollider = false;
                        foreach (RaycastHit hitInfo in hitInfos)
                        {

                            if (hitInfo.collider.gameObject != HairDesignerEditor.m_meshCollider.gameObject)                            
                                continue;//get only our custom collider

                            MeshCollider meshCollider = hitInfo.collider as MeshCollider;
                            if (meshCollider == null || meshCollider.sharedMesh == null)
                                continue;

                            hitCollider = true;
                            uv = hitInfo.textureCoord;
                            normal = hitInfo.normal;



                            Vector4 t0 = m_tangents[m_triangles[hitInfo.triangleIndex * 3 + 0]];
                            Vector4 t1 = m_tangents[m_triangles[hitInfo.triangleIndex * 3 + 1]];
                            Vector4 t2 = m_tangents[m_triangles[hitInfo.triangleIndex * 3 + 2]];
                            Transform hitTransform = hitInfo.collider.transform;

                            Vector3 baryCenter = hitInfo.barycentricCoordinate;
                            tangent = t0 * baryCenter.x + t1 * baryCenter.y + t2 * baryCenter.z;
                            tangent = hitTransform.TransformDirection(tangent).normalized;

                            if (m_currentTool == ePaintingTool.BRUSH)
                            {
                                vertexId = -1;

                                //vertexId = baryCenter.x > Mathf.Max(baryCenter.y, baryCenter.z) ? triangles[hitInfo.triangleIndex * 3 + 0] : vertexId;
                                //vertexId = baryCenter.y > Mathf.Max(baryCenter.x, baryCenter.z) ? triangles[hitInfo.triangleIndex * 3 + 1] : vertexId;
                                //vertexId = baryCenter.z > Mathf.Max(baryCenter.y, baryCenter.x) ? triangles[hitInfo.triangleIndex * 3 + 2] : vertexId;
                                bool newVertices = false;
                                for (int i = 0; i < 3; ++i)
                                {
                                    vertexId = m_triangles[hitInfo.triangleIndex * 3 + i];
                                    if (vertexId != -1)
                                    {
                                        uv = m_uvs[vertexId];
                                        normal = m_normals[vertexId];
                                        tangent = m_tangents[vertexId];

                                        if (!brushVerticesApplied.Contains(vertexId))
                                        {
                                            if (!m_verticesLink.ContainsKey(m_vertices[vertexId]))
                                            {
                                                m_verticesLink = null;
                                                InitLinkedVertices();
                                            }

                                            //brushVerticesApplied.Add(vertexId);                                            
                                            for (int l = 0; l < m_verticesLink[m_vertices[vertexId]].Count; ++l)
                                            {
                                                //add merged vertices
                                                if (!brushVerticesApplied.Contains(m_verticesLink[m_vertices[vertexId]][l]))
                                                    brushVerticesApplied.Add(m_verticesLink[m_vertices[vertexId]][l]);
                                            }

                                            newVertices = true;
                                        }
                                    }
                                }


                                hitCollider = !newVertices;
                            }

                            break;
                        }



                        if (hitCollider && m_startPaint)
                        {

                            if (m_currentTool == ePaintingTool.ADD)
                            {
                                if (s.GetTexture(2) != null)
                                    Undo.RegisterCompleteObjectUndo(s.GetTexture(2), "HairDesigner draw");
                                else
                                    return;
                            }


                            if (m_currentTool == ePaintingTool.BRUSH)
                            {
                                if (s.GetTexture(3) != null)
                                    Undo.RegisterCompleteObjectUndo(s.GetTexture(3), "HairDesigner draw");
                                else
                                    return;
                            }


                            if (m_currentTool == ePaintingTool.COLOR)
                            {
                                if (s.GetTexture(4) != null)
                                    Undo.RegisterCompleteObjectUndo(s.GetTexture(4), "HairDesigner draw");
                                else
                                    return;
                            }

                            m_startPaint = false;
                        }



                        if (!Event.current.alt && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                        {
                            //int min = -(int)(maskTex.width * .1 * m_brushSize);
                            //int max = (int)(maskTex.width * .1 * m_brushSize);
                            //Vector2 dist;
                            //float meanMaskColor = -1f;
                            Color meanTextureColor = Color.black;
                            //Color meanDir = Color.black;

                            Color c = Color.black;

                            int x = 0;
                            int y = 0;

                            //-------------------------------------------------------------
                            //ADD

                            if (m_currentTool == ePaintingTool.ADD)
                            {

                                int pxDensity = Mathf.CeilToInt(m_pixelRange * 10f);
                                for (int px = -pxDensity; px <= pxDensity; ++px)
                                {
                                    for (int py = -pxDensity; py <= pxDensity; ++py)
                                    {

                                        if (px + py > 10)
                                            continue;//circle shape

                                        Color pix = maskTex.GetPixel((int)(uv.x * 2048f) + px, (int)(uv.y * 2048f) + py);
                                        meanMaskColor = Mathf.Lerp(meanMaskColor, pix.a, .5f);
                                        if (rc == 0)
                                            meanMaskColor = pix.a;
                                        //meanDir += pix;
                                        //meanDir += ((pix.r * 2f - 1f) * normal + (pix.g * 2f - 1f) * tangent + (pix.b * 2f - 1f) * Vector3.Cross(normal, tangent)).normalized;
                                        //meanDir.Normalize();

                                        x = (int)(uv.x * (float)maskTex.width);
                                        y = (int)(uv.y * (float)maskTex.height);
                                        c = maskTex.GetPixel(x + px, y + py);

                                        //float falloff = Mathf.Lerp(1f, 1f - dist.magnitude / (float)max, m_brushFalloff);

                                        if (!(m_lockMask && c.a == 0))
                                        {
                                            if (!m_CtrlMode && !m_ShiftMode)
                                                //add
                                                c.a += m_brushIntensity * Mathf.Lerp(.01f, 1f, m_brushIntensity) * falloff;
                                            else if (m_CtrlMode && !m_ShiftMode)
                                                //remove
                                                c.a -= m_brushIntensity * Mathf.Lerp(.01f, 1f, m_brushIntensity) * falloff;
                                            else
                                                //smooth
                                                c.a = Mathf.Lerp(c.a, meanMaskColor, m_brushIntensity * falloff);

                                            c.a = Mathf.Clamp(c.a, g.m_editorData.m_furMaskMin, g.m_editorData.m_furMaskMax);

                                            maskTex.SetPixel(x + px, y + py, c);
                                            g.m_editorData.m_unsavedMaskTexFile = true;
                                        }
                                    }
                                }
                            }

                            //------------------------------------------------------
                            //Compute brush tool - DIRECTION
                            if (m_currentTool == ePaintingTool.BRUSH)
                            {
                                if (m_secondaryTool == ePaintingTool.BRUSH)
                                {


                                    Vector3 v = Vector3.zero;
                                    //Color cDir = Color.black;
                                    v = bt.brushScreenDir;
                                    v = g.transform.InverseTransformDirection(v);
                                    v.Normalize();

                                    if (!m_CtrlMode && !m_ShiftMode)
                                    {

                                        if (vertexId != -1)
                                        {
                                            //Vector3 key = m_vertices[vertexId];
                                            List<int> ids = brushVerticesApplied;// m_verticesLink[key];
                                                                                                                                 

                                            for (int vid = 0; vid < ids.Count; ++vid)
                                            {
                                                normal = m_normals[ids[vid]];
                                                tangent = m_tangents[ids[vid]];
                                                uv.x = (float)(ids[vid]) / (float)brushTex.width;
                                                uv.y = Mathf.Floor(uv.x) / (float)brushTex.width;
                                                uv.x -= Mathf.Floor(uv.x);
                                                float halfPixel = (1f / (float)brushTex.width) * .5f;
                                                int cx = (int)((uv.x + halfPixel) * (float)brushTex.width);
                                                int cy = (int)((1 - uv.y - halfPixel) * (float)brushTex.height);

                                                //Debug.Log( "vertex" + ids[vid] + " " + uv);
                                                Vector3 vProj = Vector3.zero;
                                                vProj.x = Vector3.Dot(v, normal.normalized);
                                                vProj.x = Mathf.Clamp01(vProj.x);
                                                vProj.y = Vector3.Dot(v, tangent.normalized);
                                                vProj.z = Vector3.Dot(v, Vector3.Cross(normal, tangent).normalized);
                                                vProj = vProj.normalized * .5f + Vector3.one * .5f;

                                                c = brushTex.GetPixel(cx, cy);
                                                c.r = Mathf.Lerp(c.r, vProj.x, falloff * .01f);
                                                c.g = Mathf.Lerp(c.g, vProj.y, falloff * .01f);
                                                c.b = Mathf.Lerp(c.b, vProj.z, falloff * .01f);

                                                //c = Color.blue;

                                                brushTex.SetPixel(cx, cy, c);
                                                g.m_editorData.m_unsavedBrushTexFile = true;
                                            }
                                        }
                                    }
                                    else if (m_CtrlMode && !m_ShiftMode)
                                    {
                                        List<int> ids = brushVerticesApplied;// m_verticesLink[key];
                                        for (int vid = 0; vid < ids.Count; ++vid)
                                        {
                                            //Revert fur orientation to normal
                                            uv.x = (float)(ids[vid]) / (float)brushTex.width;
                                            uv.y = Mathf.Floor(uv.x) / (float)brushTex.width;
                                            uv.x -= Mathf.Floor(uv.x);
                                            float halfPixel = (1f / (float)brushTex.width) * .5f;
                                            int cx = (int)((uv.x + halfPixel) * (float)brushTex.width);
                                            int cy = (int)((1 - uv.y - halfPixel) * (float)brushTex.height);

                                            c = brushTex.GetPixel(cx, cy);
                                            c = Color.Lerp(c, new Color(1f, .5f, .5f, c.a), m_brushIntensity * falloff);
                                            //c = new Color(.5f, .5f, .5f, c.a);
                                            //brushTex.SetPixel((int)(uv.x * (float)maskTex.width), (int)(uv.y * (float)maskTex.height), c);
                                            //brushTex.SetPixel((int)uv.x, (int)uv.y, c);
                                            brushTex.SetPixel(cx, cy, c);
                                            g.m_editorData.m_unsavedBrushTexFile = true;
                                        }
                                    }
                                    else
                                    {
                                        //shift mode -> smooth
                                        averageDir = Vector3.zero;
                                        List<int> ids = brushVerticesApplied;
                                        for (int vid = 0; vid < ids.Count; ++vid)
                                        {
                                            normal = m_normals[ids[vid]];
                                            tangent = m_tangents[ids[vid]];

                                            uv.x = (float)(ids[vid]) / (float)brushTex.width;
                                            uv.y = Mathf.Floor(uv.x) / (float)brushTex.width;
                                            uv.x -= Mathf.Floor(uv.x);
                                            float halfPixel = (1f / (float)brushTex.width) * .5f;
                                            int cx = (int)((uv.x + halfPixel) * (float)brushTex.width);
                                            int cy = (int)((1 - uv.y - halfPixel) * (float)brushTex.height);

                                            Color pix = brushTex.GetPixel(cx, cy);
                                            averageDir += ((pix.r * 2f - 1f) * normal + (pix.g * 2f - 1f) * tangent + (pix.b * 2f - 1f) * Vector3.Cross(normal, tangent)).normalized;
                                            //averageDir.Normalize();
                                        }

                                        averageDir /= (float)ids.Count;

                                        for (int vid = 0; vid < ids.Count; ++vid)
                                        {
                                            normal = m_normals[ids[vid]];
                                            tangent = m_tangents[ids[vid]];

                                            uv.x = (float)(ids[vid]) / (float)brushTex.width;
                                            uv.y = Mathf.Floor(uv.x) / (float)brushTex.width;
                                            uv.x -= Mathf.Floor(uv.x);
                                            float halfPixel = (1f / (float)brushTex.width) * .5f;
                                            int cx = (int)((uv.x + halfPixel) * (float)brushTex.width);
                                            int cy = (int)((1 - uv.y - halfPixel) * (float)brushTex.height);


                                            //Smooth fur orientation
                                            Vector3 vProj;
                                            vProj.x = Vector3.Dot(averageDir, normal.normalized);
                                            //vProj.x = Mathf.Clamp01(vProj.x);
                                            vProj.y = Vector3.Dot(averageDir, tangent.normalized);
                                            vProj.z = Vector3.Dot(averageDir, Vector3.Cross(normal, tangent).normalized);

                                            vProj = vProj * .5f + Vector3.one * .5f;

                                            c = brushTex.GetPixel(cx, cy);
                                            c.r = Mathf.Lerp(c.r, vProj.x, m_brushIntensity * falloff * .1f);
                                            c.g = Mathf.Lerp(c.g, vProj.y, m_brushIntensity * falloff * .1f);
                                            c.b = Mathf.Lerp(c.b, vProj.z, m_brushIntensity * falloff * .1f);

                                            brushTex.SetPixel(cx, cy, c);
                                            g.m_editorData.m_unsavedBrushTexFile = true;
                                        }
                                    }

                                }

                                //maskTex.SetPixel(x, y, c);
                                //g.m_editorData.m_unsavedMaskTexFile = true;

                                //svCam.transform
                                //normal
                                //bt.brushScreenDir



                                //------------------------------------------------------
                                //Compute brush tool - HEIGHT

                                if (m_secondaryTool == ePaintingTool.HEIGHT)
                                {
                                    Vector3 v = Vector3.zero;
                                    //Color cDir = Color.black;
                                    v = bt.brushScreenDir;
                                    v = g.transform.InverseTransformDirection(v);
                                    v.Normalize();
                                    float averageHeight = 0;
                                    List<int> ids = brushVerticesApplied;// m_verticesLink[key];

                                    if (!m_CtrlMode && m_ShiftMode)
                                    {
                                        //shift mode -> smooth
                                        
                                        for (int vid = 0; vid < ids.Count; ++vid)
                                        {
                                            normal = m_normals[ids[vid]];
                                            tangent = m_tangents[ids[vid]];

                                            uv.x = (float)(ids[vid]) / (float)brushTex.width;
                                            uv.y = Mathf.Floor(uv.x) / (float)brushTex.width;
                                            uv.x -= Mathf.Floor(uv.x);
                                            float halfPixel = (1f / (float)brushTex.width) * .5f;
                                            int cx = (int)((uv.x + halfPixel) * (float)brushTex.width);
                                            int cy = (int)((1 - uv.y - halfPixel) * (float)brushTex.height);

                                            Color pix = brushTex.GetPixel(cx, cy);
                                            averageHeight += pix.a;                                            
                                        }

                                        averageHeight /= (float)ids.Count;
                                    }



                                    if (vertexId != -1)
                                    {
                                        //Vector3 key = m_vertices[vertexId];
                                        

                                        for (int vid = 0; vid < ids.Count; ++vid)
                                        {
                                            normal = m_normals[ids[vid]];
                                            tangent = m_tangents[ids[vid]];
                                            uv.x = (float)(ids[vid]) / (float)brushTex.width;
                                            uv.y = Mathf.Floor(uv.x) / (float)brushTex.width;
                                            uv.x -= Mathf.Floor(uv.x);
                                            float halfPixel = (1f / (float)brushTex.width) * .5f;
                                            int cx = (int)((uv.x + halfPixel) * (float)brushTex.width);
                                            int cy = (int)((1 - uv.y - halfPixel) * (float)brushTex.height);


                                            c = brushTex.GetPixel(cx, cy);
                                            if (m_CtrlMode && !m_ShiftMode)
                                            {
                                                c.a = Mathf.Lerp(c.a, 0f, m_brushIntensity * falloff * .1f);
                                            }
                                            else if (!m_CtrlMode && !m_ShiftMode)
                                            {
                                                 c.a = Mathf.Lerp(c.a, 1f, m_brushIntensity * falloff * .1f);
                                            }
                                            else
                                            {
                                                c.a = Mathf.Lerp(c.a, averageHeight, m_brushIntensity * falloff * .1f);                                                
                                            }

                                            c.a = Mathf.Clamp(c.a, g.m_editorData.m_furHeightMin, g.m_editorData.m_furHeightMax);
                                            brushTex.SetPixel(cx, cy, c);
                                            g.m_editorData.m_unsavedBrushTexFile = true;
                                        }
                                        
                                    }
                                }
                            }
                                

                            if (m_currentTool == ePaintingTool.COLOR)
                            {
                                int pxDensity = Mathf.CeilToInt(m_pixelRange * 10f);
                                meanTextureColor = colorTex.GetPixel((int)(uv.x * 2048f), (int)(uv.y * 2048f));

                                for (int px = -pxDensity; px <= pxDensity; ++px)
                                {
                                    for (int py = -pxDensity; py <= pxDensity; ++py)
                                    {
                                        x = (int)(uv.x * (float)colorTex.width);
                                        y = (int)(uv.y * (float)colorTex.height);
                                        c = colorTex.GetPixel(x + px, y + py);
                                        meanTextureColor = Color.Lerp(meanTextureColor, colorTex.GetPixel((int)(uv.x * 2048f) + px, (int)(uv.y * 2048f) + py), .5f);


                                        m_toolColor.a = m_colorReplaceMix;

                                        if (!m_CtrlMode && !m_ShiftMode)
                                            //add                                            
                                            c = Color.Lerp(c, m_toolColor, m_brushIntensity * falloff * .5f);
                                        else if (m_CtrlMode && !m_ShiftMode)
                                            //remove
                                            c = Color.Lerp(c, Color.white, m_brushIntensity * falloff * .5f);
                                        else
                                            //smooth
                                            c = Color.Lerp(c, meanTextureColor, m_brushIntensity * falloff * .5f);

                                        //c.a = 0;
                                        colorTex.SetPixel(x + px, y + py, c);
                                        g.m_editorData.m_unsavedColorTexFile = true;
                                    }

                                }
                            }




                                //Undo.RecordObject(hd.generator, "hairDesigner brush");


                                /*
                                dt.localpos = hd.transform.InverseTransformPoint(worldPos);
                                bt.brushDir = hd.transform.InverseTransformDirection(worldPos - m_lastBrushPos).normalized;
                                hd.generator.PaintTool(dt, bt);
                                hd.generator.m_hair = null;                        
                                */
                            }

                        }//end FOR


                        if (m_currentTool == ePaintingTool.ADD && maskTex != null)
                            maskTex.Apply();

                        if (m_currentTool == ePaintingTool.BRUSH && brushTex != null)
                            brushTex.Apply();

                        if (m_currentTool == ePaintingTool.COLOR && colorTex != null)
                            colorTex.Apply();

                    }//end if
                }

            }

        }
}
