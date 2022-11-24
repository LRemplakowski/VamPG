using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FadeCameraObstruction : MonoBehaviour
{
    [SerializeField]
    private Camera myCamera;
    [SerializeField]
    private Transform cameraFocus;
    private float raycastRange;
    [SerializeField]
    private LayerMask ignoreObstruction;

    private readonly Dictionary<Material, PreTransparentProperties> cache = new();
    private Renderer lastHit;

    // Start is called before the first frame update
    private void Start()
    {
        myCamera = GetComponent<Camera>();
        if (cameraFocus != null)
        {
            raycastRange = Vector3.Distance(transform.position, cameraFocus.position) - 1f;
        }
    }

    private void LateUpdate()
    {
        Ray ray = myCamera.ScreenPointToRay(new Vector2(myCamera.pixelWidth / 2, myCamera.pixelHeight / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, ignoreObstruction, QueryTriggerInteraction.Ignore))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer)
            {
                if (renderer.Equals(lastHit))
                {
                    return;
                }
                else
                {
                    foreach (Material mat in lastHit.materials)
                    {
                        RestoreCachedProperties(mat, cache[mat]);
                    }
                    cache.Clear();
                }
                foreach (Material mat in renderer.materials)
                {
                    cache.Add(mat, CacheMaterialProperties(mat));
                    MakeMaterialTransparent(mat);
                }
                lastHit = renderer;
            }
        }

        void RestoreCachedProperties(Material mat, PreTransparentProperties cachedProperties)
        {
            mat.SetFloat("_Mode", cachedProperties.mode);
            mat.SetInt("_SrcBlend", cachedProperties.srcBlend);
            mat.SetInt("_DstBlend", cachedProperties.dstBlend);
            if (cachedProperties.alphaTest)
                mat.EnableKeyword("_ALPHATEST_ON");
            else
                mat.DisableKeyword("_ALPHATEST_ON");
            if (cachedProperties.alphaBlend)
                mat.EnableKeyword("_ALPHABLEND_ON");
            else
                mat.DisableKeyword("_ALPHABLEND_ON");
            if (cachedProperties.alphaPreMultiply)
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            else
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = cachedProperties.renderQueue;
        }

        void MakeMaterialTransparent(Material mat)
        {
            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        PreTransparentProperties CacheMaterialProperties(Material mat)
        {
            PreTransparentProperties cachedProperties = new();
            cachedProperties.mode = mat.GetFloat("_Mode");
            cachedProperties.srcBlend = mat.GetInt("_SrcBlend");
            cachedProperties.dstBlend = mat.GetInt("_DstBlend");
            cachedProperties.alphaTest = mat.IsKeywordEnabled("_ALPHATEST_ON");
            cachedProperties.alphaBlend = mat.IsKeywordEnabled("_ALPHABLEND_ON");
            cachedProperties.alphaPreMultiply = mat.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON");
            cachedProperties.renderQueue = mat.renderQueue;
            return cachedProperties;
        }
    }

    private struct PreTransparentProperties
    {
        public float mode;
        public int srcBlend, dstBlend, renderQueue;
        public bool alphaTest, alphaBlend, alphaPreMultiply;
    }
}
