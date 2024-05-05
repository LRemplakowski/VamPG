using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


namespace Kalagaan.HairDesignerExtension
{

    public class HairDesignerRuntimeLayerBase : MonoBehaviour
    {

        public List<string> m_requiredBones = new List<string>();
        public static Dictionary<string, Transform> m_boneNameTotransform = new Dictionary<string, Transform>();
        public static List<CapsuleCollider> m_hairColliders = new List<CapsuleCollider>();

        public List<string> m_bonenameLst = new List<string>();

        public void GenerateLayers(HairDesignerBase hd)
        {

            SkinnedMeshRenderer characterSkinnedMeshRenderer = hd.GetComponent<SkinnedMeshRenderer>();
            //MeshRenderer characterMeshRenderer = hd.GetComponent<MeshRenderer>();

            HairDesignerGenerator[] generators = GetComponentsInChildren<HairDesignerGenerator>(true);

            for (int k = 0; k < generators.Length; ++k)
            {
                HairDesignerGenerator gRef = generators[k];

                GameObject reference = gRef.gameObject;
                GameObject instance;


                instance = GameObject.Instantiate(reference, hd.transform);
                instance.name = reference.name;

                HairDesignerGenerator g = instance.GetComponent<HairDesignerGenerator>();
                HairDesignerShader s = instance.GetComponent<HairDesignerShader>();
                SkinnedMeshRenderer smr = instance.GetComponent<SkinnedMeshRenderer>();
                MeshRenderer mr = instance.GetComponent<MeshRenderer>();


                //move component to HairDesigner reference  gameObject

                System.Type tg = g.GetType();
                HairDesignerGenerator newG = hd.gameObject.AddComponent(tg) as HairDesignerGenerator;
                CopyComponent(g, newG);

                g.m_shaderParams = new List<HairDesignerShader>();

                if (g.m_layerType == HairDesignerBase.eLayerType.LONG_HAIR_POLY)
                    (g as HairDesignerGeneratorLongHairBase).m_dontDestroyMeshOnDestroy = true;

                if (Application.isPlaying)
                    Destroy(g);
                else
                    DestroyImmediate(g);

                g = newG;
                g.m_hd = hd;
                g.hideFlags = HideFlags.HideInInspector;

                if (s != null)
                {
                    System.Type ts = s.GetType();
                    HairDesignerShader newS = hd.gameObject.AddComponent(ts) as HairDesignerShader;
                    CopyComponent(s, newS);
                    if (Application.isPlaying)
                        Destroy(s);
                    else
                        DestroyImmediate(s);

                    s = newS;
                    s.hideFlags = HideFlags.HideInInspector;

                    //link shader params
                    g.m_shaderParams.Add(s);
                    s.m_generator = newG;
                    g.m_shaderNeedUpdate = true;
                }
                //---------------------------------------------


                hd.m_generators.Add(g);


                g.m_meshInstance = instance.gameObject;



                if (characterSkinnedMeshRenderer != null)
                {

                    //------------------------------
                    //Generate bone list
                    Transform[] m_bones = new Transform[m_bonenameLst.Count];
                    Transform[] m_avatarBones = characterSkinnedMeshRenderer.bones;

                    for (int i = 0; i < m_bonenameLst.Count; ++i)
                    {
                        for (int j = 0; j < m_avatarBones.Length; ++j)
                        {
                            if (m_avatarBones[j].name == m_bonenameLst[i])
                            {
                                m_bones[i] = m_avatarBones[j];
                            }
                        }
                        if (g.m_layerType == Kalagaan.HairDesignerExtension.HairDesignerBase.eLayerType.SHORT_HAIR_POLY)
                            smr.bones = m_bones;                       
                    }
                    //------------------------------


                    //update motion zones
                    for (int j = 0; j < g.m_motionZones.Count; ++j)
                    {
                        for (int i = 0; i < m_avatarBones.Length; ++i)
                        {
                            if (m_avatarBones[i].name == g.m_motionZones[j].parentname)
                            {
                                g.m_motionZones[j].parent = m_avatarBones[i];
                            }
                        }
                    }

                }


                if (smr != null)
                {
                    smr.updateWhenOffscreen = true;
                    g.m_skinnedMesh = smr;
                }

                if (mr != null)
                {
                    g.m_meshRenderer = mr;
                    g.m_skinnedMesh = null;
                }






                if (g.m_layerType == Kalagaan.HairDesignerExtension.HairDesignerBase.eLayerType.LONG_HAIR_POLY)
                {
                    List<Transform> bones = new List<Transform>();

                    //link bones
                    HairDesignerBoneBase[] bonesRoots = instance.GetComponentsInChildren<HairDesignerBoneBase>(false);

                    //Debug.Log("Bones root : " + bonesRoots.Length);



                    HairDesignerGeneratorLongHairBase lh = g as HairDesignerGeneratorLongHairBase;
                    lh.m_capsuleColliders = new List<CapsuleCollider>();
                    if (m_hairColliders != null)
                    {
                        for (int i = 0; i < m_hairColliders.Count; ++i)
                            lh.m_capsuleColliders.Add(m_hairColliders[i]);
                    }


                    Transform[] characterBones = null;
                    if (characterSkinnedMeshRenderer != null)
                        characterBones = characterSkinnedMeshRenderer.bones;



                    for (int i = 0; i < bonesRoots.Length; ++i)
                    {


                        //Lock bone root to the character bone
                        if (bonesRoots[i].m_targetName != "")
                        {

                            if (m_boneNameTotransform.ContainsKey(bonesRoots[i].m_targetName))
                            {
                                bonesRoots[i].m_target = m_boneNameTotransform[bonesRoots[i].m_targetName];
                            }
                            else if (characterBones != null)
                            {

                                for (int j = 0; j < characterBones.Length; ++j)
                                {
                                    //Debug.Log(characterBones[j].name);
                                    if (characterBones[j].name == bonesRoots[i].m_targetName)
                                    {
                                        bonesRoots[i].m_target = characterBones[j];
                                        break;
                                    }
                                }
                            }
                        }




                        Transform bone = bonesRoots[i].transform;


                        //clear unused groups
                        for (int j = 0; j < lh.m_groups.Count; ++j)
                            if (!lh.m_groups[j].m_generated)
                                lh.m_groups.RemoveAt(j--);

                        for (int j = 0; j < lh.m_groups[i].m_bones.Count; ++j)
                        {
                            //set bones to groups

                            lh.m_groups[i].m_bones[j] = bone;
                            bones.Add(bone);

                            //Debug.Log(bone.name);
                            if (j < lh.m_groups[i].m_bones.Count - 1)
                                bone = bone.GetChild(0);
                        }

                    }

                }

            }

        }



        void CopyComponent(MonoBehaviour src, MonoBehaviour dest)
        {
            FieldInfo[] sourceFields = src.GetType().GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < sourceFields.Length; i++)
            {
                try
                {
                    var val = sourceFields[i].GetValue(src);
                    sourceFields[i].SetValue(dest, val);
                }
                catch
                {
                    Debug.LogError("Error copy id:" + i);
                }
            }
        }

    }
}