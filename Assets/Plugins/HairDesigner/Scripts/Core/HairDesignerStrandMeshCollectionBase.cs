using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan.HairDesignerExtension
{

    public class HairDesignerStrandMeshCollectionBase : ScriptableObject
    {

      

        [System.Serializable]
        public class StrandMesh
        {
            public int id;
            public string name = "";
            public Mesh mesh;
            public Vector3 orientation = Vector3.zero;            
        }

        public List<StrandMesh> m_collection = new List<StrandMesh>();



        /// <summary>
        /// Create new id
        /// </summary>
        /// <returns></returns>
        public int CreateNewID()
        {
            int id = Random.Range(1, 100000);

            if (m_collection.Count > 0)
            {
                bool IDOk = false;
                while (!IDOk)
                {
                    StrandMesh sm = m_collection.Find(s => s.id == id);
                    if (sm == null)
                    {
                        id = Random.Range(1, 100000);
                        IDOk = true;
                    }

                }
            }

            return id;
        }


        


    }
}