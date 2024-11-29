using SunsetSystems.Inventory;

namespace SunsetSystems.Abilities
{
    public interface IAbilityTargetingData
    {
        AbilityTargetingType GetAbilityTargetingType();
        TargetableEntityType GetValidEntityTypesFlag();
        AbilityRange GetRangeType();
        RangeData GetRangeData();
    }
}
