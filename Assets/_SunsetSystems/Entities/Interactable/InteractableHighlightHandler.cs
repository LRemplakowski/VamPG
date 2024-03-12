using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class InteractableHighlightHandler : SerializedMonoBehaviour, IHighlightHandler
    {
        [Title("References")]
        [SerializeField, Required, AssetsOnly]
        private Material _highlightMaterial;
        [SerializeField]
        private bool _findRenderersAtRuntime = false;
        [SerializeField, HideIf("@this._findRenderersAtRuntime == true")]
        private List<Renderer> _highlightRenderers = new();
        [SerializeField, ShowIf("@this._findRenderersAtRuntime == true"), Required]
        private GameObject _rendererParent;
        [Title("Config")]
        [SerializeField, Min(0f)]
        private float _materialLerpSpeed = 4;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Dictionary<Renderer, List<Material>> _defaultMaterialsCache = new();
        [ShowInInspector, ReadOnly]
        private Dictionary<Renderer, List<Material>> _highlightedMaterialsCache = new();
        [ShowInInspector, ReadOnly]
        private bool _highlighted = false;
        private IEnumerator _lerpMaterialsCoroutine;

        private void Start()
        {
            if (_findRenderersAtRuntime && _rendererParent != null)
                _highlightRenderers = _rendererParent.GetComponentsInChildren<Renderer>().ToList();
            foreach (Renderer renderer in _highlightRenderers)
            {
                List<Material> defaultMaterials = new();
                List<Material> highlighted = new();
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    defaultMaterials.Add(renderer.materials[i]);
                    highlighted.Add(_highlightMaterial);
                }
                _defaultMaterialsCache[renderer] = defaultMaterials;
                _highlightedMaterialsCache[renderer] = highlighted;
            }
        }

        private IEnumerator LerpMaterialsHighlight()
        {
            float lerpTime = _highlighted ? 0f : 1f;
            float lerpTarget = 1f - lerpTime;
            if (_materialLerpSpeed == 0f)
            {
                LerpMaterialsOverTime(lerpTarget);
            }
            else
            {
                while (_highlighted ? lerpTime < lerpTarget : lerpTime > lerpTarget)
                {
                    lerpTime += Time.deltaTime * _materialLerpSpeed * (_highlighted ? 1 : -1);
                    LerpMaterialsOverTime(lerpTime);
                    yield return null;
                }
            }
            _lerpMaterialsCoroutine = null;

            void LerpMaterialsOverTime(float lerpTime)
            {
                foreach (Renderer renderer in _highlightRenderers)
                {
                    if (_defaultMaterialsCache.TryGetValue(renderer, out List<Material> defaultMats) && _highlightedMaterialsCache.TryGetValue(renderer, out List<Material> highlightMats))
                    {
                        for (int i = 0; i < renderer.materials.Length; i++)
                        {
                            Material materialInstance = renderer.materials[i];
                            materialInstance.Lerp(defaultMats[i], highlightMats[i], lerpTime);
                        }
                    }
                }
            }
        }

        public void SetHighlightActive(bool active)
        {
            _highlighted = active;
            SwapMaterials();
            //if (_lerpMaterialsCoroutine != null)
            //    StopCoroutine(_lerpMaterialsCoroutine);
            //_lerpMaterialsCoroutine = LerpMaterialsHighlight();
            //StartCoroutine(_lerpMaterialsCoroutine);
        }

        private void SwapMaterials()
        {
            foreach (Renderer renderer in _highlightRenderers)
            {
                if (_defaultMaterialsCache.TryGetValue(renderer, out List<Material> defaultMats) && _highlightedMaterialsCache.TryGetValue(renderer, out List<Material> highlightMats))
                {
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        renderer.SetMaterials(_highlighted ? highlightMats : defaultMats);
                    }
                }
            }
        }
    }
}
