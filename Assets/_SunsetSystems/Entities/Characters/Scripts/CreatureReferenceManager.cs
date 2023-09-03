using SunsetSystems.Entities.Characters;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Creatures.Interfaces
{
    public class CreatureReferenceManager : CachedReferenceManager, ICreatureReferences
    {
        [field: SerializeField]
        public CreatureData CreatureData { get; private set; }
        [field: SerializeField]
        public StatsManager StatsManager { get; private set; }
        [field: SerializeField]
        public NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField]
        public NavMeshObstacle NavMeshObstacle { get; private set; }
    }
}
