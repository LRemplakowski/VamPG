//#define HAIRDESIGNER_DLL_TIME_LIMIT
#define HAIRDESIGNER_ADVANCEDFUR_WIP

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kalagaan;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [DefaultExecutionOrder(200)]
        [ExecuteInEditMode]
        public class HairDesignerBase : MonoBehaviour
        {            
            public static string version = "1.10.3";//package version
            public string m_version = "1.10.3";//instance version
            public string m_prefabName = "Layers";
            public bool m_meshSaved = false;
            public Material m_defaultMaterial;
            public Quaternion[] m_poseBoneRotations=null;
            public bool m_debugDontHideCollider = false;
            public Material m_defaultFurMaterial;
            public Texture2D m_defaultFurDensity;

            public SkinnedMeshRenderer m_smr;
            public MeshRenderer m_mr;
            public MeshFilter m_mf;
            public TPoseUtility m_tpose = null;            
            public WindZone m_windZone = null;
            public Vector4 m_windZoneDir = Vector4.zero;
            public Vector4 m_windZoneParam = Vector4.zero;

#if HAIRDESIGNER_DLL_TIME_LIMIT
            static System.DateTime m_startTime = System.DateTime.MinValue;
#endif


            public enum eLayerType
            {
                NONE,
                SHORT_HAIR_POLY,
                LONG_HAIR_POLY,
                FUR_SHELL,
                FUR_GEOMETRY,
                STRAND
            }


            public bool checkVersion()
            {
                return version == m_version;
            }


            public static int[] m_shaderIDs = null;            
            public float globalScale
            { get
                {
                    return (Mathf.Abs(transform.lossyScale.x) + Mathf.Abs(transform.lossyScale.y) + Mathf.Abs(transform.lossyScale.z)) / 3f;
                }
            }

            public float globalScaleParent
            {
                get
                {
                    return (Mathf.Abs(transform.parent.lossyScale.x) + Mathf.Abs(transform.parent.lossyScale.y) + Mathf.Abs(transform.parent.lossyScale.z)) / 3f;
                }
            }

            [SerializeField]
            public List<HairDesignerGenerator> m_generators = new List<HairDesignerGenerator>();
            public int m_generatorId = 0;
            public HairDesignerGenerator generator
            {
                get
                {
                    if (m_generatorId < 0 || m_generatorId >= m_generators.Count)
                        return null;
                    else
                        return m_generators[m_generatorId];
                }
            }


         

            /// <summary>
            /// Get layer
            /// </summary>
            /// <param name="layerId"></param>
            /// <returns></returns>
            public HairDesignerGenerator GetLayer( int layerId )
            {
                if (layerId < m_generators.Count && layerId >= 0 )
                    return m_generators[layerId];
                return null;
            }



            public HairDesignerGenerator GetLayer(string layerName )
            {
                for( int i=0; i< m_generators.Count; ++i )                  
                    if (m_generators[i].m_name == layerName)
                        return m_generators[i];
                return null;
            }

#region Random fuctions

            static Random.State oldSeed;
            public static void InitRandSeed(int seed)
            {
                oldSeed = Random.state;
                Random.InitState(seed);//create a custom seed
            }

            public static void RestoreRandSeed()
            {
                Random.state = oldSeed;//restore old seed other scripts
            }

#endregion




            public static void InitShaderPropertyToID()
            {
                int nbIds = 17;
                if (m_shaderIDs == null || m_shaderIDs.Length != nbIds)
                {
                    m_shaderIDs = new int[nbIds];
                    m_shaderIDs[0] = Shader.PropertyToID("KHD_startPosition");
                    m_shaderIDs[1] = Shader.PropertyToID("KHD_startTangent");
                    m_shaderIDs[2] = Shader.PropertyToID("KHD_endPosition");
                    m_shaderIDs[3] = Shader.PropertyToID("KHD_endTangent");
                    m_shaderIDs[4] = Shader.PropertyToID("KHD_taper");
                    m_shaderIDs[5] = Shader.PropertyToID("KHD_bendX");
                    m_shaderIDs[6] = Shader.PropertyToID("KHD_strandLength");
                    m_shaderIDs[7] = Shader.PropertyToID("KHD_rigidity");
                    m_shaderIDs[8] = Shader.PropertyToID("KHD_motionDirection");
                    m_shaderIDs[9] = Shader.PropertyToID("KHD_scale");
                    m_shaderIDs[10] = Shader.PropertyToID("KHD_gravity");
                    m_shaderIDs[11] = Shader.PropertyToID("KHD_gravityFactor");
                    m_shaderIDs[12] = Shader.PropertyToID("KHD_lossyScale");
                    m_shaderIDs[13] = Shader.PropertyToID("KHD_rotation");
                    m_shaderIDs[14] = Shader.PropertyToID("KHD_motionZonePos");
                    m_shaderIDs[15] = Shader.PropertyToID("KHD_motionZoneDir");
                    m_shaderIDs[16] = Shader.PropertyToID("KHD_maxStrandLength");
                }

            }


          

            void Start()
            {
                m_smr = GetComponent<SkinnedMeshRenderer>();
                m_mr = GetComponent<MeshRenderer>();
                m_mf = GetComponent<MeshFilter>();
#if HAIRDESIGNER_DLL_TIME_LIMIT
                if(m_startTime == System.DateTime.MinValue)
                    m_startTime = System.DateTime.Now;
#endif
            }
            


            public void Update()
            {

#if HAIRDESIGNER_DLL_TIME_LIMIT
                if ( Application.isPlaying && (System.DateTime.Now - m_startTime).Hours >= 1)
                    Destroy(this);
#endif
                if (m_smr == null && m_mr == null && m_mf == null)
                    Start();

                if(m_shaderIDs==null)
                    InitShaderPropertyToID();



                if (m_windZone != null && m_windZone.gameObject.activeSelf && Application.isPlaying )
                {
                    if (m_windZone.mode == WindZoneMode.Directional)
                    {
                        m_windZoneDir.x = m_windZone.transform.forward.x;
                        m_windZoneDir.y = m_windZone.transform.forward.y;
                        m_windZoneDir.z = m_windZone.transform.forward.z;
                        m_windZoneDir.w = 0;//for spherical integration
                    }
                    else
                    {
                        m_windZoneDir.x = m_windZone.transform.position.x;
                        m_windZoneDir.y = m_windZone.transform.position.y;
                        m_windZoneDir.z = m_windZone.transform.position.z;
                        m_windZoneDir.w = m_windZone.radius;//for spherical integration
                    }
                    m_windZoneParam.x = m_windZone.windMain;
                    m_windZoneParam.y = m_windZone.windTurbulence;
                    m_windZoneParam.z = m_windZone.windPulseMagnitude;
                    m_windZoneParam.w = m_windZone.windPulseFrequency;
                }
                else
                {
                    m_windZoneDir = Vector4.zero;
                    m_windZoneParam = Vector4.zero;
                }

                //m_nbHairStrand = Mathf.Clamp(m_nbHairStrand, 0, 10000);



                //if (generator!= null )
                for (int i = 0; i < m_generators.Count; ++i)
                {
                    //if (m_generators[i] != null && m_generators[i].m_enable )
                    if (m_generators[i] != null )
                    {
                        m_generators[i].UpdateInstance();
                    }
                }

            }




            public void LateUpdate()
            {
                /*
                if (!Application.isPlaying)
                    return;
                    */
                for (int i = 0; i < m_generators.Count; ++i)
                {
                    if (m_generators[i] != null)
                    {
                        m_generators[i].LateUpdateInstance();
                    }
                }
            }

                
                public void OnDestroy()
                {  

                    //Debug.Log("OnDestroy HairDesigner");

                    for (int i=0; i<m_generators.Count; ++i)
                    {
                        if (m_generators[i] == null)
                            continue;
                    
                    if (Application.isPlaying)
                    {
                        Destroy(m_generators[i]);
                    }
                    else
                    {
                        //m_generators[i].Destroy();
                        m_generators[i].hideFlags = HideFlags.DontSave;
                        m_generators[i].m_HairDesignerDeleted = true;
                        //DestroyImmediate(m_generators[i]);
                    }
                    m_generators[i].Destroy();
                }

                if (Application.isEditor && !Application.isPlaying)
                {
                    HairDesignerGenerator[] generators = GetComponents<HairDesignerGenerator>();
                    for (int i = 0; i < generators.Length; ++i)
                        generators[i].hideFlags = HideFlags.HideAndDontSave;

                    HairDesignerShader[] shaders = GetComponents<HairDesignerShader>();
                    for (int i = 0; i < shaders.Length; ++i)
                        shaders[i].hideFlags = HideFlags.HideAndDontSave;

                }

            }



            void OnEnable()
            {
                for (int i = 0; i < m_generators.Count; ++i)
                    m_generators[i].Enable();
            }

            void OnDisable()
            {
                for (int i = 0; i < m_generators.Count; ++i)
                    m_generators[i].Disable();
            }


        }


       


    }
    
}
