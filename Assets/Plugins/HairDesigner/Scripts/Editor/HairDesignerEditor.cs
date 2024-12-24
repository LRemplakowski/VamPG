#define HAIRDESIGNER_ADVANCEDFUR_WIP

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;

#if UNITY_2019_1_OR_NEWER
using UnityEditor.EditorTools;
#endif


namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [CustomEditor(typeof(HairDesignerBase),true)]
        public class HairDesignerEditor : Editor
        {
            public enum eIcons
            {
                BANNER,
                DESIGN,
                MATERIAL,
                MOTION,
                PAINT,
                BRUSH,
                SCALE,
                TRASH,
                BULLET,
                LAYER,                
                SEPARATOR,
                LOCKED,
                UNLOCKED,
                SAVE,
                DUPLICATE,
                COLOR,
                VISIBLE,
                HIDDEN,
                MIRROR,
                CURVE
            }


            

            static List<Texture2D> m_icons = new List<Texture2D>();
            public static Texture2D Icon(eIcons icon)
            {
                while (m_icons.Count <= (int)icon)
                    m_icons.Add(null);

                if (m_icons[(int)icon] == null)
                    m_icons[(int)icon] = (Texture2D)Resources.Load("Icons/" + icon, typeof(Texture2D));
                return m_icons[(int)icon];
            }


            
            public static MeshCollider m_meshCollider = null;
            public static int[] m_meshColliderTriangles = null;
            public static Vector3[] m_meshColliderVertices = null;
            public static HairGeneratorEditor m_generatorEditor = null;

            
            //Quaternion[] m_boneRotations;
            public static bool m_showSceneMenu = false;

            static GameObject m_currentGameObject = null;

            
            bool m_TposeModeTrigger = false;
            bool m_addnewLayer = false;
            //bool m_debugDontHideCollider = false;
            bool m_TPoseUseUnityFunction = false;

            string colliderName = "HairDesignerCollider_Editor";

            Editor tmpEditor;
            HairDesignerRuntimeLayerBase m_runtimeLayer;




            public static bool m_lastposeMode = false;
            public void CreateColliderAndTpose( bool newSkinning )
            {
                HairDesignerBase hd = target as HairDesignerBase;

                if (m_meshCollider == null)
                {
                    //clean old colliders
                    foreach (HairDesignerTmp tmp in hd.transform.GetComponentsInChildren<HairDesignerTmp>())
                        //if (m_meshCollider.gameObject != tmp.gameObject)
                            DestroyImmediate(tmp.gameObject);
                }

                m_lastposeMode = newSkinning;
                if (!newSkinning)
                    CreateColliderAndTpose_OLD();
                else
                    CreateCollider();

                
            }


            public void RemoveCollider()
            {
                DestroyCollider();
            }

            public static void DestroyCollider()
            {
                if(m_meshCollider!=null)
                    DestroyImmediate(m_meshCollider.gameObject);
                m_meshCollider = null;
                //Debug.Log("RemoveCollider");
            }



            public void CreateCollider()
            {
                if (Application.isPlaying)
                    return;

                
                if ( m_meshCollider != null)
                    return;

                HairDesignerBase hd = target as HairDesignerBase;

                if (hd.generator != null)                
                    hd.generator.meshTmpData = null;//force to regenerate the complete mesh (needed in case of blendshape modification)
                

                GameObject go = new GameObject(colliderName);
                go.transform.SetParent(hd.transform, true);
                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;
                //go.transform.parent = null;

                m_meshCollider = go.AddComponent<MeshCollider>();
                SkinnedMeshRenderer smr = hd.GetComponent<SkinnedMeshRenderer>();


                go.AddComponent<HairDesignerTmp>();
                if (hd.GetComponent<MeshFilter>() != null)
                {
                    m_meshCollider.sharedMesh = hd.GetComponent<MeshFilter>().sharedMesh;
                    go.transform.localScale = Vector3.one;
                }
                else if (smr != null)
                {

                    if(smr.bones.Length == 0 )
                        go.transform.localScale = Vector3.one;                    

                    Mesh bakeMesh = new Mesh();
                    smr.BakeMesh(bakeMesh);


                    bakeMesh.name = "HairDesignerColliderEditor";
                    Vector3[] vertices = bakeMesh.vertices;
                    MeshUtility.ApplyBlendShape(bakeMesh, ref vertices);
                    bakeMesh.vertices = vertices;

                    m_meshCollider.sharedMesh = bakeMesh;
                }
                

                //unwrap for fur
                //UnwrapParam settings = new UnwrapParam();
                //m_meshCollider.sharedMesh.uv = Unwrapping.GeneratePerTriangleUV(m_meshCollider.sharedMesh, settings);



                m_meshColliderVertices = m_meshCollider.sharedMesh.vertices;
                m_meshColliderTriangles = m_meshCollider.sharedMesh.triangles;
                

                if (!hd.m_debugDontHideCollider)
                    go.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;
                else
                    go.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

            }















            public void CreateColliderAndTpose_OLD()
            {
                if (Application.isPlaying)
                    return;

                if (m_TposeModeTrigger && m_meshCollider!=null)
                    return;

                //Debug.Log("Create T-pose " + (m_TPoseUseUnityFunction?"Unity":"Custom") );
                //m_TPoseUseUnityFunction = true;

                m_TposeModeTrigger = true;
                HairDesignerBase hd = target as HairDesignerBase;

                if ( hd.m_debugDontHideCollider )
                    Debug.LogWarning("Debug Don't hide collider activated.");

                if (!Application.isPlaying && m_meshCollider == null && hd.generator != null)
                {

                    //string colliderName = "HairDesignerCollider_Editor";

                    //GameObject.Find(colliderName);
                    /*
                    foreach (MeshCollider mc in hd.GetComponentsInChildren<MeshCollider>())
                    {
                        if (mc.gameObject.name == "HairDesignerCollider")
                            DestroyImmediate(mc.gameObject);
                    }*/

                    if(m_meshCollider!=null)
                        DestroyImmediate(m_meshCollider.gameObject);

                    GameObject go = new GameObject(colliderName);
                    go.transform.SetParent(hd.transform, false);
                    m_meshCollider = go.AddComponent<MeshCollider>();
                    go.AddComponent<HairDesignerTmp>();
                    if (hd.GetComponent<MeshFilter>() != null)
                    {
                        m_meshCollider.sharedMesh = hd.GetComponent<MeshFilter>().sharedMesh;
                    }
                    else if (hd.GetComponent<SkinnedMeshRenderer>() != null)
                    {

                        SkinnedMeshRenderer smr = hd.GetComponent<SkinnedMeshRenderer>();
                        Mesh m = smr.sharedMesh;
                        m_meshCollider.sharedMesh = m;

                        if( hd.m_tpose== null)
                        {
                            hd.m_tpose = new TPoseUtility();
                        }
                        //m_TPoseUseUnityFunction = false;
                        hd.m_tpose.ApplyTPose(smr, m_TPoseUseUnityFunction);


                        Mesh bakeMesh = new Mesh();
                        smr.BakeMesh(bakeMesh);
                        if(m_TPoseUseUnityFunction)
                            m_meshCollider.sharedMesh = bakeMesh;

                        bool ColliserPositionOk = false;
                        int tryMaxSetPos = 200;
                        while (!ColliserPositionOk && tryMaxSetPos > 0)
                        {

                            int[] VertexVectorIds = new int[3];
                            Vector3[] dirBake = new Vector3[2];
                            dirBake[0] = Vector3.zero;
                            dirBake[1] = Vector3.zero;


                            int FindVerticesTryMax = 20;
                            while ((dirBake[0].magnitude == 0 || dirBake[1].magnitude == 0 || VertexVectorIds[1] == VertexVectorIds[2]) && FindVerticesTryMax > 0)
                            {
                                //Find 3 vertices for the rotation of the model
                                if (dirBake[0].magnitude == 0)
                                {
                                    VertexVectorIds[0] = (int)(Random.value * (float)bakeMesh.vertexCount);
                                    VertexVectorIds[1] = (int)(Random.value * (float)bakeMesh.vertexCount);
                                }

                                VertexVectorIds[2] = (int)(Random.value * (float)bakeMesh.vertexCount);
                                dirBake[0] = hd.transform.TransformDirection(bakeMesh.vertices[VertexVectorIds[0]] - bakeMesh.vertices[VertexVectorIds[1]]).normalized;
                                dirBake[1] = hd.transform.TransformDirection(bakeMesh.vertices[VertexVectorIds[0]] - bakeMesh.vertices[VertexVectorIds[2]]).normalized;
                                dirBake[1] = Vector3.Cross(dirBake[1], dirBake[0]).normalized;
                                FindVerticesTryMax--;
                            }

                            if (FindVerticesTryMax == 0)
                                Debug.LogWarning("Fail to fing 3 different vertices");


                            //rotate the collider to the skinned mesh position
                            Vector3 dirMesh = m_meshCollider.transform.TransformDirection(m_meshCollider.sharedMesh.vertices[VertexVectorIds[0]] - m_meshCollider.sharedMesh.vertices[VertexVectorIds[1]]).normalized;
                            m_meshCollider.transform.rotation = Quaternion.FromToRotation(dirMesh, dirBake[0]) * m_meshCollider.transform.rotation;
                            dirMesh = m_meshCollider.transform.TransformDirection(m_meshCollider.sharedMesh.vertices[VertexVectorIds[0]] - m_meshCollider.sharedMesh.vertices[VertexVectorIds[1]]).normalized;
                            Vector3 dirMesh2 = m_meshCollider.transform.TransformDirection(m_meshCollider.sharedMesh.vertices[VertexVectorIds[0]] - m_meshCollider.sharedMesh.vertices[VertexVectorIds[2]]).normalized;
                            dirMesh2 = Vector3.Cross(dirMesh2, dirMesh).normalized;
                            m_meshCollider.transform.rotation = Quaternion.FromToRotation(dirMesh2, dirBake[1]) * m_meshCollider.transform.rotation;

                            //smr.transform.localScale = scaleSmr;

                            //m_meshCollider.transform.localScale = Vector3.one * scale;





                            Vector3 v = bakeMesh.vertices[VertexVectorIds[0]];
                            Vector3 v2 = m_meshCollider.sharedMesh.vertices[VertexVectorIds[0]];

                            if (smr.bones.Length != 0)                                
                                m_meshCollider.transform.localScale = Vector3.one * (smr.rootBone.parent.lossyScale.magnitude / m_meshCollider.transform.parent.lossyScale.magnitude);


                            v.x /= hd.transform.lossyScale.x;
                            v.y /= hd.transform.lossyScale.y;
                            v.z /= hd.transform.lossyScale.z;

                            int c = 0;
                            //while (c < 100 && Vector3.Distance(hd.transform.TransformPoint(v), m_meshCollider.transform.TransformPoint(v2)) > 0.000001f)
                            {
                                m_meshCollider.transform.position += hd.transform.TransformPoint(v) - m_meshCollider.transform.TransformPoint(v2);
                                //m_meshCollider.transform.position += v - m_meshCollider.transform.TransformPoint(v2);
                                c++;
                            }

                            /*
                            //fix scale & position for some mesh configuration
                            if (Vector3.Distance(hd.transform.TransformPoint(v), m_meshCollider.transform.TransformPoint(v2)) != 0)
                            {
                                m_meshCollider.transform.position = Vector3.zero;
                                m_meshCollider.transform.localScale = Vector3.one;
                            }*/

                            if (smr.bones.Length == 0)
                            {
                                m_meshCollider.transform.localPosition = Vector3.zero;
                                m_meshCollider.transform.localScale = Vector3.one;
                            }


                            ColliserPositionOk = true;
                            for (int t = 0; t < 3; ++t)
                            {
                                v = bakeMesh.vertices[VertexVectorIds[t]];
                                v.x /= hd.transform.lossyScale.x;
                                v.y /= hd.transform.lossyScale.y;
                                v.z /= hd.transform.lossyScale.z;
                                v2 = m_meshCollider.sharedMesh.vertices[VertexVectorIds[t]];
                                if (Vector3.Distance(hd.transform.TransformPoint(v), m_meshCollider.transform.TransformPoint(v2)) > 0.00001f)
                                    ColliserPositionOk = false;
                            }

                            //if(ColliserPositionOk)
                            //    Debug.LogError("tryMaxSetPos : " + tryMaxSetPos);
                            tryMaxSetPos--;
                        }

                        if (tryMaxSetPos == 0)
                        {
                            if (m_TPoseUseUnityFunction)
                            {
                                Debug.LogWarning("HairDesigner : Fail to generate collider position");                                
                                m_TPoseUseUnityFunction = false;
                                m_TposeModeTrigger = false;
                                
                            }
                            else
                            {
                                //Debug.LogWarning("HairDesigner : use unity T-Pose");
                                m_TPoseUseUnityFunction = true;
                                m_TposeModeTrigger = false;
                                DestroyImmediate(m_meshCollider.gameObject);
                                hd.m_tpose.RevertTpose();
                                CreateColliderAndTpose(m_lastposeMode);
                                return;
                            }
                        }
                        else
                        {
                            //Debug.Log("Method " + (m_TPoseUseUnityFunction?"Unity T-pose":"Custom T-pose") );
                        }
                        hd.generator.m_skinMeshGenerationRot = m_meshCollider.transform.localRotation;
                        hd.generator.m_skinMeshGenerationPos = m_meshCollider.transform.localPosition;

                        //----------------------

                        m_meshColliderVertices = m_meshCollider.sharedMesh.vertices;
                        m_meshColliderTriangles = m_meshCollider.sharedMesh.triangles;

                    }
                    else
                    {
                        Debug.LogError("Hair designer require a SkinMeshRenderer or a MeshFilter component");
                    }

                    

                    //if (!m_debugDontHideCollider)
                    if (!hd.m_debugDontHideCollider)
                        //go.hideFlags = HideFlags.HideAndDontSave;
                        go.hideFlags = HideFlags.DontSaveInBuild|HideFlags.DontSaveInEditor|HideFlags.HideInHierarchy;




                    //Tools.current = Tool.None;
                }

                /*
                if(m_meshCollider!= null && m_meshCollider.transform.parent != null )
                    m_meshCollider.transform.parent = null;
                    */
            }




            public void RestorePose()
            {                
                if (!m_TposeModeTrigger)
                    return;                
                HairDesignerBase hd = target as HairDesignerBase;                
                m_TposeModeTrigger = false;

                /*
                if (!Application.isPlaying && m_meshCollider != null)
                {
                    DestroyImmediate(m_meshCollider.gameObject);
                    m_meshCollider = null;
                    if (target as HairDesignerBase != null)
                    {
                        SkinnedMeshRenderer smr = (target as HairDesignerBase).GetComponent<SkinnedMeshRenderer>();
                        if (smr != null )
                        {
                            for (int i = 0; i < smr.bones.Length; ++i)
                            {
                                if(smr.bones.Length>i && hd.m_poseBoneRotations.Length > i)
                                    smr.bones[i].localRotation = hd.m_poseBoneRotations[i];
                            }
                        }
                    }
                }
                */
                if (!Application.isPlaying && m_meshCollider != null)
                {
                    DestroyImmediate(m_meshCollider.gameObject);
                    m_meshCollider = null;
                    if (target as HairDesignerBase != null)
                    {
                        hd.m_tpose.RevertTpose();
                        //hd.m_tpose = null;
                    }
                }

                if (target as HairDesignerBase == null)
                    return;

                for (int i = 0; i < hd.m_generators.Count; ++i)
                {
                    if (hd.m_generators[i] == null)
                        continue;

                    if (!hd.m_generators[i].m_wasInDesignTab )
                        continue;

                    hd.m_generators[i].m_wasInDesignTab = false;
                    hd.m_generators[i].GenerateMeshRenderer();
                    if (hd.m_generators[i].m_meshInstance != null)
                    {
                        MeshRenderer mr = hd.m_generators[i].m_meshInstance.GetComponent<MeshRenderer>();
                        if (mr != null)
#if UNITY_5_5_OR_NEWER
                            EditorUtility.SetSelectedRenderState(mr, EditorSelectedRenderState.Hidden);
#else
                            EditorUtility.SetSelectedWireframeHidden(mr, true);
#endif

                        SkinnedMeshRenderer smr = hd.m_generators[i].m_meshInstance.GetComponent<SkinnedMeshRenderer>();
                        if (smr != null)
#if UNITY_5_5_OR_NEWER
                            EditorUtility.SetSelectedRenderState(mr, EditorSelectedRenderState.Hidden);
#else
                            EditorUtility.SetSelectedWireframeHidden(smr, true);
#endif
                    }
                    EditorUtility.SetDirty(hd.m_generators[i]);
                    //Undo.RecordObject(hd.m_generators[i], "Hair Designer Generate mesh");
                }

                CleanTmpObjects();
            }


            void CleanTmpObjects()
            {
                foreach (HairDesignerTmp tmp in FindObjectsOfType<HairDesignerTmp>())
                    DestroyImmediate(tmp.gameObject);
            }

            public static GUIStyle bgStyle = null;
            public static GUIStyle bgStyleLogo = null;
            public static GUIStyle bgStyleAlpha = null;
            public static Color unselectionColor = new Color(.65f,.65f,.65f);
            public static Color mainColor = new Color(148f/255f,16f/255f,16f/255f);

            bool m_hiddenScriptsCleaned = false;







            /// <summary>
            /// LoadDefaultMaterials
            /// </summary>
            /// <param name="g"></param>
            public void LoadDefaultMaterials(HairDesignerGenerator g)
            {
                Material newMat = g.m_hairMeshMaterial;
                Material newMatTransparent = g.m_hairMeshMaterialTransparent;

                eRP rp = GetRenderPipeline();

                switch (g.m_layerType)
                {
                    case HairDesignerBase.eLayerType.SHORT_HAIR_POLY:
                    case HairDesignerBase.eLayerType.LONG_HAIR_POLY:

                        if (rp == eRP.HDRP)
                        {
                            newMat = Resources.Load("HD_HDRP_Atlas", typeof(Material)) as Material;
                            g.m_hairMeshMaterialTransparent = null;
                            newMatTransparent = null;
                        }

                        if (rp == eRP.URP)
                        {
                            newMat = Resources.Load("HD_URP_Atlas", typeof(Material)) as Material;
                            g.m_hairMeshMaterialTransparent = null;
                            newMatTransparent = null;
                        }

                        if (rp == eRP.STANDARD)
                        {
                            newMat = Resources.Load("HD_TextureAtlas", typeof(Material)) as Material;
                            newMatTransparent = Resources.Load("HD_TextureAtlas_transparent", typeof(Material)) as Material;
                        }
                       

                        if (newMat != null) g.m_hairMeshMaterial = newMat;
                        if (newMatTransparent != null) g.m_hairMeshMaterialTransparent = newMatTransparent;

                        break;

                    case HairDesignerBase.eLayerType.FUR_SHELL:


                        if (rp == eRP.HDRP)
                            newMat = Resources.Load("HD_HDRP_FurShell", typeof(Material)) as Material;

                        if (rp == eRP.URP)
                            newMat = Resources.Load("HD_URP_FurShell", typeof(Material)) as Material;

                        if (rp == eRP.STANDARD)
                            newMat = Resources.Load("HD_FurShell", typeof(Material)) as Material;



                        if (newMat != null)
                            g.m_hairMeshMaterial = newMat;
                        break;


#if HAIRDESIGNER_ADVANCEDFUR_WIP
                    case HairDesignerBase.eLayerType.FUR_GEOMETRY:
                        g.m_hairMeshMaterial = Resources.Load("HD_AdvancedFur", typeof(Material)) as Material;
                        ((HairDesignerGeneratorAdvancedFurBase)g).m_furShellMaterial = Resources.Load("HD_AdvancedFur_Shell", typeof(Material)) as Material;
                        break;
#endif
                }

            }


#if UNITY_2019_1_OR_NEWER
            static int[] m_unityVersion = null;
#endif	
            public static void EnableSceneViewGizmos()
            {				
#if UNITY_2019_1_OR_NEWER				
                if (SceneView.lastActiveSceneView != null)
                {					
                    if (m_unityVersion == null)
                    {						
                        m_unityVersion = new int[3];
                        m_unityVersion[0] = int.Parse(Application.unityVersion.Split('.')[0]);
                        m_unityVersion[1] = int.Parse(Application.unityVersion.Split('.')[1]);
                    }
                    if (m_unityVersion[0] >= 2019 && m_unityVersion[1] >= 3)
                        SceneView.lastActiveSceneView.drawGizmos = true;
                }
#endif				
            }



            /// <summary>
            /// 
            /// </summary>
            public override void OnInspectorGUI()
            {
                HairDesignerEditorUtility.EditorHeader();


                HairDesignerBase hd = target as HairDesignerBase;


                if(hd.m_smr == null && hd.m_mr == null)
                {
                    EditorGUILayout.HelpBox("HairDesigner require a MeshRenderer or a SkinnedMeshRender", MessageType.Error);
                    return;
                }

#if UNITY_2018_3_OR_NEWER
                if (PrefabUtility.IsPartOfPrefabAsset(hd))
#else

                if (PrefabUtility.GetPrefabParent(hd.gameObject) == null && PrefabUtility.GetPrefabObject(hd.gameObject) != null)
#endif

                {
                    GUILayout.Label("Instantiate the prefab for modifications", EditorStyles.helpBox);
                    return;
                }


                CreateGeneratorEditor(hd);

                m_currentGameObject = hd.gameObject;
                if (hd.m_generators.Count == 0 && !m_hiddenScriptsCleaned)
                    CleanHiddenScripts();

                CleanGeneratorScripts();

                if (bgStyle == null)
                {
                    bgStyle = new GUIStyle();
                    bgStyle.normal.background = new Texture2D(1, 1);
                    bgStyle.normal.background.SetPixel(0, 0, Color.grey);
                    bgStyle.normal.background.Apply();
                }

                if (bgStyleLogo == null)
                {
                    bgStyleLogo = new GUIStyle();
                    bgStyleLogo.normal.background = new Texture2D(1, 1);
                    bgStyleLogo.normal.background.SetPixel(0, 0, Color.white);
                    bgStyleLogo.normal.background.Apply();
                }

                if (bgStyleAlpha == null)
                {
                    bgStyleAlpha = new GUIStyle(EditorStyles.helpBox);
                    
                    bgStyleAlpha.normal.background = new Texture2D(1, 1);
                    bgStyleAlpha.normal.background.SetPixel(0, 0, new Color(.5f,.5f,.5f,.9f));
                    bgStyleAlpha.normal.background.Apply();

                }


                if (hd.m_version != HairDesignerBase.version)
                {
                    //do upgrade here if needed
                    CleanBones();
                }

                GUILayout.Space(5);
                GUILayout.BeginHorizontal(bgStyleLogo);
                GUILayout.FlexibleSpace();
                GUILayout.Label(Icon(eIcons.BANNER));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

/*
#if UNITY_5_5_OR_NEWER
                EditorUtility.SetSelectedRenderState(hd.GetComponent<MeshRenderer>(), EditorSelectedRenderState.Highlight );
                EditorUtility.SetSelectedRenderState(hd.GetComponent<SkinnedMeshRenderer>(), EditorSelectedRenderState.Highlight);                
#else
                EditorUtility.SetSelectedWireframeHidden(hd.GetComponent<MeshRenderer>(), false );
                EditorUtility.SetSelectedWireframeHidden(hd.GetComponent<SkinnedMeshRenderer>(), false);
#endif
*/
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Layers", HairDesignerEditor.Icon(HairDesignerEditor.eIcons.LAYER), "Use different layers to setup the haircut"), EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label("version " + HairDesignerBase.version, EditorStyles.centeredGreyMiniLabel);
                GUILayout.EndHorizontal();
                bool needToSaveLayers = false;
                bool allLocked = true;
                for (int i = 0; i < hd.m_generators.Count; ++i)
                {
                    if (hd.m_generators[i] == null)
                    {
                        hd.m_generators.RemoveAt(i--);//remove null (just in case)
                        continue;
                    }


                    
                    GUILayout.BeginHorizontal(bgStyle);
                    bool e = EditorGUILayout.Toggle(hd.m_generators[i].IsActive, GUILayout.MaxWidth(20));
                    if( e != hd.m_generators[i].IsActive)
                    {                        
                        hd.m_generators[i].SetActive(e);
                    }
                    GUI.color = hd.m_generatorId == i ? Color.white : unselectionColor;
                    if (GUILayout.Button(hd.m_generators[i].m_name))
                    {
                        hd.m_generatorId = hd.m_generatorId!=i ? i : -1;
                    }
                    GUI.color = Color.white;
                    GUILayout.Label(hd.m_generators[i].m_meshLocked ? Icon(eIcons.LOCKED):Icon(eIcons.UNLOCKED), GUILayout.MaxWidth(15));
                    GUILayout.EndHorizontal();

                    

                    if (!hd.m_generators[i].m_meshLocked )
                        //&& !(hd.m_generators[i].m_layerType == HairDesignerBase.eLayerType.FUR_SHELL || hd.m_generators[i].m_layerType == HairDesignerBase.eLayerType.FUR_GEOMETRY) )
                        allLocked = false;

                    if (hd.m_generators[i].m_layerType == HairDesignerBase.eLayerType.SHORT_HAIR_POLY ||
                        hd.m_generators[i].m_layerType == HairDesignerBase.eLayerType.LONG_HAIR_POLY )
                        needToSaveLayers = true;

                    //hide inspectors
                    /*
                    hd.m_generators[i].hideFlags = HideFlags.HideInInspector;
                    for (int j = 0; j < hd.m_generators[i].m_shaderParams.Count; ++j)
                        hd.m_generators[i].m_shaderParams[j].hideFlags = HideFlags.HideInInspector;
                    */
                }


                //if (GUILayout.Button("+ New layer +", GUILayout.MinHeight(40)))
                if (!Application.isPlaying)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (!m_addnewLayer)
                    {
                        bool showNewLayerButton = true;

                        eRP rp = GetRenderPipeline();
                        if (rp == eRP.HDRP )
                        {
                            Material mat = Resources.Load("HD_HDRP_Atlas", typeof(Material)) as Material;
                            if (mat == null)
                                showNewLayerButton = false;
                        }


                        if (rp == eRP.URP)
                        {
                            Material mat = Resources.Load("HD_URP_Atlas", typeof(Material)) as Material;
                            if (mat == null)
                                showNewLayerButton = false;
                        }

                        if (showNewLayerButton)
                        {
                            if (GUILayout.Button(new GUIContent("New layer", (Icon(eIcons.LAYER)))))
                            {
                                m_addnewLayer = !m_addnewLayer;
                            }
                        }
                        else
                        {
                            if (rp == eRP.HDRP)
                                EditorGUILayout.HelpBox("HDRP requires specific shaders.\nPlease install the HDRP addon.\n'Assets/HairDesigner/Addon'", MessageType.Error);
                            if (rp == eRP.URP)
                                EditorGUILayout.HelpBox("URP requires specific shaders.\nPlease install the URP addon.\n'Assets/HairDesigner/Addon'", MessageType.Error);
                        }
                    }


                    HairDesignerBase.eLayerType lt = HairDesignerBase.eLayerType.NONE;

                    if (m_addnewLayer)
                    {
                        float btnWidth = 120;
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Choose a layer type", EditorStyles.centeredGreyMiniLabel);
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button("X"))
                            m_addnewLayer = !m_addnewLayer;
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Short hair/fur\n(Polygons)"),GUILayout.Width(btnWidth)))
                            lt = HairDesignerBase.eLayerType.SHORT_HAIR_POLY;
                        
                        if (GUILayout.Button(new GUIContent("Long hair\n(Polygons)"), GUILayout.Width(btnWidth)))
                            lt = HairDesignerBase.eLayerType.LONG_HAIR_POLY;
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Fur\n(Shells)"), GUILayout.Width(btnWidth)))
                            lt = HairDesignerBase.eLayerType.FUR_SHELL;


                        GUI.enabled = GetRenderPipeline() == eRP.STANDARD;
#if HAIRDESIGNER_ADVANCEDFUR_WIP
                        if (GUILayout.Button(new GUIContent("Advanced Fur\n(DX11 Geometry)"), GUILayout.Width(btnWidth)))
                            lt = HairDesignerBase.eLayerType.FUR_GEOMETRY;
#endif
                        GUI.enabled = true;

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(20);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Load a runtime layer", EditorStyles.centeredGreyMiniLabel);
                        GUILayout.FlexibleSpace();                        
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        m_runtimeLayer = EditorGUILayout.ObjectField(m_runtimeLayer, typeof(HairDesignerRuntimeLayerBase), false) as HairDesignerRuntimeLayerBase;
                        GUI.enabled = m_runtimeLayer != null;
                        if (GUILayout.Button("Load"))
                        {
                            m_runtimeLayer.GenerateLayers(hd);
                            m_addnewLayer = !m_addnewLayer;
                        }
                        GUILayout.EndHorizontal();
                        GUI.enabled = true;
                        GUILayout.Space(10);
                        GUILayout.EndVertical();

                        GUILayout.BeginHorizontal();


                       

                        
                    }


                    if (lt != HairDesignerBase.eLayerType.NONE)
                    {

                        m_addnewLayer = false;
                        Undo.RecordObject(hd, "add layer");

                        HairDesignerGenerator g = null;

                        if (lt == HairDesignerBase.eLayerType.SHORT_HAIR_POLY)
                        {
                            g = hd.gameObject.AddComponent<HairDesignerGeneratorMeshBase>();
                            HairGeneratorMeshEditor.m_tab = HairGeneratorMeshEditor.eTab.MATERIAL;
                        }
                        if (lt == HairDesignerBase.eLayerType.LONG_HAIR_POLY)
                        {
                            g = hd.gameObject.AddComponent<HairDesignerGeneratorLongHairBase>();
                            HairGeneratorLongHairBaseEditor.m_tab = HairGeneratorLongHairBaseEditor.eTab.MATERIAL;                            
                        }
                        if (lt == HairDesignerBase.eLayerType.FUR_SHELL)
                        {
                            g = hd.gameObject.AddComponent<HairDesignerGeneratorFurShellBase>();
                            HairDesignerGeneratorFurShellBaseEditor.m_tab = HairDesignerGeneratorFurShellBaseEditor.eTab.MATERIAL;
                        }
#if HAIRDESIGNER_ADVANCEDFUR_WIP
                        if (lt == HairDesignerBase.eLayerType.FUR_GEOMETRY)
                        {
                            g = hd.gameObject.AddComponent<HairDesignerGeneratorAdvancedFurBase>();                            
                            HairDesignerGeneratorAdvancedFurBaseEditor.m_tab = HairDesignerGeneratorAdvancedFurBaseEditor.eTab.MATERIAL;
                        }
#endif
                        if (g != null)
                        {
                            g.hideFlags = HideFlags.HideInInspector;
                            g = ReplaceBaseClass(g) as HairDesignerGenerator;
                            g.m_layerType = lt;                            
                            g.m_skinningVersion160 = true;
                            g.m_version = HairDesignerBase.version;
                            hd.m_meshSaved = false;
                            hd.m_generators.Add(g);
                            hd.m_generatorId = hd.m_generators.Count - 1;
                            hd.m_generators[hd.m_generatorId].m_hd = hd;
                            LoadDefaultMaterials(g);
                            hd.m_generators[hd.m_generatorId].m_name = "Layer " + (hd.m_generators.Count);

                            EditorUtility.SetDirty(hd.generator);
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                

                    GUILayout.Space(5);

                    if(hd.m_meshSaved || hd.m_generators.Count == 0)
                    {
                        //EditorGUILayout.HelpBox("layers saved", MessageType.None);
                    }
                    else if (allLocked && needToSaveLayers)
                    {
                        EditorGUILayout.HelpBox("Save layers to the project.\nUnsaved layers won't be stored in prefabs.", MessageType.Warning);

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        hd.m_prefabName = EditorGUILayout.TextField(hd.m_prefabName);

                        if (GUILayout.Button(new GUIContent("Save layers", (HairDesignerEditor.Icon(HairDesignerEditor.eIcons.SAVE))), GUILayout.MaxHeight(15)))
                              ExportMesh();
                    
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    else if(needToSaveLayers)
                    {
                        EditorGUILayout.HelpBox("When short hair or long hair layers are done, lock all the layers in Design tab.", MessageType.Info);
                    }
                }
                GUILayout.Space(20);

                bool isGeneratorUpdated = false;
                if (hd.generator != null)
                {
                    //GUILayout.Label("----------------------");
                    if (tmpEditor==null || tmpEditor.target != hd.generator)
                    {
                        RemoveCollider();
                        DestroyImmediate(tmpEditor);
                        tmpEditor = Editor.CreateEditor(hd.generator, null);                        
                    }

                    if (tmpEditor != null)
                    {
                        (tmpEditor as HairGeneratorEditor).m_hairDesignerEditor = this;
                        tmpEditor.OnInspectorGUI();
                        isGeneratorUpdated = (tmpEditor as HairGeneratorEditor).guiChanged;
                        

#if !UNITY_5_5_OR_NEWER
                        EditorUtility.SetSelectedWireframeHidden(hd.GetComponent<MeshRenderer>(), (tmpEditor as HairGeneratorEditor).hideWireframe);
                        EditorUtility.SetSelectedWireframeHidden(hd.GetComponent<SkinnedMeshRenderer>(), (tmpEditor as HairGeneratorEditor).hideWireframe);
#endif
                    }
                    //GUILayout.Label("----------------------");
                }


                

                GUI.enabled = true;
                GUILayout.Space(20);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Wind", EditorStyles.boldLabel);
                hd.m_windZone = EditorGUILayout.ObjectField("WindZone", hd.m_windZone, typeof(WindZone), true) as WindZone;
                
                GUILayout.EndVertical();

                GUILayout.Label("");
                GUILayout.Label("");
                GUILayout.Label("");               

                
                if (hd.generator != null && hd.generator.m_hair != null)
                    GUILayout.Label(" nb tri " + (hd.generator.m_hair.triangles.Length / 3));


                if (GUI.changed || isGeneratorUpdated)
                {
                    if (hd.generator != null)
                    {
                        hd.generator.m_hair = null;
                        hd.generator.m_hairStrand = null;
                    }
                    EditorUtility.SetDirty(hd);
                    UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
                    if(!Application.isPlaying)
                        EditorSceneManager.MarkSceneDirty(s);

                    SceneView.RepaintAll();
                }

                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    RestorePose();


                //hide wireframes
                for (int i = 0; i < hd.m_generators.Count; ++i)
                {
                    if (hd.m_generators[i] == null)
                        continue;

                    
                    if (hd.m_generators[i].m_meshInstance != null && hd.m_generators[i].m_meshLocked )
                    {
#if UNITY_5_5_OR_NEWER
                        EditorUtility.SetSelectedRenderState(hd.m_generators[i].m_meshRenderer, EditorSelectedRenderState.Hidden );
                        EditorUtility.SetSelectedRenderState(hd.m_generators[i].m_skinnedMesh, EditorSelectedRenderState.Hidden );
#else
                        EditorUtility.SetSelectedWireframeHidden(hd.m_generators[i].m_meshRenderer, true);
                        EditorUtility.SetSelectedWireframeHidden(hd.m_generators[i].m_skinnedMesh, true);
#endif
                    }


                }

               
                

#if KHD_DEBUG
                    if (GUILayout.Button("Show Generators"))
                {                                        
                    foreach(HairDesignerGenerator g in hd.GetComponents<HairDesignerGenerator>() )
                    {
                        g.hideFlags = HideFlags.None;
                    }                    
                }

                if (GUILayout.Button("Hide Generators"))
                {
                    foreach (HairDesignerGenerator g in hd.GetComponents<HairDesignerGenerator>())
                    {
                        g.hideFlags = HideFlags.HideInInspector;
                    }
                }


                if (GUILayout.Button("Show Bones"))
                {
                    foreach (Transform t in hd.GetComponentsInChildren<Transform>(true))
                    {
                        t.hideFlags = HideFlags.None;
                    }
                }
#endif

            }



            Vector3 scrollPosition;
            float m_sceneMenuX = -200f;

            
            void DrawSceneMenu( HairDesignerBase hd )
            {
                
                
                Handles.BeginGUI();                

                float m_sceneMenuWidth = 250;
                m_sceneMenuX = Mathf.Lerp(m_sceneMenuX, 0f, .2f);
                if (m_sceneMenuX < -1f)
                    SceneView.currentDrawingSceneView.Repaint();
                else
                    m_sceneMenuX = 0f;

                
                Rect area = new Rect(m_sceneMenuX, 0, m_sceneMenuWidth, SceneView.currentDrawingSceneView.camera.pixelHeight/EditorGUIUtility.pixelsPerPoint);

                GUI.Box(area, "", EditorStyles.helpBox);
                //GUI.Box(area, "", EditorStyles.helpBox);
                //GUI.Button(area, "", EditorStyles.helpBox);

                //GUILayout.BeginArea(area, EditorStyles.helpBox);// bgStyleAlpha);
                //GUILayout.BeginArea(area, bgStyleAlpha);
                GUILayout.BeginArea(area, EditorStyles.textArea);
                GUILayout.BeginVertical( GUILayout.MaxWidth(m_sceneMenuWidth));

                m_sceneMenuWidth *= .95f;//avoid crop

                //GUILayout.Label("HairDesigner");
                GUILayout.BeginHorizontal(bgStyleLogo);
                GUILayout.FlexibleSpace();
                GUILayout.Label(Icon(eIcons.BANNER));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                if (m_generatorEditor != null )
                {
                    (m_generatorEditor as HairGeneratorEditor).m_hairDesignerEditor = this;
                        m_generatorEditor.DrawSceneMenu(m_sceneMenuWidth);                    
                }
                                                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                GUILayout.EndArea();

				/*
                if (
                    Event.current.isMouse &&
                    Event.current.mousePosition.x > HandleUtility.GUIPointToScreenPixelCoordinate(new Vector2(0, 0)).x &&
                    Event.current.mousePosition.x < HandleUtility.GUIPointToScreenPixelCoordinate(new Vector2(m_sceneMenuWidth, 0)).x &&
                    Event.current.mousePosition.y < HandleUtility.GUIPointToScreenPixelCoordinate(new Vector2(0, 0)).y &&
                    Event.current.mousePosition.y > HandleUtility.GUIPointToScreenPixelCoordinate(new Vector2(0, SceneView.currentDrawingSceneView.camera.pixelHeight)).y
                    )
                    Event.current.Use();
                    */


                Handles.EndGUI();



                //---------------------------------------------------------
                
                if (!Application.isPlaying)
                {
                    
                    //clean instance not linked to generator
                    HairDesignerMeshInstanceBase[] instances = hd.GetComponentsInChildren<HairDesignerMeshInstanceBase>();

                    for (int i = 0; i < instances.Length; ++i)
                    {
                        bool found = false;
                        for (int j = 0; j < hd.m_generators.Count; ++j)
                        {
                            if (hd.m_generators[j].m_meshInstance == instances[i].gameObject)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            //no generator linked
                            DestroyImmediate(instances[i].gameObject);
                        }
                       
                    }
                }
//---------------------------------------------------------

            }



            static void CreateGeneratorEditor(HairDesignerBase hd)
            {
                if (m_generatorEditor == null)
                {
                    m_generatorEditor = Editor.CreateEditor(hd.generator, null) as HairGeneratorEditor;
                }
                else if ((m_generatorEditor.target as HairDesignerGenerator) != hd.generator)
                {
                    m_generatorEditor = Editor.CreateEditor(hd.generator, null) as HairGeneratorEditor;
                }
            }




            void OnSceneGUI()
            {
                DrawSceneGUI();
            }


            public static HairDesignerEditor m_EditorInstance;

            public void DrawSceneGUI()
            {
                                
                if (Application.isPlaying)
                    return;

                HairDesignerBase hd = target as HairDesignerBase;
                if (hd.generator == null)
                    return;

                CreateGeneratorEditor(hd);


                if (m_generatorEditor != null)                
                    m_generatorEditor.DrawSceneTools();
                

                if (!m_showSceneMenu)
                {
                    m_sceneMenuX = -200f;
                    return;
                }             


                if (hd.generator.IsActive)
                {
                    DrawSceneMenu(hd);
                    m_generatorEditor.PaintToolAction();
                    m_generatorEditor.DrawBrush();          
                    

                    if(hd.generator.m_meshInstance!=null)
                    {
#if UNITY_5_5_OR_NEWER
                        if (hd.generator.m_meshRenderer!=null)
                            EditorUtility.SetSelectedRenderState(hd.generator.m_meshRenderer, EditorSelectedRenderState.Hidden);
                        if (hd.generator.m_skinnedMesh != null)
                            EditorUtility.SetSelectedRenderState(hd.generator.m_skinnedMesh, EditorSelectedRenderState.Hidden);
#else
                        if (hd.generator.m_meshRenderer!=null)
							EditorUtility.SetSelectedWireframeHidden(hd.generator.m_meshRenderer, true);                        
                        if (hd.generator.m_skinnedMesh != null)
							EditorUtility.SetSelectedWireframeHidden(hd.generator.m_skinnedMesh, true);                        
#endif
                    }

                }
                else
                    m_sceneMenuX = -200;



                //disable selection action
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                //SceneView.currentDrawingSceneView.Repaint();
                EditorUtility.SetDirty(hd);
            }






            /*
            void DrawStrandCurv()
            {
                HairDesignerBase hf = target as HairDesignerBase;

                Handles.SphereCap(0, hf.transform.position, hf.transform.rotation, .1f);


                int precision = 100;

                Handles.color = Color.red;

                Vector3[] line = new Vector3[precision];
                //Handles.DrawBezier(hf.m_curv.m_startPosition, hf.m_curv.m_endPosition, hf.m_curv.m_startTangent, hf.m_curv.m_endTangent, Color.blue, null, 10f);
                for (int i = 0; i < precision; ++i)
                {
                    line[i] = hf.transform.TransformPoint(hf.generator.m_params.m_curv.GetPosition((float)i / (float)precision));
                }
                Handles.DrawLines(line);

                Vector3 start = hf.transform.TransformPoint(hf.generator.m_params.m_curv.m_startPosition);
                Vector3 end = hf.transform.TransformPoint(hf.generator.m_params.m_curv.m_endPosition);

                hf.generator.m_params.m_curv.m_endPosition = hf.transform.InverseTransformPoint(Handles.FreeMoveHandle(hf.transform.TransformPoint(hf.generator.m_params.m_curv.m_endPosition), Quaternion.identity, .1f, Vector3.zero, Handles.RectangleCap));
                hf.generator.m_params.m_curv.m_endTangent = hf.transform.InverseTransformDirection(Handles.FreeMoveHandle(hf.transform.TransformDirection(hf.generator.m_params.m_curv.m_endTangent) + end, Quaternion.identity, .03f, Vector3.zero, Handles.RectangleCap) - end);
                hf.generator.m_params.m_curv.m_startTangent = hf.transform.InverseTransformDirection(Handles.FreeMoveHandle(hf.transform.TransformDirection(hf.generator.m_params.m_curv.m_startTangent) + start, Quaternion.identity, .03f, Vector3.zero, Handles.RectangleCap) - start);


                Handles.DrawLine(start, start + hf.transform.TransformDirection(hf.generator.m_params.m_curv.m_startTangent));
                Handles.DrawLine(end, end + hf.transform.TransformDirection(hf.generator.m_params.m_curv.m_endTangent));


                //EditorUtility.SetDirty(hf.gameObject);
            }
            */

            



            void CleanGeneratorScripts()
            {                
                HairDesignerBase hd = target as HairDesignerBase;

                while (hd.m_generators.Contains(null))
                    hd.m_generators.Remove(null);

                int count = 0;

                HairDesignerGenerator[] generators = m_currentGameObject.GetComponents<HairDesignerGenerator>();
                for (int id=0; id< generators.Length; ++id)
                {
                    HairDesignerGenerator g = generators[id];
                    while (g.m_shaderParams.Contains(null))
                        g.m_shaderParams.Remove(null);

                    for (int i = 0; i < g.m_shaderParams.Count; ++i)
                    {
                        if (g.m_shaderParams[i] != null)
                            g.m_shaderParams[i].m_generator = g;
                    }

                    

                    if ( !hd.m_generators.Contains(g))
                    {
                        for (int i = 0; i < g.m_shaderParams.Count; ++i)
                        {
                            DestroyImmediate(g.m_shaderParams[i]); 
                        }
                        DestroyImmediate(g);
                        count++;
                    }
                    else
                    {
                        for (int i = 0; i < g.m_shaderParams.Count; ++i)
                        {
                            if(g.m_shaderParams[i] != null)
                                g.m_shaderParams[i].hideFlags = HideFlags.HideInInspector;
                        }
                        g.hideFlags = HideFlags.HideInInspector;
                    }
                }

                if (count > 0)
                {
                    Debug.Log("HairDesigner : " + count + " unused generators removed\n");
                    Undo.RecordObject(hd.gameObject, "HairDesigner remove unused generators");
                }



                HairDesignerShader[] shaders = m_currentGameObject.GetComponents<HairDesignerShader>();
                count = 0;
                for (int i=0; i< shaders.Length; ++i)
                {
                    if(shaders[i].m_generator == null)
                    {
                        if (!Application.isPlaying)
                        {
                            DestroyImmediate(shaders[i]);
                            count++;
                        }
                    }
                }
                if (count > 0)
                {
                    Debug.Log("HairDesigner : " + count + " unused shader parameters removed\n");
                    Undo.RecordObject(hd.gameObject,"HairDesigner remove unused shaders");
                }


            }

            void CleanHiddenScripts()
            {
                //Debug.Log("Clean hidden scripts");

                foreach (HairDesignerGenerator g in m_currentGameObject.GetComponents<HairDesignerGenerator>())
                    DestroyImmediate(g);

                foreach (HairDesignerShader s in m_currentGameObject.GetComponents<HairDesignerShader>())
                    DestroyImmediate(s);

                m_hiddenScriptsCleaned = true;
            }


            void OnEnable()
            {
                m_EditorInstance = this;
                m_TPoseUseUnityFunction = false;

                HairDesignerBase hd = target as HairDesignerBase;
                if (hd != null)//avoid error on start play mode
                {
                    for (int i = 0; i < hd.m_generators.Count; ++i)
                    {
                        //Add Mesh instance script
                        // required to avoid final layer duplication on Undo
                        if (!hd.m_generators[i].m_meshLocked && hd.m_generators[i].m_meshInstance != null && hd.m_generators[i].m_meshInstance.GetComponent<HairDesignerMeshInstanceBase>() == null)
                        {
                            HairDesignerMeshInstanceBase inst = hd.m_generators[i].m_meshInstance.AddComponent<HairDesignerMeshInstanceBase>();
                            ReplaceBaseClass(inst);
                        }
                    }
                }
            }


            //On old Version generator are not destroyed properly
            void CleanUnlinkedGenerator()
            {
                HairDesignerBase hd = target as HairDesignerBase;
                if (hd == null)
                    return;

                HairDesignerGenerator[] gens = hd.GetComponents<HairDesignerGenerator>();
                for( int i=0; i< gens.Length; ++i )
                {
                    if(gens[i].m_hd == null )
                    {
                        gens[i].DestroyShaderParams();                        
                        DestroyImmediate(gens[i]);
                        Debug.Log("Old generator cleaned");
                    }
                }
            }




            void OnDestroy()
            {
                HairDesignerBase hd = target as HairDesignerBase;
                               

                if (hd == null)
                    return;

                if (!hd.m_debugDontHideCollider)
                    RestorePose();


#if !UNITY_5_5_OR_NEWER
                if (hd != null)
                {
                    EditorUtility.SetSelectedWireframeHidden(hd.GetComponent<MeshRenderer>(), false);
                    EditorUtility.SetSelectedWireframeHidden(hd.GetComponent<SkinnedMeshRenderer>(), false);
                }
#endif


                /*
                else if( !EditorApplication.isPlayingOrWillChangePlaymode && !Application.isPlaying && m_currentGameObject != null )
                {
                    //foreach( HairDesignerGenerator g in  )
                    //Debug.Log("hairDesigner Destroyed");

                    foreach (HairDesignerGenerator g in m_currentGameObject.GetComponents<HairDesignerGenerator>())
                        DestroyImmediate(g);

                    foreach (HairDesignerShader s in m_currentGameObject.GetComponents<HairDesignerShader>())
                        DestroyImmediate(s);

                }*/


                /*
                //replace base instance (BASIC/PRO compatibility)
                HairDesignerMeshInstanceBase[] instances = hd.GetComponentsInChildren<HairDesignerMeshInstanceBase>();

                for (int i = 0; i < instances.Length; ++i)
                {
                    ReplaceBaseClass(instances[i]);
                }
                */
                
            }







            public static void GUIDrawPidResponse(HairDesignerPID pid, Rect area, float timeUnit)
            {

                Color c = new Color(0f, 0f, 0f, .2f);

                pid.Init();
                //unit step
                pid.m_target = 1;
                float r = 0;
                Vector2 start = new Vector2(area.x, area.y + area.height);
                Vector2 end = start;

                Handles.color = c;
                for (int i = 0; i < timeUnit; ++i)
                {
                    start = new Vector2((float)i * area.width / timeUnit, area.y + area.height);
                    end = new Vector2((float)i * area.width / timeUnit, area.y);
                    //GLDraw.DrawLine (start, end, c, 1f);
                    Handles.DrawLine(start, end);
                }

                start = new Vector2(area.x, area.y + area.height * .5f);
                end = new Vector2(area.x + area.width, area.y + area.height * .5f);
                //GLDraw.DrawLine (start, end, c, 1f);
                Handles.DrawLine(start, end);




                start = new Vector2(area.x, area.y + area.height);
                end = start;

                for (int i = 0; i < area.width; ++i)
                {
                    float dt = (float)timeUnit / (float)area.width;
                    for (int j = 0; j < 10f; ++j)
                        r = pid.Compute(r, dt * .1f);
                    end.x++;
                    end.y = area.height - r * area.height * .5f + area.y;
                    end.y = Mathf.Clamp(end.y, area.y, area.y + area.height);
                                       
                    Handles.color = mainColor;
                    Handles.DrawLine(start, end);
                    start = end;                   
                }

            }



            
            /// <summary>
            /// Replace base class to their non-DLL version
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static MonoBehaviour ReplaceBaseClass(MonoBehaviour obj)
            {
                if (obj == null)
                    return obj;

                var so = new SerializedObject(obj);
                var sp = so.FindProperty("m_Script");
                MonoScript script = GetScriptFromBase(obj.GetType().Name);
                if (script != null)
                {
                    sp.objectReferenceValue = script;
                    so.ApplyModifiedProperties();
                }
                return so.targetObject as MonoBehaviour;
            }



            //Get class for compatibility BASIC/PRO
            System.Type GetClassFromBase(string type)
            {
                if (!type.Contains("Base"))
                    return null;

                var types = Resources
                    .FindObjectsOfTypeAll(typeof(MonoScript))
                        .Where(x => x.name == type.Replace("Base", ""))
                        //.Where (x => x.GetType () == typeof(MonoScript))
                        .Cast<MonoScript>()
                        .Where(x => x.GetClass() != null)
                        //.Where( x => x.GetClass().Assembly.FullName.Split(',')[0] == "" )
                        .ToList();


                if (types.Count == 1)
                {
                    Debug.Log("class found " + type);
                    return types[0].GetClass();

                }


                Debug.Log("class not found " + type);
                return null;
            }




            //Get script for compatibility BASIC/PRO
            static MonoScript GetScriptFromBase(string type)
            {
                if (!type.Contains("Base"))
                    return null;

                var types = Resources
                    .FindObjectsOfTypeAll(typeof(MonoScript))
                        .Where(x => x.name == type.Replace("Base", ""))                        
                        .Cast<MonoScript>()
                        .Where(x => x.GetClass() != null)                        
                        .ToList();

                
                if (types.Count == 1)
                {
                    //Debug.Log ("class found " + type );
                    return types[0];

                }


                //Debug.Log ("class not found " + type );
                return null;
            }





            





            /// <summary>
            /// Export final meshes to the project folder
            /// </summary>
            void ExportMesh()
            {
                if (Application.isPlaying)
                    return;

                HairDesignerBase hd = target as HairDesignerBase;

                string path = "";


                MeshFilter mf = hd.GetComponent<MeshFilter>();
                if( mf != null )
                    path = AssetDatabase.GetAssetPath(mf.sharedMesh);
                
                SkinnedMeshRenderer smr = hd.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                    path = AssetDatabase.GetAssetPath(smr.sharedMesh);

                                                         

                string[] pathParts = path.Split('/');
                path = "";
                for (int i = 0; i < pathParts.Length - 1; ++i)
                    path += pathParts[i] + "/";

                if (path == "" || path == "Library/")//built-in models
                    path = "Assets/";
                
                string prefabName = path + hd.m_prefabName + "_HairDesigner.prefab";



                Object prefab = AssetDatabase.LoadAssetAtPath(prefabName, typeof(Object));
                bool createPrefab = true;

                if (prefab != null)
                {
                    createPrefab = EditorUtility.DisplayDialog("Error", "Prefab already exist\nOverwrite?", "ok", "cancel");
                }


                if(createPrefab)
                {
                    for (int i = 0; i < hd.m_generators.Count; ++i)
                    {
                        if(hd.m_generators[i]==null)
                        {
                            hd.m_generators.RemoveAt(i--);
                        }
                    }


                    for (int i = 0; i < hd.m_generators.Count; ++i)
                    {
                        if (hd.m_generators[i].m_meshRenderer != null)
                        {   
                            hd.m_generators[i].m_meshRenderer.GetComponent<MeshFilter>().sharedMesh = Instantiate(hd.m_generators[i].m_meshRenderer.GetComponent<MeshFilter>().sharedMesh);
                            hd.m_generators[i].m_meshRenderer.GetComponent<MeshFilter>().sharedMesh.name = hd.m_generators[i].m_name;
                        }
                        if (hd.m_generators[i].m_skinnedMesh != null)
                        {   
                            hd.m_generators[i].m_skinnedMesh.sharedMesh = Instantiate(hd.m_generators[i].m_skinnedMesh.sharedMesh);
                            hd.m_generators[i].m_skinnedMesh.sharedMesh.name = hd.m_generators[i].m_name;                         
                        }
                    }


                    //prefab = PrefabUtility.CreateEmptyPrefab(prefabName);
                    GameObject tmp = new GameObject("HairDesigner mesh");

#if UNITY_2018_3_OR_NEWER
                    prefab = PrefabUtility.SaveAsPrefabAsset(tmp, prefabName);//disable the warning message
#else
                    prefab = PrefabUtility.CreatePrefab(prefabName, tmp);//disable the warning message
#endif

                    for (int i = 0; i < hd.m_generators.Count; ++i)
                    {
                        if (hd.m_generators[i].m_meshRenderer != null)                        
                            AssetDatabase.AddObjectToAsset(hd.m_generators[i].m_meshRenderer.GetComponent<MeshFilter>().sharedMesh, prefabName);                            
                        

                        if (hd.m_generators[i].m_skinnedMesh != null)                        
                            AssetDatabase.AddObjectToAsset(hd.m_generators[i].m_skinnedMesh.sharedMesh, prefabName);                            
                        
                    }

                    hd.m_meshSaved = true;                    
                    AssetDatabase.SaveAssets();

                    List<Object> objLst = new List<Object>();                                        
                    objLst.Add(hd);

                    EditorGUIUtility.PingObject(prefab);
                    
                    for (int i = 0; i < hd.m_generators.Count; ++i)
                    {
                        if (hd.m_generators[i].m_meshRenderer != null)
                            objLst.Add(hd.m_generators[i].m_meshRenderer);

                        if (hd.m_generators[i].m_skinnedMesh != null)
                            objLst.Add(hd.m_generators[i].m_skinnedMesh);

                    }
                    DestroyImmediate(tmp);

                    Undo.RecordObjects(objLst.ToArray(), "Save HairDesigner layers");
                    UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
                    EditorSceneManager.MarkSceneDirty(s);
                }
               
                
            }


            bool m_bonesCleaned = false;
            /// <summary>
            /// Clean unused bones
            /// </summary>
            void CleanBones()
            {
                if (m_bonesCleaned)
                    return;

                HairDesignerBase hd = target as HairDesignerBase;
                Transform[] rootBones = hd.transform.GetComponentsInChildren<Transform>();

                for (int t = 0; t < rootBones.Length; ++t)
                {
                    if (rootBones[t] == null )continue;
                    if (rootBones[t].parent != hd.transform)continue;//only root bones are needed
                    if(!rootBones[t].name.StartsWith("HairDesigner_") || !rootBones[t].name.Contains("_bone ") )continue;

                    bool used = false;
                    for (int i = 0; i < hd.m_generators.Count; ++i)
                    {
                        HairDesignerGeneratorLongHairBase lh = hd.m_generators[i] as HairDesignerGeneratorLongHairBase;
                        if (lh == null)
                            continue;

                        for( int j=0; j<lh.m_groups.Count; ++j )
                        {
                            if (lh.m_groups[j].m_bones.Contains(rootBones[t]))
                            {
                                used = true;
                                break;
                            }
                        }
                        if (used)
                            break;
                    }

                    //Debug.Log(rootBones[t].name + " " + used);
                    if (!used)
                    {
                        if (Application.isPlaying)
                            Destroy(rootBones[t].gameObject);
                        else
                            DestroyImmediate(rootBones[t].gameObject);
                    }

                }

                m_bonesCleaned = true;
            }






            public enum eRP
            {
                STANDARD,
                HDRP,
                URP
            }

            public static eRP GetRenderPipeline()
            {
                
                if (UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline == null)
                    return eRP.STANDARD;
                
                if (UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.GetType().Name.Contains("HDRenderPipelineAsset"))
                    return eRP.HDRP;

                if (UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.GetType().Name.Contains("UniversalRenderPipelineAsset"))
                    return eRP.URP;

                return eRP.STANDARD;

            }
            
        }
    }
}