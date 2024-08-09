using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using UnityEngine;

namespace SunsetSystems.Core.Database
{
    [CreateAssetMenu(fileName = "Creature Database", menuName = "Entities/Creature Database")]
    public class CreatureDatabase : AbstractDatabase<CreatureConfig>
    {
        public List<string> ReadableIDs => _readableIDRegistry.Keys.ToList();

        public static CreatureDatabase Instance
        {
            get
            {
#if UNITY_EDITOR
                return EditorDatabaseHelper.CreatureDB;
#else
                return DatabaseHolder.Instance.GetDatabase<CreatureDatabase>()
#endif
            }
        }
    }
}
