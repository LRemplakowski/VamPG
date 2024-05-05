using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HairDesigner_Demo1 : MonoBehaviour {

    [System.Serializable]
    public class HairCut
    {
        public List<string> m_layers = new List<string>();
    }


    public Vector3 m_rotation = new Vector3(0f,60f,0f);
    public bool m_autoSwitch = true;
    public float m_timer = 2f;
    public float m_speed = 10f;
    public Kalagaan.HairDesignerExtension.HairDesigner m_hd;
    public List<HairCut> m_hairCuts = new List<HairCut>();

    Quaternion m_first;
    Quaternion m_second;
    bool m_switch = false;
    float m_lastSwitchTime = 0;
    int m_currentId = 0;
    int m_switchCount = 0;

    void Start()
    {
        m_first = transform.localRotation;
        m_second = transform.localRotation* Quaternion.Euler(m_rotation);

        if (m_hd == null)
        {
            enabled = false;
            return;
        }

        for (int i = 0; i < m_hd.m_generators.Count; ++i)
        {
            m_hd.m_generators[i].SetActive(m_hairCuts[m_currentId].m_layers.Contains(m_hd.m_generators[i].m_name));         
        }
    }

    void Update ()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, m_switch ? m_second : m_first, Time.deltaTime * m_speed);
        if( m_lastSwitchTime + m_timer < Time.time )
        {
            m_lastSwitchTime = Time.time;
            m_switch = !m_switch;
            m_switchCount++;

            if (m_autoSwitch && !m_switch && m_hairCuts.Count>0 && m_switchCount > 2 )
            {
                m_switchCount = 0;
                Next();

            }
        }
    }

    public void Next()
    {
        m_currentId++;
        m_currentId = m_currentId % m_hairCuts.Count;
        for (int i = 0; i < m_hd.m_generators.Count; ++i)
        {            
            m_hd.m_generators[i].SetActive(m_hairCuts[m_currentId].m_layers.Contains(m_hd.m_generators[i].m_name));
        }
    }


    public void Previous()
    {
        m_currentId--;
        m_currentId = m_currentId<0? m_hairCuts.Count-1: m_currentId;
        for (int i = 0; i < m_hd.m_generators.Count; ++i)
        {
            m_hd.m_generators[i].SetActive(m_hairCuts[m_currentId].m_layers.Contains(m_hd.m_generators[i].m_name));
        }
    }
}
