using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        [ExecuteInEditMode]
        public class HairDesignerShaderAdvancedFur : HairDesignerShader
        {
            [System.Serializable]
            public class EditorSettings
            {
                public List<bool> toggleGroups = new List<bool>();                
            }

            [System.Serializable]
            public class LODData
            {
                public Vector2 m_range = new Vector2(0, 100);
                public int m_strandDensity = 2;
                public int m_strandSubdivision = 3;
                public int m_shellCount = 5;
                public float m_shellCoverage = .5f;
                public UnityEngine.Rendering.ShadowCastingMode m_strandsShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                public UnityEngine.Rendering.ShadowCastingMode m_shellsShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                public bool m_updateMotion = true;
            }


            public HairDesignerGeneratorAdvancedFurBase m_gaf;
            public EditorSettings m_editorSettings = new EditorSettings();
            public AtlasParameters m_atlasParameters;


            public Texture2D m_mainTex;
            public Vector2 m_mainTexScale = Vector2.one;
            public Vector2 m_mainTexOffset = Vector2.zero;

            public Texture2D m_densityTex;
            public Vector2 m_densityTexScale = new Vector2(20,20);
            public Vector2 m_densityTexOffset = Vector2.zero;
            public Texture2D m_densityNormalTex;

            public Texture2D m_maskTex;
            public Vector2 m_maskTexScale = Vector2.one;
            public Vector2 m_maskTexOffset = Vector2.zero;

            public Texture2D m_brushTex;

            public Texture2D m_furTex;
            public Vector2 m_furTexScale = Vector2.one;
            public Vector2 m_furTexOffset = Vector2.zero;

            public Texture2D m_normalTex;
            public Vector2 m_normalTexScale = Vector2.one;
            public Vector2 m_normalTexOffset = Vector2.zero;

            public Texture2D m_colorTex;
            public Vector2 m_colorTexScale = Vector2.one;
            public Vector2 m_colorTexOffset = Vector2.zero;

            public float m_normalStrength = 1f;
            public float m_normalSwitch = .1f;
            public float m_colorTexIntensity = 1f;

            public float m_shellHeight = .3f;

            public float m_length = .1f;
            public float m_strandRandomLength = .1f;
            //public float m_furLength = .1f;
            public float m_ShellThickness = 1f;
            public float m_ShellCoverage = 0f;

            public float m_gravity = 0f;
            public float m_wind = 1f;
            public float m_windTurbulenceFrequency = .1f;

            public float m_brushStrength = 1f;
            public float m_scale = 1f;
            public float m_smoothness = 0f;
            public float m_metallic = 0f;
            public float m_emission = 0f;
            public Color m_rootEmissionColor = Color.black;
            public Color m_tipEmissionColor = Color.black;
            public float m_emissionThreshold = 1f;
            
            public float m_AO = .2f;

            public Color m_rimColor = Color.black;
            public float m_rimPower = 1f;

            public Color m_strandRootColor = Color.black;
            public Color m_strandTipColor = Color.white;
            public float m_strandColorThreshold = .2f;

            public Color m_shellRootColor = Color.white;
            public Color m_shellTipColor = Color.white;
            public float m_shellColorThreshold = .5f;

            public int m_shellCount = 10;
            public float m_strandDensity = 1;
            public float m_strandHeightMin = .1f;
            public float m_strandWidth = 5f;
            public float m_strandWaveFrequency = 1f;
            public float m_strandWaveAmplitude = 0f;
            public float m_strandWidthConstantOrPerFaceFactor = 0f;
            public float m_strandFlexibility = .1f;
            public int m_strandSubdivision = 2;
            public float m_strandPinch = 0;
            public float m_strandOrientation = 0f;

            public float m_translucency = .1f;//WIP
            public float m_randomSeed = 0f;
            

            public float m_cutout = .01f;
            //public float m_cutoutAlpha = .1f;

            public float m_motionFactor = 5f;


            public bool m_useLOD = false;
            public List<LODData> m_LODSettings = new List<LODData>();
            public int m_currentLOD = -1;

            public override void SetTexture(int textureID, Texture2D tex)
            {
                switch (textureID)
                {
                    case 0:m_mainTex = tex;break;
                    case 1:m_densityTex = tex;break;
                    case 2:m_maskTex = tex;break;
                    case 3:m_brushTex = tex;break;
                    case 4:m_colorTex = tex;break;
                }

            }

            public override Texture2D GetTexture(int textureID)
            {
                switch (textureID)
                {
                    case 0:return m_mainTex;
                    case 1:return m_densityTex;
                    case 2:return m_maskTex;
                    case 3:return m_brushTex;
                    case 4:return m_colorTex;
                }
                return null;
            }



            public override void UpdatePropertyBlock(ref MaterialPropertyBlock pb, HairDesignerBase.eLayerType lt)
            {
                if( !Application.isPlaying )
                    pb.Clear();

                if (m_mainTex != null)
                {
                    pb.SetTexture("_MainTex", m_mainTex);
                    pb.SetVector("_MainTex_ST", m_mainTexScale);
                    //pb.SetTextureOffset("_MainTex", m_mainTexOffset);
                }
                if (m_densityTex != null)
                {
                    pb.SetTexture("_DensityTex", m_densityTex);
                    pb.SetVector("_DensityTex_ST", m_densityTexScale);
                    //pb.SetTextureOffset("_DensityTex", m_densityTexOffset);
                }

                if (m_densityNormalTex != null)
                {
                    pb.SetTexture("_DensityNormalTex", m_densityNormalTex);
                    
                }


                if (m_maskTex != null)
                {
                    pb.SetTexture("_MaskTex", m_maskTex);
                    pb.SetVector("_MaskTex_ST", m_maskTexScale);
                    //pb.SetTextureOffset("_MaskTex", m_maskTexOffset);
                }

                if (m_colorTex != null)
                    pb.SetTexture("_ColorTex", m_colorTex);
                //pb.SetTextureScale("_ColorTex", m_colorTexScale);
                //pb.SetTextureOffset("_ColorTex", m_colorTexOffset);

                if (m_furTex != null)
                    pb.SetTexture("_FurTex", m_furTex);


                if (m_normalTex != null)
                    pb.SetTexture("_NormalMap", m_normalTex);



                if (m_brushTex != null)
                    pb.SetTexture("_BrushTex", m_brushTex);

                if (m_brushTex != null)
                {
                    pb.SetFloat("_BrushTextureSize", m_brushTex.width);
                    pb.SetFloat("_BrushStrength", m_brushStrength);
                }
                else
                {
                    pb.SetFloat("_BrushStrength", 0);
                }

                /*
                    float parentGlobalScale = 1f;
                if(transform.parent != null )
                    parentGlobalScale = (Mathf.Abs(transform.parent.lossyScale.x) + Mathf.Abs(transform.parent.lossyScale.y) + Mathf.Abs(transform.parent.lossyScale.z)) / 3f;
                    */
                if (m_hd == null)
                    return;



                pb.SetFloat("_Cutout", m_cutout);
                //pb.SetFloat("_CutoutAlpha", m_cutoutAlpha);

                pb.SetFloat("_Size", m_strandWidth);
                pb.SetFloat("_SizeFaceFactor", m_strandWidthConstantOrPerFaceFactor);

                pb.SetFloat("_ShellHeight", m_shellHeight);                
                pb.SetFloat("_Length", m_length);


                pb.SetFloat("_NormalStrength", m_normalStrength);
                pb.SetFloat("_NormalSwitch", m_normalSwitch);

                pb.SetFloat("_ColorTexIntensity", m_colorTexIntensity);

                //pb.SetFloat("_FurLength", m_furLength * (m_hd.m_smr != null ? parentGlobalScale: 1f ) );
                pb.SetFloat("_ShellThickness", m_ShellThickness);
                pb.SetFloat("_MaskLOD", m_ShellCoverage);

                pb.SetFloat("_Gravity", m_gravity);
                //pb.SetFloat("_Gravity", 0);
                pb.SetFloat("_ShellCount", m_shellCount);

                pb.SetFloat("_Smoothness", m_smoothness);
                pb.SetFloat("_Metallic", m_metallic);

                pb.SetColor("_RootEmissionColor", m_rootEmissionColor);
                pb.SetColor("_TipEmissionColor", m_tipEmissionColor);
                pb.SetFloat("_EmissionThreshold", m_emissionThreshold);
                pb.SetFloat("_Emission", m_emission);

                pb.SetFloat("_StrandAO", m_AO);
                pb.SetColor("_RimColor", m_rimColor);
                pb.SetFloat("_RimPower", m_rimPower);

                pb.SetColor("_StrandRootColor", m_strandRootColor);
                pb.SetColor("_StrandTipColor", m_strandTipColor);
                pb.SetFloat("_StrandColorThreshold", m_strandColorThreshold);

                pb.SetColor("_ShellRootColor", m_shellRootColor);
                pb.SetColor("_ShellTipColor", m_shellTipColor);
                pb.SetFloat("_ShellColorThreshold", m_shellColorThreshold);


                pb.SetFloat("_TessEdge", m_strandDensity);
                pb.SetFloat("_StrandHeightMin", m_strandHeightMin);
                pb.SetFloat("_StrandFlexibility", m_strandFlexibility);
                pb.SetFloat("_StrandRandomLength", m_strandRandomLength);
                pb.SetFloat("_StrandOrientation", m_strandOrientation);
                pb.SetFloat("_StrandWaveFrequency", m_strandWaveFrequency);
                pb.SetFloat("_StrandWaveAmplitude", m_strandWaveAmplitude);

                pb.SetFloat("_Subdivision", m_strandSubdivision);

                if(Application.isPlaying)
                    pb.SetFloat("_MotionFactor", m_motionFactor);
                else
                    pb.SetFloat("_MotionFactor", 0);

                pb.SetFloat("_Pinch", m_strandPinch);

                pb.SetFloat("_Translucency", m_translucency);
                pb.SetFloat("_RandomSeed", m_randomSeed);

                if (m_atlasParameters != null)
                    pb.SetFloat("_AtlasSize", m_atlasParameters.sizeX);

                pb.SetFloat("_GlobalScale", m_hd.globalScale);
            }



            
            public void OnDestroy()
            {
                OnDisable();
            }



            [System.NonSerialized]
            public System.Action OnLODCallback = null;

            //ignore cam list prevent to check the LOD for unwanted camera
            public static List<Camera> IgnoreCameList = new List<Camera>();


            void OnDisable()
            {
                Camera.onPreCull -= CameraPreCull;                
            }

            void OnEnable()
            {
                Camera.onPreCull += CameraPreCull;                
            }




            public void CameraPreCull(Camera cam)
            {
 
                if (Application.isPlaying)
                {
                    if (cam.cameraType != CameraType.Game)
                        return;
                }
                else
                {
                    if (cam.cameraType != CameraType.SceneView)
                        return;
                }


                if (m_gaf == null)
                    m_gaf = m_generator as HairDesignerGeneratorAdvancedFurBase;


                if (IgnoreCameList.Count>0)
                {
                    if (IgnoreCameList.Contains(cam))
                    {
                        HideFur();
                        return;
                    }
                }
                

                if (!m_useLOD || m_LODSettings.Count == 0 || m_generator.m_matPropBlkHair == null || !m_generator.IsActive)
                {
                    m_currentLOD = -1;
                    m_gaf.m_updateMotion = m_gaf.m_motionData.enable;                    
                    return;
                }

                

                float dist = Vector3.Distance(transform.position, cam.transform.position);
                bool found = false;
                for ( int i=0; i< m_LODSettings.Count; ++i )
                {
                    if(dist>=m_LODSettings[i].m_range.x && dist < m_LODSettings[i].m_range.y)
                    {
                        found = true;

                        if (m_currentLOD != i)
                        {
                            m_currentLOD = i;                            
                            
                            m_strandDensity = m_LODSettings[i].m_strandDensity;
                            m_strandSubdivision = m_LODSettings[i].m_strandSubdivision;
                            m_shellCount = m_LODSettings[i].m_shellCount;
                            m_ShellCoverage = m_LODSettings[i].m_shellCoverage;

                            m_gaf.m_updateMotion = m_LODSettings[i].m_updateMotion;
                            m_gaf.m_shellsShadowCastingMode = m_LODSettings[i].m_shellsShadowCastingMode;
                            m_gaf.m_strandsShadowCastingMode = m_LODSettings[i].m_strandsShadowCastingMode;
                            m_gaf.m_HideStrands = m_strandSubdivision <= 0;
                            m_gaf.m_HideShells = m_shellCount <= 0;

                            m_gaf.m_shaderNeedUpdate = true;
                            m_gaf.UpdateInstance();


                            if (OnLODCallback != null)
                                OnLODCallback();

                        }

                        return;
                    }
                }

                if (!found && m_LODSettings.Count > 0 )
                {
                    //out of LOD range
                    if (dist > m_LODSettings[m_LODSettings.Count-1].m_range.y)
                    {
                        HideFur();
                    }
                }
               
            }



            void HideFur()
            {
                m_gaf.m_HideStrands = true;
                m_gaf.m_HideShells = true;
                m_gaf.m_updateMotion = false;               
                if (m_currentLOD != -1)
                {
                    m_gaf.UpdateInstance();//force the update for hidding fur
                    m_currentLOD = -1;
                }

            }
        }
    }
}
