using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [System.Serializable]
        public class TriangleLock
        {

            //temporary data : need apply or lock for update
            [System.Serializable]
            public class ComputedData
            {
                public Vector3[] vPos = new Vector3[3];//vertices positions
                public float vectorProjLength = 1f;
                public Vector3 localNormalFace;
                public Vector3 localBCenter;
                public Vector3 upFaceReference;
                public float facePerimeter;

                public Vector3 worldNormalFace {get { return m_lastTransformTarget != null ? m_lastTransformTarget.TransformDirection(localNormalFace).normalized : Vector3.zero; }}
                public Vector3 worldPosition {get{return m_lastTransformTarget != null ? m_lastTransformTarget.TransformPoint(localPosition) : Vector3.zero;}}
                public Vector3 worldDirection { get { return m_lastTransformTarget != null ? m_lastTransformTarget.TransformDirection(localDirection).normalized : Vector3.zero; } }
                public Vector3 worldUp { get { return m_lastTransformTarget != null ? m_lastTransformTarget.TransformDirection(localUp) : Vector3.zero; } }
                          
                
                public Quaternion worldRotation
                {
                    get
                    {
                        if (worldDirection.sqrMagnitude > 0)
                        {

                            if (worldDirection != worldUp && worldUp.sqrMagnitude > 0)
                                return Quaternion.LookRotation(worldDirection, worldUp);
                            else
                                return Quaternion.LookRotation(worldDirection);
                        }
                        else
                        {
                            return Quaternion.LookRotation(worldNormalFace);
                        }
                    }
                }


                public Quaternion localRotation
                {
                    get
                    {

                        if (localDirection.sqrMagnitude > 0)
                        {

                            if (localDirection != localUp && localUp.sqrMagnitude > 0)
                                return Quaternion.LookRotation(localDirection, localUp);
                            else
                                return Quaternion.LookRotation(localDirection);
                        }
                        else
                        {
                            if (localNormalFace.sqrMagnitude == 0)
                                return Quaternion.identity;
                            return Quaternion.LookRotation(localNormalFace);
                        }
                    }
                }

                public float perimeter = -1f;
                public Vector3 localPosition;
                public Vector3 localDirection;
                public Vector3 localUp;
                //public Quaternion localRotation = Quaternion.identity;
                public Transform m_lastTransformTarget = null;
                public Transform m_lastTransformMesh = null;


            }


            [System.Serializable]
            public class TriangleData
            {
                public float[] weights = { 1f / 3f, 1f / 3f, 1f / 3f };//barycentric position in the triangle
                public float normalOffset = 0f;//offset position on normal

                public TriangleData Clone()
                {
                    TriangleData td = new TriangleData();
                    for (int i = 0; i < weights.Length; ++i)
                        td.weights[i] = weights[i];
                    td.normalOffset = normalOffset;
                    return td;
                }
            }
            public int m_faceId = -1;
            public float m_facePerimeterLocked = 1f;
            public TriangleData m_position = new TriangleData();
            public TriangleData m_direction = new TriangleData();
            public TriangleData m_up = new TriangleData();
            public ComputedData m_cdata = new ComputedData();
            public float scale { get { return m_cdata.facePerimeter / m_facePerimeterLocked; } }

            public TriangleLock Clone()
            {
                TriangleLock t = new TriangleLock();
                t.m_faceId = m_faceId;
                t.m_position = m_position.Clone();
                t.m_direction = m_direction.Clone();
                t.m_up = m_up.Clone();
                return t;
            }


            void ComputeWeights( Vector3 localPos, ref TriangleData tdata, bool isDirection, bool debug = false )
            {
                if(isDirection)
                    localPos = localPos.normalized* m_cdata.vectorProjLength + m_cdata.localBCenter;

                tdata.normalOffset = Vector3.Dot(localPos - m_cdata.localBCenter, m_cdata.localNormalFace);

                Vector3 vProj = localPos - tdata.normalOffset * m_cdata.localNormalFace;
                //Vector3 offsetBarycenter = vProj - m_cdata.localBCenter;
                
                if (debug)
                {
                    Debug.DrawLine(m_cdata.m_lastTransformTarget.TransformPoint(localPos), m_cdata.m_lastTransformTarget.TransformPoint(localPos+m_cdata.localNormalFace), Color.red, 2f);
                    Debug.DrawLine(m_cdata.m_lastTransformTarget.TransformPoint(vProj), m_cdata.m_lastTransformTarget.TransformPoint(localPos), Color.blue, 2f);
                }

                for (int i = 0; i < 3; ++i)
                {
                    int vv1 = (i + 1) % 3;
                    int vv2 = (i + 2) % 3;
                    float aV0V1V2 = Vector3.Angle(m_cdata.vPos[i] - m_cdata.vPos[vv1], m_cdata.vPos[vv2] - m_cdata.vPos[vv1]);
                    float aV1PMaxV0 = 180f - aV0V1V2 - Vector3.Angle(vProj - m_cdata.vPos[i], m_cdata.vPos[vv1] - m_cdata.vPos[i]);
                    float maxLength = Mathf.Sin(Mathf.Deg2Rad * aV0V1V2) * (m_cdata.vPos[i] - m_cdata.vPos[vv1]).magnitude / Mathf.Sin(Mathf.Deg2Rad * aV1PMaxV0);

                    tdata.weights[i] = 1f - ((vProj - m_cdata.vPos[i]).magnitude / maxLength);
                    /*
                    if ( !isDirection && (tdata.weights[i] < 0f || tdata.weights[i] > 1f))
                        Debug.Log("weights out of triangle " + i + " : " + tdata.weights[i]);
                        */
                }



            }



            /*
            public void ComputeFaceData( Transform transformMesh)
            {
                //update temporary
                m_cdata.worldPosition = transformMesh.TransformPoint(m_cdata.localPosition);
                m_cdata.worldDir = transformMesh.TransformDirection(m_cdata.localDirection);
                m_cdata.worldUp = transformMesh.TransformDirection(m_cdata.localUp);

                m_cdata.worldNormalFace = transformMesh.TransformDirection(m_cdata.localNormalFace);

                if (m_cdata.worldDir.sqrMagnitude > 0)
                {

                    if (m_cdata.worldDir != m_cdata.worldUp && m_cdata.worldUp.sqrMagnitude > 0)
                        m_cdata.worldRotation = Quaternion.LookRotation(m_cdata.worldDir, m_cdata.worldUp);
                    else
                        m_cdata.worldRotation = Quaternion.LookRotation(m_cdata.worldDir);
                }
                else
                {
                    m_cdata.worldRotation = Quaternion.LookRotation(m_cdata.worldNormalFace);
                }

                if (m_cdata.localDirection.sqrMagnitude > 0)
                {
                    if (m_cdata.localDirection != m_cdata.localUp && m_cdata.localUp.sqrMagnitude > 0)
                        m_cdata.localRotation = Quaternion.LookRotation(m_cdata.localDirection, m_cdata.localUp);
                    else
                        m_cdata.localRotation = Quaternion.LookRotation(m_cdata.localDirection);
                }
                else
                {
                    m_cdata.localRotation = Quaternion.LookRotation(m_cdata.localNormalFace);
                }

                m_cdata.facePerimeter = (m_cdata.vPos[0] - m_cdata.vPos[1]).magnitude + (m_cdata.vPos[0] - m_cdata.vPos[2]).magnitude + (m_cdata.vPos[2] - m_cdata.vPos[1]).magnitude;
            }
            */


/*
            public int Lock(Transform transformObject, Transform transformMesh, int faceId, Vector3[] vertices, int[] triangles, bool worldCoord )
            {
                Vector3 pos = transformObject.position;
                Quaternion rot = transformObject.rotation;
                return Lock(pos, transformObject.forward, transformObject.up, transformMesh, transformMesh, faceId, vertices, triangles, worldCoord);
            }

*/

            public void InitFaceData(Vector3[] vertices, int[] triangles, Transform transMesh, Transform transTarget)
            {
                if(vertices==null || triangles==null)
                {
                    //Debug.LogError("TriangleLock cannot init face data");
                    return;
                }

                for (int i = 0; i < m_cdata.vPos.Length; ++i)
                {
                    m_cdata.vPos[i] = vertices[triangles[m_faceId * 3 + i]];
                    m_cdata.vPos[i] = transTarget.InverseTransformPoint( transMesh.TransformPoint(m_cdata.vPos[i]) );
                }

                m_cdata.localNormalFace = Vector3.Cross((m_cdata.vPos[1] - m_cdata.vPos[0]).normalized, (m_cdata.vPos[2] - m_cdata.vPos[0]).normalized.normalized).normalized;
                
                    
                m_cdata.localBCenter = (m_cdata.vPos[0] + m_cdata.vPos[1] + m_cdata.vPos[2]) / 3f;

                m_cdata.vectorProjLength = 1f;
                for (int i = 0; i < m_cdata.vPos.Length; ++i)
                    if ((m_cdata.vPos[i] - m_cdata.localBCenter).magnitude/2f < m_cdata.vectorProjLength)
                        m_cdata.vectorProjLength = (m_cdata.vPos[i] - m_cdata.localBCenter).magnitude / 2f;

                m_cdata.facePerimeter = (m_cdata.vPos[0] - m_cdata.vPos[1]).magnitude + (m_cdata.vPos[0] - m_cdata.vPos[2]).magnitude + (m_cdata.vPos[2] - m_cdata.vPos[1]).magnitude;
            }



            public int Lock( Vector3 position, Vector3 direction, Vector3 up, Transform transformMesh, Transform transformTarget, int faceId, Vector3[] vertices, int[] triangles, bool worldCoord )
            {                

                if (transformMesh == null || vertices == null || triangles == null)
                    return -1;

                if (triangles.Length < 3)
                    return -1;

                if (faceId < 0)
                    faceId = 0;
                if (faceId >= triangles.Length / 3)
                    faceId = (triangles.Length - 1) / 3;

                m_faceId = faceId;

                if (transformMesh == null)
                {
                    m_faceId = -1;
                    return m_faceId;
                }

                m_cdata.m_lastTransformTarget = transformTarget;
                m_cdata.m_lastTransformMesh = transformMesh;

                InitFaceData(vertices, triangles, transformMesh, transformTarget);

                if (worldCoord)
                {
                    m_cdata.localPosition = transformTarget.InverseTransformPoint(position);
                    m_cdata.localDirection = transformTarget.InverseTransformDirection(direction).normalized;
                    m_cdata.localUp = transformTarget.InverseTransformDirection(up).normalized;
                }
                else
                {
                    m_cdata.localPosition = position;
                    m_cdata.localDirection = direction.normalized;
                    m_cdata.localUp = up.normalized;
                }
                
                ComputeWeights(  m_cdata.localPosition, ref m_position, false);
                ComputeWeights(  m_cdata.localDirection, ref m_direction, true );
                ComputeWeights(  m_cdata.localUp.normalized, ref m_up, true);

                m_facePerimeterLocked = m_cdata.facePerimeter;
                //m_facePerimeterLocked = (m_cdata.vPos[0] - m_cdata.vPos[1]).magnitude + (m_cdata.vPos[0] - m_cdata.vPos[2]).magnitude + (m_cdata.vPos[2] - m_cdata.vPos[1]).magnitude;

                return faceId;
            }

           

            public void UpdateWorldDirection( Vector3 worldDirection)
            {
                
                m_cdata.localDirection = m_cdata.m_lastTransformTarget.InverseTransformDirection(worldDirection).normalized;                
                ComputeWeights(m_cdata.localDirection, ref m_direction, true);
                
            }

            /*
            public void UpdateWorldDirection(Vector3 localDirection, Vector3 localUp)
            {
                localUp = Vector3.Cross(Vector3.Cross(localUp, localDirection), localDirection);
                float vectorLength = 1f;
                for (int i = 0; i < m_cdata.vPos.Length; ++i)
                    if ((m_cdata.vPos[i] - m_cdata.localBCenter).magnitude < vectorLength / 2f)
                        vectorLength = (m_cdata.vPos[i] - m_cdata.localBCenter).magnitude / 2f;

                ComputeWeights(localDirection.normalized * vectorLength + m_cdata.localBCenter, ref m_direction);
            }*/

                /*
            public void Apply(Transform transformObject, Transform transformMesh, Vector3[] vertices, int[] triangles)
            {

                Apply( transformMesh, vertices, triangles, true);
                transformObject.position = m_cdata.worldPosition;
                transformObject.rotation = m_cdata.worldRotation;

            }
            */




            public void Apply(Transform transTarget, Transform transMesh, Vector3[] vertices, int[] triangles, bool force)
            {
                               

                if (m_faceId == -1)
                    return;
                
                //needed for long hair
                if (m_cdata.m_lastTransformTarget == transMesh && !force )
                    return;
                
                m_cdata.m_lastTransformTarget = transTarget;

                InitFaceData(vertices, triangles, transMesh, transTarget);

                m_cdata.localPosition = Vector3.zero;
                for (int i = 0; i < m_cdata.vPos.Length; ++i)
                    m_cdata.localPosition += m_cdata.vPos[i] * m_position.weights[i];
                m_cdata.localPosition += m_position.normalOffset * m_cdata.localNormalFace;

                m_cdata.localDirection = Vector3.zero;
                for (int i = 0; i < m_cdata.vPos.Length; ++i)
                    m_cdata.localDirection += m_cdata.vPos[i] * m_direction.weights[i];
                m_cdata.localDirection -= m_cdata.localBCenter;
                m_cdata.localDirection += m_direction.normalOffset * m_cdata.localNormalFace;
                m_cdata.localDirection.Normalize();

                m_cdata.localUp = Vector3.zero;
                for (int i = 0; i < m_cdata.vPos.Length; ++i)
                    m_cdata.localUp += m_cdata.vPos[i] * m_up.weights[i];
                m_cdata.localUp -= m_cdata.localBCenter;
                m_cdata.localUp += m_up.normalOffset * m_cdata.localNormalFace;
                if (m_cdata.localUp == Vector3.zero)
                    m_cdata.localUp = m_cdata.localNormalFace;
                m_cdata.localUp.Normalize();

                //ComputeFaceData(transformMesh);


            
            }
        }
    }
}