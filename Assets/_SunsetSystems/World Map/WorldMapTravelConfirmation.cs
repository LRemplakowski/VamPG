using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace SunsetSystems.WorldMap
{
    public class WorldMapTravelConfirmation : AbstractWorldMapConfirmationWindow
    {
        [Title("Travel Confirmation Config")]
        [SerializeField]
        private TextMeshProUGUI _travelText;
        [SerializeField]
        private LocalizedString _localizedString;

        private void Awake()
        {
            _localizedString.StringChanged += OnStringUpdated;
        }

        private void OnDestroy()
        {
            _localizedString.StringChanged -= OnStringUpdated;
        }

        private void OnStringUpdated(string value)
        {
            _travelText.text = value;
        }

        public override void OnConfirm()
        {
            _uiManager.ConfirmTravelToSelectedArea();
            _uiManager.ToogleTravelConfirmationPopup(false);
        }

        public override void OnReject()
        {
            _uiManager.ToogleTravelConfirmationPopup(false);
        }

        protected override void HandleWorldMapData(IWorldMapData worldMapData)
        {
            if (_localizedString.TryGetValue(IWorldMapData.AREA_NAME, out var areaName) && areaName is StringVariable areaNameString)
                areaNameString.Value = worldMapData.GetAreaName();
        }
    }
}
