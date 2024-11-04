using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    public class WorldMapTravelConfirmation : AbstractWorldMapConfirmationWindow
    {
        [Title("Travel Confirmation Config")]
        [SerializeField]
        private TextMeshProUGUI _travelText;

        public override void OnConfirm()
        {
            _uiManager.ConfirmTravelToSelectedArea();
        }

        public override void OnReject()
        {
            HideConfirmationWindow();
        }

        protected override void HandleWorldMapData(IWorldMapData worldMapData)
        {
            throw new NotImplementedException();
        }
    }
}
