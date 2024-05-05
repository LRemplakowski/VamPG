#define HAIRDESIGNER_MOTIONSYSTEM_V3

using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [DefaultExecutionOrder(210)]
        [System.Serializable]
        //[ExecuteInEditMode]
        public class HairDesignerGeneratorLongHairBase : HairDesignerGenerator
        {


            #region sub-classes definition
            //---------------------------------------------------------------------
            [System.Serializable]
            public enum eMotionSystem
            {
                NONE,
                V1,
                V2,
#if HAIRDESIGNER_MOTIONSYSTEM_V3
                V3
#endif
            }

            [System.Serializable]
            public enum eMeshGenerator
            {
                HAIR_CARDS,
                MESH_COLLECTION
            }


            [System.Serializable]
            public class HairGroup
            {
                public List<StrandData> m_strands = new List<StrandData>();
                //public BZCurv m_curv = new BZCurv();
                public MBZCurv m_mCurv = null;
                public Transform m_parent;

                //public Vector3 m_parentOffset;
                //public Quaternion m_parentRotation;

                //public MBZCurv m_animationCurv = null;

                public AnimationCurve m_shape = new AnimationCurve();
                public bool m_edit = true;
                public List<Transform> m_bones = new List<Transform>();

                //realTime data
                public List<float> m_bonesDistances = new List<float>();//V3
                public List<Vector3> m_bonesPreviouspos = new List<Vector3>();//V3
                public List<Vector3> m_bonesLastpos = new List<Vector3>();
                public List<Vector3> m_bonesOriginalPos = new List<Vector3>();
                public List<Vector3> m_bonesUp = new List<Vector3>();
                public List<Quaternion> m_bonesLastLocalRot = new List<Quaternion>();
                public List<Quaternion> m_bonesOriginalRot = new List<Quaternion>();
                public List<Quaternion> m_bonesTmpRot = new List<Quaternion>();
                public List<HairPID_V3> m_bonesPid = new List<HairPID_V3>();
                public List<Vector3> m_bonesInertia = new List<Vector3>();
                public List<bool> m_boneIsColliding = new List<bool>();
                public HairPID_V3 m_pid = new HairPID_V3();


                //generator data

                public eMeshGenerator m_generationMode = eMeshGenerator.HAIR_CARDS;

                //HairCards data
                public int m_subdivisionX = 5;
                public int m_subdivisionY = 10;
                public int m_strandCount = 1;
                public float m_normalOffset = 0f;
                public float m_startOffset = 0f;//compatibility old version
                public Vector3 m_startOffsetV3 = Vector3.zero;
                public float m_endOffset = 0f;//compatibility old version
                public Vector3 m_endOffsetV3 = Vector3.zero;
                public int m_rndSeed = 0;
                public int m_UVX = 1;
                public float m_rndStrandLength = 1;
                public float m_normalSwitch = 1;
                public float m_scale = 1f;
                public float m_folding = 0f;
                public float m_waveAmplitude = 0f;
                public float m_wavePeriod = 0f;
                public float m_strandMaxAngle = 0f;
                public float m_bendAngleStart = 0f;
                public float m_bendAngleEnd = 0f;
                public float m_tangentStartOffset = 0f;
                public int m_startBoneId = 0;

                public Vector3 m_meshNormal;

                public Vector3 m_motion;
                public Vector3 m_initEndPos;
                public Vector3 m_initStartTan;
                public Vector3 m_initEndTan;
                public Vector3 m_lastEndPos;
                public bool m_dynamic = true;
                public bool m_enableCollision = true;


                public float m_rigidity = 0f;
                public float m_rootRigidity = 0f;
                public float m_gravityFactor = 1f;

                public int m_boneCount = 10;
                public float m_bonesStartOffset = 0f;

                public Vector3 m_upReference = Vector3.up;
                public HairDesignerStrandMeshCollectionBase m_strandMeshCollection = null;
                public int m_strandMeshId = 0;

                public bool m_usePhysics = false;
                public bool m_snapToSurface = true;
                public int m_layer = 0;
                public bool m_generated = true;
                public int m_modifierId = -1;
                public TriangleLock m_triLock = null;

                [System.NonSerialized]
                public Vector3[] m_tmpPositionPlotEditor = null;                
                [System.NonSerialized]
                public bool m_updatePositionPlotEditorPoints = false;

                [System.NonSerialized]
                public Vector3[] m_tmpNormalPlotEditor = null;

                public void Generate()
                {
                    m_strands.Clear();

                    for (int i = 0; i < 1; ++i)
                    {                        
                        StrandData sd = new StrandData();                        
                        sd.mCurve = m_mCurv;
                        m_strands.Add(sd);
                    }
                }

                public HairGroup Copy()
                {
                    HairGroup hg = new HairGroup();
                    //hg.m_bones
                    hg.m_mCurv = m_mCurv.Copy();
                    hg.m_parent = m_parent;
                    hg.m_shape = new AnimationCurve(m_shape.keys);
                    hg.m_scale = m_scale;
                    if (m_triLock != null)
                        hg.m_triLock = m_triLock.Clone();


                    hg.m_strandCount = m_strandCount;
                    hg.m_subdivisionX = m_subdivisionX;
                    hg.m_subdivisionY = m_subdivisionY;
                    hg.m_normalOffset = -m_normalOffset;
                    hg.m_startOffsetV3 = m_startOffsetV3;
                    hg.m_endOffsetV3 = m_endOffsetV3;
                    hg.m_rndSeed = m_rndSeed;
                    hg.m_rndStrandLength = m_rndStrandLength;
                    hg.m_normalSwitch = m_normalSwitch;
                    hg.m_UVX = m_UVX;
                    hg.m_folding = -m_folding;
                    hg.m_waveAmplitude = m_waveAmplitude;
                    hg.m_wavePeriod = -m_wavePeriod;                    

                    hg.m_strandMaxAngle = -m_strandMaxAngle;
                    hg.m_bendAngleStart = m_bendAngleStart;
                    hg.m_bendAngleEnd = m_bendAngleEnd;
                    hg.m_layer = m_layer;
                    hg.m_snapToSurface = m_snapToSurface;
                    hg.m_modifierId = m_modifierId;

                    hg.m_gravityFactor = m_gravityFactor;
                    hg.m_rootRigidity = m_rootRigidity;
                    hg.m_rigidity = m_rigidity;

                    hg.m_dynamic = m_dynamic;                    
                    hg.m_strandMeshCollection = m_strandMeshCollection;
                    hg.m_strandMeshId = m_strandMeshId;

                    hg.m_generationMode = m_generationMode;
                    hg.m_boneCount = m_boneCount;
                    hg.m_bonesStartOffset = m_bonesStartOffset;


                    return hg;
                }


                public void RemoveTmpStrandData()
                {
                    for (int i = 0; i < m_strands.Count; ++i)
                        if (m_strands[i].tmpData != null)
                            m_strands[i].tmpData.needRefresh = true;
                }

            }

            /// <summary>
            /// Bones parameters for motion
            /// </summary>
            [System.Serializable]
            public class MotionBoneData
            {
                public float gravity = .1f;
                public float rootRigidity = .9f;
                public float rigidity = 0f;
                public float elasticity = .1f;
                public float length = 1f;
                public float smooth = .1f;
                public float tipWeight = .1f;
                public float parentTransmission = 1f;
                public Vector2 motionFactor = Vector2.one;
                public Vector2 centrifugalFactor = Vector2.one;
                public bool accurateBoneRotation = true;
                public HairDesignerPID bonePID = new HairDesignerPID(5, 2, 0);
                public Vector2 windMain = Vector2.one;
                public Vector2 windTurbulance = Vector2.one;
                public AnimationCurve collisionFactor = AnimationCurve.Linear(0,1,1,1);
                public eMotionSystem motionSystem = eMotionSystem.V3;
                public int lod = 1;
                public bool freeze = false;
                public float stabilitySnap = .002f;
            }





            /// <summary>
            /// Mirror modifier for long hair
            /// </summary>
            [System.Serializable]
            public class MirrorModifier : GeneratorModifier
            {
                //public Vector3 pos = Vector3.zero;
                //public Vector3 dir = Vector3.right;

                public enum eMirrorAxis
                {
                    X,
                    Y,
                    Z
                }

                public eMirrorAxis m_axis = eMirrorAxis.X;
                public Vector3 m_offset = Vector3.zero;

                public void Update(HairDesignerGeneratorLongHairBase g)
                {
                    for (int i = 0; i < g.m_groups.Count; ++i)
                    {
                        if (g.m_groups[i].m_modifierId == id)
                        {
                            g.m_groups.RemoveAt(i--);//clean generated by this modifier
                            continue;
                        }
                    }



                    for (int i = 0; i < g.m_groups.Count; ++i)
                    {

                        if (g.m_groups[i].m_modifierId != -1)
                            continue;//don't care about other modifiers

                        if (!layers.Contains(g.m_groups[i].m_layer))
                            continue;//don't care about other layers

                        HairGroup mirrored = g.m_groups[i].Copy();

                        mirrored.m_modifierId = id;
                        //mirrored.m_bendAngleStart *= -1f;
                        //mirrored.m_bendAngleEnd *= -1f;
                        mirrored.m_startOffsetV3 *= -1f;
                        mirrored.m_endOffsetV3 *= -1f;
                        mirrored.m_normalOffset *= -1f;

                        
                        mirrored.m_mCurv.ConvertOffsetAndRotation(Vector3.zero, Quaternion.identity, false);

                        for (int ic = 0; ic < mirrored.m_mCurv.m_curves.Length; ++ic)
                        {
                            mirrored.m_mCurv.SetStartAngle(ic, mirrored.m_mCurv.GetStartAngle(ic) * -1f);
                            mirrored.m_mCurv.SetEndAngle(ic, mirrored.m_mCurv.GetEndAngle(ic) * -1f);
                        }

                        mirrored.m_triLock = null;
                        mirrored.RemoveTmpStrandData();
                        

                        mirrored.m_mCurv.Mirror((MBZCurv.eMirrorAxis)m_axis, m_offset);
                        g.m_groups.Add(mirrored);
                        
                        mirrored.Generate();

                    }
                    g.m_needMeshRebuild = true;
                }


                public void Apply(HairDesignerGeneratorLongHairBase g)
                {
                    Update(g);
                    for (int i = 0; i < g.m_groups.Count; ++i)
                    {
                        if (g.m_groups[i].m_modifierId == id)
                        {
                            g.m_groups[i].m_modifierId = -1;
                        }
                    }
                }

            }


            [System.Serializable]
            public class EditorData
            {
                public float HandlesUIPerspSize = 1f;
                public float HandlesUIOrthoSize = 1f;                
            }



            //---------------------------------------------------------------------
#endregion



#region MEMBERS

            public List<HairGroup> m_groups = new List<HairGroup>();
            public List<CapsuleCollider> m_capsuleColliders = new List<CapsuleCollider>();
            public MotionBoneData m_motionData = new MotionBoneData();

            //unity can't serialize polymorphism, so herethe original data for mirror modifiers
            public List<MirrorModifier> m_mirrorModifiers = new List<MirrorModifier>();
            public EditorData m_editorData = new EditorData();
            public List<Vector3[]> m_capsuleSphereCenters = new List<Vector3[]>();
            public bool m_GenerateBones = false;
            public float m_groupCreationLength = 1f;

            HairGroup m_currentHairGroup = null;
            public bool m_enableCollision = true;
            //public bool m_buildSelectionOnly = false;
            public bool m_lowPolyMode = false;
            public bool m_generateStrandMeshOnly = false;
            //public bool m_FastDraw = false;

            List<Transform> m_bones = new List<Transform>();



#endregion





            public override int GetStrandCount()
            {
                int c = 0;
                for (int i = 0; i < m_groups.Count; ++i)
                    c += m_groups[i].m_strands.Count;

                return c;
            }



            //---------------------------------------------------------------
#region RUNTIME

            public override void Start()
            {
                base.Start();
            }


            //public Vector3 m_lastPosition;
            public override void UpdateInstance()
            {
                if (this == null)//bug when destroy layer in editor
                    return;

                base.UpdateInstance();

                if (!Application.isPlaying)
                    return;


            }





            void UpdateCapsuleColliderData()
            {
                //update capsule position
                for (int c = 0; c < m_capsuleColliders.Count; ++c)
                {
                    if (m_capsuleColliders[c] == null)
                    {
                        m_capsuleColliders.RemoveAt(c--);
                        continue;
                    }

                    if (m_capsuleSphereCenters.Count == c)
                        m_capsuleSphereCenters.Add(new Vector3[2]);

                    for (int k = 0; k < 2; ++k)
                    {
                        m_capsuleSphereCenters[c][k] = m_capsuleColliders[c].transform.position;// + m_capsuleColliders[c].transform.TransformPoint(m_capsuleColliders[c].center);
                        float scl = m_capsuleColliders[c].transform.lossyScale.x;
                        float h = m_capsuleColliders[c].height * .5f > m_capsuleColliders[c].radius ?
                                    m_capsuleColliders[c].height * .5f - m_capsuleColliders[c].radius : 0;
                        h *= scl;

                        switch (m_capsuleColliders[c].direction)
                        {
                            case 0: m_capsuleSphereCenters[c][k] += m_capsuleColliders[c].transform.right * h * (k == 0 ? 1 : -1); break;
                            case 1: m_capsuleSphereCenters[c][k] += m_capsuleColliders[c].transform.up * h * (k == 0 ? 1 : -1); break;
                            case 2: m_capsuleSphereCenters[c][k] += m_capsuleColliders[c].transform.forward * h * (k == 0 ? 1 : -1); break;
                        }

                        //Debug.DrawLine(m_capsuleSphereCenters[c][k], m_capsuleSphereCenters[c][k] + Vector3.up * m_capsuleColliders[c].radius* scl, k==0? Color.red:Color.blue);
                    }
                }
            }


            public override void LateUpdateInstance()
            {
               
                if (!m_enable || !Application.isPlaying)
                    return;

                

                UpdateBlendshape();
                UpdateCapsuleColliderData();

                switch(m_motionData.motionSystem)
                {
                    case eMotionSystem.V1:
                        UpdateBonePositionV1();
                        break;

                    case eMotionSystem.V2:
                        UpdateBonePositionV2();
                        break;
#if HAIRDESIGNER_MOTIONSYSTEM_V3
                    case eMotionSystem.V3:                        
                        UpdateBonePositionV3();
                        break;
#endif
                }

            }

#if HAIRDESIGNER_MOTIONSYSTEM_V3

           
            public void FixedUpdate()
            {
                if (!m_enable)
                    return;

                switch (m_motionData.motionSystem)
                {

                    case eMotionSystem.V3:
                        FixedUpdateBonePositionV3();
                        break;

                }
            }
#endif


            void InitBOnePositionData(int gId, int boneid)
            {
                float distFromParent = 0f;
                if (boneid > 0)
                    distFromParent = Vector3.Distance(m_groups[gId].m_bones[boneid - 1].position, m_groups[gId].m_bones[boneid].position);


                //initialize tmp data
                m_groups[gId].m_bonesOriginalPos.Add(m_groups[gId].m_bones[boneid].localPosition);
                m_groups[gId].m_bonesOriginalRot.Add(m_groups[gId].m_bones[boneid].localRotation);
                //m_groups[gId].m_bonesTmpRot.Add(m_groups[gId].m_bones[boneid].localRotation);
                m_groups[gId].m_bonesTmpRot.Add(m_groups[gId].m_bones[boneid].rotation);
                m_groups[gId].m_bonesLastLocalRot.Add(m_groups[gId].m_bones[boneid].localRotation);
                m_groups[gId].m_bonesLastpos.Add(m_groups[gId].m_bones[boneid].position);
                m_groups[gId].m_bonesInertia.Add(Vector3.zero);
                m_groups[gId].m_boneIsColliding.Add(false);
                m_groups[gId].m_bonesPid.Add(new HairPID_V3());
                m_groups[gId].m_bonesPid[boneid].m_params = new HairDesignerPID.Parameters(m_motionData.bonePID.m_params);
                m_groups[gId].m_bonesPid[boneid].m_params.limits = new Vector2(-distFromParent, distFromParent) * .1f;
                m_groups[gId].m_bonesPid[boneid].m_target = Vector3.zero;
                m_groups[gId].m_bonesPid[boneid].Init();
                m_groups[gId].m_bonesLastpos[boneid] = m_groups[gId].m_bones[boneid].position;

            }




#if HAIRDESIGNER_MOTIONSYSTEM_V3

#region MOTION SYSTEM V3

            MotionSolver m_ms = null;
            public MotionSolver.SolverSettings m_motionSettings = new MotionSolver.SolverSettings();
            bool m_motionSystemV3initialized = false;

            void InitMotionSystemV3()
            {
                m_ms = new MotionSolver();
                m_ms.m_settings = m_motionSettings;
                m_ms.m_settings.reference = transform;
                m_ms.m_settings.startScale = transform.lossyScale.x;

                for ( int i=0; i<m_groups.Count; ++i )
                {
                    if (m_groups[i].m_dynamic && m_groups[i].m_generated)
                    {
                        //MotionSolver.Node previousNode = m_vs.GetNode(rootCount);                        
                        //rootCount++;
                        MotionSolver.Node previousNode = null;

                        for (int b=0; b< m_groups[i].m_bones.Count; ++b )
                        {
                            MotionSolver.Node n = new MotionSolver.Node();
                            n.isKinematic = b==0;
                            if(m_groups[i].m_bones[b]!=null)
                                n.SetTransform(m_groups[i].m_bones[b]);                            
                            m_ms.RegisterNode(n, previousNode);
                            n.chainFactor = (float)b / (float)(m_groups[i].m_bones.Count);
                            previousNode = n;
                        }
                    }
                }

                m_motionSystemV3initialized = true;
            }


            void UpdateBonePositionV3()
            {
                if (!IsVisible)
                {
                    //Debug.Log("not visible");
                    return;
                }                

                if (!m_motionSystemV3initialized)
                    InitMotionSystemV3();

                m_motionSettings.windZoneDir = m_hd.m_windZoneDir;
                m_motionSettings.windZoneParam = m_hd.m_windZoneParam;

                m_ms.Update(Time.deltaTime);
            }


            void FixedUpdateBonePositionV3()
            {
                if (!IsVisible)
                    return;

                if (m_motionSystemV3initialized)
                    m_ms.FixedUpdate(Time.fixedDeltaTime);
            }

#endregion

#endif


            int _currentLOD = 0;
            void UpdateBonePositionV2()
            {
                //float gFactor = m_motionData.gravity;
                float elasticity = m_motionData.elasticity;
                float time = Time.time;
                float deltaTime = Time.deltaTime;


                //--------------------------
                //DEBUG
                bool ENABLE_COLLISION = true;
                bool ENABLE_GRAVITY = true;
                bool ENABLE_WIND = true;
                bool ENABLE_INERTIA = true;
                bool ENABLE_WEIGHT = true;
                bool ENABLE_ELASTICITY = true;
                bool ENABLE_RIGIDITY = true;
                bool ENABLE_SMOOTH = true;

                if (m_motionData.lod > 1)
                {
                    _currentLOD++;
                    _currentLOD = _currentLOD % m_motionData.lod;
                    //Debug.Log("LOD ID " + _currentLOD);
                }

                for (int i = 0; i < m_groups.Count; ++i)
                //for (int i = m_groups.Count-1; i >=0 ; --i)
                {
                    if (!m_groups[i].m_dynamic || m_motionData.freeze )
                        continue;

                    

                    for (int b = 0; b < m_groups[i].m_bones.Count; ++b)
                    {
                        HairGroup hg = m_groups[i];
                        Transform bone = hg.m_bones[b];
                        Transform boneparent = null;
                        if (b > 0)
                            boneparent = hg.m_bones[b - 1];

                        

                        //initialize bone data
                        if (hg.m_bonesLastpos.Count == b)
                            InitBOnePositionData(i, b);

                        Vector3 oldLocalPosition = bone.localPosition;
                        Vector3 delta = hg.m_bonesLastpos[b] - bone.position;
                        float f = (float)b / (float)hg.m_bones.Count;
                        float omf = 1f - f;

                        


                        if ( m_motionData.lod > 1 && b > 0 && b % m_motionData.lod != _currentLOD)
                        {
                            //LOD
                            m_groups[i].m_bonesLastpos[b] = m_groups[i].m_bones[b].transform.position;
                            continue;                            
                        }
                        

                        hg.m_bonesPid[b].m_params.ki = m_motionData.bonePID.m_params.ki;
                        hg.m_bonesPid[b].m_params.kp = m_motionData.bonePID.m_params.kp;
                        hg.m_bonesPid[b].m_params.limits.x = -float.MaxValue;
                        hg.m_bonesPid[b].m_params.limits.y = float.MaxValue;

                        if (b == 0)
                        {
                            Vector3 axis1 = bone.rotation * Vector3.forward;
                            Vector3 axis2 = hg.m_bonesTmpRot[0] * Vector3.forward;
                            Vector3 axis3 = Vector3.Cross(axis1, axis2);
                            Vector3 dir = Vector3.Cross(axis1, axis3);
                            float max = 10000f;



                            hg.m_bonesPid[0].m_params.kp = 10f;
                            hg.m_bonesPid[0].m_params.ki = 1f;
                            hg.m_bonesPid[0].m_params.limits.x = -max;
                            hg.m_bonesPid[0].m_params.limits.y = max;
                            if (Vector3.Dot(axis1, axis2) < 1f)
                            {
                                //hg.m_bonesPid[b].m_target = Vector3.Lerp(dir * .1f * Vector3.Angle(axis1, axis2), hg.m_bonesPid[b].m_target, deltaTime);
                                hg.m_bonesPid[0].m_target = dir * .1f * Vector3.Angle(axis1, axis2);
                                hg.m_bonesPid[0].m_target = Vector3.ClampMagnitude(hg.m_bonesPid[0].m_target, max);
                            }
                            else
                            {
                                hg.m_bonesPid[0].m_target = Vector3.zero;
                            }

                            hg.m_bonesInertia[0] = hg.m_bonesPid[0].Compute(hg.m_bonesInertia[0]);


                            bone.localPosition = hg.m_bonesOriginalPos[0];
                            bone.localRotation = hg.m_bonesOriginalRot[0];
                            hg.m_bonesTmpRot[0] = bone.rotation;
                            hg.m_bonesLastpos[b] = bone.position;
                            continue;
                        }
                        
                        //Vector3 rootInertia = Vector3.Lerp(hg.m_bonesInertia[0] * .5f, hg.m_bonesInertia[0], (Vector3.Dot(hg.m_bonesInertia[0].normalized, bone.forward)));
                        Vector3 rootCentrifigal = hg.m_bonesInertia[0] * m_hd.globalScale * .1f;

                        

                       
                        ///-------------
                        ///Min Max motion amplitude no deltatime (already in PID)
                        hg.m_bonesPid[b].m_target = (delta ) * Mathf.Lerp(m_motionData.motionFactor.x, m_motionData.motionFactor.y, f) + rootCentrifigal* Mathf.Lerp(m_motionData.centrifugalFactor.x, m_motionData.centrifugalFactor.y, f);
                        hg.m_bonesInertia[b] = hg.m_bonesPid[b].Compute(hg.m_bonesInertia[b]);
                        Vector3 inertia = (hg.m_bonesInertia[b] + hg.m_bonesInertia[b - 1] * m_motionData.parentTransmission * omf ) * f;
                        inertia *= hg.m_boneIsColliding[b] ? .1f : 1f;
                        ///-------------
                                               

                        //gravity
                        Vector3 gravity = Physics.gravity * m_motionData.gravity * f * .1f;
                        


                         //weight
                         Vector3 hairWeight = Vector3.zero;
                        if (b < hg.m_bones.Count - 1)
                            hairWeight = (hg.m_bones[b + 1].position - bone.position).normalized * omf * m_motionData.tipWeight *.1f;
                        

                        //wind
                        Vector3 wind = Vector3.zero;
                        if (m_hd.m_windZone != null && m_hd.m_windZone.gameObject.activeSelf)
                        {
                            //wind force (same as shader)
                            wind += m_hd.m_windZone.transform.forward * m_hd.m_windZone.windMain * Mathf.Lerp(m_motionData.windMain.x, m_motionData.windMain.y, f);
                            wind += m_hd.m_windZone.transform.forward *
                                (
                                    Mathf.Sin((m_hd.m_windZone.windPulseFrequency + (float)hg.m_rndSeed % 100f * .001f) * time * 10f) * 2f
                                  + Mathf.Cos((m_hd.m_windZone.windPulseFrequency + (float)hg.m_rndSeed % 100f * .001f) * time * 20f) * .1f
                                )
                                * m_hd.m_windZone.windPulseMagnitude * m_hd.m_windZone.windMain * .1f
                                * Mathf.Lerp(m_motionData.windTurbulance.x, m_motionData.windTurbulance.y, f)
                                ;
                        }


                        if (!ENABLE_WEIGHT) hairWeight = Vector3.zero;
                        if (!ENABLE_WIND) wind = Vector3.zero;
                        if (!ENABLE_GRAVITY) gravity = Vector3.zero;
                        if (!ENABLE_INERTIA) inertia = Vector3.zero;

                        //apply forces
                        Vector3 externalForces = (inertia + gravity*.1f * m_hd.globalScale + wind*.001f * m_hd.globalScale + hairWeight);

                        //externalForces = Vector3.zero;


                        bone.position += externalForces;// * m_hd.globalScale;



                        if (ENABLE_ELASTICITY)
                        {
                            //elasticity effect
                            if (bone.localPosition.magnitude <= hg.m_bonesOriginalPos[b].magnitude * (1 - elasticity) * m_motionData.length)
                                bone.localPosition = bone.localPosition.normalized * hg.m_bonesOriginalPos[b].magnitude * (1 - elasticity) * m_motionData.length;// * hairLengthFactor;
                            if (bone.localPosition.magnitude > hg.m_bonesOriginalPos[b].magnitude * (1 + elasticity) * m_motionData.length)
                                bone.localPosition = bone.localPosition.normalized * hg.m_bonesOriginalPos[b].magnitude * (1 + elasticity) * m_motionData.length;// * hairLengthFactor;

                            bone.localPosition = Vector3.Lerp(bone.localPosition, bone.localPosition.normalized * hg.m_bonesOriginalPos[b].magnitude * m_motionData.length, .5f);
                        }



                        if (ENABLE_RIGIDITY)
                        {
                            //smooth root value to keep haircut shape
                            float rootMotion = (1 - m_motionData.rootRigidity) * (1f - hg.m_rootRigidity);
                            rootMotion = Mathf.Lerp(f, rootMotion, rootMotion);

                            float motionFactor = (1f - m_motionData.rigidity) * (1 - hg.m_rigidity);
                            float factor = (f * (1f - rootMotion) + rootMotion) * motionFactor;

                            bone.localPosition = Vector3.Lerp(hg.m_bonesOriginalPos[b], bone.localPosition, factor);
                            bone.localRotation = Quaternion.Lerp(hg.m_bonesOriginalRot[b], bone.localRotation, factor);

                        }


                        if (ENABLE_SMOOTH)
                        {
                            //smooth

                            bone.localPosition = Vector3.Lerp(bone.localPosition, m_groups[i].m_bones[b-1].InverseTransformPoint(hg.m_bonesLastpos[b]), m_motionData.smooth *.1f );
                        }


                        //COLLISIONS
                        Vector3 newWorldPos = m_groups[i].m_bones[b].position;



                        if (ENABLE_COLLISION)
                        {
                            bool collisionDetected = false;

                            Vector3 normalCollision = Vector3.zero;
                            if (m_enableCollision && m_groups[i].m_enableCollision)
                            {
                                if (CheckCapsuleCollision(hg, b, ref newWorldPos, ref normalCollision))
                                {
                                    m_groups[i].m_bones[b].position = Vector3.Lerp(m_groups[i].m_bones[b].position, newWorldPos, m_motionData.collisionFactor.Evaluate(f) * Mathf.Lerp(1f - m_motionData.rootRigidity, 1f, f));

                                    //set bone up orientation
                                    float sign = Mathf.Sign(Vector3.Dot(normalCollision, bone.transform.up));
                                    Vector3 lkat = bone.transform.forward + m_groups[i].m_bones[b].transform.position;
                                    lkat = m_groups[i].m_bones[b].transform.position;
                                    m_groups[i].m_bones[b].transform.LookAt(lkat, Vector3.Lerp(m_groups[i].m_bones[b].transform.up, normalCollision * sign, deltaTime * 10f));
                                    //m_groups[i].m_bones[b].transform.LookAt(lkat, normalCollision * sign);
                                    collisionDetected = true;

                                    //Debug.DrawLine(newWorldPos, newWorldPos + normalCollision, Color.red);

                                }
                            }
                            hg.m_boneIsColliding[b] = collisionDetected;

                        }
                        

                        
                        
                        if (b == hg.m_bones.Count - 1)
                        {
                            //fix last bone rotation
                            bone.LookAt(bone.position + boneparent.forward, boneparent.up);
                        }
                        /*
                        else if (ENABLE_FIXUP)
                        {
                            //fix up issue
                            bone.LookAt(bone.position + bone.forward, boneparent.up);
                        }
                        

                        if (ENABLE_FIXPARENT)
                        {
                            //Set bone parent
                            if (boneparent != null)
                            {
                                //fix bone parent rotation
                                Quaternion wrldR = bone.rotation;
                                boneparent.LookAt(bone.position, boneparent.up);
                                bone.localPosition = Vector3.forward * bone.localPosition.magnitude;
                                bone.rotation = wrldR;
                            }
                        }
                        */
                        hg.m_bonesLastpos[b] = bone.position;

                    }
                }
            }






            void UpdateBonePositionV1()
            {

                float gFactor = m_motionData.gravity;
                float elasticity = m_motionData.elasticity;
                bool fixBoneRotation = m_motionData.accurateBoneRotation;

                //Update bones motion
                for (int i = 0; i < m_groups.Count; ++i)
                {
                    if (m_groups[i].m_dynamic && m_groups[i].m_generated)
                    {
                        //Vector3[] initPos = new Vector3[m_groups[i].m_bones.Count];


                        //fix bone rotation
                        if (fixBoneRotation && m_groups[i].m_bonesOriginalPos.Count > 0)
                        {
                            for (int b = 1; b < m_groups[i].m_bones.Count; ++b)
                            //for (int b = m_groups[i].m_bones.Count-5; b < m_groups[i].m_bones.Count; ++b)
                            {
                                m_groups[i].m_bones[b].parent.localRotation = m_groups[i].m_bonesTmpRot[b - 1];
                                m_groups[i].m_bones[b].position = m_groups[i].m_bonesLastpos[b];
                            }
                        }

                        for (int b = 0; b < m_groups[i].m_bones.Count; ++b)
                        {
                            float f = (float)b / (float)m_groups[i].m_bones.Count;
                            float distFromParent = 0f;



                            if (b > 0)
                            {
                                distFromParent = Vector3.Distance(m_groups[i].m_bones[b - 1].position, m_groups[i].m_bones[b].position);
                            }

                            if (m_groups[i].m_bonesLastpos.Count == b)
                            {
                                InitBOnePositionData(i,b);
                                /*
                                //initialize tmp data
                                m_groups[i].m_bonesOriginalPos.Add(m_groups[i].m_bones[b].localPosition);
                                m_groups[i].m_bonesOriginalRot.Add(m_groups[i].m_bones[b].localRotation);
                                m_groups[i].m_bonesTmpRot.Add(m_groups[i].m_bones[b].localRotation);
                                m_groups[i].m_bonesLastLocalRot.Add(m_groups[i].m_bones[b].localRotation);

                                m_groups[i].m_bonesLastpos.Add(m_groups[i].m_bones[b].position);
                                m_groups[i].m_bonesInertia.Add(Vector3.zero);
                                m_groups[i].m_bonesPid.Add(new HairPID_V3());
                                m_groups[i].m_bonesPid[b].m_params = m_motionData.bonePID.m_params;
                                m_groups[i].m_bonesPid[b].m_params.limits = new Vector2(-distFromParent, distFromParent) * .5f;
                                m_groups[i].m_bonesPid[b].m_target = Vector3.zero;
                                m_groups[i].m_bonesPid[b].Init();
                                m_groups[i].m_bonesLastpos[b] = m_groups[i].m_bones[b].position;
                                */
                            }
                                                                                  

                            if (b == 0)
                            {
                                m_groups[i].m_bonesLastpos[b] = m_groups[i].m_bones[b].position;
                                //Debug.DrawLine(m_groups[i].m_bones[b].position, m_groups[i].m_bones[b].position + m_groups[i].m_bones[b].forward, Color.red);
                                continue;
                            }

                            m_groups[i].m_bones[b].transform.localRotation = m_groups[i].m_bonesLastLocalRot[b];

                            Quaternion oldRotation = m_groups[i].m_bones[b].transform.localRotation;

                            Vector3 delta = m_groups[i].m_bonesLastpos[b] - m_groups[i].m_bones[b].position;

                            //centrifugal force
                            //delta += Vector3.Normalize(m_groups[i].m_bones[b].position - m_groups[i].m_bones[b].parent.position) * delta.magnitude * 2f;


                            //clamp the distance with the max distance from root
                            delta = delta.normalized * Mathf.Clamp(delta.magnitude, 0, f * distFromParent * (float)m_groups[i].m_bones.Count);

                            m_groups[i].m_bonesPid[b].m_target = delta;// + m_groups[i].m_bonesInertia[b-1];
                            m_groups[i].m_bonesInertia[b] = m_groups[i].m_bonesPid[b].Compute(Vector3.zero);

                            Vector3 newPos = m_groups[i].m_bones[b - 1].TransformPoint(m_groups[i].m_bonesOriginalPos[b]) + m_groups[i].m_bonesInertia[b];


                            Vector3 externForces = Physics.gravity * gFactor * m_groups[i].m_gravityFactor;

                            if (m_hd.m_windZone != null && m_hd.m_windZone.gameObject.activeSelf)
                            {

                                //wind force (same as shader)
                                Vector3 wind = m_hd.m_windZone.transform.forward * m_hd.m_windZone.windMain * m_hd.globalScale;
                                wind += m_hd.m_windZone.transform.forward *
                                    (
                                        Mathf.Sin((m_hd.m_windZone.windPulseFrequency + (float)m_groups[i].m_rndSeed % 100f * .001f) * Time.time * 10f) * 2f
                                      + Mathf.Cos((m_hd.m_windZone.windPulseFrequency + (float)m_groups[i].m_rndSeed % 100f * .001f) * Time.time * 20f) * .1f
                                    )
                                    * m_hd.m_windZone.windPulseMagnitude * m_hd.m_windZone.windMain * .1f * m_hd.globalScale;

                                externForces += wind * Mathf.Lerp(m_motionData.windMain.x, m_motionData.windMain.y, f);
                            }

                            //Compute gravity with rotation
                            //m_groups[i].m_bones[b].transform.localRotation = m_groups[i].m_bonesOriginalRot[b];
                            if (delta.magnitude < externForces.magnitude)
                            {
                                //float fg = gFactor;// ( 1f - delta.magnitude / externForces.magnitude * gFactor * m_groups[i].m_gravityFactor);
                                Quaternion r = m_groups[i].m_bones[b].transform.localRotation;

                                //Vector3 up = Vector3.Lerp( m_groups[i].m_bones[b - 1].transform.up, -externForces.normalized, fg );
                                Vector3 up = m_groups[i].m_bones[b - 1].transform.up;


                                m_groups[i].m_bones[b].transform.LookAt(m_groups[i].m_bones[b].transform.position + (externForces).normalized, up);// m_groups[i].m_bones[b-1].transform.up );
                                r = Quaternion.Slerp(r, m_groups[i].m_bones[b].transform.localRotation, gFactor * (1f - f));
                                m_groups[i].m_bones[b].transform.localRotation = r;




                                //m_groups[i].m_bones[b].transform.rotation = Quaternion.Lerp( r, m_groups[i].m_bones[b].transform.rotation, Time.deltaTime);
                            }




                            //float dist = m_groups[i].m_bones[b].localPosition.magnitude;

                            //fix : capsule collider position      
                            if(m_enableCollision)
                            for (int c = m_capsuleColliders.Count - 1; c >= 0; --c) // first colliders are more important, last of the list are interraction colliders
                            {
                                float scl = m_capsuleColliders[c].transform.lossyScale.x;

                                if (!m_capsuleColliders[c].enabled)
                                    continue;


                                //test capsule cylinder
                                Vector3 dir = (m_capsuleSphereCenters[c][1] - m_capsuleSphereCenters[c][0]).normalized;
                                float dot = Vector3.Dot(dir, newPos - m_capsuleSphereCenters[c][0]);
                                Vector3 proj = m_capsuleSphereCenters[c][0] + dir * dot;

                                //bool collided = false;

                                //check cylinder radius
                                if (Vector3.Distance(proj, newPos) < m_capsuleColliders[c].radius * scl)
                                {
                                    //check cylinder length
                                    if (dot > 0 && dot < (m_capsuleColliders[c].height - m_capsuleColliders[c].radius * 2f) * scl)
                                    {
                                        newPos = (newPos - proj).normalized * m_capsuleColliders[c].radius * scl + proj;
                                        //collided = true;
                                    }
                                }


                                //test each sphere
                                for (int k = 0; k < 2; ++k)
                                {
                                    if (Vector3.Distance(m_capsuleSphereCenters[c][k], newPos) < m_capsuleColliders[c].radius * scl)
                                    {
                                        newPos = (newPos - m_capsuleSphereCenters[c][k]).normalized * m_capsuleColliders[c].radius * scl + m_capsuleSphereCenters[c][k];
                                        //collided = true;
                                    }
                                }
                            }


                            //clamp max delta : avoid instant position change
                            delta = newPos - m_groups[i].m_bones[b].position;
                            //delta = delta.normalized * Mathf.Clamp(delta.magnitude, 0f, Time.deltaTime * .1f);

                            float dirDot = Vector3.Dot(m_groups[i].m_bones[b].forward, delta.normalized);
                            delta *= dirDot < 0 ? 1f + dirDot * .5f : 1f;

                            //apply motion
                            m_groups[i].m_bones[b].position += delta;




                            //spring effect
                            if (m_groups[i].m_bones[b].localPosition.magnitude <= m_groups[i].m_bonesOriginalPos[b].magnitude * (1 - elasticity))
                                m_groups[i].m_bones[b].localPosition = m_groups[i].m_bones[b].localPosition.normalized * m_groups[i].m_bonesOriginalPos[b].magnitude * (1 - elasticity);// * hairLengthFactor;
                            if (m_groups[i].m_bones[b].localPosition.magnitude > m_groups[i].m_bonesOriginalPos[b].magnitude * (1 + elasticity))
                                m_groups[i].m_bones[b].localPosition = m_groups[i].m_bones[b].localPosition.normalized * m_groups[i].m_bonesOriginalPos[b].magnitude * (1 + elasticity);// * hairLengthFactor;


                            //smooth root value to keep haircut shape
                            float rootMotion = (1 - m_motionData.rootRigidity) * (1f - m_groups[i].m_rootRigidity * m_groups[i].m_rootRigidity);
                            float motionFactor = (1f - m_motionData.rigidity) * (1 - m_groups[i].m_rigidity);
                            m_groups[i].m_bones[b].localPosition = Vector3.Lerp(m_groups[i].m_bonesOriginalPos[b], m_groups[i].m_bones[b].localPosition, (f * (1f - rootMotion) + rootMotion) * motionFactor);
                            m_groups[i].m_bones[b].localRotation = Quaternion.Lerp(m_groups[i].m_bonesOriginalRot[b], m_groups[i].m_bones[b].localRotation, (f * (1f - rootMotion) + rootMotion) * motionFactor);
                            m_groups[i].m_bones[b].parent.localRotation = Quaternion.Lerp(m_groups[i].m_bonesOriginalRot[b - 1], m_groups[i].m_bones[b].parent.localRotation, (f * (1f - rootMotion) + rootMotion) * motionFactor);


                            //max angle rotation : smooth angle rotation
                            float a = Quaternion.Angle(m_groups[i].m_bones[b].transform.localRotation, oldRotation);
                            float maxAnglePerSec = 20f;
                            if (a > maxAnglePerSec * Time.deltaTime)
                            {
                                Quaternion r = Quaternion.Slerp(oldRotation, m_groups[i].m_bones[b].transform.localRotation, maxAnglePerSec / (a / Time.deltaTime));
                                m_groups[i].m_bones[b].transform.localRotation = r;
                            }

                            m_groups[i].m_bonesLastLocalRot[b] = m_groups[i].m_bones[b].transform.localRotation;

                            //Global smooth
                            m_groups[i].m_bones[b].localPosition = Vector3.Lerp(m_groups[i].m_bones[b].localPosition, m_groups[i].m_bones[b - 1].InverseTransformPoint(m_groups[i].m_bonesLastpos[b]), m_motionData.smooth * rootMotion);

                            m_groups[i].m_bonesLastpos[b] = m_groups[i].m_bones[b].position;
                        }

                        if (fixBoneRotation)
                        {
                            //fix bone rotation                        
                            for (int b = 1; b < m_groups[i].m_bones.Count; ++b)
                            {
                                m_groups[i].m_bonesTmpRot[b - 1] = m_groups[i].m_bones[b].parent.localRotation;
                                m_groups[i].m_bones[b].parent.LookAt(m_groups[i].m_bonesLastpos[b], m_groups[i].m_bones[b].parent.up);
                                m_groups[i].m_bones[b].position = m_groups[i].m_bonesLastpos[b];
                            }
                        }
                    }
                }
            }




            private bool CheckCapsuleCollision(HairGroup hg, int b, ref Vector3 position, ref Vector3 normalCollision)
            {
                if (!m_enableCollision)
                    return false;

                bool collisionDetected = false;



                for (int c = m_capsuleColliders.Count - 1; c >= 0; --c) // first colliders are more important, last of the list are interraction colliders
                {
                    float scl = m_capsuleColliders[c].transform.lossyScale.x;

                    if (!m_capsuleColliders[c].enabled)
                        continue;

                    //projection on capsule cylinder
                    Vector3 dir = (m_capsuleSphereCenters[c][1] - m_capsuleSphereCenters[c][0]).normalized;
                    float dot = Vector3.Dot(dir, position - m_capsuleSphereCenters[c][0]);
                    Vector3 proj = m_capsuleSphereCenters[c][0] + dir * dot;
                    float oldDist = (position - hg.m_bones[b - 1].position).magnitude;

                    //check cylinder radius
                    if (Vector3.Distance(proj, position) < m_capsuleColliders[c].radius * scl)
                    {
                        //check cylinder length
                        if (dot > 0 && dot < (m_capsuleColliders[c].height - m_capsuleColliders[c].radius) * scl)
                        {
                            //Debug.DrawLine(proj, (newPos - proj).normalized * m_capsuleColliders[c].radius * scl + proj, Color.magenta);
                            //Debug.DrawLine(proj, newPos, Color.cyan);


                            //push out of cylinder
                            position = (position - proj).normalized * m_capsuleColliders[c].radius * scl + proj;
                            //keep length from parent
                            position = (position - hg.m_bones[b - 1].position).normalized * oldDist + hg.m_bones[b - 1].position;
                            normalCollision = (position - proj).normalized;


                            collisionDetected = true;
                        }
                    }


                    //test each sphere
                    for (int k = 0; k < 2; ++k)
                    {
                        if (Vector3.Distance(m_capsuleSphereCenters[c][k], position) < m_capsuleColliders[c].radius * scl)
                        {
                            //Debug.DrawLine(m_capsuleSphereCenters[c][k], (newPos - m_capsuleSphereCenters[c][k]).normalized * m_capsuleColliders[c].radius * scl + m_capsuleSphereCenters[c][k], Color.magenta);
                            //Debug.DrawLine(m_capsuleSphereCenters[c][k], newPos, Color.blue);

                            //push out of sphere
                            position = (position - m_capsuleSphereCenters[c][k]).normalized * m_capsuleColliders[c].radius * scl + m_capsuleSphereCenters[c][k];
                            //keep length from parent
                            position = (position - hg.m_bones[b - 1].position).normalized * oldDist + hg.m_bones[b - 1].position;
                            normalCollision = (position - m_capsuleSphereCenters[c][k]).normalized;

                            collisionDetected = true;
                        }
                    }
                }

                return collisionDetected;

            }


            public void SmoothParentBoneOrientation(HairGroup g, int boneId)
            {
                if (boneId < g.m_bones.Count)
                {
                    for (int i = boneId; i > 2; --i)
                    {
                        Vector3 dirParent = (g.m_bones[i].position - g.m_bones[i - 1].position).normalized;
                        if (Vector3.Dot(dirParent, g.m_bones[i - 1].forward) < .9)
                        {
                            Vector3 tmp = g.m_bones[i].position;
                            //g.m_bones[i - 1].LookAt(g.m_bones[i].position, g.m_bones[i-1].up);
                            g.m_bones[i].position = tmp;
                        }
                    }
                }
            }



            public void UpdateBlendshape()
            {
                return;
                /*
                for (int i = 0; i < m_groups.Count; ++i)
                {
                    if (m_groups[i].m_bones.Count == 0)
                        continue;
                    Transform root = m_groups[i].m_bones[0];
                    if (root == null || m_groups[i].m_triLock == null || m_groups[i].m_triLock.m_faceId == -1)
                        continue;

                    //TODO : update root bone position and orientation according to the blendshape



                }*/
            }


            /// <summary>
            /// Destroy
            /// </summary>
            public override void Destroy()
            {
                DestroyMesh();
                if (m_ms != null)
                    m_ms.Destroy();
            }

#endregion



#region MESH_GENERATION
/*
            public override StrandRenderingData GetData(int id)
            {
                int c = 0;
                int sId = 0;
                int gId = 0;
                for (int i = 0; i < m_groups.Count; ++i)
                {
                    if (id < m_groups[i].m_strands.Count + c)
                    {
                        gId = i;
                        sId = id - c;
                        break;
                    }

                    c += m_groups[i].m_strands.Count;
                }


                m_data.rotation = m_groups[gId].m_strands[sId].rotation;
                m_data.localpos = m_groups[gId].m_strands[sId].localpos;// + m_strands[id].normal * .001f;
                m_data.scale = m_groups[gId].m_strands[sId].scale * m_scale;
                m_data.normal = m_groups[gId].m_strands[sId].normal;
                m_data.strand = m_groups[gId].m_strands[sId];
                m_data.layer = m_groups[gId].m_layer;

                if (m_groups[gId].m_strands[sId].mesh == null)
                {
                    m_currentHairGroup = m_groups[gId];

                    if (m_currentHairGroup.m_generationMode == eMeshGenerator.HAIR_CARDS)
                        m_groups[gId].m_strands[sId].mesh = GenerateMeshHairCards(m_groups[gId].m_strands[sId], null, 0, transform);             


                    if(m_currentHairGroup.m_generationMode == eMeshGenerator.MESH_COLLECTION)
                        m_groups[gId].m_strands[sId].mesh = GenerateMeshFromCollection(m_groups[gId].m_strandMeshCollection, m_groups[gId].m_strandMeshId, m_groups[gId].m_strands[sId], null, 0, transform);

                    
 //                   if (m_groups[gId].m_strandMeshCollection == null)
 //                       m_groups[gId].m_strands[sId].mesh = GenerateMesh(m_groups[gId].m_strands[sId], null, 0, transform);
 //                   else
 //                       m_groups[gId].m_strands[sId].mesh = GenerateMeshFromCollection(m_groups[gId].m_strandMeshCollection, m_groups[gId].m_strandMeshId, m_groups[gId].m_strands[sId], null, 0, transform);
                        

                }
                


                return m_data;
            }

*/


            List<Vector3> _v = new List<Vector3>();
            List<Vector3> _n = new List<Vector3>();
            List<Vector4> _tg = new List<Vector4>();
            List<int> _t = new List<int>();
            List<Vector2> _uv = new List<Vector2>();
            List<BoneWeight> _boneWeights = new List<BoneWeight>();
            List<Color> _colors = new List<Color>();



            public Vector3 MulEachAxis(Vector3 v1, Vector3 v2)
            {
                return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
            }


            /// <summary>
            /// Generate mesh from strand
            /// </summary>
            /// <param name="sd"></param>
            /// <param name="bones"></param>
            /// <param name="startId"></param>
            /// <returns></returns>
            Mesh GenerateMeshHairCards(StrandData sd, List<Transform> bones, int startBoneId, Transform t)
            {

                //Debug.Log("GenerateMeshHairCards");

                //sd.

                //Ray r = new Ray();
                //RaycastHit hitInfo;
                Mesh m = new Mesh();
                _v.Clear();
                _n.Clear();
                _tg.Clear();
                _t.Clear();
                _uv.Clear();
                _boneWeights.Clear();
                _colors.Clear();

                Transform trans = t != null ? t : transform;

                sd.localpos = Vector3.zero;
                sd.mCurve.SetUpRef(trans.InverseTransformDirection(Vector3.up));

                int X = m_currentHairGroup.m_subdivisionX;
                int Y = m_currentHairGroup.m_subdivisionY;

                
                //compatibility with old version
                if (m_currentHairGroup.m_startOffset != 0 )
                {
                    if(!float.IsNaN(m_currentHairGroup.m_startOffset))
                        m_currentHairGroup.m_startOffsetV3 = m_currentHairGroup.m_startOffset * Vector3.one;
                    m_currentHairGroup.m_startOffset = 0;
                }
                if (m_currentHairGroup.m_endOffset != 0)
                {
                    if (!float.IsNaN(m_currentHairGroup.m_endOffset))
                        m_currentHairGroup.m_endOffsetV3 = m_currentHairGroup.m_endOffset * Vector3.one;
                    m_currentHairGroup.m_endOffset = 0;
                }
                

                /*
                m_buildSelectionOnly = false;
                if (m_buildSelectionOnly)
                {
                    X = Mathf.Min(X, 3);
                    Y = Mathf.Min(Y, 5* m_currentHairGroup.m_mCurv.m_curves.Length);
                }*/

                if (m_lowPolyMode)
                {
                    X = Mathf.Min(3, X);
                    Y = Mathf.Max(10, 3 * m_currentHairGroup.m_mCurv.m_curves.Length);
                }

                Random.InitState(m_currentHairGroup.m_rndSeed);
                int K = m_currentHairGroup.m_strandCount;
                /*
                if (m_buildSelectionOnly)
                    K = Mathf.Min(K, 3);
                */
                if (m_lowPolyMode)
                {
                    K = 1;
                }


                for (int k = 0; k < K; ++k)
                {

                    Vector3 startOffset = MulEachAxis(m_currentHairGroup.m_startOffsetV3, Random.insideUnitSphere);
                    Vector3 endOffset = MulEachAxis(m_currentHairGroup.m_endOffsetV3, Random.insideUnitSphere);


                    float angle = ((float)k / (float)K) * m_currentHairGroup.m_strandMaxAngle;
                    float rnd = Random.value;
                    //Vector3 startOffsetRnd =  Random.insideUnitCircle;
                    /*
                    Vector3 startOffsetRnd = Quaternion.AngleAxis(rnd*360f,sd.mCurve.GetTangent(0))* sd.mCurve.GetUp(0);
                    Vector3 startOffset = startOffsetRnd * m_currentHairGroup.m_startOffset;
                    Vector3 endOffset = startOffsetRnd * m_currentHairGroup.m_endOffset;
                    */

                    float rndLength = k==0 ? 1f : ((1 - m_currentHairGroup.m_rndStrandLength) * rnd + m_currentHairGroup.m_rndStrandLength);

                    float nOffset0 = m_currentHairGroup.m_normalOffset * Random.value;
                    float nOffset1 = m_currentHairGroup.m_normalOffset;// * Random.value;
                    nOffset1 = nOffset0;

                    for (int x = 0; x < X; ++x)
                    {
                        for (int y = 0; y < Y; ++y)
                        {
                            //float progress = (float)y / (float)Y;
                            //float progressX = (float)x / (float)X;

                            float XStep = 1f / (float)X;
                            float YStep = 1f / (float)Y;
                            //float f = YStep * y;

                            float t0 = (YStep * y) * rndLength;
                            float t1 = (YStep * y + YStep) * rndLength;

                            //Vector3 dir = Vector3.forward;
                            Vector3 tan0 = sd.mCurve.GetTangent(t0);
                            Vector3 tan1 = sd.mCurve.GetTangent(t1);
                            Vector3 up0 = sd.mCurve.GetUp(t0);
                            Vector3 up1 = sd.mCurve.GetUp(t1);
                            Vector3 right0 = Vector3.Cross(tan0, up0);
                            Vector3 right1 = Vector3.Cross(tan1, up1);
                            float nOffset = Mathf.Lerp(nOffset0, nOffset1, t0);
                            

                            if (m_currentHairGroup.m_shape.length < 2)
                            {
                                m_currentHairGroup.m_shape.AddKey(new Keyframe(0, 1f));
                                m_currentHairGroup.m_shape.AddKey(new Keyframe(1, 1f));
                            }


                            float taper0 = m_currentHairGroup.m_shape.Evaluate((float)y / (float)Y) * m_scale * strandScaleFactor * m_currentHairGroup.m_scale;
                            float taper1 = m_currentHairGroup.m_shape.Evaluate(((float)y + 1f) / (float)Y) * m_scale * strandScaleFactor * m_currentHairGroup.m_scale;

                            //Vector3 offset0 = Mathf.Lerp(m_currentHairGroup.m_startOffset, m_currentHairGroup.m_endOffset, t0) * (Quaternion.AngleAxis(rnd * 360f, tan0) * up0).normalized;
                            //Vector3 offset1 = Mathf.Lerp(m_currentHairGroup.m_startOffset, m_currentHairGroup.m_endOffset, t1) * (Quaternion.AngleAxis(rnd * 360f, tan1) * up1).normalized;

                            Vector3 statEnd0 = Vector3.Lerp(startOffset, endOffset, t0);
                            Vector3 statEnd1 = Vector3.Lerp(startOffset, endOffset, t1);

                            Vector3 offset0 = tan0 * statEnd0.z + up0 * statEnd0.y + right0 * statEnd0.x;
                            Vector3 offset1 = tan1 * statEnd1.z + up1 * statEnd1.y + right1 * statEnd1.x;

                            if (m_lowPolyMode)
                            {
                                offset0 = Vector3.zero;
                                offset1 = Vector3.zero;
                            }

                            //Vector3 pos0 = sd.mCurve.GetPosition(t0) + Vector3.Lerp(startOffset, endOffset, (float)y / (float)Y) * taper0;
                            //Vector3 pos1 = sd.mCurve.GetPosition(t1) + Vector3.Lerp(startOffset, endOffset, ((float)y + 1f) / (float)Y) * taper1;
                            Vector3 pos0 = sd.mCurve.GetPosition(t0) + offset0 * taper0;
                            Vector3 pos1 = sd.mCurve.GetPosition(t1) + offset1 * taper1;

                            Quaternion r0 = Quaternion.AngleAxis(angle, tan0);
                            Quaternion r1 = Quaternion.AngleAxis(angle, tan1);

                            float wave = Mathf.Pow(-1, y) * m_currentHairGroup.m_folding;
                            wave += Mathf.Sin(m_currentHairGroup.m_wavePeriod * Mathf.PI * (float)y / (float)Y) * m_currentHairGroup.m_waveAmplitude;

                            float bendAngle0 = -Mathf.Lerp(m_currentHairGroup.m_bendAngleStart, m_currentHairGroup.m_bendAngleEnd, t0);
                            float bendAngle1 = -Mathf.Lerp(m_currentHairGroup.m_bendAngleStart, m_currentHairGroup.m_bendAngleEnd, t1);
                            if (m_lowPolyMode)
                            {
                                //bendAngle0 = 0;
                                //bendAngle1 = 0;
                            }
                            
                            Quaternion tanR0 = (Quaternion.AngleAxis(((XStep * x) - .5f) * (-bendAngle0), tan0));
                            Quaternion tanR1 = (Quaternion.AngleAxis(((XStep * x) + XStep - .5f) * (-bendAngle0), tan0));
                            Quaternion tanR2 = (Quaternion.AngleAxis(((XStep * x) - .5f) * (-bendAngle1), tan1));
                            Quaternion tanR3 = (Quaternion.AngleAxis(((XStep * x) + XStep - .5f) * (-bendAngle1), tan1));


                            /*
                            //Here the curl option
                            Quaternion qTwist = Quaternion.AngleAxis(900f * t0 * t0 * t0, tan0);
                            tanR0 = qTwist * tanR0;
                            tanR1 = qTwist * tanR1;
                            tanR2 = qTwist * tanR2;
                            tanR3 = qTwist * tanR3;
                            */

                            int z = 1;
                            //for (int z = 1; z < 2; z++)
                            {
                                //Add vertices 
                                if (y == 0)
                                {
                                    
                                    Vector3 v = r0 * (tanR0 * (Vector3.Cross(up0, tan0) * ((XStep * x) - .5f) + up0 * wave) * taper0) + pos0 + r0 * up0 * nOffset * taper0 - m_currentHairGroup.m_tangentStartOffset * tan0;
                                    _v.Add(v);
                                    v = r0 * (tanR1 * (Vector3.Cross(up0, tan0) * ((XStep * x) + XStep - .5f) + up0 * wave) * taper0) + pos0 + r0 * up0 * nOffset * taper0 - m_currentHairGroup.m_tangentStartOffset * tan0;
                                    _v.Add(v);
                                }
                                _v.Add(r1 * ((tanR2 * Vector3.Cross(up1, tan1) * ((XStep * x) - .5f) + up1 * wave) * taper1) + pos1 + r1 * up1 * nOffset * taper1);
                                _v.Add(r1 * ((tanR3 * Vector3.Cross(up1, tan1) * ((XStep * x) + XStep - .5f) + up1 * wave) * taper1) + pos1 + r1 * up1 * nOffset * taper1);

                                //Add uv
                                float uvx = (float)m_currentHairGroup.m_UVX;
                                if (y == 0)
                                {
                                    _uv.Add(new Vector2((XStep * x) * uvx, YStep * y));
                                    _uv.Add(new Vector2((XStep * x + XStep) * uvx, YStep * y));
                                }
                                _uv.Add(new Vector2((XStep * x) * uvx, YStep * y + YStep));
                                _uv.Add(new Vector2((XStep * x + XStep) * uvx, YStep * y + YStep));

                                //Add normals
                                //float n2t = m_params.m_normalToTangent;
                                float n2t = m_currentHairGroup.m_normalSwitch;
                                n2t = 0;

                                //normals
                                if (y == 0)
                                {
                                    _n.Add(r0 * tanR0 * Vector3.Lerp(up0, tan0, n2t).normalized);// * (z == 0 ? 1 : -1));
                                    _n.Add(r0 * tanR1 * Vector3.Lerp(up0, tan0, n2t).normalized);// * (z == 0 ? 1 : -1));
                                }
                                _n.Add(r1 * tanR2 * Vector3.Lerp(up1, tan1, n2t).normalized);// * (z == 0 ? 1 : -1));
                                _n.Add(r1 * tanR3 * Vector3.Lerp(up1, tan1, n2t).normalized);// * (z == 0 ? 1 : -1));

                                //tangents
                                if (y == 0)
                                {
                                    _tg.Add((tan0));
                                    _tg.Add((tan0));
                                }
                                _tg.Add((tan1));
                                _tg.Add((tan1));


                                //Colors 
                                if (y == 0)
                                {
                                    _colors.Add(new Color(1, 1, 1, rnd));
                                    _colors.Add(new Color(1, 1, 1, rnd));
                                }
                                _colors.Add(new Color(1, 1, 1, rnd));
                                _colors.Add(new Color(1, 1, 1, rnd));



                                //Add triangles
                                if (z == 0)
                                {
                                    _t.Add(_v.Count - 4);
                                    _t.Add(_v.Count - 3);
                                    _t.Add(_v.Count - 2);
                                    _t.Add(_v.Count - 1);
                                    _t.Add(_v.Count - 2);
                                    _t.Add(_v.Count - 3);
                                }
                                else
                                {
                                    _t.Add(_v.Count - 4);
                                    _t.Add(_v.Count - 2);
                                    _t.Add(_v.Count - 3);
                                    _t.Add(_v.Count - 1);
                                    _t.Add(_v.Count - 3);
                                    _t.Add(_v.Count - 2);
                                }



                                //Assign bones to the vertices
                                if (bones != null)
                                {
                                    int boneCount = (int)((float)m_currentHairGroup.m_subdivisionY * (1f - m_currentHairGroup.m_bonesStartOffset));

                                    //float startOnCurve = m_currentHairGroup.m_bonesStartOffset;
                                    //float bonePos = startOnCurve + t0 * (1f - startOnCurve);
                                    //int b =   (int)(t0 * (float)Y);  
                                    
                                    float f = (t0 - m_currentHairGroup.m_bonesStartOffset);
                                    if(m_currentHairGroup.m_bonesStartOffset<1f)
                                        f/= (1f - m_currentHairGroup.m_bonesStartOffset);

                                    int b = t0 > m_currentHairGroup.m_bonesStartOffset ? (int)(f * (float)boneCount):0;
                                    
                                    //int b = y;


                                    //int b = (int)(t0 * (float)Y);

                                    //Add bone weights
                                    BoneWeight bw = new BoneWeight();


                                    bw.weight0 = 1f;// t0 > m_currentHairGroup.m_bonesStartOffset ? 1f:0f;
                                    //int test = 0;

                                    if (y == 0)
                                    {                                       
                                        bw.boneIndex0 = b + startBoneId;

                                        //if (bw.boneIndex0 >= 10)
                                        //    test = 1;

                                        _boneWeights.Add(bw);
                                        _boneWeights.Add(bw);
                                    }
                                    
                                    if ((b + startBoneId + 1) < bones.Count)
                                        bw.boneIndex0 = b + startBoneId + 1;
                                    else
                                        bw.boneIndex0 = b + startBoneId;

                                    if(t0 < m_currentHairGroup.m_bonesStartOffset)
                                        bw.boneIndex0 = startBoneId;


                                    
                                    //if(bw.boneIndex0>= 10)
                                    //    test = 1;

                                    _boneWeights.Add(bw);
                                    _boneWeights.Add(bw);

                                }
                            }
                        }
                    }
                }

                //setup UV from vertices position          
                m.SetVertices(_v);
                m.SetNormals(_n);
                m.SetTangents(_tg);
                m.SetTriangles(_t, 0);
                m.SetUVs(0, _uv);
                m.SetColors(_colors);

                if (bones != null)
                {
                    m.boneWeights = _boneWeights.ToArray();

                    Matrix4x4[] bindposes = new Matrix4x4[bones.Count - startBoneId];
                    for (int i = startBoneId; i < bones.Count; ++i)
                    {
                        bindposes[i - startBoneId] = bones[i].worldToLocalMatrix * trans.localToWorldMatrix;
                    }

                    m.bindposes = bindposes;
                }
                m.name = "HairStrand";
                return m;
            }


            /// <summary>
            /// Generate mesh from collection
            /// </summary>
            /// <param name="sd"></param>
            /// <param name="bones"></param>
            /// <param name="startBoneId"></param>
            /// <returns></returns>
            Mesh GenerateMeshFromCollection(HairDesignerStrandMeshCollectionBase meshCollection, int strandId, StrandData sd, List<Transform> bones, int startBoneId, Transform trans)
            {
                if (meshCollection == null)
                    return null;

                HairDesignerStrandMeshCollectionBase.StrandMesh stdMesh = meshCollection.m_collection.Find(strand => strand.id == strandId);
                if (stdMesh == null)
                    return null;


                Mesh refMesh = stdMesh.mesh;

                if (refMesh == null)
                    return null;

                
                refMesh.RecalculateBounds();

                Bounds bnds = refMesh.bounds;


                Mesh m = new Mesh();

                List<BoneWeight> boneWeights = new List<BoneWeight>();
                List<Color> colors = new List<Color>();
                Vector3[] vrt = refMesh.vertices;
                Vector3[] nrm = refMesh.normals;
                Vector4[] tng = refMesh.tangents;

                sd.localpos = Vector3.zero;
                sd.mCurve.SetUpRef(trans.InverseTransformDirection(Vector3.up));

                Random.InitState(m_currentHairGroup.m_rndSeed);
                int K = m_currentHairGroup.m_strandCount;
                K = 1;
                for (int k = 0; k < K; ++k)
                {
                    
                    for (int vId = 0; vId < refMesh.vertexCount; ++vId)
                    {

                        //Normalized position in bounding box



                        //v[vId].z += .5f;                       
                        
                        vrt[vId].x /= bnds.size.x;
                        vrt[vId].y /= bnds.size.y;
                        vrt[vId].z /= bnds.size.z;

                        Quaternion q = Quaternion.Euler(stdMesh.orientation);
                        vrt[vId] = q * vrt[vId];
                        nrm[vId] = q * nrm[vId];
                        tng[vId] = q * tng[vId];

                        //remap Z to 0->1
                        Vector3 newCenter = q * bnds.center;
                        Vector3 newExt = q * bnds.extents;
                        Vector3 newSize = q * bnds.size;

                        float zOffset = (newCenter.z - newExt.z) / newSize.z;

                        
                        vrt[vId].z -= zOffset;
                        

                        float t = vrt[vId].z;


                        if (m_currentHairGroup.m_shape.length < 2)
                        {
                            m_currentHairGroup.m_shape.AddKey(new Keyframe(0, 1f));
                            m_currentHairGroup.m_shape.AddKey(new Keyframe(1, 1f));
                        }
                        float taper = m_currentHairGroup.m_shape.Evaluate(t) * m_scale * strandScaleFactor * m_currentHairGroup.m_scale;

                        Vector3 pos = sd.mCurve.GetPosition(t);
                        Vector3 tan = sd.mCurve.GetTangent(t);
                        Vector3 up = sd.mCurve.GetUp(t);

                        q = Quaternion.LookRotation(tan, up);
                        vrt[vId] = pos + q * new Vector3(vrt[vId].x, vrt[vId].y, 0f) * taper;
                        nrm[vId] = q * nrm[vId];
                        tng[vId] = q * tng[vId];

                        float rnd = Random.value;
                        colors.Add(new Color(1, 1, 1, rnd));

                        {                                

                            
                            if (bones != null)
                            {
                                int b = (int)(t * (float)m_currentHairGroup.m_boneCount);
                                //Add bone weights
                                BoneWeight bw = new BoneWeight();
                                bw.weight0 = 1f;
                               
                                bw.boneIndex0 = b + startBoneId;
                                bw.boneIndex0 = Mathf.Clamp(bw.boneIndex0, 0 + startBoneId, m_currentHairGroup.m_boneCount - 1 + startBoneId);
                                
                                boneWeights.Add(bw);


                            }
                            
                        }
                    }

                }//

               
                m.vertices = vrt;
                m.uv = refMesh.uv;
                m.normals = nrm;
                m.tangents = tng;                
                m.colors = colors.ToArray();
                m.triangles = refMesh.triangles;

                m.RecalculateBounds();
                m.RecalculateNormals();
                m.RecalculateTangents();

                if (bones != null)
                {
                    m.boneWeights = boneWeights.ToArray();

                    Matrix4x4[] bindposes = new Matrix4x4[bones.Count - startBoneId];
                    for (int i = startBoneId; i < bones.Count; ++i)
                    {
                        bindposes[i - startBoneId] = bones[i].worldToLocalMatrix * trans.localToWorldMatrix;
                    }

                    m.bindposes = bindposes;
                }
                m.name = "HairStrand";

                //return refMesh;
                return m;
            }





            /// <summary>
            /// Genretes the bones for each groups
            /// </summary>
            public void GenerateGroupBones()
            {

                Transform[] bones = null;
                for (int i = 0; i < m_groups.Count; ++i)
                {
                    if (m_editorLayers.Count > m_groups[i].m_layer && !m_editorLayers[m_groups[i].m_layer].visible)
                    {
                        m_groups[i].m_generated = false;
                        continue;
                    }

                    m_groups[i].m_generated = true;
                    /*
                    if (m_editorLayers.Count > m_groups[i].m_layer && !m_editorLayers[m_groups[i].m_layer].visible)
                        continue;
                        */
                    if (m_GenerateBones || m_groups[i].m_bones == null ) //|| !m_groups[i].m_generated)
                    {
                        
                        int boneCount = m_groups[i].m_boneCount;

                        if (m_groups[i].m_generationMode == eMeshGenerator.HAIR_CARDS)
                        {
                            //boneCount = m_groups[i].m_subdivisionY;
                            boneCount = (int)((float)m_groups[i].m_subdivisionY * (1f - m_groups[i].m_bonesStartOffset));
                            boneCount = Mathf.Clamp(boneCount, 1, boneCount);
                            //Debug.Log("boneCount" + boneCount);
                        }

                        bones = GenerateBones(i, boneCount, m_groups[i].m_bonesStartOffset);
                        if (bones.Length > 0)
                        {
                            HairDesignerBoneBase bb = bones[0].gameObject.AddComponent<HairDesignerBoneBase>();
                            
                            
                            //HairDesignerBoneBase bb = bones[0].gameObject.AddComponent(System.Type.GetType("HairDesignerBone")) as HairDesignerBoneBase;
                            //Debug.Log(bb.GetType());

                            bb.m_meshReference = m_meshInstanceRef;
                            bb.m_target = m_groups[i].m_parent != m_hd.transform ? m_groups[i].m_parent : null;


                            if (bb.m_target != null)
                            {
                                //set position for runtime
                                bones[0].SetParent(bb.m_target, true);
                                bb.m_targetName = bb.m_target.name;//store bone name for Runtime layer
                                bb.m_localPosition = bb.transform.localPosition;
                                bb.m_localRotation = bb.transform.localRotation;
                                bb.m_localScale = bb.transform.localScale;
                                bb.m_useLocalTransform = true;
                            }

                            //reset position to layer
                            //bones[0].parent = m_skinnedMesh.transform;
                            bones[0].SetParent(m_skinnedMesh.transform, true);
                        }


                        m_groups[i].m_startBoneId = m_bones.Count;
                        m_groups[i].m_bones = bones.ToList();

                        for (int b = 0; b < bones.Length; ++b)
                            m_bones.Add(bones[b]);
                    }
                    else
                    {
                        m_groups[i].m_startBoneId = m_bones.Count;

                        for (int b = 0; b < m_groups[i].m_bones.Count; ++b)
                            m_bones.Add(m_groups[i].m_bones[b]);
                    }
                }
            }








            public void UpdateStrandMesh()
            {
                for (int i = 0; i < m_groups.Count; ++i)
                {                   
                   
                    if (m_groups[i].m_generationMode == eMeshGenerator.MESH_COLLECTION)
                    {
                        //Check if the collection mesh is valid
                        if (m_groups[i].m_strandMeshCollection == null)
                            continue;

                        HairDesignerStrandMeshCollectionBase.StrandMesh stdMesh = m_groups[i].m_strandMeshCollection.m_collection.Find(strand => strand.id == m_groups[i].m_strandMeshId);
                        if (stdMesh == null)
                            continue;
                    }


                    if (m_editorLayers.Count > m_groups[i].m_layer && !m_editorLayers[m_groups[i].m_layer].visible)
                    {
                        m_groups[i].m_generated = false;
                        continue;
                    }


                    for (int g = 0; g < m_groups[i].m_strands.Count; ++g)
                    {

                        if (m_groups[i].m_strands[g].mesh == null)
                        {
                            m_currentHairGroup = m_groups[i];

                            if (m_currentHairGroup.m_generationMode == eMeshGenerator.HAIR_CARDS)
                                m_groups[i].m_strands[g].mesh = GenerateMeshHairCards(m_groups[i].m_strands[g], m_groups[i].m_bones, 0, transform);


                            if (m_currentHairGroup.m_generationMode == eMeshGenerator.MESH_COLLECTION)
                                m_groups[i].m_strands[g].mesh = GenerateMeshFromCollection(m_groups[i].m_strandMeshCollection, m_groups[i].m_strandMeshId, m_groups[i].m_strands[g], m_bones, m_groups[i].m_startBoneId, transform);
                        }
                    }
                }
            }



            public void UpdateMirror(bool force)
            {
                for (int i = 0; i < m_mirrorModifiers.Count; ++i)
                    if ((m_mirrorModifiers[i].autoUpdate && m_lowPolyMode) || m_forceMirrorUpdate || force)
                    //if ((m_mirrorModifiers[i].autoUpdate ) || force)
                    {
                        m_mirrorModifiers[i].Update(this);
                        //Debug.Log("Update mirrors");
                    }
            }




            public Transform m_meshGenerationTransformReference = null;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="skinRef"></param>
            public override void CreateHairMesh(Mesh skinRef)
            {

                HairDesignerBase.InitRandSeed(0);

                m_hair = new Mesh();

                //float radius = m_startRadius;

                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector4> tangents = new List<Vector4>();
                List<int> triangles = new List<int>();
                List<Vector2> uv = new List<Vector2>();
                List<BoneWeight> boneWeights = new List<BoneWeight>();
                List<Matrix4x4> bindPoses = new List<Matrix4x4>();
                //Matrix4x4[] bindPoses;
                List<Color> colors = new List<Color>();
                m_bones.Clear();
                //Transform[] bones = null;
                //int startBoneId = 0;

                //add all strand to the mesh

                if (Application.isPlaying)
                    m_GenerateBones = false;


                //update mirrors
                //m_forceMirrorUpdate = false;
                UpdateMirror(false);
                //m_forceMirrorUpdate = false;
                /*
                for (int i = 0; i < m_mirrorModifiers.Count; ++i)
                    if (m_mirrorModifiers[i].autoUpdate && m_fastMode )
                        m_mirrorModifiers[i].Update(this);
                        */

                GenerateGroupBones();

                bool oldVersion = false;

                if (!oldVersion)
                    UpdateStrandMesh();

                for (int i = 0; i < m_groups.Count; ++i)
                {

                    if (oldVersion)
                    {
                        if (m_groups[i].m_generationMode == eMeshGenerator.MESH_COLLECTION)
                        {
                            //Check if the collection mesh is valid
                            if (m_groups[i].m_strandMeshCollection == null)
                                continue;

                            HairDesignerStrandMeshCollectionBase.StrandMesh stdMesh = m_groups[i].m_strandMeshCollection.m_collection.Find(strand => strand.id == m_groups[i].m_strandMeshId);
                            if (stdMesh == null)
                                continue;
                        }


                        if (m_editorLayers.Count > m_groups[i].m_layer && !m_editorLayers[m_groups[i].m_layer].visible)
                        {
                            m_groups[i].m_generated = false;
                            continue;
                        }
                    }

                   
                    //generate final mesh
                    for (int g = 0; g < m_groups[i].m_strands.Count; ++g)
                    {

                        if (oldVersion)
                        {
                            if (m_groups[i].m_strands[g].mesh == null)
                            {
                                m_currentHairGroup = m_groups[i];

                                if (m_currentHairGroup.m_generationMode == eMeshGenerator.HAIR_CARDS)
                                    m_groups[i].m_strands[g].mesh = GenerateMeshHairCards(m_groups[i].m_strands[g], m_groups[i].m_bones, 0, transform);


                                if (m_currentHairGroup.m_generationMode == eMeshGenerator.MESH_COLLECTION)
                                    m_groups[i].m_strands[g].mesh = GenerateMeshFromCollection(m_groups[i].m_strandMeshCollection, m_groups[i].m_strandMeshId, m_groups[i].m_strands[g], m_bones, m_groups[i].m_startBoneId, transform);
                            }
                        }
                        else
                        {
                            m_currentHairGroup = m_groups[i];
                        }

                        if (!m_groups[i].m_generated)
                        {
                            continue;
                        }


                        if (m_groups[i].m_strands[g].mesh == null)
                        {
                            Debug.LogWarning("Layer '" + m_name + "' Strand mesh not generated ");
                            continue;
                        }
                        else if (vertices.Count + m_groups[i].m_strands[g].mesh.vertexCount > 65000)
                        {
                            //Debug.LogWarning("Layer '" + m_name + "' Too much vertices " + (vertices.Count + m_groups[i].m_strands[g].mesh.vertexCount));
                            //continue;
                        }

                        

                        int[] strandTriangles = m_groups[i].m_strands[g].mesh.triangles;
                        Vector3[] strandVertices = m_groups[i].m_strands[g].mesh.vertices;
                        Vector3[] strandNormals = m_groups[i].m_strands[g].mesh.normals;
                        Vector4[] strandTangents = m_groups[i].m_strands[g].mesh.tangents;
                        Vector2[] strandUV = m_groups[i].m_strands[g].mesh.uv;
                        BoneWeight[] strandBoneWeight = m_groups[i].m_strands[g].mesh.boneWeights;
                        Matrix4x4[] strandBindPose = m_groups[i].m_strands[g].mesh.bindposes;
                        Color[] strandColors = m_groups[i].m_strands[g].mesh.colors;

                        for (int t = 0; t < strandTriangles.Length; ++t)
                            triangles.Add(strandTriangles[t] + vertices.Count);

                        Vector2 _uvRnd = new Vector2(Random.Range(0, m_atlasSizeX), Random.Range(0, m_atlasSizeX)) / m_atlasSizeX;

                        for (int v = 0; v < strandVertices.Length; ++v)
                        {
                            vertices.Add(strandVertices[v]);
                            //colors.Add(new Color(1, 1, 1, m_scale));
                            colors.Add(strandColors[v]);
                            if (strandTangents.Length > v)
                                tangents.Add(strandTangents[v]);

                            uv.Add(strandUV[v] / m_atlasSizeX + _uvRnd);

                            //if (m_GenerateBones )// && !m_buildSelectionOnly)
                            {
                                /*
                                if (strandBoneWeight[v].boneIndex0 >= m_bones.Count)
                                    Debug.Log("invalid Bone id : " + strandBoneWeight[v].boneIndex0);
                                */
                                /*
                                if (m_buildSelectionOnly)
                                {                                    
                                    strandBoneWeight[v].boneIndex0 = 0;
                                    strandBoneWeight[v].weight0 = 1;
                                }
                                */
                                //for(int bw=0; bw<strandBoneWeight.Length; ++bw  )
                                {
                                    int test = 0;
                                    if (strandBoneWeight[v].boneIndex0 >= 10)
                                        //Debug.LogWarning("bone id doesn't match");
                                        test++;

                                    strandBoneWeight[v].boneIndex0 += m_currentHairGroup.m_startBoneId;

                                    if (strandBoneWeight[v].boneIndex0 >= m_bones.Count)
                                        strandBoneWeight[v].boneIndex0 = m_bones.Count - 1;
                                }


                                if (strandBoneWeight.Length > v)
                                    boneWeights.Add(strandBoneWeight[v]);

                            }
                            /*
                            else
                            {
                                strandBoneWeight[v].boneIndex0 = 0;
                                strandBoneWeight[v].weight0 = 1;
                            }*/
                        }

                        for (int n = 0; n < strandNormals.Length; ++n)
                            normals.Add(strandNormals[n]);


                        if (m_GenerateBones )
                        {
                            for (int bp = 0; bp < strandBindPose.Length; ++bp)
                                bindPoses.Add(strandBindPose[bp]);
                        }
                    }
                }


                m_hair.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                m_hair.vertices = vertices.ToArray();
                m_hair.normals = normals.ToArray();
                m_hair.tangents = tangents.ToArray();
                m_hair.triangles = triangles.ToArray();
                m_hair.uv = uv.ToArray();
                m_hair.colors = colors.ToArray();
                m_hair.name = "HairDesignerInstance";

                if (m_GenerateBones )
                {                    
                    m_hair.boneWeights = boneWeights.ToArray();
                    m_hair.bindposes = bindPoses.ToArray();
                    //Debug.Log("Bone generated " + m_buildSelectionOnly);
                }

                HairDesignerBase.RestoreRandSeed();
                m_hair.RecalculateBounds();
                //m_hair.RecalculateNormals();


                //Debug.Log("m_GenerateBones="+ m_GenerateBones + " m_buildSelectionOnly=" + m_buildSelectionOnly + "  -> Bones " + m_bones.Count);
            }




            public override void GenerateMeshRenderer()
            {
                if (GetStrandCount() == 0)
                    return;

                if (m_hd == null || m_meshInstance != null || _editorDelete)
                    return;

                if (m_matPropBlkHair == null)
                    m_matPropBlkHair = new MaterialPropertyBlock();

                DestroyMesh();
                //m_colliders.Clear();

                //SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();
                //m_enableSkinMesh = smr != null;
                //generate HairMesh                

                m_meshInstance = new GameObject(m_name);                
                m_meshInstance.transform.SetParent(m_hd.transform, true);
                m_meshInstance.transform.localRotation = Quaternion.identity;
                m_meshInstance.transform.localPosition = Vector3.zero;
                //if( !m_skinningVersion160 )
                //    m_meshInstance.transform.localScale = Vector3.one;//fix scaling issue for skinning 1.6.0
                
                
                //m_meshInstanceRef = m_meshInstance.AddComponent(System.Type.GetType("HairDesignerMeshInstance")) as HairDesignerMeshInstanceBase;
                m_meshInstanceRef = m_meshInstance.AddComponent<HairDesignerMeshInstanceBase>();
                
                m_skinnedMesh = m_meshInstance.AddComponent<SkinnedMeshRenderer>();
                m_GenerateBones = true;
                //m_skinnedMesh.enabled = false;
                //m_GenerateBones = !m_buildSelectionOnly;
                //m_GenerateBones = false;

                CreateHairMesh(null);
                //m_GenerateBones = false;

                

                //Debug.Log("Bones count " + m_bones.Count);
                
                m_skinnedMesh.bones = m_bones.ToArray();
                m_skinnedMesh.rootBone = m_skinnedMesh.transform;
                
                m_skinnedMesh.sharedMesh = m_hair;
                if (m_hairMeshMaterialTransparent == null)
                {
                    m_skinnedMesh.material = m_hairMeshMaterial;
                }
                else
                {
                    Material[] mats = new Material[2];
                    mats[0] = m_hairMeshMaterial;
                    mats[1] = m_hairMeshMaterialTransparent;
                    m_skinnedMesh.materials = mats;
                }

                /*
                m_needMeshRebuild = false;
                m_needMeshRefresh = false;
                */
                //m_skinnedMesh.bones = smr.bones;
                //m_skinnedMesh.rootBone = smr.rootBone;                                
            }









            Transform[] GenerateBones(int groupId, int nb, float bonesOffset)
            {
                MBZCurv c = m_groups[groupId].m_mCurv;
                Transform[] bones = new Transform[nb];
                //generate bones
                Transform parent = m_skinnedMesh.transform;

                m_groups[groupId].m_bones.Clear();

                Vector3[] initPos = new Vector3[nb];

                Transform trans = m_skinningVersion160 && m_groups[groupId].m_triLock != null && m_groups[groupId].m_triLock.m_cdata.m_lastTransformTarget != null ? m_groups[groupId].m_triLock.m_cdata.m_lastTransformTarget : m_hd.transform;


                for (int i = 0; i < nb; ++i)
                {
                    bones[i] = new GameObject(this.m_name + "_bone " + (m_bones.Count + i)).transform;
                    //bones[i].hideFlags = HideFlags.HideInHierarchy;
                    m_groups[groupId].m_bones.Add(bones[i]);

                    //-------------------------------------
                    //DEBUG
                    //bones[i].gameObject.AddComponent<SphereCollider>();
                    //-------------------------------------
                    float bonePos = (float)i / (float)nb;
                    if(i>0)
                        bonePos = bonesOffset + bonePos * (1f - bonesOffset);

                    bones[i].position = trans.TransformPoint(c.GetPosition(bonePos));
                    bones[i].rotation = Quaternion.LookRotation(trans.TransformDirection(c.GetTangent((float)i / (float)nb)), trans.TransformDirection(c.GetUp((float)i / (float)nb)));
                    /*
                    if (parent != null)
                        parent.LookAt(bones[i].position, parent.up);
                        */
                    bones[i].parent = parent;
                    bones[i].localScale = Vector3.one;
                    parent = bones[i].transform;
                    /*
                    if( i>0 )
                        bones[i].localPosition = new Vector3(0, 0, bones[i].localPosition.magnitude);
                        */
                    initPos[i] = bones[i].position;
                }


                //fix bone positions
                for (int i = 1; i < bones.Length; ++i)
                {
                    bones[i].parent.LookAt(initPos[i], bones[i].parent.up);
                    bones[i].position = initPos[i];
                }


                return bones;
            }




            

            #endregion





            #region EDITOR UTIL
            public override void DrawHairStrand()
            {


                //Draw hair stand
                HairDesignerBase.InitRandSeed(0);
                int m_batchingFix = 1;
                for (int i = 0; i < GetStrandCount(); ++i)
                {
                    m_batchingFix++;
                    m_batchingFix = 1;
                    StrandRenderingData dt = GetData(i);

                    if (dt.layer >= m_editorLayers.Count || !m_editorLayers[dt.layer].visible)
                    {
                        continue;
                    }

                    Vector3 worldPos = m_hd.transform.TransformPoint(dt.localpos + dt.normal * (dt.strand.offset + m_params.m_offset));
                    Matrix4x4 m = Matrix4x4.TRS(worldPos, m_hd.transform.rotation * dt.rotation, m_hd.transform.lossyScale);

                    m_matPropBlkHair.SetFloat("KHD_editor", 0);

                    Graphics.DrawMesh(dt.strand.mesh, m, m_hairMeshMaterial, 0, null, 0, m_matPropBlkHair, false, false);
                    if (m_hairMeshMaterialTransparent != null)
                        Graphics.DrawMesh(dt.strand.mesh, m, m_hairMeshMaterialTransparent, 0, null, 0, m_matPropBlkHair, false, false);

                }


                HairDesignerBase.RestoreRandSeed();
            }




            /// <summary>
            /// PaintTool selection
            /// </summary>
            /// <param name="data"></param>
            /// <param name="bt"></param>
            public override void PaintTool(StrandData data, BrushToolData bt)
            {
                switch (bt.tool)
                {
                    case ePaintingTool.ADD:
                        AddGroup(data, bt);
                        break;
                }
            }



            public int m_referenceId = -1;
            void AddGroup(StrandData data, BrushToolData bt)
            {
                HairGroup hg;

                //if (m_groups.Count == 0)
                {
                    hg = new HairGroup();
                    hg.m_mCurv = new MBZCurv();
                    hg.m_scale = .01f;

                    float length = transform.TransformPoint(Vector3.up * m_groupCreationLength).magnitude;
                    hg.m_mCurv.startPosition = data.localpos;
                    hg.m_mCurv.endPosition = hg.m_mCurv.startPosition + data.normal * length * m_scale * hg.m_scale;
                    hg.m_mCurv.startTangent = data.normal * length * .5f * m_scale * hg.m_scale;
                    hg.m_mCurv.endTangent = -data.normal * length * .5f * m_scale * hg.m_scale;

                    if (m_skinningVersion160)
                    {
                        hg.m_triLock = new TriangleLock();
                        hg.m_triLock.Lock(hg.m_mCurv.startPosition, hg.m_mCurv.startTangent, hg.m_mCurv.GetUp(0), bt.collider.transform, transform, data.meshTriId, bt.colliderVertices, bt.colliderTriangles, false);
                        //hg.m_triLock.Apply( bt.collider.transform, bt.colliderVertices, bt.colliderTriangles, true);
                        hg.m_mCurv.ConvertOffsetAndRotation(hg.m_triLock.m_cdata.localPosition, hg.m_triLock.m_cdata.localRotation, false);
                    }

                    hg.m_layer = data.layer;




                }
                /*
                else 
                {
                    if (m_referenceId < 0 || m_referenceId >= m_groups.Count)
                        m_referenceId = m_groups.Count - 1;

                    hg = m_groups[m_referenceId].Copy();
                    Vector3 offset = data.localpos - m_groups[m_referenceId].m_mCurv.startPosition;
                    Quaternion q = Quaternion.FromToRotation(m_groups[m_referenceId].m_meshNormal, data.normal);

                    hg.m_mCurv.startPosition += offset;
                    hg.m_mCurv.endPosition += q*offset;
                    hg.m_mCurv.startTangent = q * hg.m_mCurv.startTangent;
                    hg.m_mCurv.endTangent = q * hg.m_mCurv.endTangent;

                }
                */
                hg.m_meshNormal = data.normal;
                hg.m_parent = transform;
                m_groups.Add(hg);
                m_referenceId = m_groups.Count - 1;
                hg.Generate();
            }




            public void ClearStrandTmpMesh()
            {
                for (int i = 0; i < m_groups.Count; ++i)
                {
                    m_groups[i].m_generated = false;
                    for (int j = 0; j < m_groups[i].m_strands.Count; ++j)
                    {
                        if (Application.isPlaying)
                            Destroy(m_groups[i].m_strands[j].mesh);
                        else
                            DestroyImmediate(m_groups[i].m_strands[j].mesh);
                        m_groups[i].m_strands[j].mesh = null;
                    }
                }
            }


            


            public override void DestroyMesh()
            {
                //Destroy all bones
                //if (m_meshInstance != null)
                if (!m_dontDestroyMeshOnDestroy)
                {
                    for (int i = 0; i < m_groups.Count; ++i)
                    {
                        for (int j = m_groups[i].m_bones.Count - 1; j >= 0; --j)
                        {
                            if (m_groups[i].m_bones[j] != null)
                            {
                                if (Application.isPlaying)
                                    Destroy(m_groups[i].m_bones[j].gameObject);
                                else
                                    DestroyImmediate(m_groups[i].m_bones[j].gameObject);
                                m_groups[i].m_bones[j] = null;
                            }
                        }
                        m_groups[i].m_bones.Clear();

                        
                    }
                    base.DestroyMesh();
                }

            }

#endregion



        }
    }
}
