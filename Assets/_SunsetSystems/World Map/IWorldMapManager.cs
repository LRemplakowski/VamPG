using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    public interface IWorldMapManager
    {
        IEnumerable<IWorldMapData> GetUnlockedMaps();
        bool IsMapUnlocked(IWorldMapData mapData);
        void SetMapUnlocked(IWorldMapData mapData, bool unlocked);
        void TravelToMap(IWorldMapData mapData);
    }
}
