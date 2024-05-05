using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan.HairDesignerExtension
{

    public class DemoLongHair : MonoBehaviour
    {

        public HairDesigner m_hd;
        public HeadController m_headCtrl;
        int m_haircutID = 0;


        public void NextHaircut()
        {
            m_haircutID++;
            if (m_haircutID >= m_hd.m_generators.Count)
                m_haircutID = 0;
            ChangeHaircut();
        }


        public void PreviousHaircut()
        {
            m_haircutID--;
            if (m_haircutID < 0)
                m_haircutID = m_hd.m_generators.Count-1;
            ChangeHaircut();
        }

        void ChangeHaircut()
        {
            for(int i=0; i<m_hd.m_generators.Count; ++i)
            {
                m_hd.m_generators[i].SetActive(i == m_haircutID);
            }
            m_headCtrl.EnableExpression();

        }



    }
}
