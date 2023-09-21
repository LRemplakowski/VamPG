using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Equipment;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Creatures.Interfaces
{
    public interface ICreatureReferences : IEntityReferences
    {
        CreatureData CreatureData { get; }
        StatsManager StatsManager { get; }
        NavMeshAgent NavMeshAgent { get; }
        NavMeshObstacle NavMeshObstacle { get; }
        ICombatant CombatComponent { get; }
        IEquipment EquipmentComponent { get; }
    }
}
