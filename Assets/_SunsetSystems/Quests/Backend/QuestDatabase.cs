using SunsetSystems.Core.Database;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "Quest Database", menuName = "Sunset Journal/Quest Database")]
    public class QuestDatabase : AbstractDatabase<Quest>
    {
        public static QuestDatabase Instance
        {
            get
            {
#if UNITY_EDITOR
                return EditorDatabaseHelper.QuestDB;
#else
                return DatabaseHolder.Instance.GetDatabase<QuestDatabase>();
#endif
            }
        }

#if UNITY_EDITOR
        protected override AbstractDatabase<Quest> GetEditorInstance()
        {
            return Instance;
        }

        protected override void SetEditorInstance(AbstractDatabase<Quest> instance)
        {
            EditorDatabaseHelper.QuestDB = instance as QuestDatabase;
        }
#endif
    }
}
