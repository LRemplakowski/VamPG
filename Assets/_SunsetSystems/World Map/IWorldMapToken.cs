using UnityEngine.EventSystems;

namespace SunsetSystems.WorldMap
{
    public interface IWorldMapToken : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        IWorldMapData GetData();

        void SetUnlocked(bool unlocked);
        void SetVisible(bool visible);
        void InjectTokenManager(WorldMapUI uiManager);
    }
}
