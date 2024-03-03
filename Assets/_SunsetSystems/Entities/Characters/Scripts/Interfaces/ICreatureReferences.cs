using SunsetSystems.Core.UMA;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Equipment;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Creatures.Interfaces
{
    public interface ICreatureReferences : IEntityReferences
    {
        Transform BodyTransform { get; }
        GameObject BodyGameObject { get; }

        CreatureData CreatureData { get; }
        StatsManager StatsManager { get; }
        NavMeshAgent NavMeshAgent { get; }
        ICombatant CombatBehaviour { get; }
        IEquipmentManager EquipmentManager { get; }
        IWeaponManager WeaponManager { get; }
        IUMAManager UMAManager { get; }
    }
}
