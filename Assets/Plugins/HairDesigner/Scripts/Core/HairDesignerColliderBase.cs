using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


namespace Kalagaan.HairDesignerExtension
{
    [DefaultExecutionOrder(10)]
    [ExecuteInEditMode, AddComponentMenu("")]
    public class HairDesignerColliderBase : MonoBehaviour
    {
        [System.Serializable]
        public struct DualSphere
        {
            public Vector3 center1;
            public Vector3 center2;
            public float radius1;
            public float radius2;
            public int activated;
        }

        public DualSphere m_dualSphereInternalData;
        public DualSphere m_dualSphereWorldData;

        public bool m_useUnityColliderSettings1 = true;
        public bool m_useUnityColliderSettings2 = true;


        public Transform m_transform1;
        public Transform m_transform2;        

        public SphereCollider m_sc;
        public CapsuleCollider m_cc;
        

        private void Awake()
        {
            m_sc = GetComponent<SphereCollider>();
            m_cc = GetComponent<CapsuleCollider>();
        }



        public void LateUpdate()
        {
            UpdateData();
        }

        public void UpdateData()
        {
            //if(m_useUnityColliderSettings)
            {
                if(m_useUnityColliderSettings1 && m_sc!=null)
                {
                    m_dualSphereInternalData.center1 = m_sc.center;
                    m_dualSphereInternalData.radius1 = m_sc.radius;
                }

                if (m_useUnityColliderSettings2 && m_sc != null)
                {
                    m_dualSphereInternalData.center2 = m_sc.center;
                    m_dualSphereInternalData.radius2 = m_sc.radius;
                }


                if (m_cc != null)
                {
                    Vector3 orientation = m_cc.direction == 0? Vector3.right : (m_cc.direction==1? Vector3.up: Vector3.forward) ;                    
                    Vector3 offset = (m_cc.height * .5f - m_cc.radius) * orientation;

                    if (m_useUnityColliderSettings1 )
                    {                        
                        m_dualSphereInternalData.center1 = m_cc.center + offset;
                        m_dualSphereInternalData.radius1 = m_cc.radius;
                    }

                    if (m_useUnityColliderSettings2 )
                    {                     
                        m_dualSphereInternalData.center2 = m_cc.center - offset;
                        m_dualSphereInternalData.radius2 = m_cc.radius;
                    }
                }

                Transform t1, t2;
                t1 = t2 = transform;

                if (!(m_useUnityColliderSettings1 || m_transform1 == null))
                    t1 = m_transform1;

                if (!(m_useUnityColliderSettings2 || m_transform2 == null))
                    t2 = m_transform2;


                m_dualSphereWorldData.center1 = t1.TransformPoint(m_dualSphereInternalData.center1);
                m_dualSphereWorldData.center2 = t2.TransformPoint(m_dualSphereInternalData.center2);
                m_dualSphereWorldData.radius1 = t1.TransformVector(Vector3.up * m_dualSphereInternalData.radius1).magnitude;
                m_dualSphereWorldData.radius2 = t2.TransformVector(Vector3.up * m_dualSphereInternalData.radius2).magnitude;                

            }
        }



        public void OnDisable()
        {
            m_dualSphereInternalData.activated = 0;
            m_dualSphereWorldData.activated = 0;
        }

        public void OnEnable()
        {
            m_dualSphereInternalData.activated = 1;
            m_dualSphereWorldData.activated = 1;
        }


        public void SetWorldCenter( Vector3 worldPos, int id_1_or_2 )
        {            
            Transform t = transform;

            if (id_1_or_2 == 1)
            {

                if (!(m_useUnityColliderSettings1 || m_transform1 == null))
                    t = m_transform1;

                m_dualSphereInternalData.center1 = t.InverseTransformPoint(worldPos);
            }

            if (id_1_or_2 == 2)
            {

                if (!(m_useUnityColliderSettings2 || m_transform2 == null))
                    t = m_transform2;

                m_dualSphereInternalData.center2 = t.InverseTransformPoint(worldPos);
            }

        }


