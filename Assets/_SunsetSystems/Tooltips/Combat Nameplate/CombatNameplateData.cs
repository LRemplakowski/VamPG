using SunsetSystems.Combat;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Tooltips
{
    public class CombatNameplateData : ITooltipContext
    {
        private readonly ICombatant _dataSource;
        public string Name => _dataSource.References.CreatureData.FullName;
        public float CurrentHP => _dataSource.References.StatsManager.Health.GetValue();
        public float MaxHP => _dataSource.References.StatsManager.Health.MaxValue;
        public float HealthPercentage => CurrentHP / MaxHP;
        public AbilityRange CurrentWeapon => _dataSource.GetContext().WeaponManager.GetSelectedWeapon().WeaponType;

        public GameObject TooltipSource => _dataSource.References.GameObject;
        public Vector3 TooltipPosition => _dataSource.NameplatePosition;

        public CombatNameplateData(ICombatant dataSource)
        {
            _dataSource = dataSource;
        }
    }
}
