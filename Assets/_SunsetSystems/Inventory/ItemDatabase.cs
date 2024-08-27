using SunsetSystems.Inventory.Data;
using UnityEngine;

namespace SunsetSystems.Core.Database
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Sunset Inventory/Item Database")]
    public class ItemDatabase : AbstractDatabase<IBaseItem>
    {
        public static ItemDatabase Instance
        {
            get
            {
#if UNITY_EDITOR
                return EditorDatabaseHelper.ItemDB;
#else
                return DatabaseHolder.Instance.GetDatabase<ItemDatabase>();
#endif
            }
        }

#if UNITY_EDITOR
        protected override AbstractDatabase<IBaseItem> GetEditorInstance()
        {
            return Instance;
        }

        protected override void SetEditorInstance(AbstractDatabase<IBaseItem> instance)
        {
            EditorDatabaseHelper.ItemDB = instance as ItemDatabase;
        }
#endif
    }
}
