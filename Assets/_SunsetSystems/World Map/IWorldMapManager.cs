using System.Collections.Generic;

namespace SunsetSystems.WorldMap
{
    public interface IWorldMapManager
    {
        bool EnterCurrentMap();
        void TravelToSelectedMap();

        void SetMapUnlocked(IWorldMapData mapData, bool unlocked);
        void SetSelectedMap(IWorldMapData mapData);

        IEnumerable<IWorldMapData> GetUnlockedMaps();
        bool IsMapUnlocked(IWorldMapData mapData);
        bool IsCurrentMap(IWorldMapData mapData);
        IWorldMapData GetCurrentMap();
        IWorldMapData GetPlayerHavenMap();
        IWorldMapData GetSelectedMap();
        TravelTimeData GetTravelTime(IWorldMapData start, IWorldMapData end);
        float GetDistanceBetweenAreas(IWorldMapData start, IWorldMapData end);
        float GetPlayerTravelSpeed();
    }
}
