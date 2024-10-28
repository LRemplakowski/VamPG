using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunsetSystems.WorldMap
{
    public class WorldMapToken : SerializedMonoBehaviour, IWorldMapToken
    {
        [Title("References")]
        [SerializeField, Required]
        private IWorldMapData _tokenData;
        [SerializeField, Required]
        private Selectable _selectableToken;
        [SerializeField, Required]
        private TextMeshProUGUI _areaNameText;
        [SerializeField, Required]
        private Image _areaImageMiniature;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private WorldMapUI _uiManager;

        [Title("Editor Utility")]
        [SerializeField]
        private bool _autoUpdateVisuals = true;

        private void OnValidate()
        {
            if (_autoUpdateVisuals)
                SetupTokenVisuals();
        }

        private void Start()
        {
            SetupTokenVisuals();
        }

        public void InjectTokenManager(WorldMapUI uiManager) => _uiManager = uiManager;

        public void MapTokenClicked()
        {
            _uiManager.ShowAreaDescription(_tokenData);
        }

        [Button]
        private void SetupTokenVisuals()
        {
            if (_tokenData == null)
                return;
            _areaNameText.text = _tokenData.GetAreaName();
            _areaImageMiniature.sprite = _tokenData.GetIcon();
        }

        public IWorldMapData GetData()
        {
            return _tokenData;
        }

        public void SetUnlocked(bool unlocked)
        {
            _selectableToken.interactable = unlocked;
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}
