using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [DefaultExecutionOrder(210)]
        [ExecuteInEditMode]
        [System.Serializable]        
        public class HairDesignerGeneratorAdvancedFurBase : HairDesignerGenerator
        {
            /*
            [System.Serializable]
            public class LODGroup
            {
                public Vector2 m_range = new Vector2(0,100);
                public int m_shellCount = 100;
                public UnityEngine.Rendering.ShadowCastingMode m_shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                public float m_maskLOD = 0f;
            }*/

            [System.Serializable]
            public class EditorData
            {
                public float m_furMaskMin = 0f;
                public float m_furMaskMax = 1f;
                public bool m_unsavedColorTexFile = false;
                public bool m_unsavedBrushTexFile = false;
                public bool m_unsavedMaskTexFile = false;
                public float m_furHeightMin = 0f;
                public float m_furHeightMax = 1f;
            }

            [HideInInspector]
            public EditorData m_editorData;
            public int m_shellCount = 100;
            public UnityEngine.Rendering.ShadowCastingMode m_strandsShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            public UnityEngine.Rendering.ShadowCastingMode m_shellsShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            public List<bool> m_materialStrandEnabled = new List<bool>();
            public List<bool> m_materialShellEnabled = new List<bool>();

            public List<SphereCollider> m_sphereColliders = new List<SphereCollider>();
            public List<CapsuleCollider> m_capsuleColliders = new List<CapsuleCollider>();

            public bool m_updateMotion = true;
            public static bool m_UpdateIndirect = true;



            [System.Serializable]
            struct DualSphereCollider
            {
                public Vector4 sphere1;//world position & radius
                public Vector4 sphere2;//world position & radius
            }



            [System.Serializable]
            public class CSMotionData
            {
                public bool enable = true;
                public float damping = 10f;
                public float bouncing = 1f;
                public float motionFactor = 1f;
                public float gravity = .1f;
                public float random = .1f;
                public float interactionOffsetFactor = .1f;
                public float centrigugalFactor = 1f;
                public float timeScale = 1f;
                public float windMain = 1f;
                public float windTurbulence = 1f;
            }
            public CSMotionData m_motionData = new CSMotionData();

            [System.Serializable]
            public struct Vector3BufferData
            {
                public Vector3 v;
            }

#pragma warning disable 0169
            [System.Serializable]
            struct PIData
            {
                float dtStepNext;                
                float errSumX;
                float errSumY;
                float errSumZ;
                float errorX;
                float errorY;
                float errorZ;
                
                Vector3 target;
                Vector3 result;
            };
#pragma warning restore 0169

            struct SkinData
            {
                public Vector4 ids;
                public Vector4 weights;
            };




            //--------------------
            //Compute shader
            [System.NonSerialized]
            ComputeShader m_cs;

            ComputeBuffer m_argsBuffer;
            bool m_computeShaderInitialized = false;
            

            [System.NonSerialized]
            public RenderTexture m_rtex;
            int m_CSKernelIdx;

            [System.NonSerialized]
            ComputeBuffer _vertexBuffer = null;
            [System.NonSerialized]
            ComputeBuffer _normalBuffer = null;
            [System.NonSerialized]
            ComputeBuffer _lastWorldNormalBuffer = null;
            [System.NonSerialized]
            ComputeBuffer _worldPosBuffer = null;
            [System.NonSerialized]
            ComputeBuffer _PIBuffer = null;
            [System.NonSerialized]
            ComputeBuffer _SkinDataBuffer = null;
            [System.NonSerialized]
            ComputeBuffer _DualSphereCollidersDataBuffer = null;

            Matrix4x4[] m_bonesMatrix;
            SkinData[] m_CS_SkinData;
            BoneWeight[] m_meshBoneWeights;
            Matrix4x4[] meshBindposes;
            Transform[] m_bones;

            private SkinnedMeshRenderer skin;
            private Vector4 m_time = Vector4.zero;

            //--------------------

            /*
            public float m_furWidthUpscale = 0f;
            //public bool m_needRefresh = false;
            public bool m_recalculateNormals = false;
            public bool m_recalculateBounds = false;
            
            public bool m_useLOD = false;
            public bool m_selectCurrentLOD = true;
            public List<LODGroup> m_LODGroups = new List<LODGroup>();
            public bool m_firstInitialisation = false;
            */
            /*
            bool m_enableDrawMeshInstanced = false;//don't work yet with LOD
            float[] m_furFactorArray = null;
            float[] m_maskLODArray = null;
            Matrix4x4[] m_matrixArray = null;
            */

            public Material m_furShellMaterial;
            public Texture2D m_defaultFurDensity;
            //MaterialPropertyBlock m_pb;

            /*
            MeshFilter m_mf;
            MeshRenderer m_mr;
            SkinnedMeshRenderer m_smr;
            */
                   
            [HideInInspector]
            public GameObject m_furStrandObject;
            [HideInInspector]
            public GameObject m_furShellObject;
            [HideInInspector]
            public Renderer m_furRendererStrands;
            [HideInInspector]
            public Renderer m_furRendererShells;
            [HideInInspector]
            public bool m_HideStrands = false;
            [HideInInspector]
            public bool m_HideShells = false;
            [HideInInspector]
            public bool m_EnableAnimation = true;

            SkinnedMeshRenderer m_smrF;
            SkinnedMeshRenderer m_smrS;



            [HideInInspector]
            public List<MeshRenderer> m_shells = new List<MeshRenderer>();
            public List<MeshFilter> m_shellsMF = new List<MeshFilter>();
            //Mesh m_mesh;


            [HideInInspector]
            Material m_hiddenMaterial;


            DualSphereCollider[] m_dualSphereColliders = null;

            bool m_CSInitialized = false;

            public void Awake()
            {
                //init fur objects if the gameObject is a copy
                m_furStrandObject = null;
                m_furStrandObject = null;
            }


            public override void Start()
            {                
                Init();
                /*
                if (Application.isPlaying)
                    InitComputeShader();
                */
            }


            public void ResetMotion()
            {
                m_initialized = false;
            }



            public void UpdateColliders()
            {

                while (m_sphereColliders.Contains(null))
                    m_sphereColliders.Remove(null);

                while (m_capsuleColliders.Contains(null))
                    m_capsuleColliders.Remove(null);

                int colliderCount = m_sphereColliders.Count + m_capsuleColliders.Count;
                if (m_dualSphereColliders==null || m_dualSphereColliders.Length != colliderCount)
                {
                    m_dualSphereColliders = new DualSphereCollider[colliderCount];
                    if (_DualSphereCollidersDataBuffer != null)
                        _DualSphereCollidersDataBuffer.SetCounterValue((uint)m_dualSphereColliders.Length);
                }

                int idx = 0;
                for(int i=0; i<m_sphereColliders.Count; ++i )
                {
                    float scale = m_sphereColliders[i].transform.lossyScale.x;
                    m_dualSphereColliders[idx].sphere1 = m_sphereColliders[i].transform.position;
                    m_dualSphereColliders[idx].sphere1.w = m_sphereColliders[i].gameObject.activeSelf ? scale * m_sphereColliders[i].radius : 0;
                    m_dualSphereColliders[idx].sphere2 = m_sphereColliders[i].transform.position;
                    m_dualSphereColliders[idx].sphere2.w = 0;//use only 1
                    idx++;
                }

                for (int i = 0; i < m_capsuleColliders.Count; ++i)
                {
                    float scale = m_capsuleColliders[i].transform.lossyScale.x;
                    m_dualSphereColliders[idx].sphere1 = m_capsuleColliders[i].transform.position + m_capsuleColliders[i].transform.up * scale * (m_capsuleColliders[i].height * .5f - m_capsuleColliders[i].radius);
                    m_dualSphereColliders[idx].sphere1.w = m_capsuleColliders[i].gameObject.activeSelf ? scale * m_capsuleColliders[i].radius:0;
                    m_dualSphereColliders[idx].sphere2 = m_capsuleColliders[i].transform.position - m_capsuleColliders[i].transform.up * scale * (m_capsuleColliders[i].height * .5f - m_capsuleColliders[i].radius);
                    m_dualSphereColliders[idx].sphere2.w = m_capsuleColliders[i].gameObject.activeSelf ? scale * m_capsuleColliders[i].radius : 0;
                    idx++;
                }


                if(_DualSphereCollidersDataBuffer != null)
                {                    
                    _DualSphereCollidersDataBuffer.SetData(m_dualSphereColliders);
                }
            }







            void InitComputeShader()
            {

                if( !SystemInfo.supportsComputeShaders )
                {
                    Debug.LogWarning("Compute shader not supported");
                    return;
                }

                if(m_cs==null)
                    //Unity 'Assertion failed...' error fixed in 2018.2
                    m_cs = (ComputeShader)Instantiate(Resources.Load("HairDesigner_AdvancedFurMotion", typeof(ComputeShader)));
                    

                if (m_cs == null)
                {
                    Debug.LogError("Can't find HairDesigner_AdvancedFurMotion.compute");
                    return;
                }

                if(m_motionData == null)
                    m_motionData = new CSMotionData();
                Mesh meshRef;

                skin = GetComponent<SkinnedMeshRenderer>();
                if (skin != null)
                {
                    
                    meshRef = skin.sharedMesh;
                    meshBindposes = meshRef.bindposes;
                    m_bonesMatrix = new Matrix4x4[skin.bones.Length];
                    m_CS_SkinData = new SkinData[meshRef.vertexCount];

                    m_bones = skin.bones;
                    m_meshBoneWeights = meshRef.boneWeights;
                    for (int i = 0; i < meshRef.vertexCount; ++i)
                    {
                         m_CS_SkinData[i] = new SkinData();
                        m_CS_SkinData[i].ids = new Vector4(
                            (float)m_meshBoneWeights[i].boneIndex0,
                            (float)m_meshBoneWeights[i].boneIndex1,
                            (float)m_meshBoneWeights[i].boneIndex2,
                            (float)m_meshBoneWeights[i].boneIndex3);


                        m_CS_SkinData[i].weights = new Vector4(
                            m_meshBoneWeights[i].weight0,
                            m_meshBoneWeights[i].weight1,
                            m_meshBoneWeights[i].weight2,
                            m_meshBoneWeights[i].weight3);


                    }
                    
                }
                else
                {
                    MeshFilter mf = GetComponent<MeshFilter>();
                    meshRef = mf.sharedMesh;
                    m_CS_SkinData = new SkinData[1];


                }

                if (_SkinDataBuffer != null)
                    _SkinDataBuffer.Release();
                _SkinDataBuffer = new ComputeBuffer(meshRef.vertexCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(SkinData)));
                _SkinDataBuffer.SetData(m_CS_SkinData);


                _DualSphereCollidersDataBuffer = new ComputeBuffer(100, System.Runtime.InteropServices.Marshal.SizeOf(typeof(DualSphereCollider)));



                m_CSKernelIdx = m_cs.FindKernel("CSMain");
                int textureSize = 1024;
                int vCount = meshRef.vertexCount;
                int maxSize = 1024;

                while (maxSize > 8)
                {
                    if (vCount <= maxSize * maxSize)
                        textureSize = maxSize;
                    else
                        break;
                    maxSize /= 2;
                }

                m_rtex = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.DefaultHDR);
                m_rtex.enableRandomWrite = true;
                m_rtex.Create();

                int dataSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3BufferData));
                if (_vertexBuffer != null)
                    _vertexBuffer.Release();
                _vertexBuffer = new ComputeBuffer(meshRef.vertexCount, dataSize);
                _vertexBuffer.SetData(meshRef.vertices);

                if (_normalBuffer != null)
                    _normalBuffer.Release();
                _normalBuffer = new ComputeBuffer(meshRef.vertexCount, dataSize);
                _normalBuffer.SetData(meshRef.normals);
                
                if (_lastWorldNormalBuffer != null)
                    _lastWorldNormalBuffer.Release();
                _lastWorldNormalBuffer = new ComputeBuffer(meshRef.vertexCount, dataSize);
                _lastWorldNormalBuffer.SetData(meshRef.normals);

                if (_worldPosBuffer != null)
                    _worldPosBuffer.Release();
                _worldPosBuffer = new ComputeBuffer(meshRef.vertexCount, dataSize);
                

                dataSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(PIData));
                if (_PIBuffer != null)
                    _PIBuffer.Release();
                _PIBuffer = new ComputeBuffer(meshRef.vertexCount, dataSize);

                m_cs.SetBuffer(m_CSKernelIdx, "_vertexBuf", _vertexBuffer);
                m_cs.SetBuffer(m_CSKernelIdx, "_normalBuf", _normalBuffer);
                m_cs.SetBuffer(m_CSKernelIdx, "_lastWorldNormalBuf", _lastWorldNormalBuffer);
                m_cs.SetBuffer(m_CSKernelIdx, "_worldPosBuf", _worldPosBuffer);
                m_cs.SetBuffer(m_CSKernelIdx, "_PI_Data_Buffer", _PIBuffer);
                m_cs.SetBuffer(m_CSKernelIdx, "_CollidersData_Buffer", _DualSphereCollidersDataBuffer);
                m_cs.SetInt("_CollidersData_length", 0);
                m_cs.SetInt("_verticeCount", meshRef.vertexCount);
                m_cs.SetInt("_textureSize", textureSize);
                m_cs.SetTexture(m_CSKernelIdx, "Result", m_rtex);
                if (skin != null)
                {
                    m_cs.SetMatrixArray("_bonesMatrix", m_bonesMatrix);                    
                }
                m_cs.SetBuffer(m_CSKernelIdx, "_Skin", _SkinDataBuffer);
                m_cs.SetFloat("_useSkin", skin!=null?1:0);
                m_cs.SetFloat("_furLength", .1f);



                //indirect buffer
                int[] argData = new int[3];
                argData[0] = m_rtex.width / 16;
                argData[1] = m_rtex.width / 16;
                argData[2] = 1;

                m_argsBuffer = new ComputeBuffer(3, sizeof(int), ComputeBufferType.IndirectArguments);
                m_argsBuffer.SetData(argData);

                m_computeShaderInitialized = true;
            }


            void RunCS()
            {                

                if (!SystemInfo.supportsComputeShaders)                
                    return;

                if (!Application.isPlaying)
                    return;


                if (m_HideStrands)
                    return;

                if (!m_computeShaderInitialized)
                    InitComputeShader();


                UpdateColliders();

                m_cs.SetMatrix("_transformMatrix", transform.localToWorldMatrix);

                //m_cs.SetMatrixArray("_bonesMatrix", m_bonesMatrix);
                
                if(m_matPropBlkHair!=null)
                    m_cs.SetFloat("_furLength", m_matPropBlkHair.GetFloat("_Length"));
               //else
               //     m_cs.SetFloat("_furLength", .1f);
                
                m_cs.SetFloat("_dt", Time.deltaTime);
                m_cs.SetFloat("_timeScale", Time.timeScale * m_motionData.timeScale);

                m_cs.SetFloat("_damping", m_motionData.damping);
                m_cs.SetFloat("_bouncing", m_motionData.bouncing);
                m_cs.SetVector("_Gravity", Physics.gravity * m_motionData.gravity);

                float scaleFactor = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;
                scaleFactor = scaleFactor > 0 ? scaleFactor : 1f;
                m_cs.SetFloat("_MotionFactor", m_motionData.motionFactor / scaleFactor) ;
                m_cs.SetFloat("_CentrigugalFactor", m_motionData.centrigugalFactor);
                m_cs.SetFloat("_Random", m_motionData.random);
                m_cs.SetInt("_CollidersData_length", m_dualSphereColliders.Length);
                m_cs.SetFloat("_InteractionOffsetFactor", m_motionData.interactionOffsetFactor); 
                m_cs.SetBuffer(m_CSKernelIdx, "_CollidersData_Buffer", _DualSphereCollidersDataBuffer);

                m_cs.SetBool("_CSInitialized", m_CSInitialized);
                m_CSInitialized = true;


                m_time.x = Time.time / 20f;
                m_time.y = Time.time;
                m_time.z = Time.time * 2f;
                m_time.w = Time.time * 3f;
                m_cs.SetVector("_Time", m_time);

                if (m_hd.m_windZone != null)
                {
                    m_cs.SetVector("KHD_windZoneDir", m_hd.m_windZoneDir);
                    m_cs.SetVector("KHD_windZoneParam", m_hd.m_windZoneParam);

                    m_cs.SetFloat("_WindMain", m_motionData.windMain);
                    m_cs.SetFloat("_WindTurbulence", m_motionData.windTurbulence);
                }
                else
                {
                    m_cs.SetFloat("_WindMain", 0f);
                    m_cs.SetFloat("_WindTurbulence", 0f);
                }



                if (m_matPropBlkHair != null)
                {
                    m_matPropBlkHair.SetFloat("_MotionTextureSize", (float)m_rtex.width);
                    m_matPropBlkHair.SetTexture("_MotionTex", m_rtex);
                    
                    //m_matPropBlkHair.SetFloat("_MotionFactor", 1);
                }


                if (skin != null)
                {
                    for (int i = 0; i < m_bonesMatrix.Length; ++i)
                    {
                        m_bonesMatrix[i] = m_bones[i].localToWorldMatrix * meshBindposes[i];
                    }
                    m_cs.SetMatrixArray("_bonesMatrix", m_bonesMatrix);
                    
                }


                
                if(m_UpdateIndirect)
                    m_cs.DispatchIndirect(m_CSKernelIdx, m_argsBuffer);
                else
                    m_cs.Dispatch(m_CSKernelIdx, m_rtex.width / 16, m_rtex.height / 16, 1);



            }















            void InitMotionZones()
            {
                m_motionZonePos = new Vector4[m_motionZones.Count];
                for (int i = 0; i<m_motionZonePos.Length; ++i)
                    m_motionZonePos[i] = Vector4.zero;

                m_motionZoneDir = new Vector4[m_motionZones.Count];
                for (int i = 0; i<m_motionZoneDir.Length; ++i)
                    m_motionZoneDir[i] = Vector4.zero;

                for (int i = 0; i < m_motionZones.Count; ++i)
                    m_motionZones[i].pid.Init();

            }


          

            public void Init()
            {

                m_furStrandObject = GenerateFurObject("AdvancedFurStrands",ref m_furStrandObject, ref m_furRendererStrands, ref m_materialStrandEnabled, m_hairMeshMaterial);
                m_furShellObject = GenerateFurObject("AdvancedFurShells", ref m_furShellObject, ref m_furRendererShells, ref m_materialShellEnabled, m_furShellMaterial);

                m_smrF = m_furRendererStrands as SkinnedMeshRenderer;
                m_smrS = m_furRendererShells as SkinnedMeshRenderer;

                m_shaderNeedUpdate = true;
            }




            public GameObject GenerateFurObject( string name, ref GameObject furObject,ref Renderer renderTarget, ref List<bool> enableMaterial, Material material )
            {

                if (m_hd == null)
                    return null;

                if (furObject != null)
                {
                    if (Application.isPlaying)
                        Destroy(furObject);
                    else
                        DestroyImmediate(furObject);
                }


                if (m_hiddenMaterial == null)
                {
                    m_hiddenMaterial = Resources.Load<Material>("HairDesigner_Hidden");
                    if (m_hiddenMaterial == null)
                        Debug.LogError("Can't load HairDesigner_Hidden material");
                }

                furObject = new GameObject(name);
                furObject.hideFlags = HideFlags.HideAndDontSave;
                //furObject.hideFlags = HideFlags.DontSave;
                furObject.layer = gameObject.layer;

                /*
                furObject.transform.parent = m_hd.transform.parent;
                furObject.transform.localPosition = m_hd.transform.localPosition;
                furObject.transform.localScale = m_hd.transform.localScale;
                furObject.transform.localRotation = m_hd.transform.localRotation;
                */
                furObject.transform.parent = m_hd.transform;
                furObject.transform.localPosition = Vector3.zero;
                furObject.transform.localScale = Vector3.one;
                furObject.transform.localRotation = Quaternion.identity;

                if (m_hd.m_mf != null)
                {
                    MeshFilter mf = furObject.AddComponent<MeshFilter>();
                    mf.sharedMesh = Instantiate(m_hd.m_mf.sharedMesh);
                    Bounds bnd = mf.sharedMesh.bounds;
                    bnd.extents *= 2f;
                    mf.sharedMesh.bounds = bnd;

                    MeshRenderer mr = furObject.AddComponent<MeshRenderer>();
                    


                    Material[] lst = new Material[m_hd.m_mr.sharedMaterials.Length];
                    for(int i=0;i< m_hd.m_mr.sharedMaterials.Length;++i)
                    {
                        if (enableMaterial.Count == i)
                            enableMaterial.Add(true);

                        lst[i] = enableMaterial[i] ? material : m_hiddenMaterial;
                    }
                    mr.sharedMaterials = lst;

                    renderTarget = mr;
                    
                }


                if (m_hd.m_smr != null)
                {
                    SkinnedMeshRenderer smr = furObject.AddComponent<SkinnedMeshRenderer>();
                    smr.sharedMesh = m_hd.m_smr.sharedMesh;
                    smr.rootBone = m_hd.m_smr.rootBone;
                    smr.bones = m_hd.m_smr.bones;
                    smr.localBounds = m_hd.m_smr.localBounds;

                    Material[] lst = new Material[m_hd.m_smr.sharedMaterials.Length];
                    for (int i = 0; i < m_hd.m_smr.sharedMaterials.Length; ++i)
                    {
                        if (enableMaterial.Count == i)
                            enableMaterial.Add(true);
                        lst[i] = enableMaterial[i] ? material : m_hiddenMaterial;
                    }
                    smr.sharedMaterials = lst;

                    renderTarget = smr;
                }


                return furObject;
            }



            float m_lastUpdateTime;
            float[] m_blendShapeWeights = null;


            public override void UpdateInstance()
            {
                //motion zone                
                if (this == null)
                    return;



                base.UpdateInstance();




                if (m_furStrandObject == null || m_furShellObject == null)
                    Init();

               

                 //return;


                if ( !m_HideStrands && m_enable )
                {
                    if (m_furStrandObject.activeSelf != m_enable)
                        m_furStrandObject.SetActive(m_enable);                    
                }
                else
                {
                    if(m_furStrandObject.activeSelf)
                        m_furStrandObject.SetActive(false);
                }

                if (!m_HideShells && m_enable )
                {
                    if (m_furShellObject.activeSelf != m_enable)
                        m_furShellObject.SetActive(m_enable);                    
                }
                else
                {
                    if (m_furShellObject.activeSelf)
                        m_furShellObject.SetActive(false);
                }

                if (!IsActive || (m_HideStrands && m_HideShells) )
                    return;


                m_furRendererStrands.shadowCastingMode = m_strandsShadowCastingMode;
                m_furRendererShells.shadowCastingMode = m_shellsShadowCastingMode;



                if (m_furRendererStrands!=null)
                    m_furRendererStrands.SetPropertyBlock(m_matPropBlkHair);

                if (m_furRendererShells != null)
                    m_furRendererShells.SetPropertyBlock(m_matPropBlkHair);

                             


            
                if (Application.isPlaying && m_lastUpdateTime < Time.unscaledTime )//only update once per frame : ignore LOD update
                {
                    m_lastUpdateTime = Time.unscaledTime;

                    //Update Motion zones

                    if (m_motionZonePos.Length != m_motionZones.Count)
                        InitMotionZones();

                    for (int i = 0; i < m_motionZones.Count; ++i)
                    {
                        Vector3 newpos = m_motionZones[i].parent.TransformPoint(m_motionZones[i].localPosition);
                        m_motionZones[i].motion = (newpos - m_motionZones[i].lastPosition) * m_motionZones[i].motionFactor;
                        m_motionZones[i].lastPosition = newpos;
                        m_motionZones[i].pid.m_params.limits = m_motionZones[i].motionLimit * m_hd.globalScale;
                        m_motionZones[i].pid.m_target = Vector3.Lerp(m_motionZones[i].pid.m_target, m_motionZones[i].motion, Time.deltaTime * m_motionZones[i].smooth);
                        m_motionZoneDir[i] = m_motionZones[i].pid.Compute(m_motionZoneDir[i]);
                        Vector4 v = m_motionZones[i].parent.TransformPoint(m_motionZones[i].localPosition);
                        v.w = m_motionZones[i].radius * m_hd.globalScale;
                        m_motionZonePos[i] = v;
                        //Debug.Log(v);
                    }



                    //Update blendshapes
                    if (HairDesignerBase.m_shaderIDs != null && m_furShellMaterial != null)
                    {

                        if (Application.isPlaying && m_motionZonePos.Length > 0)
                        {
                            m_furShellMaterial.SetVectorArray(HairDesignerBase.m_shaderIDs[14], m_motionZonePos);
                            m_furShellMaterial.SetVectorArray(HairDesignerBase.m_shaderIDs[15], m_motionZoneDir);
                        }

                        m_furShellMaterial.SetVector(HairDesignerBase.m_shaderIDs[10], Physics.gravity);
                    }

                    if (skin != null && skin.sharedMesh.blendShapeCount>0)
                    {

                        if (m_blendShapeWeights == null)
                            m_blendShapeWeights = new float[skin.sharedMesh.blendShapeCount];

                        for (int i = 0; i < skin.sharedMesh.blendShapeCount; ++i)
                        {
                            float w = skin.GetBlendShapeWeight(i);

                            if (m_blendShapeWeights[i] != w)
                            {
                                m_blendShapeWeights[i] = w;
                                if (m_smrF != null)
                                    m_smrF.SetBlendShapeWeight(i, w);
                                if (m_smrS != null)
                                    m_smrS.SetBlendShapeWeight(i, w);
                            }
                        }

                    }
                }
                

            }

            
            public override void LateUpdateInstance()
            {
                if (!Application.isPlaying)
                    return;

                base.LateUpdateInstance();

                if (IsActive && m_updateMotion)
                    RunCS();
                
                                
            }
            



            


           
            


           



            /*

            public int GetLOD(Camera cam )
            {
                float dist = Vector3.Distance(cam.transform.position, transform.position);
                
                for (int i = 0; i < m_LODGroups.Count; ++i)
                {
                    //if (dist >= m_LODGroups[i].m_range.x && dist <= m_LODGroups[i].m_range.y)
                    if ( dist < m_LODGroups[i].m_range.y)
                        return i;
                }
                return m_LODGroups.Count - 1;
            }
            */


            /*
            void OnWillRenderObject()
            {                
                
                if (!m_enable || m_root == null)
                    return;
                //m_root.SetActive(m_enable);//doesn't work in unity 5.6
                if (!m_useLOD || m_LODGroups.Count==0)
                    return;
                int lodId = GetLOD(Camera.current);
                m_shellCount = m_LODGroups[lodId].m_shellCount;
                m_shadowCastingMode = m_LODGroups[lodId].m_shadowCastingMode;
                m_furWidthUpscale = m_LODGroups[lodId].m_maskLOD;
                allowShellGeneration = false;
                UpdateShells();
                allowShellGeneration = true;
            }
            */



            /*
            public override void EnableGenerator(bool e)
            {
                if(m_root!=null)
                    m_root.SetActive(e);
            }
            */

                
            public override void Disable()
            {
                if (m_furStrandObject != null)
                    m_furStrandObject.SetActive(false);
                if (m_furShellObject != null)
                    m_furShellObject.SetActive(false);
            }

            public override void Enable()
            {
                /*
                if (m_furStrandObject != null)
                    m_furStrandObject.SetActive(!m_HideStrands);
                if (m_furShellObject != null)
                    m_furShellObject.SetActive(!m_HideShells);
                */
            }


            

            public void CleanFurOjects()
            {
                if (m_furStrandObject != null)
                {
                    if (Application.isPlaying)
                        Destroy(m_furStrandObject);
                    else
                        DestroyImmediate(m_furStrandObject);

                    m_furStrandObject = null;
                }

                if (m_furShellObject != null)
                {
                    if (Application.isPlaying)
                        Destroy(m_furShellObject);
                    else
                        DestroyImmediate(m_furShellObject);

                    m_furShellObject = null;
                }
            }


           
            public override void Destroy()
            {
                CleanFurOjects();

                if (_vertexBuffer != null) _vertexBuffer.Release();
                _vertexBuffer = null;

                if (_normalBuffer != null) _normalBuffer.Release();
                _normalBuffer = null;

                if (_lastWorldNormalBuffer != null) _lastWorldNormalBuffer.Release();
                _lastWorldNormalBuffer = null;                

                if (_worldPosBuffer != null) _worldPosBuffer.Release();
                _worldPosBuffer = null;

                if (_PIBuffer != null) _PIBuffer.Release();
                _PIBuffer = null;

                if (_SkinDataBuffer != null) _SkinDataBuffer.Release();
                _SkinDataBuffer = null;

                if (_DualSphereCollidersDataBuffer != null) _DualSphereCollidersDataBuffer.Release();
                _DualSphereCollidersDataBuffer = null;

                if(m_rtex != null )
                    m_rtex.Release();

                if (m_argsBuffer != null)
                    m_argsBuffer.Release();


                base.Destroy();
            }
        }
    }
}
