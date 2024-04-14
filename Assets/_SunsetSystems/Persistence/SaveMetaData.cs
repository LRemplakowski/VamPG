using System;
using SunsetSystems.Core.SceneLoading;

namespace SunsetSystems.Persistence
{
    [Serializable]
    public struct SaveMetaData
    {
        public string SaveID;
        public string SaveName;
        public string SaveDate;
        public readonly string SaveFileName => $"{SaveID}.sav";
        public SceneLoadingData LevelAssetReference;
    }
}
