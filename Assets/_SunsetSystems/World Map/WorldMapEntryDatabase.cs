using SunsetSystems.Core.Database;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    [CreateAssetMenu(fileName = "New World Map Database", menuName = "Sunset World Map/Database")]
    public class WorldMapEntryDatabase : AbstractDatabase<IWorldMapData>
    {
        public static WorldMapEntryDatabase Instance
        {
            get
            {
#if UNITY_EDITOR
                return EditorDatabaseHelper.WorldMapDB;
#else
                return DatabaseHolder.Instance.GetDatabase<WorldMapEntryDatabase>();
#endif
            }
        }

#if UNITY_EDITOR
        protected override AbstractDatabase<IWorldMapData> GetEditorInstance()
        {
            return Instance;
        }

        protected override void SetEditorInstance(AbstractDatabase<IWorldMapData> instance)
        {
            EditorDatabaseHelper.WorldMapDB = instance as WorldMapEntryDatabase;
        }
#endif
    }
}
