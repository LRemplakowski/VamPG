using SunsetSystems.Combat;
using SunsetSystems.Inventory;
using SunsetSystems.Spellbook;

namespace SunsetSystems.Abilities
{
    public interface IAbility
    {
        public IAbilityTargetingData GetTargetingData();

        public bool Execute(ITargetable target);
    }

    public interface IAbilityTargetingData
    {
        AbilityTargetingType GetAbilityTargetingType();
        TargetableEntityType GetValidEntityTypesFlag();
        AbilityRange GetRangeType();
        RangeData GetRangeData();
    }
}
