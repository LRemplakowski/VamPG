using SunsetSystems.Core.Database;
using SunsetSystems.Core.SceneLoading;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    public interface IWorldMapData : IDatabaseEntry<IWorldMapData>
    {
        public const string AREA_NAME = "AREA_NAME";
        public const string AREA_DESCRIPTION = "AREA_DESCRIPTION";

        SceneLoadingDataAsset LevelData { get; }

        string GetAreaName();
        string GetDescription();
        Sprite GetIcon();
    }
}
