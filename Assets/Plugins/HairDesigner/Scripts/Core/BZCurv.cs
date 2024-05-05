using UnityEngine;
using System.Collections.Generic;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        /// <summary>
        /// Curve class
        /// </summary>
        [System.Serializable]
        public class BZCurv : System.Object
        {
            public Vector3 m_startPosition;
            public Vector3 m_endPosition;
            public Vector3 m_startTangent;
            public Vector3 m_endTangent;
            public float m_startAngle;
            public float m_endAngle;
            public float m_length = 0;
            public Vector3 m_upRef = Vector3.up;
            public bool legacy_1_5_1 = false;

            Vector3 a;
            Vector3 b;
            Vector3 c;

            [System.Serializable]
            public class Result
            {
                public Vector3 position;
                public Vector3 tangent;
                public float curvPos;
            }


            public BZCurv()
            {

            }

            public BZCurv(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
            {
                m_startPosition = startPosition;
                m_startTangent = startTangent;
                m_endTangent = endTangent;
                m_endPosition = endPosition;
            }


            public BZCurv(BZCurv original)
            {
                m_startPosition = original.m_startPosition;
                m_startTangent = original.m_startTangent;
                m_endTangent = original.m_endTangent;
                m_endPosition = original.m_endPosition;
                m_startAngle = original.m_startAngle;
                m_endAngle = original.m_endAngle;
            }

            public Vector3 GetPosition(float t)
            {
                t = Mathf.Clamp01(t);

                Vector3 A = m_startPosition;
                Vector3 B = m_startPosition + m_startTangent;
                Vector3 C = m_endPosition + m_endTangent;
                Vector3 D = m_endPosition;

                float omt = 1f - t;
                Vector3 Q = omt * A + t * B;
                Vector3 R = omt * B + t * C;
                Vector3 S = omt * C + t * D;

                Vector3 P = omt * Q + t * R;
                Vector3 T = omt * R + t * S;

                Vector3 U = omt * P + t * T;

                return U;

                /*
                c.x = 3f * ((m_startPosition.x + m_startTangent.x) - m_startPosition.x);
                b.x = 3f * ((m_endPosition.x + m_endTangent.x) - (m_startPosition.x + m_startTangent.x)) - c.x;
                a.x = m_endPosition.x - m_startPosition.x - c.x - b.x;

                c.y = 3f * ((m_startPosition.y + m_startTangent.y) - m_startPosition.y);
                b.y = 3f * ((m_endPosition.y + m_endTangent.y) - (m_startPosition.y + m_startTangent.y)) - c.y;
                a.y = m_endPosition.y - m_startPosition.y - c.y - b.y;

                c.z = 3f * ((m_startPosition.z + m_startTangent.z) - m_startPosition.z);
                b.z = 3f * ((m_endPosition.z + m_endTangent.z) - (m_startPosition.z + m_startTangent.z)) - c.z;
                a.z = m_endPosition.z - m_startPosition.z - c.z - b.z;


                float t2 = t * t;
                float t3 = t * t * t;
                float x = a.x * t3 + b.x * t2 + c.x * t + m_startPosition.x;
                float y = a.y * t3 + b.y * t2 + c.y * t + m_startPosition.y;
                float z = a.z * t3 + b.z * t2 + c.z * t + m_startPosition.z;
                return new Vector3(x, y, z);
                */
            }

            public Vector3 GetTangent(float t)
            {
                t = Mathf.Clamp01(t);

                c.x = 3f * ((m_startPosition.x + m_startTangent.x) - m_startPosition.x);
                b.x = 3f * ((m_endPosition.x + m_endTangent.x) - (m_startPosition.x + m_startTangent.x)) - c.x;
                a.x = m_endPosition.x - m_startPosition.x - c.x - b.x;

                c.y = 3f * ((m_startPosition.y + m_startTangent.y) - m_startPosition.y);
                b.y = 3f * ((m_endPosition.y + m_endTangent.y) - (m_startPosition.y + m_startTangent.y)) - c.y;
                a.y = m_endPosition.y - m_startPosition.y - c.y - b.y;

                c.z = 3f * ((m_startPosition.z + m_startTangent.z) - m_startPosition.z);
                b.z = 3f * ((m_endPosition.z + m_endTangent.z) - (m_startPosition.z + m_startTangent.z)) - c.z;
                a.z = m_endPosition.z - m_startPosition.z - c.z - b.z;

                float t2 = t * t;
                float x = 3f * a.x * t2 + 2f * b.x * t + c.x;
                float y = 3f * a.y * t2 + 2f * b.y * t + c.y;
                float z = 3f * a.z * t2 + 2f * b.z * t + c.z;
                return new Vector3(x, y, z).normalized;
            }


            public float GetLength(int stepCount)
            {
                float length = 0;
                for (int i = 0; i < stepCount; ++i)
                {
                    length += (GetPosition((float)i / (float)stepCount) - GetPosition((float)(i + 1) / (float)stepCount)).magnitude;
                }
                return length;
            }



            //public Vector3 GetUp(float t, Vector3 upRef )
            public Vector3 GetUp(float t, Vector3 startUpRef, bool useAngle)
            {
                
                {
                    //float delta = .0001f;
                    Vector3 tan = GetTangent(t);
                    //Vector3 tan2 = t<1f ? GetTangent(t+ delta) : GetTangent(t- delta);

                    Vector3 dir = ((m_endPosition) - (m_startPosition)).normalized;
                    Vector3 right;

                    Vector3 up = m_upRef;
                    Vector3 newUp = up;

                    if (startUpRef.sqrMagnitude > 0)
                    {
                        Vector3 tan0 = GetTangent(0);
                        Vector3 tan1 = GetTangent(1);


                        //float sign = 1f;

                        /*
                        if (Vector3.Dot(dir, startUpRef.normalized) > 0)
                        {
                            sign *= -1f;
                        } */

                        //Vector3 r2 = Vector3.Cross(tan0, startUpRef);

                        Vector3 endUp = Vector3.Cross( tan1, Vector3.Cross(startUpRef, dir));
                        //Vector3 endUp = Vector3.Cross(tan1, Vector3.Cross(m_upRef, dir));
                        //endUp = startUpRef;
                        //endUp = m_upRef;


                        Vector3 r1 = Vector3.Cross(dir, startUpRef).normalized;
                        Vector3 r2 = Vector3.Cross(dir, endUp).normalized;
                        if (Vector3.Dot(r1, r2) < 0)
                            endUp *= -1f;


                        if (Vector3.Dot(dir, tan0) < 0)
                        {
                            /*
                            //endUp *= -1f;
                            
                            if (Vector3.Dot(tan0, tan1) > 0)
                            {
                                //endUp *= -1f;
                                
                                r1 = Vector3.Cross(tan0, startUpRef);
                                r2 = Vector3.Cross(tan1, endUp);
                                if (Vector3.Dot(r1, r2) > 0)
                                    endUp *= -1f;
                                    
                            }
                            
                        */

                        }
                        else
                        {
                            /*
                            r1 = Vector3.Cross(tan0, startUpRef);
                            r2 = Vector3.Cross(tan1, endUp);
                            if (Vector3.Dot(r1, r2) < 0)
                                endUp *= -1f;

                            */
                        }



                        
                        
                        if (Vector3.Dot(tan0, tan1) > 0)
                        {
                            //endUp *= -1f;
                            r1 = Vector3.Cross(tan0, startUpRef);
                            r2 = Vector3.Cross(tan1, endUp);
                            if (Vector3.Dot(r1, r2) < 0)
                                endUp *= -1f;

                        }
                        else
                        {
                            r1 = Vector3.Cross(tan0, startUpRef);
                            r2 = Vector3.Cross(tan1, endUp);
                            if (Vector3.Dot(r1, r2) > 0)
                                endUp *= -1f;
                        }

                        /*
                        {

                            r1 = Vector3.Cross(tan0, dir);
                            r2 = Vector3.Cross(tan1, dir);
                            if (Vector3.Dot(r1, r2) > 0)
                                endUp *= -1f;
                        }
                        */
                        /*
                        if (Vector3.Dot(dir, tan1) < 0)
                            endUp *= -1f;
                          */

                        /*
                        if (Vector3.Dot(dir, tan0) < 0)
                            endUp *= -1f;
                            */


                        endUp = startUpRef;

                        up = Vector3.Lerp(startUpRef, endUp, t ).normalized;

                        /*
                        r1 = Vector3.Cross(tan0, startUpRef);
                        r2 = Vector3.Cross(tan, up);

                        if (Vector3.Dot(dir, tan0) < 0)
                            r2 *= -1f;

                        if (Vector3.Dot(r1, r2) < 0)
                            up *= -1f;
                            */

                        /*
                        if (Vector3.Dot(dir, tan0) < 0)
                            up *= -1;
                            */


                        /*
                        sign *= -1f;

                        //up/down
                        if (Vector3.Dot(Vector3.Cross(startUpRef, dir), Vector3.Cross(startUpRef, tan0)) < 0)
                        {
                            sign *= -1f;

                            if (Vector3.Dot(dir, tan0) >0)
                                sign *= -1f;

                        }
                        else
                        {
                            if (Vector3.Dot(dir, tan0) >0)
                                sign *= -1f;
                        }
                        */


                        //right = Vector3.Cross(dir, up);

                        right = Vector3.Cross(Vector3.Lerp(tan0, dir, t), up);//smooth from start angle
                        //right = Vector3.Cross(Vector3.Lerp(tan0, tan1, t), up);//smooth from start angle
                        //right = Vector3.Cross(tan, up);


                        /*
                        if (Vector3.Dot(Vector3.Cross(right, tan0), startUpRef) > 0)
                            right *= -1;
                        */
                        /*
                        if (Vector3.Dot(r1, r2) < 0)
                            right *= -1f;
                            */
                        newUp = Vector3.Cross(right, tan.normalized).normalized;
                        

                        /*
                        r1 = Vector3.Cross(dir, startUpRef);
                        r2 = Vector3.Cross(dir, newUp);
                        if (Vector3.Dot(r1, r2) < 0)
                            newUp *= -1f;
                            */

                        /*
                        if (Vector3.Dot(startUpRef, endUp) < 0)
                            newUp *= -1f;
                            */
                        /*
                        if (Vector3.Dot(Vector3.Cross(tan0, startUpRef), Vector3.Cross(dir, startUpRef)) < 0)
                            newUp *= -1f;
                        */

                        /*
                        if (Vector3.Dot(Vector3.Cross(tan0, startUpRef), Vector3.Cross(dir, startUpRef)) < 0)
                            newUp *= -1f;
                        */
                    }
                    else
                    {
                        //First curve

                        Vector3 tan0 = GetTangent(0);
                        //Vector3 tan1 = GetTangent(1);

                        
                        if (Vector3.Dot(Vector3.Cross(m_upRef.normalized, dir), Vector3.Cross(m_upRef.normalized, tan0)) < 0)
                            up *= -1f;

                        //Vector3 endUp = m_upRef;


                        //dir = tan1;//test

                        Vector3 r1 = Vector3.Cross(dir, m_upRef);
                        Vector3 r2 = Vector3.Cross(tan0, m_upRef);

                        if (Vector3.Dot(r1, r2) < 0)
                            dir *= -1f;

                        right = Vector3.Cross( Vector3.Lerp(tan0,dir, t)  , up);

                        
                        if (Vector3.Dot(r1, r2) < 0)
                            right *= -1f;

                        /*
                        if(Vector3.Dot(tan0, tan1) < 0)
                            right *= -1f;
                            */
                        

                        newUp = Vector3.Cross(right, tan.normalized).normalized;

                        if (Vector3.Dot(dir, tan0) < 0)
                            newUp *= -1f;
                    }


                    //Vector3 newUp = Vector3.Cross(right,tan.normalized).normalized;

                    //test
                    /*
                    Vector3 tr1 = Vector3.Cross(GetTangent(0), m_upRef);
                    Vector3 tr2 = Vector3.Cross(GetTangent(1), m_upRef);
                    newUp = Vector3.Cross(Vector3.Lerp(tr1,tr2,t), tan.normalized).normalized;
                    */
                    //newUp = m_upRef;

                    //Vector3 tan = Vector3.Lerp(GetTangent(0), GetTangent(1), t);
                    //Vector3 tan2 = Vector3.Lerp(GetTangent(0), GetTangent(1), t+.00001f);

                    //Vector3 right2 = Vector3.Cross(tan, m_upRef);

                    //Vector3 right = Vector3.Cross(m_startPosition-GetPosition(t),m_endPosition - GetPosition(t));


                    //Vector3 upRef = Vector3.Cross(right2, GetTangent(t));
                    //Vector3 right = Vector3.Cross(m_upRef.normalized, tan0) * Mathf.Sign(Vector3.Dot(Vector3.Cross(up0, tan0), Vector3.Cross(m_upRef.normalized, tan0)));
                    //right = Vector3.Cross(m_upRef.normalized, (m_endPosition - m_startPosition).normalized);

                    //Vector3 newUp = -Vector3.Cross(right, GetTangent(t)).normalized;
                    //newUp *= Mathf.Sign(Vector3.Dot(right2.normalized, right.normalized));


                    if (!useAngle)
                        return newUp;

                    Quaternion q = Quaternion.AngleAxis(Mathf.Lerp(m_startAngle, m_endAngle, t), tan);

                    return q*newUp;
                }









				/*

                Vector3 tan = GetTangent(t).normalized;

                Vector3 result2 = Vector3.zero;
                result2 = Vector3.Lerp(up0, up1, t);//smooth result

                Vector3 right = Vector3.Cross(tan, Vector3.Lerp(up0, up1, t)).normalized;
                //result2 = m_upRef;

                result2 = Vector3.Cross(right, tan);

                return result2;


                //up0 = Quaternion.FromToRotation(tan0, tan) * up0;
                //up1 = Quaternion.FromToRotation(tan1, tan) * up1;
                
                
                //tan = Vector3.Lerp(tan0, tan1, t);

                Vector3 r = Vector3.Cross(tan, m_upRef.normalized).normalized;
                if (t > 0f || t < 1f)
                {
                    r = Vector3.Cross(tan, Vector3.Lerp(up0, up1, t)).normalized;
                    //r = Vector3.Cross(GetTangent(Mathf.Clamp01(t - 0.01f)), tan).normalized;
                }

                Vector3 up = Vector3.Cross(r, tan);

                Quaternion q = Quaternion.AngleAxis(Mathf.Lerp(m_startAngle, m_endAngle,t), tan);
                
                Vector3 result = q*Vector3.Normalize(up);

                result += Vector3.Lerp( up0 , up1, t);//smooth result

                return result.normalized;

				*/
            }



            public float InitLength(int stepCount)
            {
                m_length = GetLength(stepCount);
                return m_length;
            }


            public Result GetLinearData(float t, int stepCount)
            {
                Result r = new Result();
                float target = m_length * t;
                float tmp = 0;
                //float stepSize = m_length / (float)stepCount;
                for (int i = 0; i < stepCount; ++i)
                {
                    float delta = (GetPosition((float)i / (float)stepCount) - GetPosition((float)(i + 1) / (float)stepCount)).magnitude;
                    if (target > tmp && target < tmp + delta)
                    {
                        r.position = (GetPosition((float)i / (float)stepCount) + GetPosition((float)(i + 1) / (float)stepCount)) / 2f;
                        r.tangent = (GetTangent((float)i / (float)stepCount) + GetTangent((float)(i + 1) / (float)stepCount)) / 2f;
                        r.curvPos = (float)i + .5f;
                        return r;
                    }

                    tmp += delta;
                }
                r.position = GetPosition(1);
                r.tangent = GetTangent(1);
                r.curvPos = 1;
                return r;
            }


            public BZCurv Copy()
            {
                BZCurv c = new BZCurv();
                c.m_startAngle = m_startAngle;                
                c.m_startPosition = m_startPosition;
                c.m_startTangent = m_startTangent;

                c.m_endAngle = m_endAngle;
                c.m_endPosition = m_endPosition;
                c.m_endTangent = m_endTangent;

                c.m_upRef = m_upRef;


                return c;
            }
                       

        }




        /// <summary>
        /// Multiple curve class
        /// </summary>
        [System.Serializable]
        public class MBZCurv : System.Object
        {
            public BZCurv[] m_curves;
            public float m_startAngle;
            public float m_endAngle;
            public Vector3 m_offset = Vector3.zero;
            public Quaternion m_rotation = Quaternion.identity;

            public Vector3 startPosition
            {
                get { return m_curves[0].m_startPosition + m_offset; }
                set { m_curves[0].m_startPosition = value - m_offset; }
            }


            public Vector3 endPosition
            {
                get { return m_rotation*m_curves[m_curves.Length-1].m_endPosition + m_offset; }
                set { m_curves[m_curves.Length - 1].m_endPosition = Quaternion.Inverse(m_rotation)*(value - m_offset); }
            }

            public Vector3 startTangent
            {
                get { return m_rotation*m_curves[0].m_startTangent; }
                set { m_curves[0].m_startTangent = Quaternion.Inverse(m_rotation) * value; }
            }


            public Vector3 endTangent
            {
                get { return m_rotation * m_curves[m_curves.Length - 1].m_endTangent; }
                set { m_curves[m_curves.Length - 1].m_endTangent = Quaternion.Inverse(m_rotation) * value; }
            }

            public BZCurv GetCurv(int id)
            {
                return m_curves[id];
            }




            public Vector3 GetStartPosition(int id)
            { 
                return m_rotation * m_curves[id].m_startPosition + m_offset;
            }

            public void SetStartPosition(int id, Vector3 pos )
            {
                m_curves[id].m_startPosition = Quaternion.Inverse(m_rotation) * (pos - m_offset);
                if( id>0 )
                    m_curves[id-1].m_endPosition = m_curves[id].m_startPosition;
            }
            


            public Vector3 GetEndPosition(int id)
            {
                return m_rotation * m_curves[id].m_endPosition + m_offset;
            }


            public void SetEndPosition(int id, Vector3 pos)
            { 
                m_curves[id].m_endPosition = Quaternion.Inverse(m_rotation) * (pos - m_offset);
                if (id < m_curves.Length-1)
                    m_curves[id + 1].m_startPosition = m_curves[id].m_endPosition;

            }


            public Vector3 GetStartTangent(int id)
            {
                return m_rotation * m_curves[id].m_startTangent;
            }

            public void SetStartTangent(int id, Vector3 tangent)
            { 
                m_curves[id].m_startTangent = Quaternion.Inverse(m_rotation) * tangent;
            }


            public Vector3 GetEndTangent(int id)
            {
                return m_rotation * m_curves[id].m_endTangent;
            }

            public void SetEndTangent(int id, Vector3 tangent)
            {
                m_curves[id].m_endTangent = Quaternion.Inverse(m_rotation) * tangent;
            }


            public void SetStartAngle(int id, float angle)
            {
                m_curves[id].m_startAngle = angle;
                if(id == 0)
                    m_startAngle = angle;
            }

            public float GetStartAngle(int id)
            {
                return m_curves[id].m_startAngle;                
            }


            public void SetEndAngle(int id, float angle)
            {
                m_curves[id].m_endAngle = angle;
                if (id == (m_curves.Length-1))
                    m_endAngle = angle;
            }

            public float GetEndAngle(int id)
            {
                return m_curves[id].m_endAngle;
            }


            public MBZCurv()
            {
                m_curves = new BZCurv[1];
                m_curves[0] = new BZCurv();
            }


            public MBZCurv(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
            {
                startPosition -= m_offset;
                endPosition = Quaternion.Inverse(m_rotation)*( endPosition - m_offset );
                startTangent = Quaternion.Inverse(m_rotation) * startTangent;
                endTangent = Quaternion.Inverse(m_rotation) * endTangent;

                m_curves = new BZCurv[1];
                m_curves[0] = new BZCurv(startPosition, endPosition, startTangent, endTangent);
            }

            public MBZCurv( BZCurv c )
            {
                m_curves = new BZCurv[1];
                m_curves[0] = new BZCurv(c.m_startPosition, c.m_endPosition, c.m_startTangent, c.m_endTangent);
            }

            public Vector3 GetPosition(float t)
            {
                /*
                int id = Mathf.Clamp((int)(t * m_curves.Length),0, m_curves.Length-1);
                float split = 1 / (float) m_curves.Length;
                t = (t - (split * id)) / split;
                */
                int id = GetCurveId(t, out t);
                return GetPosition(id, t);

                //return m_rotation * m_curves[id].GetPosition( t ) + m_offset;
            }


            public Vector3 GetPosition(int subCurv, float t)
            {
                return m_rotation * m_curves[subCurv].GetPosition(t) + m_offset;
            }



            public int GetCurveId(float t, out float tLocal)
            {
                /*
                int id = Mathf.Clamp((int)(t * m_curves.Length), 0, m_curves.Length - 1);
                float split = 1 / (float)m_curves.Length;
                tLocal = (t - (split * id)) / split;

                return id;
                */

                int id = 0;
                tLocal = 0;

                float totalLength = 0;
                for (int i = 0; i < m_curves.Length; ++i)
                {
                    totalLength += m_curves[i].InitLength(5);
                }

                
                float accLength = 0;
                for (int i = 0; i < m_curves.Length; ++i)
                {
                    float prev = accLength;
                    accLength += m_curves[i].m_length;
                    if (t <= (accLength / totalLength))
                    {
                        id = i;
                        tLocal = ((t * totalLength) - prev) / m_curves[i].m_length;
                        return id;
                    }

                }

                //Debug.DrawLine(GetPosition(t), m_rotation * m_curves[id].GetPosition(localT), Color.black, 5f);

                
                return id;
            }




            public Vector3 GetUp(float t)
            {                 
                int id = 0;
                float localT = t;
                id = GetCurveId(t, out localT);
                return GetUp(id, localT);
            }




            public Vector3 GetUp(int subCurv, float t)
            {
                //Quaternion q = Quaternion.AngleAxis(Mathf.Lerp(m_curves[subCurv].m_startAngle, m_curves[subCurv].m_endAngle, t), GetTangent(t));
                /*
                Vector3 tan = GetTangent(t);
                Vector3 tan0 = GetTangent(0);
                Vector3 tan1 = GetTangent(1);
                Quaternion q0 = Quaternion.AngleAxis(m_curves[subCurv].m_startAngle * Vector3.Dot(tan,tan0), tan0);
                Quaternion q1 = Quaternion.AngleAxis(m_curves[subCurv].m_endAngle * Vector3.Dot(tan, tan1), tan1);
                Quaternion q = Quaternion.Lerp(q0,q1,t);
                */
                Vector3 startUpRef = Vector3.zero;

                if (subCurv != 0)
                {                    
                    for (int i = 1; i <= subCurv; ++i)
                    {
                        startUpRef = m_curves[i-1].GetUp(1f, startUpRef, false);//get the up value of the previous curve as ref
                    }
                }

                return ((m_rotation * m_curves[subCurv].GetUp(t, startUpRef, true))).normalized;//disable angle for test

                //return q * ((m_rotation * m_curves[subCurv].GetUp(t, startUpRef) )).normalized;
            }


            public Vector3 GetTangent(float t)
            {
                int id = 0;
                float localT = t;
                id = GetCurveId(t, out localT);
                return (m_rotation * m_curves[id].GetTangent(localT)).normalized;
                /*
                int id = Mathf.Clamp((int)(t * m_curves.Length), 0, m_curves.Length - 1);
                float split = 1f / (float)m_curves.Length;
                t = (t - (split * id)) / split;
                return (m_rotation * m_curves[id].GetTangent(t)).normalized;
                */
            }

            public float GetLength(int stepCount)
            {
                float l = 0;
                for (int i = 0; i < m_curves.Length; ++i)
                    l += m_curves[i].GetLength(stepCount);
                return l;
            }









            public void RemoveControlPoint(int n)
            {
                if (n < 1 || n >= m_curves.Length )
                    return;

                List<BZCurv> curves = new List<BZCurv>();

                for (int i = 0; i < m_curves.Length; ++i)
                {
                    if (i != n)
                    {
                        curves.Add(m_curves[i]);
                    }
                    else
                    {
                        //m_curves[i-1].m_startTangent = m_curves[i-1].m_startTangent * 2f;
                        m_curves[i-1].m_endPosition = m_curves[i].m_endPosition;
                        //m_curves[i-1].m_endTangent = m_curves[i].m_endTangent * 2f;
                        m_curves[i - 1].m_endTangent = m_curves[i].m_endTangent;
                        m_curves[i-1].m_endAngle = m_curves[i].m_endAngle;
                    }
                }

                m_curves = curves.ToArray();
            }







            public void Split( int n )
            {
                if (n >= m_curves.Length)
                    return;

                List<BZCurv> curves = new List<BZCurv>();
                bool isNext = false;

                for( int i=0; i< m_curves.Length; ++i )
                {
                    if (i != n)
                    {
                        if (isNext)
                        {
                            isNext = false;
                            m_curves[i].m_endTangent *= .5f;

                        }
                        curves.Add(m_curves[i]);
                        
                    }
                    else
                    {
                        //Debug.Log("SA=" + m_curves[i].m_startAngle + "  EA=" + m_curves[i].m_endAngle );

                        curves.Add(m_curves[i]);
                        BZCurv newCurv = new BZCurv(m_curves[i]);
                        curves.Add(newCurv);

                        Vector3 A = m_curves[i].m_startPosition;
                        Vector3 B = m_curves[i].m_startPosition + m_curves[i].m_startTangent;
                        Vector3 C = m_curves[i].m_endPosition + m_curves[i].m_endTangent;
                        Vector3 D = m_curves[i].m_endPosition;

                        Vector3 E = (A + B) / 2f;
                        Vector3 F = (B + C) / 2f;
                        Vector3 G = (C + D) / 2f;
                        Vector3 H = (E + F) / 2f;
                        Vector3 J = (F + G) / 2f;
                        Vector3 K = (H + J) / 2f;

                        m_curves[i].m_startPosition = A;
                        m_curves[i].m_startTangent = E - A;
                        m_curves[i].m_endPosition = K;
                        m_curves[i].m_endTangent = H - K;

                        newCurv.m_startPosition = K;
                        newCurv.m_startTangent = J - K;
                        newCurv.m_endPosition = D;
                        newCurv.m_endTangent = G - D;

                        newCurv.m_startAngle = (m_curves[i].m_endAngle + m_curves[i].m_startAngle) * .5f;
                        newCurv.m_endAngle = curves[i].m_endAngle;
                        curves[i].m_endAngle = newCurv.m_startAngle;

                        //Debug.Log("SA=" + newCurv.m_startAngle + "  EA=" + newCurv.m_endAngle);
                        //curves[i].m_startTangent *= .5f;

                        newCurv.m_upRef = m_curves[i].m_upRef;
                        //newCurv.m_startAngle
                        isNext = true;
                    }
                }
                m_curves = curves.ToArray();
            }



            public MBZCurv Copy()
            {
                MBZCurv c = new MBZCurv();

                c.m_rotation = m_rotation;
                c.m_offset = m_offset;
                c.m_curves = new BZCurv[m_curves.Length];
                c.m_startAngle = m_startAngle;
                c.m_endAngle = m_endAngle;
                for ( int i=0; i<m_curves.Length; ++i )
                {
                    c.m_curves[i] = m_curves[i].Copy();
                }
                return c;
            }


            public enum eMirrorAxis
            {
                X,
                Y,
                Z
            }

            public void Mirror(eMirrorAxis axis, Vector3 offset )
            {
                for (int i = 0; i < m_curves.Length; ++i)
                {
                    //m_curves[i].m_startPosition += Vector3.up * .1f;
                    //m_curves[i].m_endPosition -= Vector3.up * .1f;
                    switch (axis)
                    {
                        case eMirrorAxis.X:
                            m_curves[i].m_startPosition.x *= -1f;
                            m_curves[i].m_endPosition.x *= -1f;
                            m_curves[i].m_startTangent.x *= -1f;
                            m_curves[i].m_endTangent.x *= -1f;

                            //m_curves[i].m_startPosition.x -= offset;
                            //m_curves[i].m_endPosition.x -= offset;
                            break;

                        case eMirrorAxis.Y:
                            m_curves[i].m_startPosition.y *= -1f;
                            m_curves[i].m_endPosition.y *= -1f;
                            m_curves[i].m_startTangent.y *= -1f;
                            m_curves[i].m_endTangent.y *= -1f;

                            //m_curves[i].m_startPosition.y -= offset;
                            //m_curves[i].m_endPosition.y -= offset;
                            break;

                        case eMirrorAxis.Z:
                            m_curves[i].m_startPosition.z *= -1f;
                            m_curves[i].m_endPosition.z *= -1f;
                            m_curves[i].m_startTangent.z *= -1f;
                            m_curves[i].m_endTangent.z *= -1f;

                            //m_curves[i].m_startPosition.z -= offset;
                            //m_curves[i].m_endPosition.z -= offset;
                            break;
                    }

                    m_curves[i].m_startPosition -= offset;
                    m_curves[i].m_endPosition -= offset;

                }                
            }


            public void SetUpRef(Vector3 upRef)
            {
                upRef = Quaternion.Inverse( m_rotation ) * upRef;
                upRef.Normalize();
                for (int i = 0; i < m_curves.Length; ++i)
                {
                    m_curves[i].m_upRef = upRef;
                }
            }


            public void ConvertOffsetAndRotation( Vector3 offset, Quaternion q, bool rootOnly )
            {
                /*
                m_curves[0].m_startPosition = m_curves[0].m_startPosition + m_offset - offset;
                m_curves[0].m_endPosition = Quaternion.Inverse(q) *  ( m_rotation *(m_curves[0].m_endPosition) + m_offset - offset );
                m_curves[0].m_startTangent = Quaternion.Inverse(q) * m_rotation * m_curves[0].m_startTangent;
                m_curves[0].m_endTangent = Quaternion.Inverse(q) * m_rotation * m_curves[0].m_endTangent;
                */

                int max = rootOnly ? 1 :m_curves.Length;
                for(int i=0; i< max; ++i)
                {
                    if(i==0)
                        m_curves[i].m_startPosition = m_curves[i].m_startPosition + m_offset - offset;
                    else
                        //    m_curves[i].m_startPosition = Quaternion.Inverse(q) * (m_rotation * (m_curves[i].m_startPosition) + m_offset - offset);
                        m_curves[i].m_startPosition = m_curves[i-1].m_endPosition;

                    m_curves[i].m_endPosition = Quaternion.Inverse(q) * (m_rotation * (m_curves[i].m_endPosition) + m_offset - offset);
                    m_curves[i].m_startTangent = Quaternion.Inverse(q) * m_rotation * m_curves[i].m_startTangent;
                    m_curves[i].m_endTangent = Quaternion.Inverse(q) * m_rotation * m_curves[i].m_endTangent;
                }

                m_offset = offset;
                m_rotation = q;

            }

        }

    }
}