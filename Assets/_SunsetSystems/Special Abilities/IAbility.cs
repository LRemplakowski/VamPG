using System;
using SunsetSystems.Combat;
using SunsetSystems.Inventory;

namespace SunsetSystems.Abilities
{
    public interface IAbility
    {
        public ref IAbilityTargetingData GetTargetingData(IAbilityUser abilityUser);
        public ref IAbilityCostData GetActionPointCost(IAbilityUser abilityUser);
        public ref IAbilityUIData GetAbilityUIData();
        public AbilityCategory GetCategories();

        public bool Execute(IAbilityUser abilityUser, ITargetable target, Action onCompleted = null);
        public bool IsValidTarget(IAbilityUser abilityUser, ITargetable target);
    }

    public interface IAbilityTargetingData
    {
        AbilityTargetingType GetAbilityTargetingType();
        TargetableEntityType GetValidEntityTypesFlag();
        AbilityRange GetRangeType();
        RangeData GetRangeData();
    }
}
