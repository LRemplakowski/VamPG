using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Kalagaan.HairDesignerExtension
{
    public class HairDesignerEditorUtility : MonoBehaviour
    {


        public static void EditorHeader()
        {
            //string txt = "";
            //GUILayout.Label(txt, EditorStyles.miniBoldLabel);
        }



        public static void ExportRuntimeLayer(HairDesignerBase hd, int LayerId)
        {
            HairDesignerGenerator layer = hd.m_generators[LayerId];
            ExportRuntimeLayer(layer);
        }


        public static string _lastExportPath = "Assets";

        public static void ExportRuntimeLayer(HairDesignerGenerator layer)
        {
            //if (exporter.m_hd.m_generators[0].m_layerType == Kalagaan.HairDesignerExtension.HairDesignerBase.eLayerType.SHORT_HAIR_POLY)
            {
                GameObject tmp = new GameObject("EXPORT ROOT");

                tmp.transform.position = layer.m_hd.transform.position;
                tmp.transform.rotation = layer.m_hd.transform.rotation;

                //string path = "Assets/HairDesignerRuntime_" + layer.m_name + ".prefab";
                string path = "";

                string defaultName = "HairDesignerRuntime_" + layer.m_name;

                path = EditorUtility.SaveFilePanelInProject("Export runtime layer", defaultName, "prefab", "test", _lastExportPath);

                if (path != "" && path.Contains("/"))
                {
                    _lastExportPath = path.Remove(path.LastIndexOf('/'));
                }
                else
                {
                    DestroyImmediate(tmp);
                    return;
                }


                Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

                if (prefab != null)
                {
                    //if (id == 0)
                    {
                        if (!EditorUtility.DisplayDialog("Error", "Prefab already exist\nOverwrite?", "ok", "cancel"))
                        {
                            DestroyImmediate(tmp);
                            return;
                        }

#if UNITY_2018_3_OR_NEWER
                        //prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(tmp, path, InteractionMode.AutomatedAction);
#else
                        prefab = PrefabUtility.CreatePrefab(path, tmp);
#endif
                        
                    }
                }
                else
                {
#if UNITY_2018_3_OR_NEWER
                    //prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(tmp, path, InteractionMode.AutomatedAction);
#else
                    prefab = PrefabUtility.CreatePrefab(path, tmp);
#endif
                }


                HairDesignerRuntimeLayerBase runtimeLayer = tmp.AddComponent<HairDesignerRuntimeLayerBase>();
                //HairDesignerRuntimeLayerBase runtimeLayer = tmp.AddComponent(System.Type.GetType("Kalagaan.HairDesignerExtension.HairDesignerRuntimeLayer")) as HairDesignerRuntimeLayerBase;
                runtimeLayer = HairDesignerEditor.ReplaceBaseClass(runtimeLayer) as HairDesignerRuntimeLayerBase;

                GameObject meshInstance;
                SkinnedMeshRenderer smr = null;
                SkinnedMeshRenderer instanceSmr = null;

                MeshFilter mf = null;
                MeshFilter instanceMf = null;

                MeshRenderer mr = null;
                MeshRenderer instanceMr = null;

                Mesh meshForPrefab = null;

                //Fix motionZone bone name
                for( int i=0; i<layer.m_motionZones.Count; ++i)
                {
                    layer.m_motionZones[i].parentname = layer.m_motionZones[i].parent == null ? "" : layer.m_motionZones[i].parent.name; 
                }

                if (layer.m_meshInstance != null)
                {
                    meshInstance = GameObject.Instantiate(layer.m_meshInstance, tmp.transform);
                    smr = layer.m_meshInstance.GetComponent<SkinnedMeshRenderer>();
                    mf = layer.m_meshInstance.GetComponent<MeshFilter>();
                    mr = layer.m_meshInstance.GetComponent<MeshRenderer>();


                    meshInstance.name = layer.m_meshInstance.name;

                    if (layer.m_layerType != Kalagaan.HairDesignerExtension.HairDesignerBase.eLayerType.FUR_SHELL)
                    {
                        if (smr != null)
                        {
                            meshForPrefab = new Mesh();
                            //EditorUtility.CopySerialized(smr.sharedMesh, meshForPrefab);
                            meshForPrefab = MeshCopy(smr.sharedMesh);
                            meshForPrefab.name = layer.m_name;

                            instanceSmr = meshInstance.GetComponent<SkinnedMeshRenderer>();

                            if (layer.m_layerType == Kalagaan.HairDesignerExtension.HairDesignerBase.eLayerType.SHORT_HAIR_POLY)
                            {
                                //save avatar bone list for skinning data
                                runtimeLayer.m_bonenameLst = new List<string>();
                                for (int i = 0; i < smr.bones.Length; ++i)
                                {
                                    runtimeLayer.m_bonenameLst.Add(smr.bones[i].name);
                                }
                            }

                        }

                        if (mf != null)
                        {
                            meshForPrefab = new Mesh();
                            //EditorUtility.CopySerialized(mf.sharedMesh, meshForPrefab);
                            meshForPrefab = MeshCopy(mf.sharedMesh);
                            meshForPrefab.name = layer.m_name;

                            instanceMf = meshInstance.GetComponent<MeshFilter>();
                            instanceMr = meshInstance.GetComponent<MeshRenderer>();
                        }


                        //UMAhc.m_hairMeshInstance = instanceSmr;
                    }
                }
                else
                {
                    meshInstance = new GameObject();
                    meshInstance.name = layer.m_name;
                    meshInstance.transform.parent = tmp.transform;
                }





                HairDesignerGenerator g = meshInstance.AddComponent(layer.GetType()) as HairDesignerGenerator;
                EditorUtility.CopySerialized(layer, g);
                g.m_meshLocked = true;
                g.SetActive(true);

                HairDesignerShader s = meshInstance.AddComponent(layer.GetShaderParams().GetType()) as HairDesignerShader;
                EditorUtility.CopySerialized(layer.GetShaderParams(), s);
                g.m_shaderParams[0] = s;
                s.m_generator = g;


                if (instanceSmr != null)
                    instanceSmr.sharedMaterial = smr.sharedMaterial;
                if (instanceMr != null)
                    instanceMr.sharedMaterial = mr.sharedMaterial;



                if (meshForPrefab != null)
                {
                    string meshAssetpath = path.Replace(".prefab", "_mesh.asset");
                    AssetDatabase.CreateAsset(meshForPrefab, meshAssetpath);
                    AssetDatabase.ImportAsset(meshAssetpath, ImportAssetOptions.ForceUpdate);


                    if (instanceSmr)
                        instanceSmr.sharedMesh = meshForPrefab;
                    if (instanceMf)
                        instanceMf.sharedMesh = meshForPrefab;
                    
                }


                if (layer.m_layerType == Kalagaan.HairDesignerExtension.HairDesignerBase.eLayerType.LONG_HAIR_POLY)
                {
                    HairDesignerBoneBase[] bonesRoots = g.GetComponentsInChildren<HairDesignerBoneBase>(true);

                    for (int i = 0; i < bonesRoots.Length; ++i)
                    {
                        //HairDesignerEditor.ReplaceBaseClass(bonesRoots[i]);
                        if (bonesRoots[i].m_target != null)
                        {
                            if (!runtimeLayer.m_requiredBones.Contains(bonesRoots[i].m_targetName))
                                runtimeLayer.m_requiredBones.Add(bonesRoots[i].m_targetName);
                        }

                    }
                }

#if UNITY_2018_3_OR_NEWER
                prefab = PrefabUtility.SaveAsPrefabAsset(tmp, path);
#else
                prefab = PrefabUtility.ReplacePrefab(tmp, prefab);
#endif

                AssetDatabase.SaveAssets();
                EditorGUIUtility.PingObject(prefab);
                //AssetDatabase.Refresh();
                DestroyImmediate(tmp);                
            }
        }




        public static Mesh MeshCopy(Mesh src)
        {
            if (src == null)
                return null;

            Mesh dst = Instantiate(src);

            return dst;
        }
    }
}