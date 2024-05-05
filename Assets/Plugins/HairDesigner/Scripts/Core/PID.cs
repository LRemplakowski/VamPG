using UnityEngine;
using System.Collections;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [System.Serializable]
        public class HairDesignerPID
        {

            public static Color orange = new Color(240f / 255f, 158f / 255f, 0);

            public float lastTime;
            double errSum;
            //double lastErr;
            double error;

            public float m_target = 0f;
            public float m_timeScale = 1f;




            public Parameters m_params = new Parameters();
            float dtMax = 1f;//max dt for computing PID
            public static float dtStep = 1f / 100f;//60fps step
                                                   //float dtStep = 1f;//60fps step
            float dtStepNext = 0f;

            float delta;

            public HairDesignerPID() { }

            public HairDesignerPID(float kp, float ki, float kd)
            {
                m_params.kp = kp;
                m_params.ki = ki;
                m_params.kd = kd;
            }

            [System.Serializable]
            public class Parameters
            {
                public float kp = 2f, ki = 2f, kd = 0f;
                public float deltaMax = 1f;
                public Vector2 limits = new Vector2(-float.MaxValue, float.MaxValue);

                public Parameters() { }
                public Parameters(Parameters p)
                {
                    kp = p.kp;
                    ki = p.ki;
                    kd = p.kd;
                    limits = p.limits;
                }
            }


            public float Compute(float input)
            {
                //Debug.Log (""+Time.time);

                float result = input;
                float dt = Time.time - lastTime;
                //if( dt >= dtStep )
                {
                    result = Compute(input, dt);
                    lastTime = Time.time;
                }
                return result;
            }



            float m_lastInput = 0;
            public float Compute(float input, float dt)
            {
                dt *= m_timeScale;

                if (dt == 0)
                    return input;

                if (dt > dtMax)
                {
                    ForceTarget(1f);
                    return m_target;
                }
                float result = input;
                float step = dt;


                float target = m_target;




                //check limits

                float deltaLimit = m_params.ki * (float)errSum + m_params.kp * (float)error;
                //target = Mathf.Clamp(target, m_params.limits.x, m_params.limits.y);


                if (input + deltaLimit * 2f > m_params.limits.y)
                {
                    target -= (float)errSum;
                    errSum = 0;
                }


                if (input + deltaLimit * 2f < m_params.limits.x)
                {
                    target += (float)errSum;
                    errSum = 0;
                }

                dt += dtStepNext;
                dtStepNext = 0f;

                float dtStepLocal = Mathf.Min(dtStep * m_timeScale, dtStep);

                while (dt >= dtStepLocal)
                //while (dt > 0 )
                {
                    //this is a real compute step

                    //initialise input from last real step
                    result = m_lastInput;

                    //step = dt > dtStepLocal ? dtStepLocal : dt;
                    step = dtStepLocal;

                    //if( step>= dtStep )
                    {
                        //compute error
                        error = (target - result);
                        error *= (double)step;
                        errSum += error;
                    }

                    if (m_params.ki == 0)
                        errSum = 0;

                    //double dErr = (error - lastErr) / (double)step;

                    //lastErr = error;				
                    delta = m_params.kp * (float)error + m_params.ki * (float)errSum;// + m_params.kd * (float)dErr;
                    result += delta;

                    //set limit
                    //result = Mathf.Clamp(result, m_params.limits.x, m_params.limits.y);
                    result = Mathf.Clamp(result - target, m_params.limits.x, m_params.limits.y) + target;
                    dt -= step;

                    m_lastInput = result;
                }

                dtStepNext += dt;

                //add delta of current step to fake "out of step" result
                //keep a smooth result between step
                //result will be override by next real step
                result = Mathf.Lerp(m_lastInput, m_lastInput + delta, (dtStepNext / dtStepLocal));
                return result;
            }


            /// <summary>
            /// Forces the target to be set earlier
            /// </summary>
            /// <param name="percent">Percent.</param>
            public void ForceTarget(float percent)
            {
                errSum = (double)Mathf.Lerp((float)errSum, 0f, percent);
                //lastErr = 0f;
            }


            /// <summary>
            /// Initialize 
            /// </summary>
            public void Init()
            {
                errSum = 0f;
                //lastErr = 0f;
                lastTime = Time.time;
                m_lastInput = m_target;
            }

            /*	
                public void GUIDrawResponse( Rect area, float target, float timeUnit )
                {

                    Color c = new Color (1f, 1f, 1f, .1f); 
                    GLUtility.DrawBox (area, c, 1f);

                    Init ();
                    //unit step
                    m_target = target;
                    //m_limits.y = 1f;//TEST limit
                    float r = 0;
                    Vector2 start = new Vector2( area.x, area.y+area.height );
                    Vector2 end = start;


                    for (int i=0; i<timeUnit; ++i)
                    {
                        start = new Vector2( (float)i*area.width / timeUnit, area.y+area.height );
                        end = new Vector2( (float)i*area.width / timeUnit, area.y );
                        GLUtility.DrawLine (start, end, c, 1f);
                    }

                    start = new Vector2( area.x, area.y+area.height*.5f );
                    end = new Vector2( area.x+area.width, area.y+area.height*.5f );
                    GLUtility.DrawLine (start, end, c, 1f);


                    start = new Vector2( area.x, area.y+area.height );
                    end = start;

                    for( int i=0; i<area.width; ++i )
                    {
                        r = Compute( r , timeUnit / area.width );


                        end.x++;			
                        end.y = area.height-r*area.height*.5f + area.y;
                        end.y = Mathf.Clamp( end.y,  area.y,  area.y+area.height );
                        //end.y = Mathf.Clamp( end.y,  1f,  area.y+area.height );

                        GLUtility.DrawLine (start, end, orange, 1f);
                        start = end;
                    }

                }
                */


        }




        [System.Serializable]
        public class HairPID_V3
        {


            public Vector3 m_target = Vector3.zero;
            public HairDesignerPID.Parameters m_params = new HairDesignerPID.Parameters();

            public HairDesignerPID m_pidX = new HairDesignerPID();
            public HairDesignerPID m_pidY = new HairDesignerPID();
            public HairDesignerPID m_pidZ = new HairDesignerPID();



            public float timeScale
            {
                get
                {
                    return m_pidX.m_timeScale;
                }
                set
                {
                    m_pidX.m_timeScale = value;
                    m_pidY.m_timeScale = value;
                    m_pidZ.m_timeScale = value;
                }
            }


            public Vector3 Compute(Vector3 input)
            {

                m_pidX.m_params = m_params;
                m_pidX.m_target = m_target.x;

                m_pidY.m_params = m_params;
                m_pidY.m_target = m_target.y;

                m_pidZ.m_params = m_params;
                m_pidZ.m_target = m_target.z;


                Vector3 r = Vector3.zero;
                r.x = m_pidX.Compute(input.x);
                r.y = m_pidY.Compute(input.y);
                r.z = m_pidZ.Compute(input.z);

                return r;
            }

            public Vector3 Compute(Vector3 input, float dt)
            {

                m_pidX.m_params = m_params;
                m_pidX.m_target = m_target.x;

                m_pidY.m_params = m_params;
                m_pidY.m_target = m_target.y;

                m_pidZ.m_params = m_params;
                m_pidZ.m_target = m_target.z;


                Vector3 r = Vector3.zero;
                r.x = m_pidX.Compute(input.x,dt);
                r.y = m_pidY.Compute(input.y, dt);
                r.z = m_pidZ.Compute(input.z, dt);

                return r;
            }





            public void Init()
            {
                m_pidX.m_target = m_target.x;
                m_pidX.Init();
                m_pidY.m_target = m_target.y;
                m_pidY.Init();
                m_pidZ.m_target = m_target.z;
                m_pidZ.Init();
            }

            public void ForceTarget(float percent)
            {
                m_pidX.ForceTarget(percent);
                m_pidY.ForceTarget(percent);
                m_pidZ.ForceTarget(percent);
            }


        }
    }
}

