using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interactable;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunsetSystems.WorldMap
{
    public class WorldSpaceMapToken : SerializedMonoBehaviour, IWorldMapToken
    {
        [Title("Config")]
        [SerializeField]
        private IWorldMapData _representedArea;
        [SerializeField]
        private IHighlightHandler _highlightHanger;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private WorldMapUI _uiManager;
        [ShowInInspector, ReadOnly]
        private bool _canHighlight;

        public IWorldMapData GetData()
        {
            return _representedArea;
        }

        public void InjectTokenManager(WorldMapUI uiManager) => _uiManager = uiManager;

        public void SetUnlocked(bool unlocked)
        {
            _canHighlight = unlocked;
        }

        public void SetVisible(bool visible)
        {
            //gameObject.SetActive(visible);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _uiManager.LockTokenDescription(true, _representedArea);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _uiManager.HandleTokenHoveredOver(true, _representedArea);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _uiManager.HandleTokenHoveredOver(false, _representedArea);
        }
    }
}
