using SunsetSystems.UMA;
using SunsetSystems.Entities.Characters.Navigation;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Equipment;
using SunsetSystems.Combat;

namespace SunsetSystems.Entities.Characters
{
    public interface ICreatureReferences : IEntityReferences
    {
        CreatureData CreatureData { get; }
        StatsManager StatsManager { get; }
        INavigationManager NavigationManager { get; }
        ICombatant CombatBehaviour { get; }
        IEquipmentManager EquipmentManager { get; }
        IWeaponManager WeaponManager { get; }
        IUMAManager UMAManager { get; }
    }
}
