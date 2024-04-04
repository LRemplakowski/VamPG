using UnityEngine;
using System.Collections;
using Kalagaan.HairDesignerExtension;
using UnityEngine.UI;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerFurDemo : MonoBehaviour
        {

            public HairDesigner m_hairDesigner;
            //public string m_layerName = "Fur";


            public void Start()
            {
                SelectFur();
            }


            bool shell = false;
            public void SelectFur()
            {
                m_hairDesigner.GetLayer(0).SetActive(!shell);
                m_hairDesigner.GetLayer(1).SetActive( shell);
                shell = !shell;
            }



            public void SetDensity(UnityEngine.UI.Slider slider)
            {
                HairDesignerShaderProcedural hdsp = m_hairDesigner.GetLayer("Fur Polygons").GetShaderParams() as HairDesignerShaderProcedural;
                if (hdsp != null)
                    hdsp.m_hairDensity = Mathf.Lerp(0, 50, slider.value);
                HairDesignerShaderAtlas hdsp2 = m_hairDesigner.GetLayer("Fur Polygons").GetShaderParams() as HairDesignerShaderAtlas;
                if (hdsp2 != null)
                    hdsp2.m_length = Mathf.Lerp(0, 1, slider.value);

                HairDesignerShaderFurShell fs = m_hairDesigner.GetLayer("Fur Shells").GetShaderParams() as HairDesignerShaderFurShell;
                fs.m_furLength = Mathf.Lerp(.1f, .2f, slider.value);
            }
        }
    }
}