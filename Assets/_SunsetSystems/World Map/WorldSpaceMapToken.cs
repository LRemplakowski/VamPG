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
        private IHighlightHandler _highlightHanger;
        [SerializeField]
        private GameObject _linkedGameObject;
        [SerializeField, Required]
        private TextMeshPro _areaTitle;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private WorldMapUI _uiManager;
        [ShowInInspector, ReadOnly]
        private bool _canHighlight;

        private void Start()
        {
            MoveTokenToLinkedObjectPosition();
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
            if (_linkedGameObject != null)
                _linkedGameObject.SetActive(active);
            gameObject.SetActive(active);
        }

        public Vector3 GetTokenPosition()
        {
            return transform.position;
        }

        [Button]
        private void MoveTokenToLinkedObjectPosition()
        {
            if (_linkedGameObject != null)
                transform.SetPositionAndRotation(_linkedGameObject.transform.position, _linkedGameObject.transform.rotation);
        }
    }
}
