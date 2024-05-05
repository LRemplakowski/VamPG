using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kalagaan.HairDesignerExtension
{
    [System.Serializable]
    public class MotionSolver
    {
        #region SubClasses

        public enum eMotionConstraint
        {            
            NONE = 0,
            LOCK_TO_ORIGIN,
            //LOCK
        }

        [System.Serializable]
        public class SolverSettings
        {
            public int solverSteps = 10;
            
            public float gravityFactor = 1f;
            public Vector2 lengthFactor = Vector2.one;
            public Vector2 rigidity = Vector2.zero;
            /*
            [Range(0, 1)]
            public float rootRigidity = .2f;
            [Range(0, 1)]
            public float rootRigidityUpsideDown = 0f;

            [Range(0, 10)]
            public float rootRigidityPower = 3f;
            
             [Range(0, 1)]
            public float rigidity = 0f;
             
             */


            public Vector2 motionStability = new Vector2(1f, .5f);


            

            //[Range(0, 1)]
            public Vector2 shapeFactor = new Vector2(1,0);


            [Range(0, 2)]
            public float elasticity = .1f;

            [Range(0, 1)]
            public float globalSmooth = .01f;

            public float distanceSmooth = 0f;

            //[Range(0, 1)]
            //public float angleFactor = 0f;

            public Vector2 RootTipSmoothFactor;

            //public bool testBool = false;

            //[Range(0, 1)]
            //public float testFloat = 1f;
            [Range(0, 180)]
            public float maxAngle = 120f;

            //public float forceMaxLimit = 1;
            [Range(0, 1)]
            public float ratioWeightRootTip = .5f;


            public bool ComputeCollisions = true;
            //public Transform collider;

            //[HideInInspector]
            public List<HairDesignerColliderBase> colliders = new List<HairDesignerColliderBase>();
            public List<HairDesignerMotionConstraintAreaBase> constraints = new List<HairDesignerMotionConstraintAreaBase>();

            public float collisionOffset = 0f;

            //[Range(1, 3)]
            //public float clampDistance = 2f;


            public float windMain = 1f;
            public float windTurbulence = 1f;
            [HideInInspector]
            public Vector4 windZoneDir = Vector4.zero;
            [HideInInspector]
            public Vector4 windZoneParam = Vector4.zero;

            public Vector3 customForce;

            public float timeScale = 1f;
            public float previous_dt = 0f;
            public float previous_timescale = 1f;


            public bool updateOnFixedUpdate = true;
            public bool useFixedTimeStep = true;
            public float dtStep = .01f;
            public bool timeCorrected = true;
            public bool extrapolate = true;
            public bool updateOrientation = true;

            [HideInInspector]
            public float startScale = 1f;
            public float scale = 1f;

            [HideInInspector]
            public bool GPU = true;
            public bool DEBUG = false;
            public bool updateTransforms = true;

            public Transform reference;


        }





        //[System.Serializable]
        public class Node
        {
            //internal data
            public int id = -1;         //current id
            public int parentId = -1;   //parent id
            public int childId = -1;    //child id
            public int rootId = -1;     //root id
            public int rootArrayId = -1;     //the reference in the root array
            public float chainFactor = .5f;//0->root 1->tip
            public Vector3 position;
            public Vector3 forward;
            public Vector3 up;
            public Vector3 prevPosition;
            public Vector3 InitPosition;
            public Vector3 localScale;
            public eMotionConstraint constraint;

            //Vector3 InitForward;
            public Vector3 InitUp;
            //float InitLength = 1f;

            //external data
            public List<Node> nodes;
            public Transform transform = null;
            public Node parent
            {
                get { return parentId != -1 ? nodes[parentId] : null; }
            }
            public Node child
            {
                get { return childId != -1 ? nodes[childId] : null; }
            }
            public Node root
            {
                get { return rootId != -1 ? nodes[rootId] : null; }
            }

            //Computed data
            Vector3 ExtrapolatePosition;
            Vector3 ExtrapolateVelocity;
            float ExtrapolateLastDt;

            

            public List<Link> links = new List<Link>();
            public bool isKinematic = false;


            public float GetParentDistance()
            {
                return links[0].length;
            }


            public float GetRootDistance()
            {
                float d = 0;
                Node parent1 = parent;

                if(parent1 != null)
                    d += Vector3.Distance(parent1.position, position);


                while (parent1!=null)
                {
                    Node parent2 = parent1.parent;
                    if (parent2 != null)
                        d += Vector3.Distance(parent1.position, parent2.position);

                    parent1 = parent2;
                }
                return d;
            }


            public float GetTipDistance()
            {
                float d = 0;
                Node child1 = child;

                if (child1 != null)
                    d += Vector3.Distance(child1.position, position);


                while (child1 != null)
                {
                    Node child2 = child1.child;
                    if (child2 != null)
                        d += Vector3.Distance(child1.position, child2.position);

                    child1 = child2;
                }
                return d;
            }


            public void UpdateKinematic()
            {
                if (transform != null)
                {
                    prevPosition = position;
                    position = transform.position;
                    up = transform.up;
                }
            }



            public void UpdatePosition(Vector3 force, float dt, SolverSettings settings)
            {
                if (isKinematic)
                {                    
                    return;
                }

                //---------------------------------------------
                //world -> local (root) transformation
                if (root != null)
                {
                    prevPosition += (root.position - root.prevPosition) * Mathf.Lerp(settings.motionStability.x, settings.motionStability.y, chainFactor * chainFactor);
                    position += (root.position - root.prevPosition) * Mathf.Lerp(settings.motionStability.x, settings.motionStability.y, chainFactor * chainFactor);
                }



                //force *= Mathf.Lerp(.5f,1f,chainFactor);

                //---------------------------------------------
                //smooth
                Vector3 delta = (position - prevPosition);
                float rootTipSmooth = Mathf.Lerp(settings.RootTipSmoothFactor.x, settings.RootTipSmoothFactor.x, chainFactor * chainFactor * chainFactor);
                prevPosition += delta * dt * (rootTipSmooth);


                float globalSmooth = Mathf.Lerp(1f - settings.globalSmooth, 1f, chainFactor * chainFactor * chainFactor);





                //---------------------------------------------
                //Verlet

                Vector3 newPos = position;
                //add delta before the fixing pass
                if (settings.previous_dt == 0 || !settings.timeCorrected)
                {
                    //original Verlet
                    //x[i+1] = x[i] + (x[i] - x[i-1]) + a * dt * dt                
                    newPos += (position - prevPosition) * globalSmooth;
                    if (!isKinematic)
                        newPos += force * dt * dt;//add forces
                }
                else
                {
                    //Time corrected : https://stackoverflow.com/questions/10139670/time-corrected-verlet-integration-and-too-big-timesteps/28061393#28061393            
                    //x[i+1] = x[i] + (x[i] – x[i-1]) * (dt[i] / dt[i-1]) + a* dt[i] * (dt[i] + dt[i-1]) / 2
                    newPos += (position - prevPosition) * globalSmooth * dt / settings.previous_dt;                    
                    newPos += force * dt * ((dt + settings.previous_dt) * .5f);//add forces
                }


                /*
                //---------------------------------------------------------
                //ROOT rigitity      
                //root.up = new Vector3(0, 1, 0);
                //force = new Vector3(0, -1, 0);
                float dotFRUP = Vector3.Dot(force.normalized, root.up);
                float rootRigidity = Mathf.Lerp(settings.rootRigidity, settings.rootRigidityUpsideDown, (dotFRUP+1f)*.5f);

                //rootRigidity = settings.rootRigidityUpsideDown;

                if (root != null && parent != null)
                {

                    float power = settings.rootRigidityPower;// Mathf.Lerp(0f, 10f, rootRigidity);
                    float f = Mathf.Pow(chainFactor, power * power * rootRigidity);
                    //f = Mathf.Pow( chainFactor, power);
                    Vector3 origine = GetInitPosition(Mathf.Lerp(settings.lengthFactor.x, settings.lengthFactor.y, chainFactor));
                    Vector3 deltaRoot = newPos - root.position;

                    newPos = LimitVectorAngle(origine - root.position, deltaRoot, Mathf.PI * f) + root.position;
                }
                */
                prevPosition = position;
                position = newPos;


                if (parent != null)
                    parent.forward = (position - parent.position).normalized;

                ExtrapolateLastDt = dt;

            }




            /*
            public void FinalUpdate(GPUNodeData gpuData)
            {
                if (!isKinematic)
                {
                    position = gpuData.position;
                    prevPosition = gpuData.prevPosition;
                }
                FinalUpdate();
            }
            */


            public void FinalUpdate()
            {                

                if (!isKinematic)
                {

                    if (child != null)
                        forward = (child.position - position).normalized;
                    else if (parent != null)
                        forward = (position - parent.position).normalized;


                    up = LimitVectorAngle(parent.up, up, chainFactor * 10f * Mathf.Deg2Rad).normalized;
                    up = Vector3.Cross(forward, Vector3.Cross(up, forward));

                    ComputeExtrapolateVelocity();
                }

            }


            public void ComputeExtrapolateVelocity()
            {
                if (ExtrapolateLastDt > 0)
                    ExtrapolateVelocity = (position - prevPosition) / ExtrapolateLastDt;
                else
                    ExtrapolateVelocity = Vector3.zero;

                ExtrapolatePosition = position;
            }



            public void UpdateTransform()
            {
                if (transform != null && !isKinematic)                
                {
                    transform.position = position;
                    if (parent != null && child != null)
                    {
                        if (transform != null)
                            transform.LookAt(position + forward, up);
                    }
                }
            }


            public void UpdateTransform(GPUNodeData gpuData)
            {
                if (transform != null && !isKinematic)
                {
                    
                    transform.position = gpuData.position;
                    if (parent != null && child != null)
                    //if ( child != null)
                    {
                        if (transform != null)
                            transform.LookAt(gpuData.position + gpuData.forward, gpuData.up);
                    }

                    ComputeExtrapolateVelocity();
                    //transform.localScale = localScale + localScale * chainFactor * ExtrapolateVelocity.magnitude;
                }
            }



            public void UpdateExtrapolate(float dt)
            {
                if (ExtrapolateLastDt == 0)
                    return;

                ExtrapolatePosition += ExtrapolateVelocity * dt * .9f;

                if (parent != null)
                    ExtrapolatePosition = Vector3.ClampMagnitude(ExtrapolatePosition - parent.transform.position, Vector3.Distance(parent.position, position)) + parent.transform.position;

                transform.position = Vector3.Lerp(transform.position, ExtrapolatePosition, .5f);

                Quaternion q = transform.rotation;
                if (child != null)
                    transform.LookAt(child.ExtrapolatePosition, transform.up);
                transform.rotation = Quaternion.Lerp(q, transform.rotation, dt * 10f);

            }





            public void SetTransform(Transform t)
            {
                transform = t;
                position = t.position;
                prevPosition = position;
                forward = t.forward;
                up = t.up;
                localScale = transform.localScale;
            }





            public void LinkToParent(Node n)
            {
                Link l = new Link(n, this);
                links.Add(l);
                n.links.Add(l);

                parentId = n.id;
                n.childId = this.id;

                rootId = parentId;

                while (root.parent != null)
                    rootId = root.parentId;
                
                if (root.transform != null)
                {
                    InitPosition = root.transform.InverseTransformPoint(position);
                    //InitForward = root.transform.InverseTransformDirection(forward);
                    InitUp = root.transform.InverseTransformDirection(up);
                    //InitLength = (n.position - position).magnitude;
                }
            }



            public Vector3 GetInitPosition(float lengthFactor)
            {
                if (root == null)
                    return position;

                //return root.transform.TransformPoint(InitPosition * lengthFactor);
                Matrix4x4 mat = Matrix4x4.TRS(root.transform.position, root.transform.rotation, root.transform.localScale);                
                return mat.MultiplyPoint3x4(InitPosition * lengthFactor);
                //return mat * (InitPosition * lengthFactor);


            }


            /*
            public Vector3 GetInitForward()
            {
                if (root == null)
                    return forward;

                return root.transform.TransformDirection(InitForward);
            }*/



            public Vector3 GetInitUp()
            {
                if (root == null)
                    return up;

                return root.transform.TransformDirection(InitUp);
            }
            

            public GPUNodeData GetGPUData()
            {
                GPUNodeData dt = new GPUNodeData();
                dt.position = position;
                dt.prevPosition = prevPosition;
                dt.up = up;
                dt.forward = forward;
                return dt;
            }


            public GPUNodeInfo GetGPUInfo()
            {
                GPUNodeInfo inf = new GPUNodeInfo();
                inf.parentId = parentId;
                inf.childId = childId;
                inf.rootId = rootId;
                inf.rootArrayId = rootArrayId;
                inf.chainFactor = chainFactor;
                inf.initPosition = InitPosition;
                inf.initUp = InitUp;
                inf.rootDistance = GetRootDistance();
                inf.constraint = (int)constraint;
                inf.strandLength = inf.rootDistance + GetTipDistance();

                for (int i = 0; i < links.Count; ++i)
                {
                    if (links[i].parentId == id)
                    {
                        inf.childDistance = links[i].length;                        
                    }

                    if (links[i].childId == id)
                        inf.parentDistance = links[i].length;
                }

                /*
                if (parent != null && parent.parent != null)
                {
                    Vector3 delta = transform.position - parent.parent.transform.position;
                    inf.localposition.x = Vector3.Dot(delta, parent.parent.transform.right);
                    inf.localposition.y = Vector3.Dot(delta, parent.parent.transform.up);
                    inf.localposition.z = Vector3.Dot(delta, parent.parent.transform.forward);
                }
                */

                return inf;
            }


            /*
            public void UnLink(Node n)
            {
                for (int i = 0; i < links.Count; ++i)
                {
                    if( links[i].parent == n || links[i].child == n)
                    {
                        Link l = links[i];
                        l.parent.links.Remove(l);
                        l.child.links.Remove(l);
                        i--;
                    }                
                }
            }*/

        }

        //[System.Serializable]//can't be serialized because of the link system -> replace by ID?
        public class Link
        {
            public int parentId;
            public int childId;
            public float length;

            public Link(Node parent, Node child)
            {
                this.parentId = parent.id;
                this.childId = child.id;
                length = (parent.position - child.position).magnitude;
            }

            /*
            public Link(Node a, Node b, float l)
            {
                parent = a;
                child = b;
                length = l;
            }
            */

            /*
            public Node LinkedNode(Node n)
            {
                if (n == null)
                    return null;
                return (parent == n ? child : (child==n?parent:null));
            }
            */

        }


       

        #endregion




        public List<Node> m_nodes = new List<Node>();
        protected List<Link> m_links = new List<Link>();
        protected List<Node> m_roots = new List<Node>();
        //public List<HairDesignerCollider> m_colliders = new List<HairDesignerCollider>();

        [HideInInspector]
        public SolverSettings m_settings = null;


        public int RootCount
        {
            get { return m_roots.Count; }
        }
        

        bool lastUpdateMethodWasGPU = false;
        //int m_lastRootId = 0;

        public Node GetNode(int i)
        {
            if (i >= 0 && i < m_nodes.Count)
                return m_nodes[i];
            return null;
        }


        public void RegisterNode(Node n, Node parent)
        {

            if(parent != null)
            {
                if (!m_nodes.Contains(parent))
                {
                    Debug.LogError("Verlet Solver : parent must be registered first");
                    return;
                }
            }

            //if (!m_nodes.Contains(n))
            {
                /*
                if (m_nodes.Count > 0 && parent == null && m_nodes[m_nodes.Count - 1].parent != null)
                    Debug.LogError("Verlet Solver : Root must be registered first");*/

                for( int i=0; i<m_settings.constraints.Count; ++i )
                {
                    //eMotionConstraint cstr = m_settings.constraints[i].GetConstraint(n.position);
                    //if (cstr != eMotionConstraint.UNDEFINED)
                    //    n.constraint = cstr;
                    m_settings.constraints[i].RegisterConstraintLink(n);

                }

                n.id = m_nodes.Count;
                m_nodes.Add(n);
                n.nodes = m_nodes;

                if (parent != null)
                {
                    n.LinkToParent(parent);
                    n.rootArrayId = parent.rootArrayId;
                }

                if (parent == null)
                {
                    n.rootArrayId = m_roots.Count;
                    m_roots.Add(n);
                    //m_lastRootId = m_nodes.Count - 1;
                    if (n.transform != null)
                        n.InitUp = n.transform.up;
                }
            }

        }



        static Vector3 LimitVectorAngle(Vector3 target, Vector3 v, float limitAngleRad)
        {
            target = target.normalized;

            float cos = Vector3.Dot(target, v) / (target.magnitude * v.magnitude);
            if (Mathf.Acos(cos) > limitAngleRad)
            {
                cos = Mathf.Cos(limitAngleRad);
                float sin = Mathf.Sin(limitAngleRad);
                Vector3 rotateAxis = Vector3.Cross(target, v.normalized).normalized;
                Vector3 vr = target * cos + Vector3.Cross(rotateAxis, target) * sin + rotateAxis * Vector3.Dot(rotateAxis, target) * (1 - cos);
                v = vr.normalized * v.magnitude;

            }
            return v;
        }



        public void UpdateKinematics(Vector3 force, float dt)
        {
            for (int n = 0; n < m_nodes.Count; ++n)
            {
                if(m_nodes[n].isKinematic)
                    m_nodes[n].UpdateKinematic();               
            }
        }


        public void UpdatePositions(Vector3 force, float dt)
        {
            for (int n = 0; n < m_nodes.Count; ++n)
            {

                m_nodes[n].UpdatePosition(force, dt, m_settings);
                ComputeCollisions(m_nodes[n]);
                    
            }
        }



        public void AngleConstraint(Node n)
        {
            if (n.parent == null)
                return;

            Vector3 refAxis = n.parent.forward;
            if (n.parent.parent != null)
                refAxis = n.parent.parent.forward;

            Vector3 d1 = n.position - n.parent.position;
            //Vector3 d2 = LimitVectorAngle(refAxis, d1, (1f - m_settings.rigidity) * m_settings.maxAngle * Mathf.Deg2Rad);
            Vector3 d2 = LimitVectorAngle(refAxis, d1,  m_settings.maxAngle * Mathf.Deg2Rad);
            n.position = d2 + n.parent.position;
            
        }



        public bool ComputeCollisions(Node n)
        {
            bool collided = false;
            if (m_settings.ComputeCollisions && !n.isKinematic)
            {

                for (int i = 0; i < m_settings.colliders.Count; ++i)
                {
                    HairDesignerColliderBase col = m_settings.colliders[i];

                    if (!col.enabled)
                        continue;

                    Vector3 colliderCenter;
                    float colliderRadius;

                    
                    //center
                    Vector3 axis = col.m_dualSphereWorldData.center2 - col.m_dualSphereWorldData.center1;
                    Vector3 proj = Vector3.Dot(axis.normalized, n.position - col.m_dualSphereWorldData.center1) * axis.normalized + col.m_dualSphereWorldData.center1;

                    //Debug.DrawLine(col.m_dualSphereWorldData.center1, proj, Color.red);
                    //Debug.DrawLine(col.m_dualSphereWorldData.center1, col.m_dualSphereWorldData.center1+axis, Color.yellow);

                    float orientation = Vector3.Dot((proj - col.m_dualSphereWorldData.center1).normalized, axis.normalized);
                    if (orientation>0 &&(proj- col.m_dualSphereWorldData.center1).magnitude < axis.magnitude && axis.magnitude>0)
                    {
                        float f = proj.magnitude / axis.magnitude;
                        float radius = Mathf.Lerp(col.m_dualSphereWorldData.radius1, col.m_dualSphereWorldData.radius2, f);

                        Vector3 delta = n.position - proj;
                        if( delta.magnitude< radius + m_settings.collisionOffset)
                        {
                            n.position = proj + delta.normalized * (radius + m_settings.collisionOffset);                            
                        }
                        collided = true;
                    }


                    //sphere 1
                    colliderCenter = col.m_dualSphereWorldData.center1;
                    colliderRadius = col.m_dualSphereWorldData.radius1;
                    float dist = (n.position - colliderCenter).magnitude;
                    if (dist < colliderRadius + m_settings.collisionOffset)
                    {
                        n.position = (n.position - colliderCenter).normalized * (colliderRadius + m_settings.collisionOffset) + colliderCenter;
                        collided = true;
                    }

                    //sphere 2
                    colliderCenter = col.m_dualSphereWorldData.center2;
                    colliderRadius = col.m_dualSphereWorldData.radius2;
                    dist = (n.position - colliderCenter).magnitude;
                    if (dist < colliderRadius + m_settings.collisionOffset)
                    {
                        n.position = (n.position - colliderCenter).normalized * (colliderRadius + m_settings.collisionOffset) + colliderCenter;
                        collided = true;
                    }

                }
            }

            return collided;
        }



        public void Update(float dt)
        {
            if (!m_settings.updateOnFixedUpdate || Time.timeScale < 1f)
                Solve(dt);

            if (m_settings.extrapolate)
            {
                for (int n = 0; n < m_nodes.Count; ++n)
                    m_nodes[n].UpdateExtrapolate(dt);
            }
        }

        public void FixedUpdate(float dt)
        {
            if (m_settings.updateOnFixedUpdate && Time.timeScale >= 1f)
                Solve(dt);
        }


        bool m_showComputeShaderWarning = true;

        public void Solve(float dt)
        {
            m_settings.GPU = true;

            if (m_settings.GPU && !UnityEngine.SystemInfo.supportsComputeShaders)
            {
                if(m_showComputeShaderWarning)
                    Debug.LogWarning("HairDesigner : System doesn't support Compute shader. Switch to CPU mode.");
                m_showComputeShaderWarning = false;
                m_settings.GPU = false;
            }
            
            if(!UnityEngine.SystemInfo.supportsComputeShaders)
            {
                return;
            }

            m_settings.solverSteps = Mathf.Clamp(m_settings.solverSteps, 1, 100);

            if (m_settings == null)
                m_settings = new SolverSettings();


            m_settings.dtStep = Mathf.Clamp01(m_settings.dtStep);
            m_settings.motionStability.x = Mathf.Clamp01(m_settings.motionStability.x);
            m_settings.motionStability.y = Mathf.Clamp01(m_settings.motionStability.y);

            if (dt == 0 || m_settings.dtStep == 0)
                return;


            if (m_settings.previous_timescale != Time.timeScale)
                m_settings.previous_dt = 0;


            float dtStep = dt * m_settings.timeScale;
            int nbSteps = 1;


            if ((m_settings.useFixedTimeStep && !m_settings.updateOnFixedUpdate) || Time.timeScale < 1f)
            {
                dtStep = m_settings.dtStep;// * Mathf.Min(Time.timeScale, 1f);
                dtStepAcc += dt;
                dtStepAcc = Mathf.Clamp(dtStepAcc, 0f, dtStep * 10f);
                if (dtStepAcc < dtStep)
                    return;
                nbSteps = Mathf.FloorToInt(dtStepAcc / dtStep);
                dtStepAcc -= (float)nbSteps * dtStep;
            }


            if( !m_settings.GPU && lastUpdateMethodWasGPU )
            {
                ReleaseBuffers();
                m_gpuInitialized = false;
                lastUpdateMethodWasGPU = false;
            }

            if (m_settings.solverSteps > 0 && nbSteps > 0 && dtStep > 0)
            {
                m_settings.lengthFactor.x = Mathf.Max(m_settings.lengthFactor.x, .001f);
                m_settings.lengthFactor.y = Mathf.Max(m_settings.lengthFactor.y, .001f);


                if (m_settings.GPU)
                    SolveGPU(nbSteps, dtStep);
                else
                    SolveCPU(nbSteps, dtStep);
            }

            m_settings.previous_timescale = Time.timeScale;
            m_settings.previous_dt = dtStep;

        }


        

        
        #region CPU_SOLVER
        //-------------------------------------------------------------------------------------
        // CPU SOLVER FALLBACK NOT AVAILABLE YET

        Vector3[] newPositions = null;
        //Vector3[] deltaPositions = null;

        float dtStepAcc = 0f;
        public void SolveCPU(int nbSteps, float dtStep)
        {

            if (newPositions == null)
                newPositions = new Vector3[m_nodes.Count];
            //if (deltaPositions == null)
            //    deltaPositions = new Vector3[m_nodes.Count];

            nbSteps = Mathf.Clamp(nbSteps, 0,50);
            while ( nbSteps > 0)
            {
                nbSteps--;
                UpdateKinematics(Physics.gravity * m_settings.gravityFactor, dtStep);
                UpdatePositions(Physics.gravity * m_settings.gravityFactor, dtStep);

                for (int i = 0; i < m_settings.solverSteps; ++i)
                {
                    for (int n = 0; n < m_nodes.Count; ++n)
                    {

                        if (i > 0)
                        {
                            m_nodes[n].position = newPositions[n];//get from previous step like GPU swap
                        }

                        float lengthSettings = Mathf.Lerp(m_settings.lengthFactor.x, m_settings.lengthFactor.y, m_nodes[n].chainFactor);


                        Vector3 newPos = m_nodes[n].position;

                        for (int l = 0; l < m_nodes[n].links.Count; ++l)
                        {                          

                            //fix the position with the Verlet algorithm
                            Node a = m_nodes[m_nodes[n].links[l].parentId];
                            Node b = m_nodes[m_nodes[n].links[l].childId];                            
                            float connectionlength = m_nodes[n].links[l].length * lengthSettings;

                            AngleConstraint(b);
                            //Vector3 dir = b.position - a.position;


                            /*
                            //if (i == m_settings.solverSteps - 1)//clamp magnitude on the latest step only
                            if (i == m_settings.solverSteps - 1 && dir.magnitude > connectionlength)//clamp magnitude on the latest step only
                            //if ( dir.magnitude > connectionlength)//clamp magnitude on the latest step only
                                b.position = Vector3.ClampMagnitude(dir, connectionlength) + a.position;
                            */
                            Vector3 delta = a.position - b.position;

                            float length = delta.magnitude;
                            float f = (length != 0) ? (length - connectionlength) / length : 0f;

                            /*
                            if (a.isKinematic || (a.parent != null && a.parent.isKinematic))
                            {
                                f *= 0f;
                                b.position = dir.normalized * connectionlength + a.position;
                            }
                            */
                            float ratioParentChild = m_settings.ratioWeightRootTip;
                            if (!a.isKinematic && a== m_nodes[n])
                                newPos -= f * ratioParentChild * delta;
                            if (!b.isKinematic && b == m_nodes[n])
                                newPos += f * (1 - ratioParentChild) * delta;

                            //a.forward = (b.position - a.position).normalized;

                            
                        }

                        if (m_nodes[n].parent != null)
                        {
                            if (Vector3.Distance(newPos, m_nodes[n].parent.position) > (m_nodes[n].GetParentDistance() * lengthSettings * 2f))
                            {
                                Vector3 dir = newPos - m_nodes[n].parent.position;
                                newPos = Vector3.ClampMagnitude(dir, m_nodes[n].GetParentDistance()* lengthSettings * 2f) + m_nodes[n].parent.position;
                            }
                        }
                        newPositions[n] = newPos;
                        //deltaPositions[n] = newPos - m_nodes[n].position;
                        //m_nodes[n].position = newPos;

                        ComputeCollisions(m_nodes[n]);
                    }
                    
                }
                               
            }
            for (int n = 0; n < m_nodes.Count; ++n)
            {
                float lengthSettings = Mathf.Lerp(m_settings.lengthFactor.x, m_settings.lengthFactor.y, m_nodes[n].chainFactor);

                m_nodes[n].position = newPositions[n];
                if (m_nodes[n].parent != null)
                {
                    Vector3 dir = m_nodes[n].position - m_nodes[n].parent.position;
                    m_nodes[n].position = Vector3.ClampMagnitude(dir, m_nodes[n].GetParentDistance()* lengthSettings) + m_nodes[n].parent.position;
                }                
                m_nodes[n].FinalUpdate();
                m_nodes[n].UpdateTransform();
            }


        }


        #endregion



        #region GPU_SOLVER
        //-------------------------------------------------------------------------------------

        //[StructLayout(LayoutKind.Sequential)]
        [System.Serializable]
        public struct GPUNodeInfo
        {
            public int parentId;
            public int childId;
            public int rootId;
            public int rootArrayId;
            public float parentDistance;
            public float childDistance;
            public float rootDistance;
            public float strandLength;
            public float chainFactor;
            public Vector3 initPosition;
            public Vector3 initUp;
            public int constraint;
            //public Vector3 localposition;
        }


        //[StructLayout(LayoutKind.Sequential)]
        [System.Serializable]
        public struct GPUNodeData
        {
            public Vector3 position;
            public Vector3 prevPosition;
            public Vector3 forward;
            public Vector3 up;
            //public bool collided;
        }

        [System.Serializable]
        public struct GPURootTransform
        {
            public Vector3 pos;
            public Matrix4x4 mat;
            public Matrix4x4 previousMat;
        }




        public ComputeShader m_CS = null;
        /*
        List<GPUNodeInfo> m_gpuNodeinfos = new List<GPUNodeInfo>();
        List<GPUNodeData> m_gpuNodesData = new List<GPUNodeData>();
        */
        ComputeBuffer m_nodesInfoBuffer = null;

        ComputeBuffer m_nodesBufferWrite = null;
        ComputeBuffer m_nodesBufferRead = null;

        ComputeBuffer m_rootTransformBuffer = null;

        ComputeBuffer m_colliderDataBuffer = null;
        


        public ComputeBuffer GetNodesDataBuffer
        {
            get { return m_nodesBufferWrite; }
        }

        public ComputeBuffer GetNodesInfoBuffer
        {
            get { return m_nodesInfoBuffer; }
        }




        int UpdatePositionsKernel;
        int[] UpdatePositionsKernelTGS;
        int UpdateRootPositionsKernel;
        int[] UpdateRootPositionsKernelTGS;
        int SolveKernel;
        int[] SolveKernelTGS;
        int UpdateOrientationKernel;
        int[] UpdateOrientationKernelTGS;
        int BufferCopyKernel;
        //int[] BufferCopyKernelTGS;


        GPURootTransform[] m_rootTransforms;

        bool m_gpuInitialized = false;
        GPUNodeData[] nodeDataArray;

        void SwapBuffers(ref ComputeBuffer a, ref ComputeBuffer b)
        {
            var c = a;
            a = b;
            b = c;
        }


        GPUNodeInfo[] m_nodesInfo;
        void FeedNodeBuffers()
        {
            nodeDataArray = m_nodes.Select(node => node.GetGPUData()).ToArray<GPUNodeData>();
            m_nodesBufferRead.SetData(nodeDataArray);
            m_nodesBufferWrite.SetData(nodeDataArray);

            m_nodesInfo = m_nodes.Select(node => node.GetGPUInfo()).ToArray<GPUNodeInfo>();
            m_nodesInfoBuffer.SetData(m_nodesInfo);
            

        }






        void InitGpuSolver()
        {
            if (m_CS == null)
            {                
                m_CS = Resources.Load<ComputeShader>("HairDesigner_MotionSolver");
                m_CS = Object.Instantiate(m_CS);
                if (m_CS == null)
                {
                    Debug.LogError("HairDesigner_MotionSolver.compute not found");
                    return;
                }
            }

            uint tgx, tgy, tgz;

            UpdatePositionsKernel = m_CS.FindKernel("UpdatePositions");            
            m_CS.GetKernelThreadGroupSizes(UpdatePositionsKernel, out tgx, out tgy, out tgz);
            UpdatePositionsKernelTGS = new int[] { (int)tgx, (int)tgy, (int)tgz };

            UpdateRootPositionsKernel = m_CS.FindKernel("UpdateRootPositions");            
            m_CS.GetKernelThreadGroupSizes(UpdateRootPositionsKernel, out tgx, out tgy, out tgz);
            UpdateRootPositionsKernelTGS = new int[] { (int)tgx, (int)tgy, (int)tgz };

            SolveKernel = m_CS.FindKernel("Solve");
            m_CS.GetKernelThreadGroupSizes(SolveKernel, out tgx, out tgy, out tgz);
            SolveKernelTGS = new int[] { (int)tgx, (int)tgy, (int)tgz };

            UpdateOrientationKernel = m_CS.FindKernel("UpdateOrientation");
            m_CS.GetKernelThreadGroupSizes(UpdateOrientationKernel, out tgx, out tgy, out tgz);
            UpdateOrientationKernelTGS = new int[] { (int)tgx, (int)tgy, (int)tgz };

            BufferCopyKernel = m_CS.FindKernel("BufferCopy");
            m_CS.GetKernelThreadGroupSizes(BufferCopyKernel, out tgx, out tgy, out tgz);
            //BufferCopyKernelTGS = new int[] { (int)tgx, (int)tgy, (int)tgz };


            int dataSize = 0;

            //dataSize = Marshal.SizeOf(typeof(GPUNodeData));            
            dataSize = 48;
            m_nodesBufferRead = new ComputeBuffer(m_nodes.Count, dataSize);
            m_nodesBufferWrite = new ComputeBuffer(m_nodes.Count, dataSize);
            
            dataSize = Marshal.SizeOf(typeof(GPUNodeInfo));
            m_nodesInfoBuffer = new ComputeBuffer(m_nodes.Count, dataSize);

            FeedNodeBuffers();

            dataSize = Marshal.SizeOf(typeof(GPURootTransform));
            //m_rootTransformBuffer = new ComputeBuffer(m_lastRootId + 1, dataSize);
            //m_rootTransforms = new GPURootTransform[m_lastRootId + 1];
            m_rootTransformBuffer = new ComputeBuffer(m_roots.Count, dataSize);
            m_rootTransforms = new GPURootTransform[m_roots.Count];

            m_CS.SetBuffer(UpdateRootPositionsKernel, "_nodesInfos", m_nodesInfoBuffer);
            m_CS.SetBuffer(UpdatePositionsKernel, "_nodesInfos", m_nodesInfoBuffer);
            m_CS.SetBuffer(SolveKernel, "_nodesInfos", m_nodesInfoBuffer);


            for (int i = 0; i < m_settings.constraints.Count; ++i)
            {
                m_settings.constraints[i].OnConstraintChanged += OnConstrantChanged;
            }


            m_gpuInitialized = true;
        }


        void ReleaseBuffer(ref ComputeBuffer Buffer )
        {
            if (Buffer != null)
                Buffer.Release();
            Buffer = null;
        }


        public void ReleaseBuffers()
        {
            ReleaseBuffer(ref m_nodesBufferWrite);
            ReleaseBuffer(ref m_nodesBufferRead);
            ReleaseBuffer(ref m_nodesInfoBuffer);
            ReleaseBuffer(ref m_colliderDataBuffer);
            ReleaseBuffer(ref m_rootTransformBuffer);            
        }


        public void Destroy()
        {
            ReleaseBuffers();
            for( int i=0; i< m_settings.constraints.Count; ++i)
            {
                m_settings.constraints[i].OnConstraintChanged -= OnConstrantChanged;
            }

        }



        public bool m_needUpdateNodeInfo = false;
        void OnConstrantChanged(HairDesignerMotionConstraintAreaBase mcstr)
        {
            if (!m_settings.constraints.Contains(mcstr))
                return;

            for( int i=0; i< mcstr.m_nodes.Count; ++i )
            {
                m_nodesInfo[mcstr.m_nodes[i].id].constraint = (int)mcstr.m_nodes[i].constraint;
            }
            m_needUpdateNodeInfo = true;
        }





        void UpdateColliderBuffer()
        {

            if (m_colliderDataBuffer == null || m_colliderDataBuffer.count != m_settings.colliders.Count)
            {
                if (m_colliderDataBuffer != null)
                    m_colliderDataBuffer.Release();

                int dataSize = Marshal.SizeOf(typeof(HairDesignerColliderBase.DualSphere));
                m_colliderDataBuffer = new ComputeBuffer(Mathf.Max(1,m_settings.colliders.Count), dataSize);//require 1 index
            }

            if (m_settings.ComputeCollisions && m_settings.colliders.Count > 0)
            {
                m_colliderDataBuffer.SetData(m_settings.colliders.Select(col => col.m_dualSphereWorldData).ToArray());
                m_CS.SetBuffer(UpdatePositionsKernel, "_colliders", m_colliderDataBuffer);
                m_CS.SetBuffer(SolveKernel, "_colliders", m_colliderDataBuffer);
                m_CS.SetInt("_collidersCount", m_settings.colliders.Count);
            }
            else
            {
                //int dataSize = Marshal.SizeOf(typeof(HairDesignerColliderBase.DualSphere));
                //m_colliderDataBuffer = new ComputeBuffer(1, dataSize);
                m_CS.SetInt("_collidersCount", 0);
                m_CS.SetBuffer(UpdatePositionsKernel, "_colliders", m_colliderDataBuffer);
                m_CS.SetBuffer(SolveKernel, "_colliders", m_colliderDataBuffer);
            }
            
        }




        private Vector4 m_time = Vector4.zero;

        protected void SolveGPU(int nbSteps, float dtStep)
        {
            if (m_nodes.Count == 0)
                return;

            if (!m_gpuInitialized)
                InitGpuSolver();

            if (m_CS == null)
                return;

            UpdateColliderBuffer();

            if (m_needUpdateNodeInfo)
            {                
                m_nodesInfoBuffer.SetData(m_nodesInfo);
                m_CS.SetBuffer(UpdatePositionsKernel, "_nodesInfos", m_nodesInfoBuffer);
                m_needUpdateNodeInfo = false;
            }


            m_CS.SetBool("_timeCorrected", m_settings.timeCorrected);           

            m_CS.SetInt("_nodeCount", m_nodes.Count);
            m_CS.SetFloat("_dt", dtStep);
            m_CS.SetFloat("_previous_dt", m_settings.previous_dt);
            m_CS.SetVector("_length", m_settings.lengthFactor);
            m_CS.SetVector("_rigidity", m_settings.rigidity);
            m_CS.SetFloat("_maxAngle", m_settings.maxAngle*Mathf.Deg2Rad);

            /*
            m_CS.SetFloat("_rootRigidity", m_settings.rootRigidity);
            m_CS.SetFloat("_rootRigidityUpsideDown", m_settings.rootRigidityUpsideDown);
            m_CS.SetFloat("_rootRigidityPower", m_settings.rootRigidityPower);
            */

            m_CS.SetFloat("_elasticity", m_settings.elasticity);
            m_CS.SetFloat("_globalSmooth", m_settings.globalSmooth);
            m_CS.SetFloat("_collisionOffset", m_settings.collisionOffset);
            m_CS.SetFloat("_ratioWeightRootTip", m_settings.ratioWeightRootTip);

            m_settings.scale = Mathf.Max(m_settings.scale, .01f);
            m_CS.SetFloat("_scale", (m_settings.reference != null ? m_settings.reference.lossyScale.x / m_settings.startScale : 1f) * m_settings.scale);

            
            //m_CS.SetFloat("_distanceSmooth", m_settings.distanceSmooth);
            m_CS.SetVector("_shapeFactor", m_settings.shapeFactor);

            m_CS.SetVector("_force", Physics.gravity * m_settings.gravityFactor + m_settings.customForce);
            m_CS.SetVector("_rootTipSmoothFactor", m_settings.RootTipSmoothFactor);
            m_CS.SetVector("_motionStability", m_settings.motionStability);

            m_CS.SetVector("KHD_windZoneDir", m_settings.windZoneDir);
            m_CS.SetVector("KHD_windZoneParam", m_settings.windZoneParam);
            m_CS.SetFloat("_WindMain", m_settings.windMain);
            m_CS.SetFloat("_WindTurbulence", m_settings.windTurbulence);

            m_time.x = Time.time / 20f;
            m_time.y = Time.time;
            m_time.z = Time.time * 2f;
            m_time.w = Time.time * 3f;
            m_CS.SetVector("_Time", m_time);



            int count = 0;
            while (count < 10 && nbSteps > 0)
            {
                count++;
                nbSteps--;

                //nodeDataArray

                
                //UPDATE posisitons root
               //for( int i=0;i<=m_lastRootId; ++i )
               for(int i=0; i<m_roots.Count; ++i)
               {
                    /*
                    nodeDataArray[i].prevPosition = nodeDataArray[i].position;
                    nodeDataArray[i].position = m_roots[i].transform.position;
                    nodeDataArray[i].up = m_roots[i].transform.up;
                    nodeDataArray[i].forward = m_roots[i].transform.forward;
                    */
                    if (m_roots[i].transform != null)
                    {
                        Matrix4x4 trs = Matrix4x4.TRS(m_roots[i].transform.position, m_roots[i].transform.rotation, m_roots[i].transform.lossyScale);
                        m_rootTransforms[i].mat = trs;
                        m_rootTransforms[i].pos = m_roots[i].transform.position;
                    }
                }

                //m_nodesBufferRead.SetData(nodeDataArray,0, 0, m_lastRootId + 1);
                //m_nodesBufferWrite.SetData(nodeDataArray, 0, 0, m_lastRootId + 1);
                m_rootTransformBuffer.SetData(m_rootTransforms);



                m_CS.SetBuffer(UpdateRootPositionsKernel, "_nodesRead", m_nodesBufferRead);
                m_CS.SetBuffer(UpdateRootPositionsKernel, "_nodes", m_nodesBufferWrite);
                m_CS.SetBuffer(UpdateRootPositionsKernel, "_rootTransform", m_rootTransformBuffer);
                //m_CS.SetBuffer(UpdatePositionsKernel, "_nodesInfos", m_nodesInfoBuffer);
                m_CS.Dispatch(UpdateRootPositionsKernel, Mathf.FloorToInt(m_nodes.Count / UpdateRootPositionsKernelTGS[0]) + 1, UpdateRootPositionsKernelTGS[1], UpdateRootPositionsKernelTGS[2]);


                m_CS.SetBuffer(BufferCopyKernel, "_nodesRead", m_nodesBufferWrite);
                m_CS.SetBuffer(BufferCopyKernel, "_nodes", m_nodesBufferRead);
                m_CS.Dispatch(BufferCopyKernel, Mathf.FloorToInt(m_nodes.Count / UpdateOrientationKernelTGS[0]) + 1, UpdateOrientationKernelTGS[1], UpdateOrientationKernelTGS[2]);

                //swap buffers
                SwapBuffers(ref m_nodesBufferWrite, ref m_nodesBufferRead);

                m_CS.SetBuffer(UpdatePositionsKernel, "_nodesRead", m_nodesBufferRead);
                m_CS.SetBuffer(UpdatePositionsKernel, "_nodes", m_nodesBufferWrite);
                m_CS.SetBuffer(UpdatePositionsKernel, "_rootTransform", m_rootTransformBuffer);
                //m_CS.SetBuffer(UpdatePositionsKernel, "_nodesInfos", m_nodesInfoBuffer);
                m_CS.Dispatch(UpdatePositionsKernel, Mathf.FloorToInt(m_nodes.Count / UpdatePositionsKernelTGS[0]) + 1, UpdatePositionsKernelTGS[1], UpdatePositionsKernelTGS[2]);

                
                for (int i = 0; i < m_settings.solverSteps*2; ++i)
                {
                    //swap buffers
                    SwapBuffers(ref m_nodesBufferWrite, ref m_nodesBufferRead);
                    
                    m_CS.SetBool("_lastStep", i==(m_settings.solverSteps-1));

                    //GPU solver                       
                    m_CS.SetBuffer(SolveKernel, "_nodesRead", m_nodesBufferRead);
                    m_CS.SetBuffer(SolveKernel, "_nodes", m_nodesBufferWrite);
                    m_CS.SetBuffer(SolveKernel, "_nodesInfos", m_nodesInfoBuffer);
                    m_CS.SetBuffer(SolveKernel, "_rootTransform", m_rootTransformBuffer);
                    m_CS.Dispatch(SolveKernel, Mathf.FloorToInt(m_nodes.Count / SolveKernelTGS[0]) + 1, SolveKernelTGS[1], SolveKernelTGS[2]);
                    
                }
                /*
                if(m_settings.solverSteps%2>0)
                    SwapBuffers(ref m_nodesBufferWrite, ref m_nodesBufferRead);
                    */
            }

            
            m_CS.SetBuffer(BufferCopyKernel, "_nodesRead", m_nodesBufferWrite);
            m_CS.SetBuffer(BufferCopyKernel, "_nodes", m_nodesBufferRead);            
            m_CS.Dispatch(BufferCopyKernel, Mathf.FloorToInt(m_nodes.Count / UpdateOrientationKernelTGS[0]) + 1, UpdateOrientationKernelTGS[1], UpdateOrientationKernelTGS[2]);
            




            //for (int i = 0; i < 5; ++i)
            if (m_settings.updateOrientation)
            {
                //swap buffers
                //SwapBuffers(ref m_nodesBufferWrite, ref m_nodesBufferRead);

                m_CS.SetBuffer(UpdateOrientationKernel, "_nodesRead", m_nodesBufferRead);
                m_CS.SetBuffer(UpdateOrientationKernel, "_nodes", m_nodesBufferWrite);
                m_CS.SetBuffer(UpdateOrientationKernel, "_nodesInfos", m_nodesInfoBuffer);
                m_CS.SetBuffer(UpdateOrientationKernel, "_rootTransform", m_rootTransformBuffer);
                m_CS.Dispatch(UpdateOrientationKernel, Mathf.FloorToInt(m_nodes.Count / UpdateOrientationKernelTGS[0]) + 1, UpdateOrientationKernelTGS[1], UpdateOrientationKernelTGS[2]);
            }



            if (m_settings.updateTransforms)
            {
                //Get data GPU -> CPU
                m_nodesBufferWrite.GetData(nodeDataArray);

                for (int n = 0; n < m_nodes.Count; ++n)
                {
                    //m_nodes[n].FinalUpdate(nodeDataArray[n]);


                    m_nodes[n].UpdateTransform(nodeDataArray[n]);
                    if (m_settings.extrapolate)
                        m_nodes[n].ComputeExtrapolateVelocity();

                    if (m_settings.DEBUG)
                    {

                        //Debug.DrawLine(m_nodes[n].GetInitPosition(m_settings.lengthFactor), m_nodes[n].GetInitPosition(m_settings.lengthFactor) + m_nodes[n].GetInitUp() * .1f, Color.magenta);

                        Debug.DrawLine(nodeDataArray[n].position, nodeDataArray[n].position + nodeDataArray[n].forward * .1f, Color.blue);
                        Debug.DrawLine(nodeDataArray[n].position, nodeDataArray[n].position + nodeDataArray[n].up * .1f, Color.green);

                        /*
                        if (n > 1)
                        {
                            Vector3 origine = nodeDataArray[n - 2].position;
                            origine += nodeDataArray[n-1].forward * m_nodesInfo[n].localposition.z;
                            origine += nodeDataArray[n-1].up * m_nodesInfo[n].localposition.y;
                            origine += Vector3.Cross( nodeDataArray[n-1].forward, nodeDataArray[n-1].up) * m_nodesInfo[n].localposition.x;

                            Debug.DrawLine(nodeDataArray[n].position, origine, Color.cyan);
                        }*/

                    }
                }
            }

            lastUpdateMethodWasGPU = true;

        }


        #endregion
    }
}