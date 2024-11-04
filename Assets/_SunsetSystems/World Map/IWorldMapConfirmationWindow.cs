using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    public interface IWorldMapConfirmationWindow
    {
        void ShowConfirmationWindow(IWorldMapData mapData);
        void HideConfirmationWindow();
    }
}
