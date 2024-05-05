using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        /// <summary>
        /// Generator for painting strands on mesh
        /// </summary>
        [System.Serializable]
        public class HairDesignerGeneratorMeshBase : HairDesignerGenerator
        {

            public List<StrandData> m_strands = new List<StrandData>();

            public float m_strandSpacing = .01f;


            public override StrandRenderingData GetData(int id)
            {
                m_data.rotation = m_strands[id].rotation;
                m_data.localpos = m_strands[id].localpos;// + m_strands[id].normal * .001f;
                m_data.scale = m_strands[id].scale * m_scale;
                m_data.normal = m_strands[id].normal;
                m_data.strand = m_strands[id];
                m_data.layer = m_strands[id].layer;
                return m_data;
            }

            public override int GetStrandCount()
            {
                return m_strands.Count;
            }

            /*
            public float strandScaleFactor
            {
                get
                {
                    if( m_hd == null )
                    {
                        Debug.LogWarning("HairDesigner instance not set");
                        return 1;
                    }

                    MeshRenderer mr = m_hd.GetComponent<MeshRenderer>();
                    SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();

                    if (mr != null)
                        return mr.bounds.size.magnitude * 0.02f / m_hd.globalScale;
                    if (smr != null)
                        return smr.bounds.size.magnitude * 0.02f / m_hd.globalScale;

                    return 1;
                }
            }*/


            public override void InitEditor()
            {
                base.InitEditor();

                /*
                MeshRenderer mr = m_hd.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer smr = m_hd.GetComponent<SkinnedMeshRenderer>();

                if (mr != null)
                    m_scale = mr.bounds.size.magnitude * 0.03f / m_hd.globalScale;
                if (smr != null)
                    m_scale = smr.bounds.size.magnitude * 0.03f / m_hd.globalScale;

                Debug.Log("scale " + m_scale + "  global " + m_hd.globalScale);
                */

                m_scale = strandScaleFactor * .01f;
                m_strandSpacing = m_scale / 4f;

                
            }


            //public int m_BrushAddStrandCount = 0;
            [System.NonSerialized]
            public List<Vector3> m_BrushLastStrandPositions = new List<Vector3>();

            public override void PaintTool(StrandData data, BrushToolData bt)
            {                

                switch (bt.tool)
                {

                    case ePaintingTool.ADD:
                        if (!bt.ShiftMode)
                        {
                            AddStrand(data, bt);
                            m_needMeshRebuild = true;
                        }
                        else
                        {
                            bt.ShiftMode = false;
                            BrushTool(data, bt);//brush on add strand
                            m_needMeshRefresh = true;
                        }
                        break;
                    case ePaintingTool.BRUSH:
                        BrushTool(data, bt);
                        m_needMeshRefresh = true;
                        break;
                    case ePaintingTool.SCALE:
                        ScaleTool(data, bt);
                        m_needMeshRefresh = true;
                        break;
                }

            }


            //public override void AddTool(StrandpaintingData data, bool erase)
            void AddStrand(StrandData data, BrushToolData bt)
            {
                bool erase = bt.CtrlMode;


                if (!erase)
                {
                    //if (m_strands.Count < 2000)
                    {
                        bool spacingOk = true;

                        for(int i=0; i< m_BrushLastStrandPositions.Count; ++i)
                        {

                            Vector3 wpos = m_hd.transform.TransformPoint(data.localpos);
                            if (Vector3.Distance(wpos, m_BrushLastStrandPositions[i]) < bt.brushSpacing * m_strandSpacing)
                                spacingOk = false;
                        }
                        /*
                        if (bt.brushSpacing > 0)
                        {
                            for (int i = 0; i < m_strands.Count; ++i)
                            {
                                if (m_strands[i].layer != data.layer)
                                    continue;

                                if (m_strands[i].triLock != null && m_strands[i].triLock.m_faceId != -1)
                                {
                                    if (Vector3.Distance(m_strands[i].triLock.m_cdata.localPosition, data.localpos) < bt.brushSpacing * m_strandSpacing * (1.5 - bt.brushIntensity))
                                        spacingOk = false;
                                }
                                else
                                {
                                    if (Vector3.Distance(m_strands[i].localpos, data.localpos) < bt.brushSpacing * m_strandSpacing * (1.5 - bt.brushIntensity))
                                        spacingOk = false;
                                }
                            }
                        }
                        */

                        if (spacingOk)
                        {

                            if (bt.brushScreenDir.sqrMagnitude > 0)
                            {

                                Vector3 dir = bt.transform.InverseTransformDirection(bt.brushScreenDir);

                                float maxAngle = 80f;

                                if (Vector3.Angle(dir, data.normal) > maxAngle)
                                {
                                    dir = Quaternion.AngleAxis(maxAngle, Vector3.Cross(data.normal, dir)) * data.normal;
                                }

                                Quaternion q = Quaternion.LookRotation(dir, Quaternion.Euler(0f, 0f, 90f) * data.normal);
                                q = Quaternion.LookRotation(q * Vector3.forward, data.normal);
                                data.rotation = q;
                            }
                            else
                            {
                                data.rotation = Quaternion.identity;
                            }

                            float brushWeight = bt.GetBrushWeight(bt.transform.TransformPoint(data.localpos), bt.transform.TransformDirection(data.normal));// * bt.brushIntensity;
                            data.scale *= ( brushWeight * .5f + .5f ) * bt.brushScale;
                            //data.rotation = Quaternion.Lerp(data.rotation, q, .5f);
                            data.atlasId = bt.brushAtlasId;

                            //if (!bt.ShiftMode)//for test only
                            if(m_skinningVersion160)
                            {
                                data.triLock = new TriangleLock();
                                Vector3 worldPos = m_hd.transform.TransformPoint(data.localpos);
                                Vector3 worldNormal = m_hd.transform.TransformDirection(data.normal);
                                worldPos = data.worldpos;
                                
                                //data.triLock.Lock(worldPos, bt.collider.transform.TransformDirection(q * Vector3.forward), bt.brushScreenDir, bt.collider.transform, data.meshTriId, bt.colliderVertices, bt.colliderTriangle);
                                //data.triLock.Lock(worldPos, bt.brushScreenDir, data.normal, bt.collider.transform, data.meshTriId, bt.colliderVertices, bt.colliderTriangle);
                                Vector3 dirProj = bt.brushScreenDir;
                                
                                if (Vector3.Dot(bt.brushScreenDir, worldNormal.normalized) < bt.raise)
                                {
                                    //Projection on the face
                                    dirProj = bt.brushScreenDir - Vector3.Dot(bt.brushScreenDir, worldNormal.normalized) * worldNormal.normalized;                                //dirProj = data.triLock.m_cdata.worldNormalFace;
                                    dirProj = Vector3.Lerp(dirProj, worldNormal, bt.raise);
                                }                                
                                data.triLock.Lock(worldPos,
                                    dirProj.normalized,
                                    bt.worldNormal.normalized,
                                    bt.collider.transform,
                                    transform,
                                    data.meshTriId,
                                    bt.colliderVertices,
                                    bt.colliderTriangles,
                                    true);
                                
                                m_BrushLastStrandPositions.Add(worldPos);
                            }

                            m_strands.Add(data);
                            
                        }
                    }
                }
                else
                {                    
                    //Delete strand
                    for (int i = 0; i < m_strands.Count; ++i)
                    {
                        if (data.layer != m_strands[i].layer)
                            continue;

                        Vector3 worldPos;
                        Vector3 worldNormal;
                        
                        if (m_strands[i].triLock != null && m_strands[i].triLock.m_faceId != -1)
                        {
                            m_strands[i].triLock.Apply(transform, bt.collider.transform, bt.colliderVertices, bt.colliderTriangles, true);
                            worldPos = m_strands[i].triLock.m_cdata.worldPosition;
                            worldNormal = m_strands[i].triLock.m_cdata.worldNormalFace;
                        }
                        else
                        {
                            worldPos = bt.transform.TransformPoint(m_strands[i].localpos);
                            worldNormal = bt.transform.TransformDirection(m_strands[i].normal);
                        }
                        

                        //if (Vector3.Distance(m_strands[i].localpos, data.localpos) < m_strandSpacing)
                        if (bt.GetBrushWeight(worldPos, worldNormal) > 0)
                        {   
                            m_strands.RemoveAt(i);
                            i--;
                        }
                    }
                }



            }

            void BrushTool(StrandData data, BrushToolData bt)
            {
                if (bt.brushRoot)
                    BrushRootTool(data, bt);
                else
                    BrushTipTool(data, bt);
            }







            /// <summary>
            /// Brush the root of the strand
            /// </summary>
            /// <param name="data"></param>
            /// <param name="bt"></param>
            void BrushRootTool(StrandData data, BrushToolData bt)
            {
                List<StrandData> selected = null;
                List<float> weights = null;
                Vector3 globalDir = Vector3.zero;

                


                if (bt.ShiftMode && !bt.CtrlMode)
                {
                    selected = new List<StrandData>();
                    weights = new List<float>();
                }


                //List<StrandData> strandLst = bt.selectedStrands != null ? bt.selectedStrands : m_strands;
                List<StrandData> strandLst = m_strands;

                for (int i = 0; i < strandLst.Count; ++i)
                {
                    if (data.layer != strandLst[i].layer)
                        continue;

                    float brushWeight = 0f;

                    if (strandLst[i].triLock != null && strandLst[i].triLock.m_faceId != -1)
                    {

                        strandLst[i].triLock.Apply(transform, bt.collider.transform, bt.colliderVertices, bt.colliderTriangles, false);

                        if (Vector3.Dot(strandLst[i].triLock.m_cdata.worldNormalFace, bt.cam.transform.forward) > -.1f)
                            continue;                        
                        brushWeight = bt.GetBrushWeight(strandLst[i].triLock.m_cdata.worldPosition, strandLst[i].triLock.m_cdata.worldNormalFace) * bt.brushIntensity;
                    }
                    else
                    {
                        if (Vector3.Dot(bt.transform.TransformDirection(strandLst[i].normal).normalized, bt.cam.transform.forward) > 0)
                            continue;
                        brushWeight = bt.GetBrushWeight(bt.transform.TransformPoint(strandLst[i].localpos), bt.transform.TransformDirection(strandLst[i].normal)) * bt.brushIntensity;
                    }
                    



                    if (brushWeight > 0)
                    {
                        if (strandLst[i].tmpData != null)
                            strandLst[i].tmpData.needRefresh = true;

                        if (bt.ShiftMode && bt.CtrlMode)
                        {
                            strandLst[i].atlasId = bt.brushAtlasId;
                            m_needMeshRebuild = true;                            
                            continue;
                        }

                        if (strandLst[i].triLock != null && strandLst[i].triLock.m_faceId != -1)
                        {
                            //new skinning info : v 1.6.0

                            

                            if (bt.ShiftMode)
                            {
                                if (!bt.CtrlMode)
                                {
                                    selected.Add(strandLst[i]);
                                    weights.Add(brushWeight);
                                    globalDir += strandLst[i].triLock.m_cdata.worldDirection;
                                }
                            }
                            else
                            {
                                /*
                                Quaternion newRot = m_strands[i].rotation;
                                Quaternion oldRot = m_strands[i].rotation;
                                Vector3 normal = m_strands[i].normal;
                                oldRot = m_strands[i].triLock.m_cdata.localRotation;
                                normal = m_strands[i].triLock.m_cdata.localNormalFace;
                                */
                                Vector3 oldDirection = strandLst[i].triLock.m_cdata.worldDirection;
                                Vector3 worldNormal = strandLst[i].triLock.m_cdata.worldNormalFace;//m_hd.transform.TransformDirection(data.normal);


                                if (!bt.CtrlMode)
                                {
                                    //Brush mode
                                    Vector3 dirProj = bt.brushScreenDir;

                                    if (Vector3.Dot(bt.brushScreenDir, worldNormal.normalized) < bt.raise)
                                    {
                                        //Projection on the face
                                        dirProj = bt.brushScreenDir - Vector3.Dot(bt.brushScreenDir, worldNormal) * worldNormal;
                                        dirProj = Vector3.Lerp(dirProj, worldNormal, bt.raise);
                                    }

                                    strandLst[i].triLock.UpdateWorldDirection(Vector3.Lerp(strandLst[i].triLock.m_cdata.worldDirection, dirProj.normalized, brushWeight * brushWeight));




                                }
                                else
                                {
                                    //Raise mode
                                    //m_strands[i].triLock.UpdateWorldDirection(Vector3.Lerp(m_strands[i].triLock.m_cdata.worldDirection, m_strands[i].triLock.m_cdata.worldNormalFace, brushWeight* brushWeight), m_strands[i].triLock.m_cdata.localUp, bt.colliderVertices, bt.colliderTriangles);
                                    strandLst[i].triLock.UpdateWorldDirection(Vector3.Lerp(strandLst[i].triLock.m_cdata.worldDirection, strandLst[i].triLock.m_cdata.worldNormalFace, brushWeight * brushWeight));
                                }
                                
                            }


                        }
                        else
                        {
                            //old skinning info : version < 1.6.0

                            if (bt.ShiftMode)
                            {
                                selected.Add(strandLst[i]);
                                weights.Add(brushWeight);                                
                                globalDir += strandLst[i].rotation * Vector3.forward;
                            }
                            else
                            {
                                Quaternion newRot = strandLst[i].rotation;
                                Quaternion oldRot = strandLst[i].rotation;
                                Vector3 normal = strandLst[i].normal;                                

                                if (!bt.CtrlMode)
                                {
                                    Vector3 dir = bt.transform.InverseTransformDirection(bt.brushScreenDir);
                                    float maxAngle = 80f;
                                    if (Vector3.Angle(dir, normal) > maxAngle)
                                    {
                                        dir = Quaternion.AngleAxis(maxAngle, Vector3.Cross(strandLst[i].normal, dir)) * normal;
                                    }
                                    Quaternion q = Quaternion.LookRotation(dir, Quaternion.Euler(0f, 0f, 90f) * normal);
                                    newRot = Quaternion.Lerp(oldRot, q, .05f);
                                    newRot = Quaternion.LookRotation(newRot * Vector3.forward, normal);
                                }
                                else
                                {
                                    newRot = Quaternion.Lerp(oldRot, Quaternion.FromToRotation(oldRot * Vector3.forward, normal) * oldRot, .1f * brushWeight);
                                }

                                strandLst[i].rotation = Quaternion.Lerp(strandLst[i].rotation, newRot, brushWeight);                                
                            }
                        }//end old skinning info
                    }
                }

                if (bt.ShiftMode && !bt.CtrlMode)
                {
                    globalDir.Normalize();
                    for (int i = 0; i < selected.Count; ++i)
                    {
                        if(selected[i].tmpData!=null)
                            selected[i].tmpData.needRefresh = true;
                        if (selected[i].triLock != null && selected[i].triLock.m_faceId != -1)
                        {
                            //selected[i].triLock.m_cdata.m_lastTransform = bt.collider.transform;
                            selected[i].triLock.UpdateWorldDirection( Vector3.Lerp( selected[i].triLock.m_cdata.worldDirection, globalDir, weights[i]*.1f) );
                        }
                        else
                        {
                            Quaternion q = Quaternion.LookRotation(globalDir, selected[i].normal);
                            selected[i].rotation = Quaternion.Lerp(selected[i].rotation, q, weights[i] * .1f);
                        }
                    }
                }
            }




            /// <summary>
            /// Brush the tip of the strand
            /// </summary>
            /// <param name="data"></param>
            /// <param name="bt"></param>
            void BrushTipTool(StrandData data, BrushToolData bt)
            {
                for (int i = 0; i < m_strands.Count; ++i)
                {
                    //float brushDistance = data.cam.
                    if (Vector3.Dot(bt.transform.TransformDirection(m_strands[i].normal).normalized, bt.cam.transform.forward) > 0)
                        continue;

                    if (data.layer != m_strands[i].layer)
                        continue;

                    float brushWeight = bt.GetBrushWeight(bt.transform.TransformPoint(m_strands[i].localpos), bt.transform.TransformDirection(m_strands[i].normal)) * bt.brushIntensity;
                    if (brushWeight > 0)
                    {
                        m_strands[i].tmpData.needRefresh = true;

                        if (m_strands[i].curve == null || m_strands[i].curve.m_length == 0 )
                        {
                            //generate a custom curve for this strand
                            m_strands[i].curve = new BZCurv( m_params.m_curv );
                            m_strands[i].curve.InitLength(20);
                        }

                        if (bt.CtrlMode)
                        {                
                            //revert the curve to the original one            
                            m_strands[i].curve.m_endPosition = Vector3.Lerp(m_strands[i].curve.m_endPosition, m_params.m_curv.m_endPosition, brushWeight);
                            if( Vector3.Distance(m_strands[i].curve.m_endPosition, m_params.m_curv.m_endPosition)<.001f )
                            {                                
                                m_strands[i].curve = null;//remove it when the reference is the same
                            }

                        }
                        else
                        {

                            //float dist = Vector3.Distance(m_strands[i].curve.m_endPosition, m_strands[i].curve.m_startPosition);
                            Vector3 newpos = m_strands[i].curve.m_endPosition;

                            Vector3 dir = bt.transform.InverseTransformDirection(bt.brushScreenDir);
                            Quaternion q = Quaternion.Inverse(m_strands[i].rotation);

                            dir = (q * dir.normalized);
                            newpos = m_strands[i].curve.m_endPosition + (dir).normalized * m_strands[i].curve.m_endPosition.magnitude * .1f;
                            newpos = (dir).normalized * m_strands[i].curve.m_endPosition.magnitude;

                            m_strands[i].curve.m_endPosition = Vector3.Lerp(m_strands[i].curve.m_endPosition, newpos, brushWeight);
                            
                            m_strands[i].curve.m_endPosition = m_strands[i].curve.m_endPosition.normalized * m_params.m_curv.m_endPosition.magnitude;
                            
                            m_strands[i].curve.m_endTangent = Vector3.Lerp(m_strands[i].curve.m_endTangent, (dir).normalized * m_params.m_curv.m_endTangent.magnitude, brushWeight);

                            
                        }

                        

                        

                    }
                }
            }


            List<StrandData> _selected = new List<StrandData>();
            List<float> _weights = new List<float>();
            void ScaleTool(StrandData data, BrushToolData bt)
            {
                
                float globalScale = 0f;
                if (bt.ShiftMode)
                {
                    _selected.Clear();
                    _weights.Clear();
                }
                for (int i = 0; i < m_strands.Count; ++i)
                {
                    if (Vector3.Dot(bt.transform.TransformDirection(m_strands[i].normal).normalized, bt.cam.transform.forward) > 0)
                        continue;

                    if (data.layer != m_strands[i].layer)
                        continue;

                    float brushWeight = bt.GetBrushWeight(bt.transform.TransformPoint(m_strands[i].localpos), bt.transform.TransformDirection(m_strands[i].normal)) * bt.brushIntensity;
                    if (brushWeight > 0)
                    {
                        m_strands[i].tmpData.needRefresh = true;

                        if (bt.ShiftMode)
                        {
                            _selected.Add(m_strands[i]);
                            _weights.Add(brushWeight);
                            globalScale += m_strands[i].scale;
                        }
                        else
                        {
                            m_strands[i].scale += (bt.CtrlMode ? -.02f : .02f) * brushWeight;
                            m_strands[i].scale = Mathf.Clamp(m_strands[i].scale, 0.1f, m_strands[i].scale);
                        }
                    }
                }

                if (bt.ShiftMode)
                {
                    globalScale /= (float)_selected.Count;
                    for (int i = 0; i < _selected.Count; ++i)
                    {
                        _selected[i].tmpData.needRefresh = true;
                        _selected[i].scale = Mathf.Lerp(_selected[i].scale, globalScale, _weights[i] * .1f);
                    }
                }
            }



            public override void UpdateInstance()
            {                
                base.UpdateInstance();
                UpdateBlenshapes();
                if (m_needMeshRefresh)
                {
                    RefreshMesh(null);                    
                    m_needMeshRefresh = false;
                }
            }




            public override void RefreshMesh(Mesh skinRef)
            {
                base.RefreshMesh(skinRef);

                //bool skinDataUpdated = false;
                //skinDataUpdated = true; //debug : force full regeneration
                if (meshTmpData == null)
                {
                    //skinDataUpdated = true;
                    meshTmpData = new CreateMeshReferenceData();
                    if (skinRef != null)
                    {
                        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
                        meshTmpData.skinRefTransform = smr.transform;
                        meshTmpData.skinRefVertices = skinRef.vertices;
                        meshTmpData.skinRefBoneWeights = skinRef.boneWeights;
                        meshTmpData.skinRefTriangles = skinRef.triangles;
                        MeshUtility.ApplyBlendShape(smr, ref meshTmpData.skinRefVertices);
                    }
                    else
                    {
                        MeshFilter mf = m_hd.GetComponent<MeshFilter>();
                        if (mf != null)
                        {
                            meshTmpData.skinRefVertices = mf.sharedMesh.vertices;
                            meshTmpData.skinRefTriangles = mf.sharedMesh.triangles;
                        }
                    }
                }


                int strandIdx = -1;

                if (m_hair == null)
                    return;

                

                Vector3[] vertices = m_hair.vertices;
                Vector3[] normals = m_hair.normals;

                Vector3[] strandVertices = m_hairStrand.vertices;
                //Vector3[] strandNormals = m_hairStrand.normals;
                //Vector4[] strandTangents = new Vector4[m_hairStrand.vertices.Length];
                //int[] strandTriangles = m_hairStrand.triangles;
                Vector2[] strandUV = m_hairStrand.uv;

                //Vector3[] strandVerticesNew = new Vector3[strandVertices.Length];

                

                for (int i = 0; i < GetStrandCount(); ++i)
                {
                    StrandRenderingData dt = GetData(i);

                    if (!m_editorLayers[dt.layer].visible)
                        continue;

                    strandIdx++;

                    if (dt.strand.tmpData == null || !dt.strand.tmpData.needRefresh)
                        continue;

                    dt.strand.tmpData.needRefresh = false;

                   

                    //-----------------------------------------------
                    //Apply strand curv                    
                    Vector3 rnd = Random.insideUnitSphere * m_params.m_randomSrandFactor;

                    for (int v = 0; v < strandVertices.Length; ++v)
                    {
                        
                        Vector3 vertex = strandVertices[v];
                        //vertex.z *= 2f;

                        vertex.x *= Mathf.Lerp(m_params.m_taper.x, m_params.m_taper.y, strandUV[v].y);//taper
                        vertex.y -= Mathf.Abs(vertex.x) * Mathf.Abs(vertex.x) * m_params.m_bendX * (.1f + strandUV[v].y * .9f);//bend
                        //vertex.z *= m_params.m_length;//length

                        float t = strandUV[v].y;
                        //float t2 = t * t;

                        BZCurv curv = m_params.m_curv;
                        if (dt.strand.curve == null || dt.strand.curve.m_length == 0)
                            curv = new BZCurv(m_params.m_curv.m_startPosition, m_params.m_curv.m_endPosition + rnd, m_params.m_curv.m_startTangent, m_params.m_curv.m_endTangent);
                        else
                            curv = new BZCurv(dt.strand.curve.m_startPosition, dt.strand.curve.m_endPosition + rnd, dt.strand.curve.m_startTangent, dt.strand.curve.m_endTangent);


                        Vector3 c = curv.GetPosition(t);

                        //Vector3 n = Vector3.Lerp(new Vector3(c.x, c.y, c.z), new Vector3(c.x * t2, c.y * t2, c.z), m_params.m_rigidity * t);
                        Vector3 n = new Vector3(c.x, c.y, c.z * m_params.m_length);
                        vertex.x += n.x;
                        vertex.y += n.y;
                        vertex.z = n.z;

                        int vid = strandIdx * strandVertices.Length + v;

                        if (!m_skinningVersion160)
                        {
                            vertex = (dt.rotation * vertex * dt.scale) + dt.localpos + dt.normal * (dt.strand.offset + m_params.m_offset);
                        }
                        else
                        {
                            dt.strand.triLock.Apply(transform, meshTmpData.skinRefTransform != null ? meshTmpData.skinRefTransform : transform, meshTmpData.skinRefVertices, meshTmpData.skinRefTriangles, true);
                            vertex = dt.strand.triLock.m_cdata.localPosition + (dt.strand.triLock.m_cdata.localRotation * vertex * dt.scale);
                        }

                        //vertex += dt.strand.triLock.m_cdata.localPosition;
                        vertices[vid] = vertex;
                        //dt.strand.tmpData.vertices[v] = vertex;

                        //Vector3 normal = Vector3.Lerp(dt.rotation * strandNormals[v], dt.strand.triLock.m_cdata.localNormalFace, m_params.m_normalToTangent).normalized;
                        //dt.strand.tmpData.normals[v] = normal;
                        //vertices[vid] += Vector3.up * .001f;

                        /*
                        strandVerticesNew[v] = vertex;
                        strandTangents[v] = curv.GetTangent(t).normalized;
                        strandTangents[v].w = -1;
                        strandTangents[v] = new Vector4(0, 0, 1, -1);
                        */
                    }

                }

                m_hair.vertices = vertices;
                //finalMesh.vertices = normals;
                //m_hair.UploadMeshData(false);

                //m_hair = finalMesh;
                m_hair.name = "Brushed";

                /*
                MeshFilter mf = m_meshInstance.GetComponent<MeshFilter>();
                mf.sharedMesh = m_hair;
                */
            }







            //public bool m_autoRebuildBlendShapes = true;

            public void UpdateBlenshapes()
            {

                if (!m_meshLocked)
                    return;

                if (m_hd.m_smr == null)
                    return;

                SkinnedMeshRenderer smr = m_hd.m_smr;

                if (smr!=null && m_skinnedMesh != null)
                {
                    /*
                    if(m_autoRebuildBlendShapes && smr.sharedMesh.blendShapeCount != m_skinnedMesh.sharedMesh.blendShapeCount )
                    {
                        CreateHairMesh(smr.sharedMesh);                        
                    }*/

                    for ( int i=0; i<smr.sharedMesh.blendShapeCount; ++i )
                    {
                        if(i< m_skinnedMesh.sharedMesh.blendShapeCount) 
                            m_skinnedMesh.SetBlendShapeWeight(i, smr.GetBlendShapeWeight(i));

                    }
                }
            }

        }
    }
}