        public void SetWorldRadius(float worldRadius, float minDelta, int id)
        {
            Transform t = transform;

            if (id == 1)
            {

                if (!(m_useUnityColliderSettings1 || m_transform1 == null))
                    t = m_transform1;


                
                float radius = t.InverseTransformVector(worldRadius*Vector3.up).magnitude;
                if (Mathf.Abs(radius - m_dualSphereInternalData.radius1) > minDelta)
                    m_dualSphereInternalData.radius1 = radius;
            }

            if (id == 2)
            {

                if (!(m_useUnityColliderSettings2 || m_transform2 == null))
                    t = m_transform2;

                float radius = t.InverseTransformVector(worldRadius * Vector3.up).magnitude;
                if (Mathf.Abs(radius - m_dualSphereInternalData.radius2) > minDelta)
                    m_dualSphereInternalData.radius2 = radius;
            }

        }





        public void GizmosDrawCircle( Vector3 axis, Vector3 center, float radius, int nblines, bool half)
        {

            Vector3 r = Vector3.Cross(axis, Camera.current.transform.forward).normalized;
            Vector3 t = Vector3.Cross(r, axis).normalized;
            Vector3 p1, p2 = Vector3.zero;

            for (int i = 0; i <= nblines; ++i)
            {
                float f = (float)i / (float)nblines;
                p1 = r * Mathf.Cos(Mathf.PI * ( half ? 1f : 2f) * f);
                p1 += t * Mathf.Sin(Mathf.PI * (half ? 1f : 2f) * f);
                p1 *= radius;
                p1 = p1 + center;

                if (i > 0)
                {
                    Gizmos.DrawLine(p1, p2);
                }
                p2 = p1;
            }
        }

        
        public void OnDrawGizmos()
        {
            Color c = Color.green;
            c.a = .4f;
            Gizmos.color = c;

            Vector3 center1 = m_dualSphereWorldData.center1;
            Vector3 center2 = m_dualSphereWorldData.center2;

            Vector3 p1, p2, offset, axis;            
            Vector3 dir = center1 - center2;

            if( dir.magnitude == 0 )
            {
                Gizmos.DrawWireSphere(center1, m_dualSphereWorldData.radius1);
                Gizmos.DrawWireSphere(center2, m_dualSphereWorldData.radius2);
                return;
            }


            /*
            offset = Vector3.Cross(dir, Camera.current.transform.right);

            p1 = center1 + offset.normalized * m_dualSphereData.radius1;
            p2 = center2 + offset.normalized * m_dualSphereData.radius2;
            Gizmos.DrawLine(p1, p2);

            p1 = center1 - offset.normalized * m_dualSphereData.radius1;
            p2 = center2 - offset.normalized * m_dualSphereData.radius2;
            Gizmos.DrawLine(p1, p2);
            */
            offset = Vector3.Cross(dir, Camera.current.transform.forward);

            p1 = center1 + offset.normalized * m_dualSphereWorldData.radius1;
            p2 = center2 + offset.normalized * m_dualSphereWorldData.radius2;
            Gizmos.DrawLine(p1, p2);

            p1 = center1 - offset.normalized * m_dualSphereWorldData.radius1;
            p2 = center2 - offset.normalized * m_dualSphereWorldData.radius2;
            Gizmos.DrawLine(p1, p2);

            int circleEdgeCount = 24;

            GizmosDrawCircle(dir, center1, m_dualSphereWorldData.radius1, circleEdgeCount, false);
            GizmosDrawCircle(dir, center2, m_dualSphereWorldData.radius2, circleEdgeCount, false);

            axis = Vector3.Cross(dir, Camera.current.transform.forward);
            GizmosDrawCircle(axis, center1, m_dualSphereWorldData.radius1, circleEdgeCount, false);
            GizmosDrawCircle(Vector3.Cross(dir, axis), center1, m_dualSphereWorldData.radius1, 64, false);


            axis = Vector3.Cross(dir, Camera.current.transform.forward);
            GizmosDrawCircle(axis, center2, m_dualSphereWorldData.radius2, circleEdgeCount, false);
            GizmosDrawCircle(Vector3.Cross(dir, axis), center2, m_dualSphereWorldData.radius2, 64, false);

        }
        

    }





    

}