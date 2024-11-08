using UnityEngine;
using UnityEngine.EventSystems;

namespace SunsetSystems.WorldMap
{
    public interface IWorldMapToken : IPointerClickHandler
    {
        IWorldMapData GetData();
        Vector3 GetTokenPosition();
        Vector3 GetPlayerTokenDestination();

        void SetUnlocked(bool unlocked);
        void SetVisible(bool visible);
        void InjectTokenManager(WorldMapUI uiManager);
    }
}
