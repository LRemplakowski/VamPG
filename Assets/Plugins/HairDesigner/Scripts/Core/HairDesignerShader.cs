using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerShader : MonoBehaviour
        {
            

            public Shader m_shader;
            public Material m_defaultTransparent;
            HairDesignerBase _hd;
            public HairDesignerGenerator m_generator;

            public HairDesignerBase m_hd
            {
                get { if(_hd == null) _hd = m_generator.m_hd; return _hd; }
            }


            public virtual bool InstancingModeCompatibility( eInstancingMode imode)
            {
                switch(imode)
                {
                    case eInstancingMode.UNDEFINED:
                    case eInstancingMode.GPU_INSTANCING:
                        return true;
                }
                return   false;
            }

            public virtual void UpdatePropertyBlock(ref MaterialPropertyBlock pb, HairDesignerBase.eLayerType lt)
            {

            }

            public virtual void UpdateMaterialProperty(ref Material mat, HairDesignerBase.eLayerType lt)
            {

            }

            public virtual void SetTexture(int textureID, Texture2D tex )
            {

            }

            public virtual Texture2D GetTexture(int textureID)
            {
                return null;
            }

            /*
            public virtual void OnDestroy()
            {
                //Debug.LogWarning("DestroyShaderScript");
            }
            */
        }
    }
}
