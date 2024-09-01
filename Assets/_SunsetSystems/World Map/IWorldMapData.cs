using SunsetSystems.Core.Database;
using SunsetSystems.Core.SceneLoading;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    public interface IWorldMapData : IDatabaseEntry<IWorldMapData>
    {
        SceneLoadingDataAsset LevelData { get; }

        string GetAreaName();
        string GetDescription();
        Sprite GetIcon();
    }
}
