using SunsetSystems.Core.Database;
using SunsetSystems.Journal;
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
    }
}
