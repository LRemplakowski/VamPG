using UnityEngine;
using System.Collections;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerFurDemo2 : MonoBehaviour {

            public HairDesigner m_hd;
            public float m_timer = 2f;
            public int m_id = 0;
            float m_lastTime = 0f;
            void Start() {

                if (m_hd == null)
                {
                    enabled = false;
                    return;
                }

                for (int i = 0; i < m_hd.m_generators.Count; ++i)
                    m_hd.GetLayer(i).SetActive(i == m_id);
            }


            void Update()
            {
                if (Time.time > m_lastTime + m_timer)
                {
                    m_lastTime = Time.time;
                    m_hd.GetLayer(m_id).SetActive(false);
                    m_id++;
                    m_id %= m_hd.m_generators.Count;
                    m_hd.GetLayer(m_id).SetActive(true);                    
                }

            }
        }
    }
}
