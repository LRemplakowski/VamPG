using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerShaderHDRP_FurShell : HairDesignerShader
        {

            HairDesignerGeneratorFurShell m_gfs;

            public Texture2D m_mainTex;
            public Vector2 m_mainTexScale = Vector2.one;
            public Vector2 m_mainTexOffset = Vector2.zero;

            public Texture2D m_densityTex;
            public Vector2 m_densityTexScale = new Vector2(20, 20);
            public Vector2 m_densityTexOffset = Vector2.zero;

            public Texture2D m_normalTex;
            public Vector2 m_normalStrength = new Vector2(.5f, .1f);

            public Texture2D m_maskTex;
            public Vector2 m_maskTexScale = Vector2.one;
            public Vector2 m_maskTexOffset = Vector2.zero;

            public Texture2D m_brushTex;

            public Texture2D m_colorTex;
            public Vector2 m_colorTexScale = Vector2.one;
            public Vector2 m_colorTexOffset = Vector2.zero;

            public float m_furLength = .1f;
            public float m_furDensity = 1.5f;
            public float m_thickness = 0f;

            public float m_curlNumber = 0f;
            public float m_curlRadius = 0f;
            //public float m_curlRnd = 0f;


            public float m_maskLOD = 0.5f;

            public float m_gravity = 0f;
            public float m_wind = 1f;
            public float m_windTurbulenceFrequency = .1f;

            public float m_brushFactor = 1f;
            public float m_lengthFactor = 1f;

            public float m_scale = 1f;
            public float m_smoothness1 = 0f;
            public float m_smoothness2 = 0f;
            public Color m_specular1 = Color.black;
            public Color m_specular2 = Color.black;
            public float m_specularShift1 = 0f;
            public float m_specularShift2 = 0f;

            //public float m_metallic = 0f;
            public float m_emission = 0f;
            public Vector2 m_emissionPower = new Vector2(.1f,.1f);            
            public float m_AO = .5f;

            public Color m_RimColor = Color.black;
            public float m_RimPower = 1f;

            public Color m_RootColor = Color.black;
            public Color m_TipColor = Color.white;
            public float m_ColorBias = .5f;
            public float m_ColorBiasLength = .5f;

            public float m_transparencyThreshold = .5f;
            public Vector2 m_BrushBend = Vector2.zero;

            public Vector2 m_furNoise = Vector2.zero;

            public int m_sortingPriority = 0;
            public override void SetTexture(int textureID, Texture2D tex)
            {
                switch (textureID)
                {
                    case 0: m_mainTex = tex; break;
                    case 1: m_densityTex = tex; break;
                    case 2: m_maskTex = tex; break;
                    case 3: m_brushTex = tex; break;
                    case 4: m_colorTex = tex; break;
                }

            }

            public override Texture2D GetTexture(int textureID)
            {
                switch (textureID)
                {
                    case 0: return m_mainTex;
                    case 1: return m_densityTex;
                    case 2: return m_maskTex;
                    case 3: return m_brushTex;
                    case 4: return m_colorTex;
                }
                return null;
            }


            public override bool InstancingModeCompatibility(eInstancingMode imode)
            {
                switch (imode)
                {
                    case eInstancingMode.UNDEFINED:
                    case eInstancingMode.GPU_INSTANCING:
                    case eInstancingMode.MESH_INSTANCING_INDIRECT:
                        return true;
                }
                return false;
            }



            public void GeneratorUpdate( Material mat )
            {
                if (m_generator != null)
                {
                    if (m_gfs == null)
                        m_gfs = (m_generator as HairDesignerGeneratorFurShell);

                    if (m_gfs != null && m_gfs.m_furMaterialOpaque != mat && m_gfs.m_furMaterialOpaque == null)
                    {
                        m_gfs.m_hiddenMaterial = Resources.Load("HD_HDRP_Hidden", typeof(Material)) as Material;
                        if (m_gfs.m_furMaterialOpaque == null)
                            m_gfs.m_furMaterialOpaque = new Material(Resources.Load("HD_HDRP_FurShell_Opaque", typeof(Material)) as Material);
                    }

                    if (m_gfs != null)
                    {
                        m_gfs.m_bakeVertexID = true;//bake the vertex ID in the UV2 for ShaderGraph compatibility.
                        m_gfs.m_transparencyThreshold = m_transparencyThreshold;
                        //m_gfs._useHDRPInstancingTrick = true;//DrawInstanced features are missing in current shadergraph version

                        if(m_gfs.RuntimeInstancingMode == eInstancingMode.UNDEFINED )
                        {
                            m_gfs.RuntimeInstancingMode = eInstancingMode.MESH_INSTANCING_INDIRECT;
                        }

                        switch (m_gfs.GetInstancingMode)
                        {                          

                            case eInstancingMode.MESH_INSTANCING:
                                mat.SetFloat("_InstancedTrick", 1);
                                mat.DisableKeyword("PROCEDURAL_INSTANCING_ON");
                                m_gfs._useHDRPInstancingTrick = true;
                                break;

                            case eInstancingMode.MESH_INSTANCING_INDIRECT:
                                mat.SetFloat("_InstancedTrick", -1);
                                mat.EnableKeyword("PROCEDURAL_INSTANCING_ON");
                                m_gfs._useHDRPInstancingTrick = false;
                                break;

                            default:
                                mat.DisableKeyword("PROCEDURAL_INSTANCING_ON");
                                mat.SetFloat("_InstancedTrick", 0);
                                m_gfs._useHDRPInstancingTrick = false;
                                break;
                        }


                    }
                    //gfs.m_shells
                    //gfs.m_meshRenderer.sortingOrder = m_sortingPriority;
                }


            }

            public override void UpdatePropertyBlock(ref MaterialPropertyBlock pb, HairDesignerBase.eLayerType lt)
            {
                //main shader doesn't use property block
            }


            public override void UpdateMaterialProperty(ref Material mat, HairDesignerBase.eLayerType lt)
            {

                GeneratorUpdate(mat);


                mat.SetTexture("_MainTex", m_mainTex);
                //mat.SetTextureScale("_MainTex", m_mainTexScale);
                //mat.SetTextureOffset("_MainTex", m_mainTexOffset);
                mat.SetVector("_MainTexScale", m_mainTexScale);

                mat.SetTexture("_DensityTex", m_densityTex);
                //mat.SetTextureScale("_DensityTex", m_densityTexScale);
                //mat.SetTextureOffset("_DensityTex", m_densityTexOffset);
                mat.SetVector("_DensityTexScale", m_densityTexScale);

               
                mat.SetTexture("_NormalTex", m_normalTex);
                mat.SetVector("_NormalStrength", m_normalStrength);

                mat.SetTexture("_MaskTex", m_maskTex);
                mat.SetTextureScale("_MaskTex", m_maskTexScale);
                mat.SetTextureOffset("_MaskTex", m_maskTexOffset);

                mat.SetTexture("_ColorTex", m_colorTex);
                mat.SetTextureScale("_ColorTex", m_colorTexScale);
                mat.SetTextureOffset("_ColorTex", m_colorTexOffset);

                mat.SetTexture("_BrushTex", m_brushTex);
                if (m_brushTex != null)
                {
                    //Debug.Log("-> Texture size" + m_brushTex.width);
                    mat.SetInt("_BrushTextureSize", m_brushTex.width);
                    mat.SetFloat("_BrushFactor", m_brushFactor);
                    mat.SetFloat("_LengthFactor", m_lengthFactor);
                    mat.SetVector("_BrushBend", m_BrushBend);
                }
                else
                {
                    mat.SetFloat("_BrushFactor", 0);
                    mat.SetFloat("_LengthFactor", 0);
                }


                float parentGlobalScale = 1f;
                if (transform.parent != null)
                    parentGlobalScale = (Mathf.Abs(transform.parent.lossyScale.x) + Mathf.Abs(transform.parent.lossyScale.y) + Mathf.Abs(transform.parent.lossyScale.z)) / 3f;


                


                if (m_hd == null)
                    return;

                mat.SetFloat("_FurLength", m_furLength * (m_hd.m_smr != null ? parentGlobalScale : 1f));
                mat.SetFloat("_Density", m_furDensity);
                mat.SetFloat("_Thickness", m_thickness);
                mat.SetFloat("_CurlNumber", m_curlNumber);
                mat.SetFloat("_CurlRadius", m_curlRadius * (m_hd.m_smr != null ? 1f : 1f));
                //mat.SetFloat("_CurlRnd", m_curlRnd);
                mat.SetFloat("_MaskLOD", m_maskLOD);

                mat.SetFloat("_Gravity", m_gravity);
                mat.SetFloat("KHD_windFactor", m_wind);
                mat.SetFloat("KHD_windZoneTurbulenceFrequency", m_windTurbulenceFrequency);
                mat.SetVector("KHD_windZoneDir", m_hd.m_windZoneDir);
                mat.SetVector("KHD_windZoneParam", m_hd.m_windZoneParam);

                mat.SetFloat("_Scale", m_scale);
                mat.SetFloat("_Smoothness1", m_smoothness1);
                mat.SetFloat("_Smoothness2", m_smoothness2);
                mat.SetFloat("_SpecularShift1", m_specularShift1);
                mat.SetFloat("_SpecularShift2", m_specularShift2);

                mat.SetColor("_Specular1", m_specular1);
                mat.SetColor("_Specular2", m_specular2);

                mat.SetFloat("_Emission", m_emission);
                mat.SetVector("_EmissionPower", m_emissionPower);                

                mat.SetFloat("_AO", m_AO);
                mat.SetColor("_RimColor", m_RimColor);
                mat.SetFloat("_RimPower", m_RimPower);

                mat.SetColor("_RootColor", m_RootColor);
                mat.SetColor("_TipColor", m_TipColor);
                mat.SetFloat("_ColorBias", m_ColorBias);
                mat.SetFloat("_ColorBiasLength", m_ColorBiasLength);

                mat.SetFloat("_TransparencyThreshold", m_transparencyThreshold);

                mat.SetVector("_FurNoise", m_furNoise);

                if (m_gfs != null)
                {
                    if (m_gfs.m_furMaterialOpaque != null && m_gfs.m_furMaterialOpaque != mat)
                        UpdateMaterialProperty(ref m_gfs.m_furMaterialOpaque, lt);                        
                }
            }

            /*
            public override void OnDestroy()
            {
                base.OnDestroy();
            }*/
             
    }
    }
}