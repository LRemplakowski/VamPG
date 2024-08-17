using SunsetSystems.Core.Database;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "Objective Database", menuName = "Sunset Journal/Objective Database")]
    public class ObjectiveDatabase : AbstractDatabase<Objective>
    {
        public static ObjectiveDatabase Instance
        {
            get
            {
#if UNITY_EDITOR
                return EditorDatabaseHelper.ObjectiveDB;
#else
                return DatabaseHolder.Instance.GetDatabase<ObjectiveDatabase>();
#endif
            }
        }

#if UNITY_EDITOR
        protected override AbstractDatabase<Objective> GetEditorInstance()
        {
            return Instance;
        }

        protected override void SetEditorInstance(AbstractDatabase<Objective> instance)
        {
            EditorDatabaseHelper.ObjectiveDB = instance as ObjectiveDatabase;
        }
#endif
    }
}
