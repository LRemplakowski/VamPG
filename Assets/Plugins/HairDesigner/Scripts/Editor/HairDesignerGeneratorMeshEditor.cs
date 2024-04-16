   using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace Kalagaan
{
    namespace HairDesignerExtension
    {


        [CustomEditor(typeof(HairDesignerGeneratorMeshBase), true)]
        public class HairGeneratorMeshEditor : HairGeneratorEditor
        {


            //GUIStyle bgStyle = null;

            public enum eTab
            {
                //INFO,
                DESIGN,
                MATERIAL,
                MOTION,
                ADVANCED,
                DEBUG = 100
            }
            public static eTab m_tab = eTab.DESIGN;

            Vector3 m_lastHairPos = Vector3.zero;

            ePaintingTool m_currentTool = ePaintingTool.ADD;
            //bool m_showMotionZone = false;
            public bool m_brushRoot = true;
            public bool m_triangleDataUpdated = false;
            public bool m_OldSkinningMethodChecked = false;
            

            public override void OnInspectorGUI()
            {
                hideWireframe = false;
                eTab lastTab = m_tab;
                base.OnInspectorGUI();
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;

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

                

                GUI.SetNextControlName("TAB");

                GUIContent[] toolbar = new GUIContent[] {
                    //new GUIContent("Info",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.INFO))),
                    new GUIContent("Design",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.DESIGN))),
                    new GUIContent("Material",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.MATERIAL))),
                    new GUIContent("Motion",(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.MOTION)))
                };

                m_tab = (eTab)Toolbar((int)m_tab, toolbar);

                                

                if (m_tab != lastTab)
                    GUI.FocusControl("TAB");

                

                if (g.m_layerType == HairDesignerBase.eLayerType.NONE)//fix old versions
                    g.m_layerType = HairDesignerBase.eLayerType.SHORT_HAIR_POLY;


                if (!g.IsActive)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Layer is disabled.", MessageType.Warning);
                    if (GUILayout.Button("Enable", GUILayout.Height(40)))
                        g.SetActive(true);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }



                /*
                if (m_tab == eTab.INFO)
                {
                    if (m_hairDesignerEditor != null)
                        m_hairDesignerEditor.RestorePose();

                    //base.OnInspectorGUI();
                    //GUILayout.Label("Mesh Generator");
                    //g.m_requirePainting = false;
                    //HairDesignerEditor.m_showBrush = false;
                }
                */

                

                if (m_tab == eTab.DESIGN)
                {
                    if (Application.isPlaying)
                    {
                       EditorGUILayout.HelpBox("Polygon edition disabled in play mode",MessageType.Info);
                    }
                    else if(g.m_meshLocked)
                    {
                        GUILayout.Space(20);
                        EditorGUILayout.HelpBox( "Layer is locked\nAlways lock a layer when painting is over.", MessageType.Info );
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Unlock layer", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.UNLOCKED)))))
                        {
                            g.m_hd.m_meshSaved = false;
                            g.m_meshLocked = false;
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        hideWireframe = true;
                        g.m_requirePainting = false;
                        HairDesignerEditor.m_showSceneMenu = false;                       

                    }
                    else
                    {                        
                        g.m_wasInDesignTab = true;
                        DisableSceneTool();

                        if( !m_triangleDataUpdated && HairDesignerEditor.m_meshCollider!=null)
                        {
                            //Update triangle information for current collider
                            for( int i=0; i<g.m_strands.Count; ++i )
                            {
                                if (g.m_strands[i].triLock != null && g.m_strands[i].triLock.m_faceId != -1)
                                    g.m_strands[i].triLock.Apply( g.m_hd.transform, HairDesignerEditor.m_meshCollider.transform, HairDesignerEditor.m_meshColliderVertices, HairDesignerEditor.m_meshColliderTriangles, true);
                            }
                            m_triangleDataUpdated = true;
                        }
                        else if(HairDesignerEditor.m_meshCollider == null)
                        {
                            m_triangleDataUpdated = false;
                        }

                        
                        //g.m_oldSkinningVersionDetected = EditorGUILayout.Toggle("old Version", g.m_oldSkinningVersionDetected);

                        //check skinning data
                        if (!m_OldSkinningMethodChecked)
                        {
                            for (int i = 0; i < g.m_strands.Count; ++i)
                            {
                                if (g.m_strands[i].triLock == null || g.m_strands[i].triLock.m_faceId == -1)
                                {
                                    g.m_skinningVersion160 = false;
                                    break;
                                }
                            }
                            m_OldSkinningMethodChecked = true;
                        }


                        if (m_hairDesignerEditor != null)
                        {
                           m_hairDesignerEditor.CreateColliderAndTpose(g.m_skinningVersion160);                              
                        }

                        if (CheckBlendshapesModifications())
                        {
                            m_hairDesignerEditor.RemoveCollider();
                            m_hairDesignerEditor.CreateColliderAndTpose(g.m_skinningVersion160);
                            m_triangleDataUpdated = false;
                            g.m_needMeshRebuild = true;
                        }

                        /*
                        if ( g.m_allowFastMeshDraw )
                            g.DestroyMesh();//force hair mesh as single strand rendering
                        */
                        hideWireframe = true;
                        g.m_requirePainting = true;
                        HairDesignerEditor.m_showSceneMenu = true;

                                               


                        if (!g.m_skinningVersion160 || g.m_skinningVersion160_Converted )
                        {

                            GUILayout.Label(new GUIContent("Old version", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                            EditorGUILayout.HelpBox("This layer was made with an older version, you should convert it to the new version. It will enable the blendshape compatibility.",MessageType.Info);

                            if (!g.m_skinningVersion160_Converted)
                            {
                                if (GUILayout.Button("Convert to version 1.6"))
                                {
                                    g.m_skinningVersion160 = true;
                                    m_hairDesignerEditor.CreateCollider();
                                    for (int i = 0; i < g.m_strands.Count; ++i)
                                    {
                                        g.m_strands[i].triLock = new TriangleLock();
                                        Vector3 worldPos = g.m_hd.transform.TransformPoint(g.m_strands[i].localpos);
                                        if (g.m_strands[i].meshTriId == -1)
                                        {
                                            Vector3 worlDir = g.m_hd.transform.TransformDirection(g.m_strands[i].rotation*Vector3.forward).normalized;
                                            RaycastHit[] hits = Physics.RaycastAll(worldPos + worlDir*.1f, -worlDir);
                                            //Debug.DrawLine(worldPos + worlDir * .1f, worldPos + worlDir * .1f - worlDir, Color.green, 2f);

                                            for (int h=0; h< hits.Length; ++h)
                                            {
                                                if (hits[h].collider == HairDesignerEditor.m_meshCollider)
                                                {
                                                    g.m_strands[i].meshTriId = hits[h].triangleIndex;
                                                    //Debug.DrawLine(hits[h].point, hits[h].point + hits[h].normal, Color.cyan, 2f);
                                                }
                                            }

                                        }

                                        if (g.m_strands[i].meshTriId != -1)
                                            g.m_strands[i].triLock.Lock(worldPos, 
                                                g.m_hd.transform.TransformDirection(g.m_strands[i].rotation * Vector3.forward), 
                                                HairDesignerEditor.m_meshCollider.transform.TransformDirection(g.m_strands[i].rotation * Vector3.up), 
                                                HairDesignerEditor.m_meshCollider.transform, 
                                                g.m_hd.transform, 
                                                g.m_strands[i].meshTriId,
                                                HairDesignerEditor.m_meshCollider.sharedMesh.vertices, 
                                                HairDesignerEditor.m_meshCollider.sharedMesh.triangles, 
                                                true);

                                    }
                                    g.m_skinningVersion160_Converted = true;
                                }

                                if(g.m_hd.m_smr != null)
                                    TPoseModeChoose(g.m_hd);
                            }
                            else
                            {
                                if (GUILayout.Button("Revert from version 1.6"))
                                {
                                    for (int i = 0; i < g.m_strands.Count; ++i)
                                    {
                                        g.m_strands[i].triLock = null;
                                    }
                                    g.m_skinningVersion160 = false;
                                    m_hairDesignerEditor.CreateColliderAndTpose(g.m_skinningVersion160);
                                    g.m_skinningVersion160_Converted = false;
                                    //HairDesignerEditor.
                                }
                            }

                            
                        }

                        GUILayout.Label(new GUIContent("Layer parameters", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                        g.m_scale = EditorGUILayout.FloatField(new GUIContent("Global scale","strand scale for this layer"), 100f * g.m_scale / g.strandScaleFactor) * g.strandScaleFactor / 100f;
                        g.m_scale = Mathf.Clamp(g.m_scale, 0, g.m_scale);

                        g.m_strandSpacing = EditorGUILayout.FloatField("Min spacing", 100f* g.m_strandSpacing / g.strandScaleFactor) * g.strandScaleFactor / 100f;
                        g.m_strandSpacing = Mathf.Clamp(g.m_strandSpacing, 0, g.m_strandSpacing);

                        g.m_atlasSizeX = EditorGUILayout.IntSlider("Atlas size", g.m_atlasSizeX, 1, 10);

                        //g.m_allowFastMeshDraw = EditorGUILayout.Toggle("Fast draw", g.m_allowFastMeshDraw);
                        //g.m_allowFastMeshDraw = false;//fast draw is finaly slower :p


                        GUILayout.Label(new GUIContent("Strand shape", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);

                        g.m_params.m_taper = EditorGUILayout.Vector2Field("Taper", g.m_params.m_taper);
                        g.m_params.m_bendX = EditorGUILayout.FloatField("Bend", g.m_params.m_bendX);
                        g.m_params.m_HairResolutionX = Mathf.Clamp(EditorGUILayout.IntField("Strand subdivision X", g.m_params.m_HairResolutionX), 1, 20);
                        g.m_params.m_HairResolutionY = Mathf.Clamp(EditorGUILayout.IntField("Strand subdivision Y", g.m_params.m_HairResolutionY), 1, 20);
                        g.m_params.m_randomSrandFactor = EditorGUILayout.FloatField("Random", g.m_params.m_randomSrandFactor);
                        g.m_params.m_length = Mathf.Clamp(EditorGUILayout.FloatField("Length", g.m_params.m_length), 0, 100);

                        float old = g.m_params.m_normalToTangent;
                        g.m_params.m_normalToTangent = EditorGUILayout.FloatField("Normal switch", g.m_params.m_normalToTangent);
                        if (g.m_params.m_normalToTangent != old)
                            g.m_hairStrand = null;

                        g.m_params.m_gravityFactor = EditorGUILayout.FloatField("Gravity", g.m_params.m_gravityFactor);

                        

                        GUILayout.Label(new GUIContent("Strand curve", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);
                        
                        g.m_params.m_curv.m_startPosition = EditorGUILayout.Vector3Field("Start position", g.m_params.m_curv.m_startPosition);
                        g.m_params.m_curv.m_startTangent = EditorGUILayout.Vector3Field("Start tangent", g.m_params.m_curv.m_startTangent);
                        g.m_params.m_curv.m_endPosition = EditorGUILayout.Vector3Field("End position", g.m_params.m_curv.m_endPosition);
                        g.m_params.m_curv.m_endTangent = EditorGUILayout.Vector3Field("End tangent", g.m_params.m_curv.m_endTangent);


                        if (g.m_hairStrand != null)
                        {
                            int strandCount = 0;
                            for (int i= 0; i < g.m_strands.Count; ++i)
                            {
                                if (g.m_editorLayers[g.m_strands[i].layer].visible)
                                    strandCount += 1;
                            }

                            int triCount = (g.m_hairStrand.triangles.Length / 3) * strandCount;
                            if (triCount < 30000)
                                EditorGUILayout.HelpBox("Mesh info : " + strandCount + " strands | " + triCount + " tri", MessageType.Info);
                            else if (triCount < 65208)
                                EditorGUILayout.HelpBox("You should reduce poly count!\n Check Strand subdivisions or remove some strand\n Mesh info : " + strandCount + " strands | " + triCount + " tri", MessageType.Warning);
                            else
                                EditorGUILayout.HelpBox("Too many polygons!\n Reduce Strand resolution or remove some strand\n Mesh info : " + strandCount + " strands | " + (g.m_hairStrand.triangles.Length / 3) * strandCount + " tri", MessageType.Error);
                        }


                        

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Clear layer", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH)))))
                        {
                            if (EditorUtility.DisplayDialog("Clear strands", "Delete all strands in layer "+(m_currentEditorLayer+1) +" ?", "Ok", "Cancel"))
                            {
                                Undo.RecordObject(g, "clear strands");
                                for( int id=0; id< g.m_strands.Count; ++id  )
                                {
                                    if (g.m_strands[id].layer == m_currentEditorLayer)
                                        g.m_strands.RemoveAt(id--);
                                }
                                
                            }
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        


                        GUILayout.Space(20);

                        EditorGUILayout.HelpBox("Layer is unlocked\nAlways lock a layer when painting is over.\nBlendshapes are only available for locked layers.", MessageType.Info);

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Lock layer", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.LOCKED)))))
                        {
                            g.m_generateBlendshapes = true;
                            g.GenerateMeshRenderer();
                            //EditorUtility.SetDirty(target);
                            g.m_meshLocked = true;
                            m_tab = eTab.MATERIAL;
                            //g.m_shaderNeedUpdate = true;                            
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        

                        GUILayout.Space(20);

                        if (!g.m_materialInitialized && g.m_matPropBlkHair != null )
                        {                           
                            DrawMaterialSettings();
                            g.m_materialInitialized = true;                            
                        }

                        if (GUI.changed)
                            g.m_needMeshRebuild = true;

                    }

                    
                }
                else
                {
                    if (g.m_meshInstance == null)
                    {
                        g.GenerateMeshRenderer();
                        //EditorUtility.SetDirty(target);
                    }
                }

               

                if (m_tab == eTab.MATERIAL)
                {
                    EnableSceneTool();

                    if (m_hairDesignerEditor != null)
                    {
                        m_hairDesignerEditor.RestorePose();
                        if(HairDesignerEditor.m_meshCollider != null)
                            DestroyImmediate(HairDesignerEditor.m_meshCollider.gameObject);
                    }
                    g.m_requirePainting = false;
                    HairDesignerEditor.m_showSceneMenu = false;
                    hideWireframe = true;
                    DrawMaterialSettings();
                }

                

                    //m_showMotionZone = false;

                if (m_tab == eTab.MOTION)
                {
                    EnableSceneTool();

                    if (m_hairDesignerEditor != null && !Application.isPlaying )
                        m_hairDesignerEditor.CreateColliderAndTpose(g.m_skinningVersion160);

                    hideWireframe = true;
                    HairDesignerEditor.m_showSceneMenu = false;
                    //m_showMotionZone = true;
                                        
                    MotionZoneInspectorGUI();
                }
                
            }

            

            void MotionZoneInspectorGUI()
            {
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;               


                for (int i = 0; i < g.m_motionZones.Count; ++i)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent("Motion zones " + (i+1), (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BULLET))), EditorStyles.boldLabel);
                    if (GUILayout.Button(HairDesignerEditor.Icon(HairDesignerEditor.eIcons.TRASH), GUILayout.MaxWidth(25)))
                    {
                        if (EditorUtility.DisplayDialog("Motion zone", "Delete motion zone", "Ok", "Cancel"))
                        {
                            Undo.RecordObject(g, "Delete motion zone");
                            g.m_motionZones.RemoveAt(i--);
                        }
                    }
                    GUILayout.EndHorizontal();


                    GUILayout.Label("Position & hierachy", EditorStyles.boldLabel);
                    g.m_motionZones[i].parent = EditorGUILayout.ObjectField("Parent", g.m_motionZones[i].parent, typeof(Transform), true) as Transform;


                    Transform bone = SelectBone(g.m_motionZones[i].parent);

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

            


            /// <summary>
            /// DrawSceneMenu
            /// </summary>
            /// <param name="width"></param>
            public override void DrawSceneMenu( float width)
            {
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;

                GUILayout.BeginHorizontal();
                GUI.color = m_currentTool == ePaintingTool.ADD ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Paint", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.PAINT)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.ADD;
                }

                GUI.color = m_currentTool == ePaintingTool.BRUSH ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Brush", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.BRUSH)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.BRUSH;
                }
                

                GUI.color = m_currentTool == ePaintingTool.SCALE ? Color.white : HairDesignerEditor.unselectionColor;
                if (GUILayout.Button(new GUIContent("Scale", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.SCALE)), GUILayout.MaxWidth(width)))
                {
                    m_currentTool = ePaintingTool.SCALE;
                }
                

                GUILayout.EndHorizontal();


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

                GUILayout.BeginHorizontal();
                GUILayout.Label("Falloff", GUILayout.MinWidth(width * .3f));
                m_brushFalloff = EditorGUILayout.Slider(m_brushFalloff, 0f, 1f, GUILayout.MaxWidth(width * .7f));
                GUILayout.EndHorizontal();

                if (m_currentTool == ePaintingTool.ADD || m_currentTool == ePaintingTool.SCALE)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Scale", GUILayout.MinWidth(width * .3f));
                    m_brushScale = EditorGUILayout.Slider(m_brushScale, 0f, 2f, GUILayout.MaxWidth(width * .7f));
                    GUILayout.EndHorizontal();
                }


                if (m_currentTool == ePaintingTool.ADD || m_currentTool == ePaintingTool.BRUSH)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Raise", GUILayout.MinWidth(width * .3f));
                    m_brushRaise = EditorGUILayout.Slider(m_brushRaise, 0f, 1f, GUILayout.MaxWidth(width * .7f));
                    GUILayout.EndHorizontal();


                    if (m_currentTool == ePaintingTool.ADD)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Spacing", GUILayout.MinWidth(width * .3f));
                        m_brushSpacing = EditorGUILayout.Slider(m_brushSpacing, 0f, 1f, GUILayout.MaxWidth(width * .7f));
                        GUILayout.EndHorizontal();

                    }


                    GUILayout.Space(20);
                    m_brushRandomAtlasId = EditorGUILayout.Toggle("Random atlas Id", m_brushRandomAtlasId);

                    if(m_brushRandomAtlasId)
                    {
                        m_brushAtlasID = -1;
                        GUI.enabled = false;
                    }
                    

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Atlas Id", GUILayout.MinWidth(width * .3f));
                    if (!m_brushRandomAtlasId)
                        m_brushAtlasID = EditorGUILayout.IntSlider(m_brushAtlasID, 0, (g.m_shaderAtlasSize * g.m_shaderAtlasSize) - 1, GUILayout.MaxWidth(width * .7f));
                    else
                        GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Atlas size", GUILayout.MinWidth(width * .3f));
                    g.m_shaderAtlasSize = EditorGUILayout.IntSlider(g.m_shaderAtlasSize, 1, 10, GUILayout.MaxWidth(width * .7f));
                    GUILayout.EndHorizontal();

                    GUI.enabled = true;

                    if (m_currentTool == ePaintingTool.ADD)
                    {
                        GUILayout.Space(20);

                        if (GUILayout.Button("Fill all mesh"))
                        {
                            FillAllMesh();
                        }
                    }
                }

                
                //m_strandSpacing = EditorGUILayout.FloatField("Hair spacing", m_strandSpacing);



                switch (m_currentTool)
                {
                    case ePaintingTool.ADD:
                        GUILayout.Space(20);
                        EditorGUILayout.HelpBox("Ctrl : remove mode\nShift : brush mode", MessageType.Info);
                        //EditorGUILayout.HelpBox("tips : Increase brush intensity to reduce the spacing between strands", MessageType.Info);
                        break;

                    case ePaintingTool.BRUSH:

                        //Here switch root/tip
                        m_brushRoot = true;
                        /*
                        //WIP
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Target", GUILayout.Width(width * .3f));
                        GUI.color = m_brushRoot ? Color.white:Color.grey;
                        if (GUILayout.Button("Root", EditorStyles.miniButtonLeft))
                            m_brushRoot = true;
                        GUI.color = !m_brushRoot ? Color.white : Color.grey;
                        if (GUILayout.Button("Tip", EditorStyles.miniButtonRight))
                            m_brushRoot = false;
                        GUI.color = Color.white;
                        GUILayout.EndHorizontal();
                        */

                        GUILayout.Space(20);
                        EditorGUILayout.HelpBox("Ctrl  : raise the hair\nShift : smooth direction\nCtrl+Shift : Atlas Id", MessageType.Info);
                        break;

                    case ePaintingTool.SCALE:
                        GUILayout.Space(20);
                        EditorGUILayout.HelpBox("Ctrl  : scale down\nShift : smooth", MessageType.Info);
                        break;
                }


                

            }



            public override void DrawSceneTools()
            {
                if (m_tab == eTab.DESIGN)
                    DrawLayerPanel();

                if (m_tab == eTab.MOTION)
                    MotionZoneSceneGUI();
               
            }

            



            /// <summary>
            /// PaintToolAction
            /// </summary>
            /// <param name="svCam"></param>
            public override void PaintToolAction()
            {

                

                Camera svCam = SceneView.currentDrawingSceneView.camera;
                if (Event.current.alt || HairDesignerEditor.m_meshCollider == null)
                    return;

                m_CtrlMode = Event.current.control;
                m_ShiftMode = Event.current.shift;
                m_AltMode = Event.current.alt;

                HairDesignerBase hd = (target as HairDesignerGenerator).m_hd;
                HairDesignerGeneratorMeshBase g = (target as HairDesignerGeneratorMeshBase);

                if (hd == null)
                    return;

                if (g.m_editorLayers.Count < m_currentEditorLayer || m_currentEditorLayer == -1)
                    return;

                if (!g.m_editorLayers[m_currentEditorLayer].visible)
                    return;


                int strandCounMax = (int)Mathf.Lerp( 1f,10f, m_brushIntensity);


                if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    g.m_BrushLastStrandPositions.Clear();                    
                }

                for (int i = 0; i < strandCounMax; ++i)
                {


                    Vector3 mp = Event.current.mousePosition * EditorGUIUtility.pixelsPerPoint;
                    mp.y = svCam.pixelHeight - mp.y;

                    if (m_currentTool == ePaintingTool.ADD)
                    {
                        float pixelSize = Vector3.Distance(svCam.WorldToScreenPoint(svCam.transform.position + svCam.transform.forward), svCam.WorldToScreenPoint(svCam.transform.position + svCam.transform.forward + svCam.transform.right * m_brushSize));
                        Vector2 rnd = Random.insideUnitCircle * pixelSize;
                        mp.x += rnd.x;
                        mp.y += rnd.y;
                    }

                    Ray r = svCam.ScreenPointToRay(mp);




                    //Setup painting info
                    StrandData dt = new StrandData();
                    dt.layer = m_currentEditorLayer;
                    Vector2 mp2d = Event.current.mousePosition * EditorGUIUtility.pixelsPerPoint;
                    mp2d.y = svCam.pixelHeight - mp2d.y;

                    BrushToolData bt = new BrushToolData();
                    bt.mousePos = mp2d;
                    bt.transform = hd.transform;
                    bt.tool = m_currentTool;
                    bt.cam = svCam;
                    bt.brushScale = m_brushScale;
                    bt.brushSpacing = m_brushSpacing;
                    bt.CtrlMode = m_CtrlMode;
                    bt.ShiftMode = m_ShiftMode;
                    bt.brushSize = m_brushSize;
                    bt.brushScreenDir = (svCam.transform.right * Event.current.delta.x - svCam.transform.up * Event.current.delta.y).normalized;
                    bt.raise = m_brushRaise;
                    bt.brushFalloff = m_brushFalloff;
                    bt.brushIntensity = m_brushIntensity;
                    //if (m_currentTool == ePaintingTool.ADD)
                    //if (Event.current.type == EventType.MouseDrag )

                    {
                        Vector3 worldPos = Vector3.zero;
                        RaycastHit[] hitInfos = Physics.RaycastAll(r);

                        //Debug.DrawRay(r.origin, r.direction, Color.blue, 10f);

                        foreach (RaycastHit hitInfo in hitInfos)
                        {

                            if (hitInfo.collider.gameObject != HairDesignerEditor.m_meshCollider.gameObject)
                                continue;//get only our custom collider

                            worldPos = hitInfo.point;


                            if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                                m_lastHairPos = worldPos;

                            dt.normal = hd.transform.InverseTransformDirection(hitInfo.normal);
                            dt.rotation = Quaternion.FromToRotation(Vector3.forward, hd.transform.InverseTransformDirection(hitInfo.normal));


                            if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            {
                                Undo.RegisterCompleteObjectUndo(g, "hairDesigner brush");
                                EditorUtility.SetDirty(g);
                                return;
                            }

                            if (!Event.current.alt && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                            {
                                /*
                                if (m_triId2Strand == null)
                                {
                                    m_triId2Strand = new Dictionary<int, StrandData>();
                                    for (int i=0;i<g.m_strands.Count; ++i)
                                    {
                                        m_triId2Strand.Add(g.m_strands[i].meshTriId, g.m_strands[i]);
                                    }
                                }
                                */


                                dt.localpos = hd.transform.InverseTransformPoint(worldPos);
                                dt.worldpos = worldPos;
                                dt.meshTriId = hitInfo.triangleIndex;
                                /*
                                if (m_triId2Strand.ContainsKey(dt.meshTriId))
                                {
                                    bt.selectedStrands = new List<StrandData>();
                                    bt.selectedStrands.Add(m_triId2Strand[dt.meshTriId]);
                                }*/

                                bt.brushDir = hd.transform.InverseTransformDirection(worldPos - m_lastHairPos).normalized;
                                bt.brushRoot = m_brushRoot;
                                bt.collider = HairDesignerEditor.m_meshCollider;
                                bt.colliderTriangles = HairDesignerEditor.m_meshColliderTriangles;
                                bt.colliderVertices = HairDesignerEditor.m_meshColliderVertices;
                                bt.worldNormal = hitInfo.normal;
                                bt.brushAtlasId = (m_brushAtlasID == -1) ? -1 : (float)m_brushAtlasID / (float)(g.m_shaderAtlasSize* g.m_shaderAtlasSize);
                                
                                hd.generator.PaintTool(dt, bt);
                                
                            }
                            //return;
                        }
                    }
                }
            }




            /// <summary>
            /// Delete the layer with all the  strands
            /// </summary>
            /// <param name="idx"></param>
            public override void DeleteEditorLayer(int idx)
            {
                base.DeleteEditorLayer(idx);
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;
                bool isEmpty = true;
                for (int i = 0; i < g.m_strands.Count; ++i)
                {
                    if (g.m_strands[i].layer == idx)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                bool delete = true;
                if (!isEmpty)
                {
                    delete = EditorUtility.DisplayDialog("Delete layer", "Remove all the strands in layer " + (idx + 1) + " ?", "ok", "cancel");
                }
                else
                {
                    delete = EditorUtility.DisplayDialog("Delete layer", "Remove layer" + (idx + 1) + " ?", "ok", "cancel");
                }

                if (delete)
                {
                    Undo.RecordObject(g, "Delete layer");//save generator for undo

                    for (int i = 0; i < g.m_strands.Count; ++i)
                    {
                        if (g.m_strands[i].layer == idx)
                        {
                            g.m_strands.RemoveAt(i--);
                            continue;
                        }

                        if (g.m_strands[i].layer > idx)
                            g.m_strands[i].layer--;//update layer idx                        
                    }

                    g.m_editorLayers.RemoveAt(idx);
                }
            }

            public override void OnLayerChange()
            {
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;
                DestroyImmediate(g.m_meshInstance);
                g.GenerateMeshRenderer();
            }





            void FillAllMesh()
            {
                HairDesignerGeneratorMeshBase g = target as HairDesignerGeneratorMeshBase;
                HairDesignerBase hd = (target as HairDesignerGenerator).m_hd;

                g.m_needMeshRebuild = true;
                g.DestroyMesh();

                BrushToolData bt = new BrushToolData();
                //bt.mousePos = mp2d;
                bt.transform = hd.transform;
                bt.tool = ePaintingTool.ADD;
                //bt.cam = svCam;
                bt.brushScale = m_brushScale;
                bt.brushSpacing = 0;// m_brushSpacing;
                bt.CtrlMode = m_CtrlMode;
                bt.ShiftMode = m_ShiftMode;
                bt.brushSize = m_brushSize;
                //bt.brushScreenDir = (svCam.transform.right * Event.current.delta.x - svCam.transform.up * Event.current.delta.y).normalized;
                bt.raise = m_brushRaise;
                bt.brushFalloff = m_brushFalloff;
                bt.brushIntensity = m_brushIntensity;

                //StrandData dt = new StrandData();
                //dt.layer = m_currentEditorLayer;

                Mesh mesh = null;
                if(hd.m_mf != null)
                {
                    mesh = hd.m_mf.sharedMesh;
                }
                else if(hd.m_smr != null)
                {
                    mesh = hd.m_smr.sharedMesh;
                }

                if (mesh == null)
                    return;

                int[] triangles = mesh.triangles;
                Vector3[] vertices = mesh.vertices;
                Vector3[] normals = mesh.normals;


                string title = "Generate strands";
                string info = "please wait";
                float progress = 0f;               


                for (int i = 0; i < (triangles.Length) / 3 - 1; ++i)
                {
                    progress = (float)i / (float)((triangles.Length) / 3);
                    if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }


                    int v0 = triangles[i * 3];
                    int v1 = triangles[i * 3 + 1];
                    int v2 = triangles[i * 3 + 2];

                    if (v2 >= vertices.Length)
                        Debug.Log("Error");


                    Vector3 V0 = vertices[v0];
                    Vector3 V1 = vertices[v1];
                    Vector3 V2 = vertices[v2];

                    Vector3 N0 = normals[v0];
                    Vector3 N1 = normals[v1];
                    Vector3 N2 = normals[v2];


                    StrandData data = new StrandData();

                    data.localpos = (V0 + V1 + V2) / 3f;
                    data.normal = (N0 + N1 + N2).normalized;
                    data.worldpos = hd.transform.TransformPoint(data.localpos);                    
                    
                    data.layer = m_currentEditorLayer;
                                        
                    //data.normal = (normals[v0] + normals[v1] + normals[v2]).normalized;
                    Vector3 worldNormal = hd.transform.TransformDirection(data.normal).normalized;

                    Quaternion q = Quaternion.LookRotation(data.normal, Quaternion.Euler(0f, 0f, 90f) * data.normal);
                    q = Quaternion.LookRotation(q * Vector3.forward, data.normal);
                    data.rotation = q;

                    data.meshTriId = i;
                    data.triLock = new TriangleLock();
                    data.triLock.m_faceId = i;


                    data.triLock.Lock(data.localpos,
                    data.normal,
                    data.normal,
                    HairDesignerEditor.m_meshCollider.transform,
                    hd.transform,
                    data.meshTriId,
                    vertices,
                    triangles,
                    false);

                    g.m_strands.Add(data);

                    Debug.DrawLine(data.worldpos, data.worldpos + hd.transform.TransformDirection(data.normal).normalized*.01f, Color.blue, 5);

                }
                EditorUtility.ClearProgressBar();

            }

        }

    }
}