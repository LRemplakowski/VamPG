using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan.HairDesignerExtension
{

    public class HairDesignerRuntimeLayerLoader : MonoBehaviour {


        public HairDesigner m_target;
        public bool m_loadOnStart = true;
        public List<HairDesignerRuntimeLayerBase> m_layers = new List<HairDesignerRuntimeLayerBase>();
        public List<CapsuleCollider> m_hairColliders = new List<CapsuleCollider>();

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            if (m_target == null)
                m_target = GetComponent<HairDesigner>();
        }


        /// <summary>
        /// 
        /// </summary>
        void Start() {
            if (m_loadOnStart)
            {
                Load();
            }
        }



        /// <summary>
        /// Load the layers
        /// </summary>
        public void Load() {
            for (int i = 0; i < m_layers.Count; ++i)
            {
                HairDesignerRuntimeLayerBase.m_hairColliders = m_hairColliders;
                m_layers[i].GenerateLayers(m_target);
                HairDesignerRuntimeLayerBase.m_hairColliders.Clear();
            }
        }
    }
}