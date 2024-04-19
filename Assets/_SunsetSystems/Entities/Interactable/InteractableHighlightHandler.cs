using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class InteractableHighlightHandler : SerializedMonoBehaviour, IHighlightHandler
    {
        [Title("Config")]
        [SerializeField, ValueDropdown("GetLayerNames")]
        private string _highlightLayer = "Highlighted";
        [SerializeField]
        private bool _findRenderersAtRuntime = false;
        [SerializeField, HideIf("@this._findRenderersAtRuntime == true")]
        private List<Renderer> _highlightRenderers = new();
        [SerializeField, ShowIf("@this._findRenderersAtRuntime == true"), Required]
        private GameObject _rendererParent;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Dictionary<Renderer, int> _rendererLayerCache = new();

#if UNITY_EDITOR
        public string[] GetLayerNames()
        {
            return Enumerable.Range(0, 31).Select(index => LayerMask.LayerToName(index)).Where(l => !string.IsNullOrEmpty(l)).ToArray();
        }
#endif

        private void Start()
        {
            if (_findRenderersAtRuntime && _rendererParent != null)
                _highlightRenderers = _rendererParent.GetComponentsInChildren<Renderer>().ToList();
            foreach (Renderer renderer in _highlightRenderers)
            {
                _rendererLayerCache[renderer] = renderer.gameObject.layer;
            }
        }

        public void SetHighlightActive(bool active)
        {
            int highlightedLayer = LayerMask.NameToLayer(_highlightLayer);
            foreach (var renderer in _highlightRenderers)
            {
                renderer.gameObject.layer = active ? highlightedLayer : _rendererLayerCache[renderer];                
            }
        }
    }
}
