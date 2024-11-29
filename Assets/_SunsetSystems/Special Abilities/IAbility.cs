using System;
using SunsetSystems.Combat;

namespace SunsetSystems.Abilities
{
    public interface IAbility
    {
        public IAbilityTargetingData GetTargetingData(IAbilityUser abilityUser);
        public IAbilityCostData GetAbilityCosts(IAbilityUser abilityUser, ITargetable target);
        public IAbilityUIData GetAbilityUIData();
        public AbilityCategory GetCategories();

        public bool Execute(IAbilityUser abilityUser, ITargetable target, Action onCompleted = null);
        public bool IsValidTarget(IAbilityUser abilityUser, ITargetable target);
    }
}
