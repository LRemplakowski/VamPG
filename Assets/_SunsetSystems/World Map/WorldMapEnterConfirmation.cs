using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.WorldMap;
using TMPro;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    public class WorldMapEntryConfirmation : AbstractWorldMapConfirmationWindow
    {
        [Title("Enter Confirmation Config")]
        [SerializeField]
        private TextMeshProUGUI _confirmationText;

        public override void OnConfirm()
        {
            _uiManager.ConfirmEntryToCurrentArea();
        }

        public override void OnReject()
        {
            HideConfirmationWindow();
        }

        protected override void HandleWorldMapData(IWorldMapData worldMapData)
        {
            throw new System.NotImplementedException();
        }
    }
}
