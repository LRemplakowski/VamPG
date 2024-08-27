using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters.Navigation;
using SunsetSystems.Equipment;
using SunsetSystems.Spellbook;
using SunsetSystems.UMA;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    public class CreatureReferenceManager : CachedReferenceManager, ICreatureReferences
    {
        [field: SerializeField]
        public CreatureData CreatureData { get; private set; }
        [field: SerializeField]
        public StatsManager StatsManager { get; private set; }
        [field: SerializeField]
        public INavigationManager NavigationManager { get; private set; }
        [field: SerializeField]
        public ICombatant CombatBehaviour { get; private set; }
        [field: SerializeField]
        public IEquipmentManager EquipmentManager { get; private set; }
        [field: SerializeField]
        public IWeaponManager WeaponManager { get; private set; }
        [field: SerializeField]
        public IUMAManager UMAManager { get; private set; }
        [field: SerializeField]
        public IMagicUser SpellbookManager { get; private set; }
    }
}
