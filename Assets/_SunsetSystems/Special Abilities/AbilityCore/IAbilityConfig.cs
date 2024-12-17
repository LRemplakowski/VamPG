namespace SunsetSystems.Abilities
{
    public interface IAbilityConfig
    {
        IAbilityTargetingData GetTargetingData(IAbilityContext context);
        IAbilityCostData GetAbilityCosts(IAbilityContext context);
        IAbilityUIData GetAbilityUIData();
        AbilityCategory GetCategories();

        bool IsContextValidForExecution(IAbilityContext context);

        IAbilityExecutionStrategy GetExecutionStrategy();
        IAbilityTargetingStrategy GetTargetingStrategy();
    }
}
