using UnityEngine.EventSystems;

namespace SunsetSystems.WorldMap
{
    public interface IWorldMapToken : IPointerClickHandler
    {
        IWorldMapData GetData();

        void SetUnlocked(bool unlocked);
        void SetVisible(bool visible);
        void InjectTokenManager(WorldMapUI uiManager);
    }
}
