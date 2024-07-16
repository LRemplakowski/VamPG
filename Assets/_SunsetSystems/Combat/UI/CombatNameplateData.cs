using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Inventory;

namespace SunsetSystems.Combat.UI
{
    public readonly struct CombatNameplateData
    {
        public readonly string Name;
        public readonly float CurrentHP, MaxHP;
        public readonly float HealthPercentage;
        public readonly WeaponType CurrentWeapon;

        public CombatNameplateData(ICombatant dataSource)
        {
            Name = dataSource.References.CreatureData.FullName;
            CurrentHP = dataSource.References.StatsManager.Health.GetValue();
            MaxHP = dataSource.References.StatsManager.Health.MaxValue;
            CurrentWeapon = dataSource.CurrentWeapon.WeaponType;
            HealthPercentage = CurrentHP / MaxHP;
        }
    }
}
