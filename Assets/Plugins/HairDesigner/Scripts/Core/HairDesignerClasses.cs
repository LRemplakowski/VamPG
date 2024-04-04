using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [System.Serializable]
        public class TextureToolColor
        {
            public Color root = Color.white;
            public Color tip = Color.white;
            public float bias = .5f;

            public TextureToolColor Copy()
            {
                TextureToolColor c = new TextureToolColor();
                c.root = root;
                c.tip = tip;
                c.bias = bias;
                return c;
            }
        }


        [System.Serializable]
        public class TextureToolShaderParameters
        {
            public Vector4 m_scale = new Vector4(1f, 1f, 1f, 1f);

            public int m_hairCount = 100;

            /*
            public Color m_color1 = Color.white;
            public Color m_color2 = Color.white;
            public float m_colorBias = 1f;
            */
            public List<TextureToolColor> m_colors = new List<TextureToolColor>();
            public int m_nbColors = 1;

            public float m_rnd;
            public Vector2 m_width = new Vector2(.1f, .1f);
            public Vector2 m_length = new Vector2(.1f, 1f);
            public Vector2 m_wave0 = new Vector2(0f, 10f);
            public Vector4 m_wave1 = new Vector4(0f, 1f, .25f, 10f);
            public Vector4 m_wave2 = new Vector4(0f, 1f, .25f, 10f);

            public Vector2 m_rootOffset = new Vector2(0f, .1f);
            public float m_rootThickness = 0.2f;

            public Vector4 m_noise = new Vector4(500f, 5f, .5f, .5f);
            public Vector4 m_depth = new Vector4(.1f, .1f, 0f, 0f);
            public float m_ao = .5f;
            public float m_specularRoot = .5f;
            public float m_specularTip = 1f;
            public Vector3 m_specularShift = new Vector3(.5f,.5f,.5f);
            public Vector3 m_specularMask = new Vector3(.5f, .5f,.5f);

            public float m_test;


            public TextureToolShaderParameters()
            {
                m_colors.Add(new TextureToolColor());
            }


            public TextureToolShaderParameters Copy()
            {
                TextureToolShaderParameters c = new TextureToolShaderParameters();
                c.m_scale = m_scale;
                c.m_hairCount = m_hairCount;

                c.m_colors = new List<TextureToolColor>();
                for (int i = 0; i < m_colors.Count; ++i)
                    c.m_colors.Add(m_colors[i].Copy());

                c.m_nbColors = m_nbColors;
                c.m_rnd = m_rnd;
                c.m_width = m_width;
                c.m_length = m_length;
                c.m_wave0 = m_wave0;
                c.m_wave1 = m_wave1;
                c.m_wave2 = m_wave2;
                c.m_rootOffset = m_rootOffset;
                c.m_rootThickness = m_rootThickness;
                c.m_noise = m_noise;
                c.m_ao = m_ao;
                c.m_specularRoot = m_specularRoot;
                c.m_specularTip = m_specularTip;
                c.m_specularShift = m_specularShift;
                c.m_specularMask = m_specularMask;
                c.m_depth = m_depth;
                

                return c;
            }


            public Vector4[] GetColorArray(bool root )
            {
                if(m_colors == null)
                    m_colors = new List<TextureToolColor>();

                Vector4[] a = new Vector4[3];

                if (m_nbColors > m_colors.Count)
                    return a;

                for (int i=0; i<m_nbColors; ++i)
                {
                    a[i] = Vector4.zero;
                    a[i].x = root ? m_colors[i].root.r : m_colors[i].tip.r;
                    a[i].y = root ? m_colors[i].root.g : m_colors[i].tip.g;
                    a[i].z = root ? m_colors[i].root.b : m_colors[i].tip.b;
                    a[i].w = root ? m_colors[i].root.a : m_colors[i].tip.a;
                }
                return a;
            }

            public float[] GetColorBias()
            {
                float[] b = new float[3];

                if (m_nbColors > m_colors.Count)
                    return b;

                for (int i = 0; i < m_nbColors; ++i)
                {
                    b[i] = m_colors[i].bias;
                }
                return b;
            }

        }


        [System.Serializable]
        public class AtlasParameters
        {
            public string path = "";
            public string filename;
            public float editorBgLight = .5f;            
            public int textureSize = 1024;
            public int sizeX = 1;
            public int sizeY = 1;
            public List<TextureToolShaderParameters> m_shaderParameters = new List<TextureToolShaderParameters>();

            public bool m_enableDiffuse = true;
            public bool m_enableNormal = true;
            public bool m_enableAO = true;
            public bool m_enableSpecular = true;
        }


        [System.Serializable]
        public class HairToolLayer
        {
            public bool visible = true;
        }




        [System.Serializable]
        public enum eInstancingMode
        {
            UNDEFINED,
            GPU_INSTANCING,
            MESH_INSTANCING,
            MESH_INSTANCING_INDIRECT
        }


        
    }
}