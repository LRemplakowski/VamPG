using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [System.Serializable, ExecuteInEditMode]
        public class ShellData : MonoBehaviour
        {
            public int m_goId;
            public HairDesignerGeneratorFurShellBase m_fur;

            void OnStart()
            {
                if (m_fur == null || m_fur.m_rootData != this)
                {
                    if (Application.isPlaying)
                        Destroy(gameObject);
                    else
                        DestroyImmediate(gameObject);

                }
            }
        }


        [DefaultExecutionOrder(210)]
        [System.Serializable, ExecuteInEditMode]
        public class HairDesignerGeneratorFurShellBase : HairDesignerGenerator
        {
            [System.Serializable]
            public class LODGroup
            {
                public Vector2 m_range = new Vector2(0, 100);
                public int m_shellCount = 30;
                public UnityEngine.Rendering.ShadowCastingMode m_shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                public float m_maskLOD = 0f;
            }

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
            public int m_shellCount = 30;
            public UnityEngine.Rendering.ShadowCastingMode m_shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            public float m_furWidthUpscale = 0f;
            //public bool m_needRefresh = false;
            /*
            public bool m_recalculateNormals = false;
            public bool m_recalculateBounds = false;
            */
            public List<bool> m_materialsEnabled = new List<bool>();
            public bool m_useLOD = false;
            public bool m_selectCurrentLOD = true;
            public List<LODGroup> m_LODGroups = new List<LODGroup>();
            public bool m_firstInitialisation = false;

            /*
            bool m_enableDrawMeshInstanced = false;//don't work yet with LOD
            float[] m_furFactorArray = null;
            float[] m_maskLODArray = null;
            Matrix4x4[] m_matrixArray = null;
            */

            public Material m_furMaterial;
            [System.NonSerialized]
            public Material m_furMaterialOpaque;//used in HDRP

            public float m_transparencyThreshold = 1f;
            public Texture2D m_defaultFurDensity;
            MaterialPropertyBlock m_pb;
            MeshFilter m_mf;
            public MeshRenderer m_mr;
            public SkinnedMeshRenderer m_smr;
            public Renderer m_r;
            [System.NonSerialized]
            GameObject m_root = null;
            public ShellData m_rootData;

            [HideInInspector]
            [System.NonSerialized]
            public List<MeshRenderer> m_shells = new List<MeshRenderer>();
            [System.NonSerialized]
            public List<MeshFilter> m_shellsMF = new List<MeshFilter>();

            [System.NonSerialized]
            Mesh m_mesh;
            [HideInInspector]
            public Material m_hiddenMaterial;

            Camera m_lodCam = null;

            public Vector2[] m_verticeIds = null;
            public bool m_bakeVertexID = false;
            public bool m_gpuSkinning = false;

            Matrix4x4[] m_shellMatrixLst = null;
            float[] m_shellFactor = null;

            int _SP_GPUSkinning;
            int _SP_FurFactor;
            int _SP_MaskLOD;


            [SerializeField]
            private eInstancingMode m_instancingMode = eInstancingMode.UNDEFINED;            

            public eInstancingMode GetInstancingMode
            {
                get { return Application.isPlaying ? m_instancingMode : eInstancingMode.GPU_INSTANCING;  }             
            }

            public eInstancingMode RuntimeInstancingMode
            {
                get { return m_instancingMode; }
                set { m_instancingMode = value; }
            }
                     


            public bool _useHDRPInstancingTrick = false;
            public bool m_MII_useRendererBoundingBox = true;
            public Vector3 m_MII_boundingBoxExtents = Vector3.one;
            public Vector3 m_MII_boundingBoxCenter = Vector3.zero;
            public bool m_MII_showBoundingBox = false;
            public float m_MII_boundingBoxScale = 1.5f;
            public bool m_MII_drawBoundingBox = false;

            private ComputeBuffer[] m_argsOpaqueBuffer = null;
            private ComputeBuffer[] m_argsTransparentBuffer = null;

            private ComputeBuffer[] m_furOpaqueBuffer = null;
            private List<BufferData> m_furOpaqueBufDataLst = new List<BufferData>();
            private ComputeBuffer[] m_furTransparentBuffer = null;
            private List<BufferData> m_furTransparentBufDataLst = new List<BufferData>();

            private uint[] m_argsOpaque = new uint[5] { 0, 0, 0, 0, 0 };
            private uint[] m_argsTransparent = new uint[5] { 0, 0, 0, 0, 0 };

            public Bounds m_MII_Bounbs = new Bounds();

            [System.NonSerialized]
            private bool m_registerEventInitialized = false;
            


            public void RegisterEvents()
            {
                HairDesignerEvents.RegisterPlayStateChanged(OnPlayStateChanged);
                HairDesignerEvents.RegisterPauseStateChanged(OnPauseStateChanged);
                m_registerEventInitialized = true;
            }


           

            public void Awake()
            {
                InitShaderid();
                CleanComputeBuffers();
                /*
                if (m_hd != null)
                {
                    m_mf = m_hd.m_mf;
                    m_mr = m_hd.m_mr;
                    m_smr = m_hd.m_smr;

                }
                else
                */
                {
                    m_mf = GetComponent<MeshFilter>();
                    m_mr = GetComponent<MeshRenderer>();
                    m_smr = GetComponent<SkinnedMeshRenderer>();
                }

                m_r = m_mr == null ? m_smr as Renderer : m_mr as Renderer;

                
                InitMotionZones();
                m_csInitialized = false;

                if (!Application.isEditor)
                {
                    ClearShells();
                }

                //fix duplication
                {
                    m_furMaterial = null;
                    m_furMaterialOpaque = null;
                    m_materialInitialized = false;
                }

                if (Application.isPlaying && m_hd != null)
                {
                    InitMaterial();
                    //Create an instance for this generator                    
                    m_furMaterial = new Material(m_furMaterial);
                    m_furMaterial.name += "_";
                }

                
            }


            void InitMotionZones()
            {
                m_motionZonePos = new Vector4[m_motionZones.Count];
                for (int i = 0; i < m_motionZonePos.Length; ++i)
                    m_motionZonePos[i] = Vector4.zero;

                m_motionZoneDir = new Vector4[m_motionZones.Count];
                for (int i = 0; i < m_motionZoneDir.Length; ++i)
                    m_motionZoneDir[i] = Vector4.zero;

                for (int i = 0; i < m_motionZones.Count; ++i)
                    m_motionZones[i].pid.Init();

            }


            public override void Start()
            {
                if (m_enable)
                    Init();
            }



            void OnPlayStateChanged(HairDesignerEvents.PlayModeStateChange state )
            {
                if (state == HairDesignerEvents.PlayModeStateChange.ExitingEditMode)
                {
                    if (m_instancingMode == eInstancingMode.MESH_INSTANCING_INDIRECT)
                    {
                        CleanMIIComputeBuffers();//Fix GC warnings                    
                        m_instancingMode = eInstancingMode.GPU_INSTANCING;
                        UpdateShells();                        
                        m_instancingMode = eInstancingMode.MESH_INSTANCING_INDIRECT;
                        m_destroyed = true;
                    }
                }
            }


            eInstancingMode m_instancingModeBeforePause;
            void OnPauseStateChanged(HairDesignerEvents.PauseState state)
            {
                if (Application.isPlaying)
                {
                    if (state == HairDesignerEvents.PauseState.Paused)
                    {
                        m_instancingModeBeforePause = m_instancingMode;

                        if (m_instancingModeBeforePause == eInstancingMode.MESH_INSTANCING_INDIRECT || m_instancingModeBeforePause == eInstancingMode.MESH_INSTANCING )
                        {
                            m_instancingMode = eInstancingMode.GPU_INSTANCING;
                            UpdateShells();
                        }
                    }
                    else
                    {
                        if (m_instancingModeBeforePause == eInstancingMode.MESH_INSTANCING_INDIRECT || m_instancingModeBeforePause == eInstancingMode.MESH_INSTANCING)
                        {
                            m_instancingMode = m_instancingModeBeforePause;
                            ClearShells();
                        }
                    }
                }
            }

           




            public void Init()
            {


                if (m_gpuSkinning && !SystemInfo.supportsComputeShaders)
                {
                    Debug.LogWarning("Compute shader not supported\n->GPU skinning disabled");
                    m_gpuSkinning = false;
                }

                //if( m_rootData == null )
                ClearShells();


                m_root = new GameObject("FurShell_root_" + gameObject.GetInstanceID());

                //if (Application.isEditor)
                m_root.hideFlags = HideFlags.HideAndDontSave;
                //m_root.hideFlags = HideFlags.DontSave;
                //m_root.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;

                /*
                m_root.transform.parent = null;
                m_root.transform.localScale = Vector3.one;
                m_root.transform.position = transform.position;
                m_root.transform.rotation = transform.rotation;
                */
                m_root.transform.parent =  m_hd == null ? transform : m_hd.transform;
                m_root.transform.localScale = Vector3.one;
                m_root.transform.localPosition = Vector3.zero;
                m_root.transform.localRotation = Quaternion.identity;


                m_rootData = m_root.AddComponent<ShellData>();
                m_rootData.m_goId = gameObject.GetInstanceID();
                m_rootData.m_fur = this;

                //m_pb = new MaterialPropertyBlock();



                m_mf = GetComponent<MeshFilter>();
                m_mr = GetComponent<MeshRenderer>();
                m_smr = GetComponent<SkinnedMeshRenderer>();
                m_r = m_mr == null ? m_smr as Renderer : m_mr as Renderer;
                m_gpuSkinning = false;


                if (m_smr != null)
                {
                    m_mesh = new Mesh();
                    //m_mesh.MarkDynamic();
                }

                //m_matPropBlkHair.SetVectorArray(HairDesignerBase.m_shaderIDs[14], new Vector4[50]);
                //m_matPropBlkHair.SetVectorArray(HairDesignerBase.m_shaderIDs[15], new Vector4[50]);

                if (m_hiddenMaterial == null)
                    m_hiddenMaterial = new Material(Shader.Find("Hidden/HairDesigner/Hidden"));

#if UNITY_5_6_OR_NEWER
                m_hiddenMaterial.enableInstancing = true;
#endif



                if (!Application.isPlaying)
                    UpdateShells();

            }



            public override void UpdateInstance()
            {
                
                //motion zone                
                if (this == null)
                    return;


                base.UpdateInstance();


                if (m_enable && m_root == null)
                    Init();


                if (!m_registerEventInitialized)
                    RegisterEvents();


                if (m_root == null)
                    return;


                if (m_root.activeSelf != m_enable)
                    m_root.SetActive(m_enable);

                if (!m_enable || m_destroyed)
                    return;

                /*
                if (!m_useLOD)
                    UpdateShells();
                    */
                /*
                m_root.transform.parent = null;
                m_root.transform.localScale = Vector3.one;
                if (m_smr != null)
                    m_root.transform.localScale = Vector3.one;
                else
                    m_root.transform.localScale = transform.lossyScale;
                m_root.transform.position = transform.position;
                m_root.transform.rotation = transform.rotation;
                */


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


                if (HairDesignerBase.m_shaderIDs != null && m_furMaterial != null)
                {

                    if (m_motionZonePos.Length > 0)
                    {
                        m_furMaterial.SetVectorArray(HairDesignerBase.m_shaderIDs[14], m_motionZonePos);
                        m_furMaterial.SetVectorArray(HairDesignerBase.m_shaderIDs[15], m_motionZoneDir);
                    }

                    m_furMaterial.SetVector(HairDesignerBase.m_shaderIDs[10], Physics.gravity);
                }

                if (HairDesignerBase.m_shaderIDs != null && m_furMaterialOpaque != null)
                {

                    if (m_motionZonePos.Length > 0)
                    {
                        m_furMaterialOpaque.SetVectorArray(HairDesignerBase.m_shaderIDs[14], m_motionZonePos);
                        m_furMaterialOpaque.SetVectorArray(HairDesignerBase.m_shaderIDs[15], m_motionZoneDir);
                    }

                    m_furMaterialOpaque.SetVector(HairDesignerBase.m_shaderIDs[10], Physics.gravity);
                }


            }


            public override void LateUpdateInstance()
            {
                if (!m_useLOD)
                    UpdateShells();
            }




            //Destroy all shells
            public void ClearShells()
            {
                /*
                if (m_rootData == null)
                    return;
                    */

                if (m_rootData != null && m_rootData.m_goId != gameObject.GetInstanceID())
                    return;

                for (int i = 0; i < m_shells.Count; ++i)
                {
                    if (m_shells[i] == null)
                        continue;

                    if (Application.isPlaying)
                        Destroy(m_shells[i].gameObject);
                    else
                        DestroyImmediate(m_shells[i].gameObject);
                }
                if (m_root != null)
                {
                    if (Application.isPlaying)
                        Destroy(m_root.gameObject);
                    else
                        DestroyImmediate(m_root.gameObject);
                }
                m_shells.Clear();
            }


            void InitShaderid()
            {
                _SP_GPUSkinning = Shader.PropertyToID("_GPUSkinning");
                _SP_FurFactor = Shader.PropertyToID("_FurFactor");
                _SP_MaskLOD = Shader.PropertyToID("_MaskLOD");
            }


            void InitMaterial()
            {
                if (m_hairMeshMaterial == null)
                    return;
                if (m_hd == null)
                    return;

                if (m_furMaterial == null || m_furMaterial.shader != m_hairMeshMaterial.shader || m_furMaterial == m_hairMeshMaterial)
                {


                    HairDesignerShader s = GetShaderParams();
                    if (s == null) return;
                    if (m_hd == null) return;

                    //MeshRenderer mr = m_hd.GetComponent<MeshRenderer>();
                    //SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();                    

                    if (!m_firstInitialisation)
                    {
                        m_firstInitialisation = true;
                        if (s.GetTexture(1) == null)
                        {
                            if (m_defaultFurDensity != null)
                                s.SetTexture(1, m_defaultFurDensity);
                            else
                                s.SetTexture(1, m_hd.m_defaultFurDensity);
                        }
                    }


                    m_furMaterial = new Material(m_hairMeshMaterial);
                    m_furMaterial.name += "cpy";
#if UNITY_5_6_OR_NEWER
                    m_furMaterial.enableInstancing = true;
#endif
                }

            }


            bool allowShellGeneration = true;
            Material[] m_sharedMaterials = null;



            #region DrawIndirect

            public struct BufferData
            {
                public float furFactor;
            }

            

            void UpdateIndirectBuffer(  ref ComputeBuffer furBuff, ref List<BufferData> bufLst,
                                        ref ComputeBuffer argsBuf, ref uint[] args ,
                                        int startIdx, int length, int subMesh)                                
            {

                if (bufLst.Count != length)
                {
                    if(furBuff != null)
                        furBuff.Release();
                    furBuff = null;
                }


                if (furBuff == null )
                {                  

                    int dataSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(BufferData));                    
                    furBuff = new ComputeBuffer(m_shellCount, dataSize, ComputeBufferType.Default);                    
                    bufLst.Clear();

                    while (bufLst.Count < length)
                    {
                        BufferData bd = new BufferData();
                        bd.furFactor = (float)(bufLst.Count + startIdx) / (float)m_shellCount;
                        bufLst.Add(bd);
                    }

                    furBuff.SetData(bufLst);
                }               

                


                // Indirect args
                if (argsBuf == null)
                {
                    argsBuf = new ComputeBuffer(1, m_argsOpaque.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
                }

                if (m_mesh != null)
                {
                    args[0] = (uint)m_mesh.GetIndexCount(subMesh);
                    args[1] = (uint)length;
                    args[2] = (uint)m_mesh.GetIndexStart(subMesh);
                    args[3] = (uint)m_mesh.GetBaseVertex(subMesh);
                }
                else
                {
                    args[0] = args[1] = args[2] = args[3] = 0;
                }
                argsBuf.SetData(args);
            }


            void UpdateDrawIndirectBuffers(int subMesh)
            {


                if (m_furTransparentBuffer == null)
                    m_furTransparentBuffer = new ComputeBuffer[m_mesh.subMeshCount];
                if (m_furOpaqueBuffer == null)
                    m_furOpaqueBuffer = new ComputeBuffer[m_mesh.subMeshCount];
                if (m_argsTransparentBuffer == null)
                    m_argsTransparentBuffer = new ComputeBuffer[m_mesh.subMeshCount];
                if (m_argsOpaqueBuffer == null)
                    m_argsOpaqueBuffer = new ComputeBuffer[m_mesh.subMeshCount];




                int threshold = (int)((float)m_shellCount * m_transparencyThreshold);

                                
                //int startTransparent = threshold;

                if (m_furMaterialOpaque == null)
                {
                    UpdateIndirectBuffer(ref m_furTransparentBuffer[subMesh], ref m_furTransparentBufDataLst, ref m_argsTransparentBuffer[subMesh], ref m_argsTransparent, 0, m_shellCount, subMesh);
                }
                else
                {
                    UpdateIndirectBuffer(ref m_furOpaqueBuffer[subMesh], ref m_furOpaqueBufDataLst, ref m_argsOpaqueBuffer[subMesh], ref m_argsOpaque, 1, Mathf.Clamp(threshold+1,0, m_shellCount), subMesh);
                    UpdateIndirectBuffer(ref m_furTransparentBuffer[subMesh], ref m_furTransparentBufDataLst, ref m_argsTransparentBuffer[subMesh], ref m_argsTransparent, threshold, m_shellCount- threshold, subMesh);
                    
                }
            }

            #endregion


            //update the shell pool
            public void UpdateShells()
            {               

                if (m_hd == null)
                    return;

                if (m_r == null || m_root == null || m_rootData == null || (m_smr==null && m_mf == null))
                {
                    Init();
                }

                InitMaterial();

                //if (m_root.activeSelf != m_enable)
                m_root.SetActive(m_enable);

                if (!m_enable || m_destroyed)
                    return;


                if (m_smr != null)
                {
                    if (m_gpuSkinning)
                    {
                        //if(!m_csInitialized)
                        GPU_Skinning();
                    }
                    else
                    {
                        if(m_mesh == null)
                        	m_mesh = new Mesh();
                        m_smr.BakeMesh(m_mesh);
                        m_csInitialized = false;
                    }
                }

                if (m_mf != null)
                {
                    if (m_mesh == null)
                        m_mesh = m_mf.sharedMesh;
                }

                //m_mesh.RecalculateBounds();

                /*
                if (!m_gpuSkinning && m_recalculateBounds)
                    m_mesh.RecalculateBounds();
                if (!m_gpuSkinning && m_recalculateNormals)
                    m_mesh.RecalculateNormals();
                */



                if (m_furMaterial != null)
                    m_furMaterial.SetInt(_SP_GPUSkinning, m_gpuSkinning ? 1 : 0);

                if (m_furMaterialOpaque != null)
                    m_furMaterialOpaque.SetInt(_SP_GPUSkinning, m_gpuSkinning ? 1 : 0);


                //update current shader
                //GetShaderParams().UpdatePropertyBlock(ref m_matPropBlkHair, HairDesignerBase.eLayerType.FUR_SHELL);
                if (GetShaderParams() != null && m_furMaterial != null)
                    GetShaderParams().UpdateMaterialProperty(ref m_furMaterial, HairDesignerBase.eLayerType.FUR_SHELL);
                //m_matPropBlkHair = new MaterialPropertyBlock();

                if (m_pb == null)
                    m_pb = new MaterialPropertyBlock();

                if (m_smr != null)
                    m_smr.GetPropertyBlock(m_pb);

                if (m_mr != null)
                    m_mr.GetPropertyBlock(m_pb);


                

                if (m_bakeVertexID && m_mesh != null)
                {
                    //generate vertexIDs in uv2 (ShaderGraph doesn't provide the access to VID :/ )
                    if (m_verticeIds == null || m_verticeIds.Length != m_mesh.vertexCount)
                    {
                        m_verticeIds = new Vector2[m_mesh.vertexCount];
                        for (int i = 0; i < m_verticeIds.Length; ++i)
                            m_verticeIds[i].x = (float)i;
                    }
                    m_mesh.uv2 = m_verticeIds;
                    //Debug.Log("VID -> UV2");
                }



                //-------------------------------------


                if (!SystemInfo.supportsInstancing)
                    m_instancingMode = eInstancingMode.GPU_INSTANCING;

                if (_SP_FurFactor == 0)
                    InitShaderid();


                if (GetInstancingMode == eInstancingMode.MESH_INSTANCING)
                {


                    if (m_furMaterial == null || m_mesh == null || m_shellCount == 0)
                        return;

                    int shellCount = m_shellCount;
                    if (m_shellMatrixLst == null || m_shellMatrixLst.Length < shellCount)
                        m_shellMatrixLst = new Matrix4x4[shellCount];

                    if (m_shellFactor == null || m_shellFactor.Length < shellCount)
                        m_shellFactor = new float[shellCount];



                    // m_pb.SetFloat(_SP_FurFactor, .5f);
                    m_pb.SetFloat(_SP_MaskLOD, m_furWidthUpscale);

                    while (m_materialsEnabled.Count < m_mesh.subMeshCount)
                        m_materialsEnabled.Add(true);


                    if (m_furMaterialOpaque == null)
                    {
                        for (int i = 0; i < shellCount; ++i)
                        {
                            float f = ((float)i + 1f) / (float)m_shellCount;
                            m_shellMatrixLst[i] = Matrix4x4.TRS(m_root.transform.position, m_root.transform.rotation, m_root.transform.parent.localScale);
                            m_shellFactor[i] = f;
                        }

                        if (!_useHDRPInstancingTrick)
                            m_pb.SetFloatArray(_SP_FurFactor, m_shellFactor);

                        for (int m = 0; m < m_mesh.subMeshCount; ++m)
                        {
                            if (m_materialsEnabled[m])
                                Graphics.DrawMeshInstanced(m_mesh, m, m_furMaterial, m_shellMatrixLst, m_shellCount, m_pb, m_shadowCastingMode, true, gameObject.layer, m_useLOD ? m_lodCam : null);
                        }
                    }
                    else
                    {
                        int threshold = (int)((float)shellCount * m_transparencyThreshold);

                        for (int i = 0; i < threshold; ++i)
                        {
                            float f = ((float)i + 1f) / (float)m_shellCount;
                            m_shellMatrixLst[i] = Matrix4x4.TRS(m_root.transform.position, m_root.transform.rotation, m_root.transform.localScale);

                            if (_useHDRPInstancingTrick)
                                m_shellMatrixLst[i].m33 = f;//HDRP shadergraph don't support Instanced array
                            else
                                m_shellFactor[i] = f;


                        }
                        if (!_useHDRPInstancingTrick)
                            m_pb.SetFloatArray(_SP_FurFactor, m_shellFactor);//HDRP shadergraph don't support Instanced array


                        for (int m = 0; m < m_mesh.subMeshCount; ++m)
                            if (m_materialsEnabled[m])
                                Graphics.DrawMeshInstanced(m_mesh, m, m_furMaterialOpaque, m_shellMatrixLst, threshold, m_pb, m_shadowCastingMode, true, gameObject.layer, m_useLOD ? m_lodCam : null);


                        for (int i = threshold; i < shellCount; ++i)
                        {
                            float f = ((float)i + 1f) / (float)m_shellCount;
                            m_shellMatrixLst[i - threshold] = Matrix4x4.TRS(m_root.transform.position, m_root.transform.rotation, m_root.transform.parent.localScale);

                            if (_useHDRPInstancingTrick)
                                m_shellMatrixLst[i - threshold].m33 = f;//HDRP shadergraph don't support Instanced array
                            else
                                m_shellFactor[i - threshold] = f;//HDRP shadergraph don't support Instanced array


                        }

                        if (!_useHDRPInstancingTrick)
                            m_pb.SetFloatArray(_SP_FurFactor, m_shellFactor);//HDRP shadergraph don't support Instanced array


                        for (int m = 0; m < m_mesh.subMeshCount; ++m)
                            if (m_materialsEnabled[m])
                            {
                                m_furMaterial.enableInstancing = true;
                                //m_mesh.
                                //Graphics.DrawMeshInstanced(m_mesh, m, m_furMaterial, m_shellMatrixLst, m_shellCount - threshold, m_pb, UnityEngine.Rendering.ShadowCastingMode.Off, true, gameObject.layer, m_useLOD ? m_lodCam : null);
                                Graphics.DrawMeshInstanced(m_mesh, m, m_furMaterial, m_shellMatrixLst, m_shellCount - threshold, m_pb);
                            }







                    }

                    /*
                    for (int i = 0; i < shellCount; ++i)
                    {
                        float f = ((float)i + 1f) / (float)m_shellCount;
                        m_pb.SetFloat(_SP_FurFactor, f);
                        m_pb.SetFloat(_SP_MaskLOD, m_furWidthUpscale);

                        Matrix4x4[] mat = new Matrix4x4[1];
                        mat[0] = Matrix4x4.TRS(m_root.transform.position, m_root.transform.rotation, m_root.transform.localScale);


                        for (int m = 0; m < m_mesh.subMeshCount; ++m)
                        {
                            if (m_materialsEnabled[m] == false)
                                continue;

                            if (f > m_transparencyThreshold || m_furMaterialOpaque == null)
                                Graphics.DrawMeshInstanced(m_mesh, m, m_furMaterial, mat, 1, m_pb, m_shadowCastingMode, true);
                            else
                                Graphics.DrawMeshInstanced(m_mesh, m, m_furMaterialOpaque, mat, 1, m_pb, m_shadowCastingMode, true);

                            
                        }

                    }*/


                    return;
                }



                if( RuntimeInstancingMode == eInstancingMode.MESH_INSTANCING_INDIRECT )
                {
                    if (m_MII_useRendererBoundingBox)
                    {
                        m_MII_Bounbs.center = m_r.bounds.center;
                        m_MII_Bounbs.extents = m_r.bounds.extents * m_MII_boundingBoxScale;
                    }
                    else
                    {
                        m_MII_Bounbs.center = transform.parent.TransformPoint(m_MII_boundingBoxCenter);
                        m_MII_Bounbs.extents = transform.parent.TransformVector(m_MII_boundingBoxExtents);
                    }
                }

                
                if (GetInstancingMode == eInstancingMode.MESH_INSTANCING_INDIRECT )
                {
                    if (m_furMaterial == null || m_mesh == null || m_shellCount == 0)
                        return;
                    
                    Matrix4x4 matrix = Matrix4x4.TRS(m_root.transform.position, m_root.transform.rotation, m_root.transform.parent.localScale);
                    m_furMaterial.SetMatrix("_O2W", matrix);
                    m_furMaterial.SetMatrix("_W2O", matrix.inverse);
                    //m_furMaterial.SetFloat("_InstancedTrick", -1);
                    //m_furMaterial.SetBuffer("FurInstanceDataBuffer", m_furTransparentBuffer);

                    m_pb.SetFloat(_SP_MaskLOD, m_furWidthUpscale);


                    if (m_furMaterialOpaque != null)
                    {
                        m_furMaterialOpaque.SetMatrix("_O2W", matrix);
                        m_furMaterialOpaque.SetMatrix("_W2O", matrix.inverse);
                        //m_furMaterialOpaque.SetFloat("_InstancedTrick", -1);
                        //m_furMaterialOpaque.SetBuffer("FurInstanceDataBuffer", m_furOpaqueBuffer);

                        //if(m_instancingIndirectBounbs == null)
                        //    m_instancingIndirectBounbs = new Bounds(m_root.transform.position, m_mesh.bounds.extents * 1.5f);

                       

                        for (int m = 0; m < m_mesh.subMeshCount; ++m)
                        {
                            if (m_materialsEnabled[m])
                            {
                                UpdateDrawIndirectBuffers(m);
                                m_furMaterial.SetBuffer("FurInstanceDataBuffer", m_furTransparentBuffer[m]);
                                m_furMaterialOpaque.SetBuffer("FurInstanceDataBuffer", m_furOpaqueBuffer[m]);
                                Graphics.DrawMeshInstancedIndirect(m_mesh, m, m_furMaterial, m_MII_Bounbs, m_argsTransparentBuffer[m], 0, m_pb, m_shadowCastingMode, true, gameObject.layer, m_useLOD ? m_lodCam : null);
                                Graphics.DrawMeshInstancedIndirect(m_mesh, m, m_furMaterialOpaque, m_MII_Bounbs, m_argsOpaqueBuffer[m], 0, m_pb, m_shadowCastingMode, true, gameObject.layer, m_useLOD ? m_lodCam : null);
                            }
                        }
                    }
                    else
                    {
                        for (int m = 0; m < m_mesh.subMeshCount; ++m)
                        {
                            if (m_materialsEnabled[m])
                            {
                                UpdateDrawIndirectBuffers(m);
                                m_furMaterial.SetBuffer("FurInstanceDataBuffer", m_furTransparentBuffer[m]);
                                //m_furMaterialOpaque.SetBuffer("FurInstanceDataBuffer", m_furOpaqueBuffer[m]);
                                Graphics.DrawMeshInstancedIndirect(m_mesh, m, m_furMaterial, m_MII_Bounbs, m_argsTransparentBuffer[m], 0, m_pb, m_shadowCastingMode, true, gameObject.layer, m_useLOD ? m_lodCam : null);                                
                            }
                        }
                    }

                    
                    return;
                }



                if (allowShellGeneration)
                    while (m_shellCount > m_shells.Count)
                        GenerateShell();






                for (int i = 0; i < m_shells.Count; ++i)
                {
                    if (m_shells[i] == null)
                        continue;

                    if (m_sharedMaterials == null)
                        m_sharedMaterials = m_shells[i].sharedMaterials;

                    if (i < m_shellCount)
                    {
                        float f = ((float)i + 1f) / (float)m_shellCount;
                        m_pb.SetFloat(_SP_FurFactor, f);
                        m_pb.SetFloat(_SP_MaskLOD, m_furWidthUpscale);
                        m_shells[i].SetPropertyBlock(m_pb);
                        m_shells[i].enabled = true;
                        m_shells[i].shadowCastingMode = m_shadowCastingMode;
                        
                        if (m_shellsMF[i] == null)
                            m_shellsMF[i] = m_shells[i].GetComponent<MeshFilter>();
                        m_shellsMF[i].sharedMesh = m_mesh;


                        
                        for (int m = 0; m < m_sharedMaterials.Length; ++m)
                        {
                            if (m_materialsEnabled.Count == m)
                                m_materialsEnabled.Add(true);
                            if (f > m_transparencyThreshold || m_furMaterialOpaque == null)
                                m_sharedMaterials[m] = m_materialsEnabled[m] ? m_furMaterial : m_hiddenMaterial;
                            else
                                m_sharedMaterials[m] = m_materialsEnabled[m] ? m_furMaterialOpaque : m_hiddenMaterial;
                        }

                        m_shells[i].sharedMaterials = m_sharedMaterials;




                    }
                    else
                    {
                        m_shells[i].enabled = false;
                        //m_shells[i].gameObject.SetActiveRecursively(false;

                        for (int m = 0; m < m_sharedMaterials.Length; ++m)
                        {
                            if (m_materialsEnabled.Count == m)
                                m_materialsEnabled.Add(false);
                            m_sharedMaterials[m] = m_hiddenMaterial;
                        }
                        m_shells[i].sharedMaterials = m_sharedMaterials;
                    }
                }


                //UpdateInstance();

            }





            void GenerateShell()
            {
                if (m_pb == null)
                    m_pb = new MaterialPropertyBlock();
                /*
                if(m_mr != null)
                    m_mr.GetPropertyBlock(m_matPropBlkHair);

                if (m_smr!=null)
                    m_smr.GetPropertyBlock(m_matPropBlkHair);
                */
                float f = (float)(m_shells.Count) / (float)m_shellCount;
                //f = 1 - Mathf.Pow(1 - f, 3);
                f = Mathf.Clamp01(f);
                m_pb.SetFloat(_SP_FurFactor, f);
                m_pb.SetFloat(_SP_MaskLOD, m_furWidthUpscale);

                if (m_root == null)
                    Init();


                GameObject go = new GameObject("shell_" + m_shells.Count);
                go.transform.parent = m_root.transform;
                go.transform.localScale = Vector3.one;
                //go.transform.localScale = transform.localScale;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;

                go.hideFlags = HideFlags.HideAndDontSave;
                //go.hideFlags = HideFlags.DontSave;
                //go.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;
                go.layer = gameObject.layer;

                
                MeshFilter mf = go.AddComponent<MeshFilter>();


                mf.sharedMesh = m_mesh;



                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                //mr.sharedMaterial = m_furMaterial;
                mr.sortingOrder = (int)(f * (float)m_shellCount);
                Material[] sharedMaterials = m_r.sharedMaterials;
                for (int i = 0; i < m_r.sharedMaterials.Length; ++i)
                {
                    if (m_materialsEnabled.Count == i)
                        m_materialsEnabled.Add(true);
                    sharedMaterials[i] = m_materialsEnabled[i] ? m_furMaterial : sharedMaterials[i];
                }

                mr.sharedMaterials = sharedMaterials;
                mr.SetPropertyBlock(m_matPropBlkHair);



                m_shells.Add(mr);
                m_shellsMF.Add(mf);
            }



            public int GetLOD(Camera cam)
            {
                if (cam == null)
                    return 0;

                float dist = Vector3.Distance(cam.transform.position, transform.position);

                for (int i = 0; i < m_LODGroups.Count; ++i)
                {
                    //if (dist >= m_LODGroups[i].m_range.x && dist <= m_LODGroups[i].m_range.y)
                    if (dist < m_LODGroups[i].m_range.y)
                        return i;
                }
                return m_LODGroups.Count - 1;
            }




            public void CameraPreCull(Camera cam)
            {
                if (cam.cameraType == CameraType.Game
                    || cam.cameraType == CameraType.SceneView
                    || cam.cameraType == CameraType.VR)
                    RenderLOD(cam);
            }



            public void RenderLOD(Camera cam)
            {

                if (!m_enable || m_root == null)
                    return;

                //m_root.SetActive(m_enable);//doesn't work in unity 5.6
                if (!m_useLOD || m_LODGroups.Count == 0)
                    return;


                int lodId = GetLOD(cam);
                m_shellCount = m_LODGroups[lodId].m_shellCount;
                m_shadowCastingMode = m_LODGroups[lodId].m_shadowCastingMode;
                m_furWidthUpscale = m_LODGroups[lodId].m_maskLOD;

                m_lodCam = cam;

                /*
                if (!Application.isPlaying && cam.cameraType != CameraType.SceneView)
                    return;
                    */

                allowShellGeneration = false;
                UpdateShells();
                allowShellGeneration = true;
            }




            /*
            public override void EnableGenerator(bool e)
            {
                if(m_root!=null)
                    m_root.SetActive(e);
            }
            */


            void OnDisable()
            {
                if (m_root != null)
                    m_root.SetActive(false);

                //if (m_rootData == null || m_rootData.m_goId != gameObject.GetInstanceID())
                //    ClearShells();

                Camera.onPreCull -= CameraPreCull;
                
            }

            void OnEnable()
            {

                Camera.onPreCull += CameraPreCull;

                if (m_root != null)
                    m_root.SetActive(true);

                ClearShells();
            }





            public override void OnActiveGenerator(bool enable)
            {
                if (m_root != null)
                    m_root.SetActive(enable);


                /*
                //Quick fix for HDRP
                for (int i = 0; i < m_hd.m_generators.Count; ++i)
                {
                    if (m_hd.m_generators[i].m_layerType == HairDesignerBase.eLayerType.FUR_SHELL )
                    {
                        (m_hd.m_generators[i] as HairDesignerGeneratorFurShellBase).ClearShells();
                    }
                }
                */
                /*
            if (enable)
                OnEnable();
            else
                OnDisable();
                */
                //ClearShells();
                //m_csInitialized = false;
            }


            public override void Destroy()
            {
                ClearShells();
                CleanComputeBuffers();
                base.Destroy();                 
            }



            #region GPU skinning
            //-------------------------------------------------------------------------
            //GPU skinning

            [System.Serializable]
            public struct Vector3BufferData
            {
                public Vector3 v;
            }

            [System.Serializable]
            public struct Vector4BufferData
            {
                public Vector4 v;
            }

            struct SkinData
            {
                public Vector4 ids;
                public Vector4 weights;
            };

            [System.NonSerialized]
            ComputeBuffer _vertexBuf = null;
            [System.NonSerialized]
            ComputeBuffer _normalBuf = null;
            [System.NonSerialized]
            ComputeBuffer _tangentBuf = null;
            [System.NonSerialized]
            ComputeBuffer _SkinDataBuffer = null;

            Vector3[] m_verticesArray;
            Vector3[] m_normalsArray;
            Vector4[] m_tangentsArray;
            Matrix4x4[] m_bonesMatrix;
            SkinData[] m_CS_SkinData;
            BoneWeight[] m_meshBoneWeights;
            Matrix4x4[] meshBindposes;
            Transform[] m_bones;

            [System.NonSerialized]
            bool m_csInitialized = false;
            [System.NonSerialized]
            ComputeShader m_cs;
            [System.NonSerialized]
            int m_CSKernelId;

            [System.NonSerialized]
            public RenderTexture _vertexTex = null;
            [System.NonSerialized]
            public RenderTexture _normalTex = null;
            [System.NonSerialized]
            public RenderTexture _tangentTex = null;

            int _SP_bonesMatrix;

            void InitCS()
            {

                m_csInitialized = true;

                _SP_bonesMatrix = Shader.PropertyToID("_bonesMatrix");


                if (m_cs == null)
                {
                    m_cs = Resources.Load<ComputeShader>("HairDesigner_FurSkinning");
                    m_cs = Instantiate(m_cs);
                }

                m_CSKernelId = m_cs.FindKernel("CSMain");

                CleanComputeBuffers();

                m_smr.BakeMesh(m_mesh);
                m_mesh.vertices = m_smr.sharedMesh.vertices;
                m_mesh.normals = m_smr.sharedMesh.normals;
                m_mesh.tangents = m_smr.sharedMesh.tangents;
                m_mesh.name = "GPU_Skinning";

                /*
                if (m_recalculateNormals)
                    m_mesh.RecalculateNormals();
                if (m_recalculateBounds)
                    m_mesh.RecalculateBounds();
                   
                 */

                int vCount = m_mesh.vertexCount;
                m_verticesArray = m_mesh.vertices;
                m_normalsArray = m_mesh.normals;
                m_tangentsArray = m_mesh.tangents;
                /*
                m_verticesLst = new List<Vector3>(vertices);
                m_normalsLst = new List<Vector3>(normals);
                */

                int dataSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3BufferData));




                _vertexBuf = new ComputeBuffer(vCount, dataSize);
                _vertexBuf.SetData(m_verticesArray);
                /*
                _vertexBufRef = new ComputeBuffer(vCount, dataSize);
                _vertexBufRef.SetData(m_verticesArray);
                */

                _normalBuf = new ComputeBuffer(vCount, dataSize);
                _normalBuf.SetData(m_normalsArray);

                dataSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4BufferData));

                _tangentBuf = new ComputeBuffer(vCount, dataSize);
                _tangentBuf.SetData(m_tangentsArray);

                /*
                _normalBufRef = new ComputeBuffer(vCount, dataSize);
                _normalBufRef.SetData(m_normalsArray);
                */
                /*
                m_cs.SetBuffer(m_CSKernelId, "_vertexBufRef", _vertexBufRef);
                m_cs.SetBuffer(m_CSKernelId, "_normalBufRef", _normalBufRef);
                */
                m_bones = m_smr.bones;
                meshBindposes = m_smr.sharedMesh.bindposes;
                m_bonesMatrix = new Matrix4x4[m_smr.bones.Length];
                m_CS_SkinData = new SkinData[vCount];

                m_meshBoneWeights = m_smr.sharedMesh.boneWeights;
                for (int i = 0; i < vCount; ++i)
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

                _SkinDataBuffer = new ComputeBuffer(vCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(SkinData)));
                _SkinDataBuffer.SetData(m_CS_SkinData);


                int textureSize = 1024;
                int maxSize = 1024;

                while (maxSize > 8)
                {
                    if (vCount <= maxSize * maxSize)
                        textureSize = maxSize;
                    else
                        break;
                    maxSize /= 2;
                }

                _vertexTex = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.DefaultHDR);
                _vertexTex.enableRandomWrite = true;
                _vertexTex.Create();

                _normalTex = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.DefaultHDR);
                _normalTex.enableRandomWrite = true;
                _normalTex.Create();

                _tangentTex = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.DefaultHDR);
                _tangentTex.enableRandomWrite = true;
                _tangentTex.Create();


                m_cs.SetBuffer(m_CSKernelId, "_vertexBuf", _vertexBuf);
                m_cs.SetBuffer(m_CSKernelId, "_normalBuf", _normalBuf);
                m_cs.SetBuffer(m_CSKernelId, "_tangentBuf", _tangentBuf);

                m_cs.SetTexture(m_CSKernelId, "_vertexTex", _vertexTex);
                m_cs.SetTexture(m_CSKernelId, "_normalTex", _normalTex);
                m_cs.SetTexture(m_CSKernelId, "_tangentTex", _tangentTex);

                m_cs.SetInt("_vertexCount", vCount);
                m_cs.SetInt("_textureSize", _vertexTex.width);

                m_cs.SetBuffer(m_CSKernelId, "_Skin", _SkinDataBuffer);


                if (m_furMaterial != null)
                {
                    m_furMaterial.SetTexture("_VertexSkinningTex", _vertexTex);
                    m_furMaterial.SetTexture("_NormalSkinningTex", _normalTex);
                    m_furMaterial.SetTexture("_TangentSkinningTex", _tangentTex);
                    m_furMaterial.SetInt("_BrushTextureSize", _vertexTex.width);
                }

                if (m_furMaterialOpaque != null)
                {
                    m_furMaterialOpaque.SetTexture("_VertexSkinningTex", _vertexTex);
                    m_furMaterialOpaque.SetTexture("_NormalSkinningTex", _normalTex);
                    m_furMaterial.SetTexture("_TangentSkinningTex", _tangentTex);
                    m_furMaterialOpaque.SetInt("_BrushTextureSize", _vertexTex.width);
                }





                m_csInitialized = true;
            }


            /*
            public override void OnDestroy()
            {
                Debug.Log("OnDestroy()");
                base.OnDestroy();
                CleanMIIComputeBuffers();
            }*/


            void CleanMIIComputeBuffers()
            {
                //Debug.Log("m_furOpaqueBuffer == null ? " + (m_furOpaqueBuffer == null) + " -> CleanMIIComputeBuffers  /  is playing ? " + Application.isPlaying);
                
                if(m_argsOpaqueBuffer!=null)
                    for (int m = 0; m < m_argsOpaqueBuffer.Length; ++m)
                        ClearBuffer(ref m_argsOpaqueBuffer[m]);

                if (m_argsTransparentBuffer != null)
                    for (int m = 0; m < m_argsTransparentBuffer.Length; ++m)
                        ClearBuffer(ref m_argsTransparentBuffer[m]);

                if (m_furOpaqueBuffer != null)
                    for (int m = 0; m < m_furOpaqueBuffer.Length; ++m)
                        ClearBuffer(ref m_furOpaqueBuffer[m]);

                if (m_furTransparentBuffer != null)
                    for (int m = 0; m < m_furTransparentBuffer.Length; ++m)
                        ClearBuffer(ref m_furTransparentBuffer[m]);
                

                m_furOpaqueBufDataLst.Clear();
                m_furTransparentBufDataLst.Clear();
            }


            void CleanComputeBuffers()
            {
                //Debug.Log("Clean CB");

                if (_vertexBuf != null)
                    _vertexBuf.Release();
                if (_normalBuf != null)
                    _normalBuf.Release();
                if (_tangentBuf != null)
                    _tangentBuf.Release();

                /*
                if (_vertexBufRef != null)
                    _vertexBufRef.Dispose();
                if (_normalBufRef != null)
                    _normalBufRef.Dispose();
                    */

                if (_vertexTex != null)
                    _vertexTex.Release();
                if (_normalTex != null)
                    _normalTex.Release();
                if (_tangentTex != null)
                    _tangentTex.Release();

                if (_SkinDataBuffer != null)
                    _SkinDataBuffer.Release();



                //Draw Indirect buffers

                CleanMIIComputeBuffers();



        }

            void ClearBuffer(ref ComputeBuffer cpb )
            {
                if (cpb != null)
                    cpb.Release();
                cpb = null;
            }



            void GPU_Skinning()
            {
                //m_csInitialized = false;

                //return;
                if (_vertexTex == null)
                    m_csInitialized = false;

                if (!m_csInitialized)
                    InitCS();


                Matrix4x4 trs = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, transform.lossyScale);
                for (int i = 0; i < m_bonesMatrix.Length; ++i)
                {
                    m_bonesMatrix[i] = trs * transform.worldToLocalMatrix * m_bones[i].localToWorldMatrix * meshBindposes[i];
                }
                m_cs.SetMatrixArray(_SP_bonesMatrix, m_bonesMatrix);

                uint x, y, z;
                m_cs.GetKernelThreadGroupSizes(m_CSKernelId, out x, out y, out z);

                int X = Mathf.Max(1, _vertexTex.width / ((int)x) + 1);
                int Y = Mathf.Max(1, Mathf.Min(m_mesh.vertexCount / _vertexTex.width + 1, _vertexTex.height) / ((int)y) + 1);


                m_cs.Dispatch(m_CSKernelId, X, Y, 1);

            }

            #endregion

            /*
            void OnDrawGizmos()
            {

                var bb = GetComponent<Renderer>().bounds;
                Gizmos.DrawWireCube(bb.center, bb.size);

                if (m_MII_useRendererBoundingBox && m_MII_drawBoundingBox && m_instancingMode == eInstancingMode.MESH_INSTANCING_INDIRECT)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(m_MII_Bounbs.center, m_MII_Bounbs.size);
                }
            }
            */
        }
    }
}
