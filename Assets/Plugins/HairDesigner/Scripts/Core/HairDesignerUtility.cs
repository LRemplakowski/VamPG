using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        

        [System.Serializable]
        public class MeshUtility
        {

            /// <summary>
            /// Apply current blendShape transformations to the vertices array
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="vertices"></param>
            public static void ApplyBlendShape( SkinnedMeshRenderer smr, ref Vector3[] vertices )
            {
                if (smr == null)
                    return;

                Vector3[] deltaVertices = new Vector3[vertices.Length];
                Vector3[] deltaNormals = new Vector3[vertices.Length];
                Vector3[] deltaTangents = new Vector3[vertices.Length];

                for (int i = 0; i < smr.sharedMesh.blendShapeCount; ++i)
                {
                    smr.sharedMesh.GetBlendShapeFrameVertices(i, 0, deltaVertices, deltaNormals, deltaTangents);
                    float w = smr.GetBlendShapeWeight(i)/100f;

                    for (int j = 0; j < vertices.Length; ++j)
                        vertices[j] += deltaVertices[j] * w;

                }
            }


            public static void ApplyBlendShape(Mesh bakedMesh, ref Vector3[] vertices)
            {
                if (bakedMesh == null)
                    return;

                Vector3[] deltaVertices = new Vector3[vertices.Length];
                Vector3[] deltaNormals = new Vector3[vertices.Length];
                Vector3[] deltaTangents = new Vector3[vertices.Length];

                for (int i = 0; i < bakedMesh.blendShapeCount; ++i)
                {
                    bakedMesh.GetBlendShapeFrameVertices(i, 0, deltaVertices, deltaNormals, deltaTangents);
                    float w = bakedMesh.GetBlendShapeFrameWeight(i, 0) / 100f;

                    for (int j = 0; j < vertices.Length; ++j)
                        vertices[j] += deltaVertices[j] * w;

                }
            }


        }









        [System.Serializable]
        public class TPoseUtility
        {
            public bool m_inititialized = false;
            public Vector3[] m_initPos;
            public Quaternion[] m_initRot;
            public Vector3[] m_initScale;
            public Vector3 m_rootInitPos;
            public Quaternion m_rootInitRot;
            public Transform m_rootParent;
            public int m_rootBoneID;
            public SkinnedMeshRenderer m_smr;
            public eTPoseMode m_TPoseMode = eTPoseMode.Mode_1;

            public enum eTPoseMode
            {
                NONE,
                Mode_1,
                Mode_2
            }


            public void InitTPose(SkinnedMeshRenderer smr)
            {
                if (m_inititialized)
                    return;

                if (smr.bones.Length == 0)
                    return;

                m_smr = smr;

                m_rootBoneID = -1;
                int parentCount = int.MaxValue;
                for (int i = 0; i < m_smr.bones.Length; ++i)
                {
                    //bool isRoot = true;
                    Transform[] parentLst = m_smr.bones[i].GetComponentsInParent<Transform>(true);
                    if (parentLst.Length < parentCount)
                    {
                        m_rootBoneID = i;
                        parentCount = parentLst.Length;
                    }
                }

                //Find the root transform of the hierarchy
                if (m_smr.bones.Length > m_rootBoneID && m_rootBoneID >= 0)
                    m_rootParent = m_smr.bones[m_rootBoneID].parent;
                else
                    m_rootParent = null;

                bool isParentOfAllBones = false;

                if (m_rootParent != null)
                {
                    while (m_rootParent.parent != null && !isParentOfAllBones)
                    {
                        isParentOfAllBones = true;
                        Transform[] ChildrenLst = m_rootParent.GetComponentsInChildren<Transform>(true);
                        for (int i = 0; i < m_smr.bones.Length; ++i)
                        {
                            if (!ChildrenLst.Contains(m_smr.bones[i]))
                            {
                                isParentOfAllBones = false;
                                m_rootParent = m_rootParent.parent;
                                break;
                            }
                        }
                        /*
                        if (isParentOfAllBones && !ChildrenLst.Contains(m_smr.transform))
                        {
                            isParentOfAllBones = false;
                            m_rootParent = m_rootParent.parent;
                        }*/
                    }
                }




                m_initPos = new Vector3[m_smr.bones.Length];
                m_initRot = new Quaternion[m_smr.bones.Length];
                m_initScale = new Vector3[m_smr.bones.Length];
                //m_rootInitRot = m_smr.bones[m_rootBoneID].parent.localRotation;
                m_rootInitRot = m_rootParent.localRotation;
                m_rootInitPos = m_rootParent.localPosition;

                for (int i = 0; i < m_smr.bones.Length; ++i)
                {
                    m_initPos[i] = m_smr.bones[i].localPosition;
                    m_initRot[i] = m_smr.bones[i].localRotation;
                    m_initScale[i] = m_smr.bones[i].localScale;
                }

                m_inititialized = true;
            }


            public void RevertTpose()
            {
                if (!m_inititialized)
                    return;

                for (int i = 0; i < m_smr.bones.Length; ++i)
                {
                    m_smr.bones[i].localPosition = m_initPos[i];
                    m_smr.bones[i].localRotation = m_initRot[i];
                    m_smr.bones[i].localScale = m_initScale[i];
                    //Debug.Log(m_smr.bones[i].name);
                }

                m_rootParent.localRotation = m_rootInitRot;
                m_rootParent.localPosition = m_rootInitPos;
            }



            public void ApplyTPose(SkinnedMeshRenderer smr, bool unityMode )
            {
                

                if (!m_inititialized)
                    InitTPose(smr);


                if (m_TPoseMode == eTPoseMode.NONE)
                    return;

                if (m_smr.bones.Length == 0)
                    return;

                if (unityMode)
                {
                    ReflectionRestoreToBindPose();
                    return;
                }
                

                if( m_TPoseMode == eTPoseMode.Mode_2 )
                {
                    for (int i = 0; i < smr.bones.Length; ++i)
                    {
                        
                        //restore T-pose                   
                        Matrix4x4 parentBindPose = Matrix4x4.identity;
                        bool isRoot = true;
                        for (int j = 0; j < smr.bones.Length; ++j)
                        {
                            if (smr.bones[i].parent == smr.bones[j])
                            {
                                parentBindPose = smr.sharedMesh.bindposes[j];
                                isRoot = false;
                            }
                        }

                        if (!isRoot)
                        {

                            // Recreate the local transform matrix of the bone
                            //Matrix4x4 localMatrix = smr.sharedMesh.bindposes[i].inverse;
                            Matrix4x4 localMatrix = (smr.sharedMesh.bindposes[i] * parentBindPose.inverse).inverse;
                            // Recreate local transform from that matrix
                            smr.bones[i].localPosition = localMatrix.MultiplyPoint(Vector3.zero);
                            smr.bones[i].localRotation = Quaternion.LookRotation(localMatrix.GetColumn(2), localMatrix.GetColumn(1));
                            smr.bones[i].localScale = new Vector3(localMatrix.GetColumn(0).magnitude, localMatrix.GetColumn(1).magnitude, localMatrix.GetColumn(2).magnitude);
                        }
                    }

                    return;
                }



                List<Matrix4x4> bindposes = new List<Matrix4x4>();
#if UNITY_5_6_OR_NEWER
                m_smr.sharedMesh.GetBindposes(bindposes);
#else
                bindposes = m_smr.sharedMesh.bindposes.ToList();
#endif

                Matrix4x4 m = bindposes[m_rootBoneID].inverse;
                Vector3 rootPosition = (m.MultiplyPoint(Vector3.zero));
                rootPosition.x *= m_smr.bones[m_rootBoneID].lossyScale.x;
                rootPosition.y *= m_smr.bones[m_rootBoneID].lossyScale.y;
                rootPosition.z *= m_smr.bones[m_rootBoneID].lossyScale.z;

                Quaternion rootBoneRotation = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
                Vector3 oldRootPos = m_smr.bones[m_rootBoneID].position;
                Quaternion oldRootBoneRot = m_smr.bones[m_rootBoneID].rotation;
                Quaternion q = Quaternion.identity;                
                q = oldRootBoneRot * Quaternion.Inverse(rootBoneRotation);
                Vector3 deltaPos = q * rootPosition - m_smr.bones[m_rootBoneID].position;

                //bones are child/parent so it require to be ordered                
                List<Transform> bones = m_smr.bones.ToList();
                List<Transform> bonesRefList = m_smr.bones.ToList();
                bones = bones.OrderBy(b => b.GetComponentsInParent<Transform>().Length).ToList();
                
                for (int i = 0; i < bones.Count; ++i)
                {
                    //if (m_smr.bones[m_rootBoneID] != bones[i])
                    {
                        m = bindposes[bonesRefList.IndexOf(bones[i])].inverse;
                        Vector3 pos = (m.MultiplyPoint(Vector3.zero));
                        pos.x *= Mathf.Abs(bones[i].lossyScale.x);
                        pos.y *= Mathf.Abs(bones[i].lossyScale.y);
                        pos.z *= Mathf.Abs(bones[i].lossyScale.z);
                        bones[i].position = pos - deltaPos;
                        bones[i].rotation = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
                        bones[i].localScale = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
                    }
                }
                
                
                m_rootParent.rotation = q * m_rootParent.rotation;
                m_rootParent.position += oldRootPos - m_smr.bones[m_rootBoneID].position;//fix ssg                
                
            }




            private void ReflectionRestoreToBindPose()
            {
                if (m_smr.gameObject == null && !Application.isEditor)
                    return;
                System.Type type = System.Type.GetType("UnityEditor.AvatarSetupTool, UnityEditor");
                if (type != null)
                {
                    System.Reflection.MethodInfo info = type.GetMethod("SampleBindPose", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    if (info != null)
                    {
                        info.Invoke(null, new object[] { m_smr.gameObject });
                    }
                }
            }

        }

    }
}

