#if UNITY_EDITOR
using SunsetSystems.Journal;

namespace SunsetSystems.Core.Database
{
    public static class EditorDatabaseHelper
    {
        public static ItemDatabase ItemDB { get; set; }
        public static CreatureDatabase CreatureDB { get; set; }
        public static QuestDatabase QuestDB { get; set; }
        public static ObjectiveDatabase ObjectiveDB { get; set; }
    }
}
#endif
