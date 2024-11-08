using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunsetSystems.WorldMap
{
    public class WorldSpaceMapToken : SerializedMonoBehaviour, IWorldMapToken
    {
        [Title("Config")]
        [SerializeField, Required]
        private IWorldMapData _representedArea;
        [SerializeField]
        private IHighlightHandler _highlightHandler;
        [SerializeField, Required]
        private TextMeshPro _areaTitle;
        [SerializeField]
        private Transform _playerTokenDestination;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private WorldMapUI _uiManager;
        [ShowInInspector, ReadOnly]
        private bool _canHighlight;

        private void Start()
        {
            UpdateAreaTitle();
        }

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
            UpdateActiveGameObjects(visible);
            UpdateAreaTitle();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_canHighlight)
                if (eventData.button == PointerEventData.InputButton.Left)
                    _uiManager.LockTokenDescription(true, _representedArea, this);
        }

        private void UpdateAreaTitle()
        {
            if (_areaTitle != null && _representedArea != null)
                _areaTitle.SetText(_representedArea.GetAreaName());
        }

        private void UpdateActiveGameObjects(bool active)
        {
            gameObject.SetActive(active);
        }

        public Vector3 GetTokenPosition()
        {
            return transform.position;
        }

        public Vector3 GetPlayerTokenDestination()
        {
            return _playerTokenDestination.position;
        }
    }
}
