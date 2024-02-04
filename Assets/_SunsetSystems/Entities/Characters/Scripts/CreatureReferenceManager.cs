using SunsetSystems.Core.UMA;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Equipment;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Creatures.Interfaces
{
    public class CreatureReferenceManager : CachedReferenceManager, ICreatureReferences
    {
        public override Transform Transform => NavMeshAgent.transform;

        [field: SerializeField]
        public CreatureData CreatureData { get; private set; }
        [field: SerializeField]
        public StatsManager StatsManager { get; private set; }
        [field: SerializeField]
        public NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField]
        public ICombatant CombatBehaviour { get; private set; }
        [field: SerializeField]
        public IEquipmentManager EquipmentManager { get; private set; }
        [field: SerializeField]
        public IWeaponManager WeaponManager { get; private set; }
        [field: SerializeField]
        public IUMAManager UMAManager { get; private set; }
    }
}
