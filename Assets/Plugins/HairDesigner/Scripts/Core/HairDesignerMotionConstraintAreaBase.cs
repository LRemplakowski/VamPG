using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Kalagaan.HairDesignerExtension
{
    public class HairDesignerMotionConstraintAreaBase : MonoBehaviour
    {
        [System.Serializable]
        public class Area
        {
            public Vector3 position;
            public float radius;
        }

        

        public MotionSolver.eMotionConstraint m_type;
        //public float m_radius = 1f;
        public Action<HairDesignerMotionConstraintAreaBase> OnConstraintChanged = null;

        private MotionSolver.eMotionConstraint m_internalType = MotionSolver.eMotionConstraint.LOCK_TO_ORIGIN;


        public List<Area> m_areas = new List<Area>();

        [HideInInspector]
        public List<MotionSolver.Node> m_nodes = new List<MotionSolver.Node>();







        public void Awake()
        {
            m_internalType = m_type;
        }



        public void Update()
        {
            if (m_internalType != m_type)
            {
                ApplyConstraintLink();

                if (OnConstraintChanged != null)
                    OnConstraintChanged(this);
                m_internalType = m_type;
            }
        }


        public MotionSolver.eMotionConstraint GetConstraint( Vector3 position )
        {
            MotionSolver.eMotionConstraint cstr = MotionSolver.eMotionConstraint.NONE;

            if(m_type == MotionSolver.eMotionConstraint.NONE)
                return m_type;

            if (CheckConstraint(position))
            {
                cstr = m_type;
            }
            
            return cstr;
        }




        public void ApplyConstraintLink()
        {
            for (int i = 0; i < m_nodes.Count; ++i)
            {
                if(m_nodes[i] == null)
                {
                    m_nodes.RemoveAt(i--);
                    continue;
                }

                m_nodes[i].constraint = m_type;
            }            
        }






        public bool CheckConstraint( Vector3 position )
        {
           
            //Check spheres
            for (int i = 0; i < m_areas.Count; ++i)
            {
                if (Vector3.Distance(position, transform.TransformPoint(m_areas[i].position)) <= m_areas[i].radius * transform.lossyScale.x)
                {
                    return true;
                }
            }


            //check cylinders
            for (int i = 1; i < m_areas.Count; ++i)
            {

                Vector3 w1 = transform.TransformPoint(m_areas[i - 1].position);
                Vector3 w2 = transform.TransformPoint(m_areas[i].position);
                Vector3 axis = (w1 - w2);
                float axisMag = axis.magnitude;
                axis.Normalize();

                float dot = Vector3.Dot(position - w2, axis);

                if (dot < 0 || dot > axisMag)
                    continue;

                Vector3 proj = dot * axis + w2;
                if( Vector3.Distance(position,proj) <= Mathf.Lerp(m_areas[i - 1].radius, m_areas[i].radius,dot/ axisMag) )
                {
                    return true;
                }

            }

            return false;
        }











        public void RegisterConstraintLink(MotionSolver.Node node)
        {
            if (node == null)
                return;

            if (CheckConstraint(node.position))
            {
                if (!m_nodes.Contains(node))
                    m_nodes.Add(node);
                node.constraint = m_type;
            }            
        }



        /*
        public void OnDrawGizmos()
        {
            if (m_type == MotionSolver.eMotionConstraint.NONE)
                return;

            Color c = Color.red;
            c.a = .4f;
            Gizmos.color = c;

            for (int i = 0; i < m_areas.Count; ++i)
                Gizmos.DrawWireSphere(transform.position, m_areas[i].radius);
        }
        */

    }
}
