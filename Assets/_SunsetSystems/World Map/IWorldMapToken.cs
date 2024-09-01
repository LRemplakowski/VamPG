using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    public interface IWorldMapToken
    {
        IWorldMapData GetData();

        void SetUnlocked(bool unlocked);
        void SetVisible(bool visible);

        void InjectTokenManager(WorldMapUI uiManager);
    }
}
