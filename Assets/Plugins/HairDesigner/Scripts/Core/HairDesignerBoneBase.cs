using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [ExecuteInEditMode]
        [AddComponentMenu("")]
        public class HairDesignerBoneBase : MonoBehaviour {

            public HairDesignerMeshInstanceBase m_meshReference;
            public Transform m_target;
            public string m_targetName = "";
            public Vector3 m_localPosition;
            public Quaternion m_localRotation;
            public Vector3 m_localScale;
            public bool m_useLocalTransform = false;

            void Start()
            {
                if(m_meshReference==null)
                {
                    if (Application.isPlaying)
                        Destroy(gameObject);
                    else
                        DestroyImmediate(gameObject);
                    return;
                }
                
                //if( transform.parent.GetComponent<HairDesignerBoneBase>() == null )
                {
                    if(Application.isPlaying && m_target!=null)
                    {
                        transform.parent = m_target;
                        if(m_useLocalTransform)
                        {
                            transform.localPosition = m_localPosition;
                            transform.localRotation = m_localRotation;
                            transform.localScale = m_localScale;
                        }
                    }                    
                }
            }
        }
    }
}