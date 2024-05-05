using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kalagaan.HairDesignerExtension
{
    
    
    public class HeadController : MonoBehaviour
    {

        public Transform m_headBone;
        public Transform m_CharacterOrientation;
        public Transform m_LookAtTarget;
        public SkinnedMeshRenderer m_face;
        public Slider m_ikSlider;

        //public Kalagaan.BlendShapesPresetTool.BlendShapesPresetControllerBase m_bspm;

        [Range(0f, 1f)]
        public float m_lookAtWeight = 0f;

        public float m_reactionSpeed = 2f;

        public int m_parentMax = 0;
        public AnimationCurve m_curve = new AnimationCurve();

        public Vector2 m_blinkTimerMinMax = new Vector2(2f, 5f);
        float m_blinkTimer;
        float m_blinkStartTime;
        bool m_blinkTrigger = false;

        public bool m_eyeTracking = true;
        public bool m_lookAtMouse = true;

        public float m_faceExpressionWeight = 0f;
        public bool m_faceAnimationEnabled = false;
        public int[] m_faceIDs;
        int m_faceExpressionId;

        Quaternion[] m_original;
        Quaternion[] m_lastRotation;
        Transform[] m_IKChain;
        Vector3 m_targetPosition;
        Vector3 m_eyesTargetPosition;
        Vector3 m_noTarget;



        // Use this for initialization
        void Start()
        {
            //initialize data for the IK
            m_original = new Quaternion[m_parentMax + 1];
            m_lastRotation = new Quaternion[m_parentMax + 1];
            m_IKChain = new Transform[m_parentMax + 1];

            Transform t = m_headBone;
            for (int i = m_original.Length - 1; i >= 0; --i)
            {
                m_original[i] = t.localRotation;
                m_lastRotation[i] = t.rotation;
                m_IKChain[i] = t;
                t = t.parent;
            }

            m_noTarget = m_headBone.position + m_headBone.forward * 10f;
            m_targetPosition = m_LookAtTarget.position;

            
        }

            


        public void SetEyeTracking( bool On )
        {
            m_eyeTracking = On;
        }


            

        void LateUpdate()
        {
            if (m_headBone == null || m_LookAtTarget == null)
                return;

            if (m_ikSlider != null)
                m_lookAtWeight = m_ikSlider.value;

            Vector3 targetPosition = m_LookAtTarget.position;
            m_lookAtMouse = Input.GetMouseButton(0);

            if (m_lookAtMouse)
            {
                Vector3 mp = Input.mousePosition;
                mp.z = Vector3.Distance(Camera.main.transform.position, m_headBone.position) * .7f;
                targetPosition = Camera.main.ScreenToWorldPoint(mp);
                //targetPosition = m_noTarget;
                //targetPosition = Vector3.up;

                //Debug.DrawLine(m_headBone.position, targetPosition, Color.magenta);
            }

            if (Vector3.Dot((targetPosition - m_headBone.position).normalized, m_CharacterOrientation.forward) > -.5
                && Vector3.Dot((targetPosition - m_headBone.position).normalized, Vector3.up) < .9
                && Vector3.Dot((targetPosition - m_headBone.position).normalized, Vector3.up) > -.9 )
            {
                m_targetPosition = Vector3.Lerp(m_targetPosition, targetPosition, Time.deltaTime * m_reactionSpeed);
            }
            else
            {
                m_targetPosition = Vector3.Lerp(m_targetPosition, m_noTarget, Time.deltaTime * .1f);
            }




            if (m_headBone != null && m_LookAtTarget != null)
            {
                Quaternion q;

                //if( Vector3.Dot((m_targetPosition-m_headBone.position).normalized, m_headBone.forward) < .995f )
                for (int i = 0; i < m_IKChain.Length; ++i)
                {
                    float f = (float)i / (float)m_IKChain.Length;
                    m_IKChain[i].localRotation = m_original[i];
                    q = m_IKChain[i].rotation;
                    m_IKChain[i].LookAt(m_targetPosition);
                    m_IKChain[i].rotation = Quaternion.Lerp(q, m_IKChain[i].rotation,  m_curve.Evaluate(f) * m_lookAtWeight);
                    m_IKChain[i].rotation = Quaternion.Lerp(m_lastRotation[i], m_IKChain[i].rotation, Time.deltaTime * m_reactionSpeed);
                    m_lastRotation[i] = m_IKChain[i].rotation;
                }
                    
            }

            EyeControl();

            FaceExpression();
        }




        public void EnableExpression()
        {
            m_faceAnimationEnabled = true;
        }



        void FaceExpression()
        {
            if (m_faceIDs.Length == 0)
                return;

            if (m_faceAnimationEnabled)
            {                
                if(m_faceExpressionWeight == 0)
                {
                    //if (m_faceIDs.Length > 0)
                    {
                        int id = Random.Range((int)0, (int)m_faceIDs.Length);
                        m_faceExpressionId = m_faceIDs[id];
                    }
                }

                m_faceExpressionWeight += Time.deltaTime * 3f;

                if (m_faceExpressionWeight > 1f)
                    m_faceAnimationEnabled = false;
            }
            else
            {
                m_faceExpressionWeight -= Time.deltaTime * 1;
            }

            m_faceExpressionWeight = Mathf.Clamp(m_faceExpressionWeight, 0, 1f);


            m_face.SetBlendShapeWeight(m_faceExpressionId, m_faceExpressionWeight * 150f);

        }


        void EyeControl()
        {
            if (m_face == null)
                return;

            m_eyesTargetPosition = Vector3.Lerp(m_eyesTargetPosition, m_targetPosition, Time.deltaTime * m_reactionSpeed * 10f);
            float dotDir = Vector3.Dot(m_headBone.forward, (m_eyesTargetPosition - m_headBone.position).normalized);

            if (dotDir > 0)
            {
                float dotRL = Vector3.Dot(m_headBone.right, (m_eyesTargetPosition - m_headBone.position).normalized);
                if (!m_eyeTracking) dotRL = 0f;

                if (dotRL > 0)
                {
                    m_face.SetBlendShapeWeight(16, Mathf.Clamp01(dotRL * 2f) * 100f);
                    m_face.SetBlendShapeWeight(17, 0f);
                    //m_bspm.SetWeight("Eyes_R", Mathf.Clamp01(dotRL * 2f));
                    //m_bspm.SetWeight("Eyes_L", 0f);
                }
                else
                {
                    m_face.SetBlendShapeWeight(16, 0f);
                    m_face.SetBlendShapeWeight(17, Mathf.Clamp01(-dotRL * 2f) * 100f);
                    //m_bspm.SetWeight("Eyes_R", 0f);
                    //m_bspm.SetWeight("Eyes_L", Mathf.Clamp01(-dotRL * 2f));
                }

                float dotUD = Vector3.Dot(m_headBone.up, (m_eyesTargetPosition - m_headBone.position).normalized);
                if (!m_eyeTracking) dotUD = 0f;

                if (dotUD > 0)
                {
                    m_face.SetBlendShapeWeight(18, Mathf.Clamp01(dotUD * 2f) * 100f);
                    m_face.SetBlendShapeWeight(19, 0f);
                    //m_bspm.SetWeight("Eyes_U", Mathf.Clamp01(dotUD * 2f));
                    //m_bspm.SetWeight("Eyes_D", 0f);
                }
                else
                {
                    m_face.SetBlendShapeWeight(18, 0f);
                    m_face.SetBlendShapeWeight(19, Mathf.Clamp01(-dotUD * 2f) * 100f);
                    //m_bspm.SetWeight("Eyes_U", 0f);
                    //m_bspm.SetWeight("Eyes_D", Mathf.Clamp01 (- dotUD * 2f));
                }
            }
            else
            {
                /*
                m_bspm.GetPresetState("Eyes_L").weight = Mathf.Lerp(0f, m_bspm.GetPresetState("Eyes_L").weight, Time.deltaTime * 10f);
                m_bspm.GetPresetState("Eyes_R").weight = Mathf.Lerp(0f, m_bspm.GetPresetState("Eyes_R").weight, Time.deltaTime * 10f);
                m_bspm.GetPresetState("Eyes_D").weight = Mathf.Lerp(0f, m_bspm.GetPresetState("Eyes_D").weight, Time.deltaTime * 10f);
                m_bspm.GetPresetState("Eyes_U").weight = Mathf.Lerp(0f, m_bspm.GetPresetState("Eyes_U").weight, Time.deltaTime * 10f);
                */
            }



            if (m_blinkTrigger)
            {
                float f = Mathf.Sin((Time.time - m_blinkStartTime) * 10f);
                //m_bspm.GetPresetState("Blink").weight = Mathf.Clamp01(f);
                m_face.SetBlendShapeWeight(10, Mathf.Clamp01(f) * 100f);
                m_face.SetBlendShapeWeight(11, Mathf.Clamp01(f) * 100f);

                if (f < 0)
                {
                    m_blinkTrigger = false;
                    m_blinkTimer = m_blinkTimerMinMax.x + Random.value * (m_blinkTimerMinMax.y - m_blinkTimerMinMax.x) + Time.time;
                }
            }

            if (!m_blinkTrigger && Time.time > m_blinkTimer)
            {
                m_blinkTrigger = true;
                m_blinkStartTime = Time.time;
            }

        }
    }
    
}