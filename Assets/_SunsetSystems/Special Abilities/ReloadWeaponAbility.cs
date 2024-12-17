using System;
using SunsetSystems.Abilities.Execution;
using SunsetSystems.ActionSystem;
using SunsetSystems.Combat;
using SunsetSystems.Entities;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Reload Ability", menuName = "Sunset Abilities/Weapon Reload")]
    public class ReloadWeaponAbility : AbstractAbilityConfig
    {
        private IAbilityExecutionStrategy _executionStrategy;
        private IAbilityTargetingStrategy _targetingStrategy;

        protected override bool HasValidTarget(IAbilityContext context, TargetableEntityType validTargetsFlag)
        {
            return IsTargetNotNull(context)
                   && GetTargetHasValidFlag(context, validTargetsFlag)
                   && IsTargetCombatContextProvider(context, out var combatContext)
                   && IsTargetWeaponUsingAmmo(combatContext);

            static bool GetTargetHasValidFlag(IAbilityContext context, TargetableEntityType flag) => context.TargetObject.IsValidTarget(flag);
            static bool IsTargetNotNull(IAbilityContext context) => context.TargetObject != null;

            static bool IsTargetCombatContextProvider(IAbilityContext context, out ICombatContext combatContext)
            {
                combatContext = default;
                if (context.TargetObject is IContextProvider<ICombatContext> combatContextProvider)
                {
                    combatContext = combatContextProvider.GetContext();
                    return true;
                }
                return false;
            }

            static bool IsTargetWeaponUsingAmmo(ICombatContext context)
            {
                return context != null && context.IsSelectedWeaponUsingAmmo;
            }
        }

        protected override RangeData GetAbilityRangeData(IAbilityContext context)
        {
            return new();
        }

        public override IAbilityExecutionStrategy GetExecutionStrategy() => _executionStrategy ??= new ReloadStrategy();

        public override IAbilityTargetingStrategy GetTargetingStrategy() => throw new NotImplementedException();
    }
}
