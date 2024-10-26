using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
using SunsetSystems.Core.SceneLoading;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    [CreateAssetMenu(fileName = "New World Map Asset", menuName = "Sunset World Map/World Map Asset")]
    public class WorldMapDataAsset : AbstractDatabaseEntry<IWorldMapData>, IWorldMapData
    {
        [field: SerializeField, ReadOnly]
        public override string DatabaseID { get; protected set; }
        [field: SerializeField]
        public override string ReadableID { get; protected set; }

        [SerializeField]
        private string _fallbackName;
        [SerializeField, MultiLineProperty]
        private string _fallbackDescription;
        [SerializeField]
        private Sprite _areaIcon;

        [field: SerializeField]
        public SceneLoadingDataAsset LevelData { get; private set; }

        public string GetAreaName()
        {
            if (TryGetLocalizedName(out string localized))
                return localized;
            else
                return _fallbackName;
        }

        public string GetDescription()
        {
            if (TryGetLocalizedDescription(out string localized))
                return localized;
            else
                return _fallbackDescription;
        }

        public Sprite GetIcon()
        {
            return _areaIcon;
        }

        private bool TryGetLocalizedName(out string localizedName)
        {
            localizedName = "";
            return false;
        }

        private bool TryGetLocalizedDescription(out string localizedDesc)
        {
            localizedDesc = "";
            return false;
        }

        #region Database
        protected override void RegisterToDatabase()
        {
            WorldMapEntryDatabase.Instance.Register(this);
        }

        protected override void UnregisterFromDatabase()
        {
            WorldMapEntryDatabase.Instance.Unregister(this);
        }
        #endregion
    }
}
