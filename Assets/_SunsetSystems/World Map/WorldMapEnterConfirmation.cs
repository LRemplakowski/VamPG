using System.Collections;
using System.Collections.Generic;
using SunsetSystems.WorldMap;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    public class WorldMapEntryConfirmation : AbstractWorldMapConfirmationWindow
    {
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
