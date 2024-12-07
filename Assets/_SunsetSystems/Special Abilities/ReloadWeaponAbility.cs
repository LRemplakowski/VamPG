using System;
using SunsetSystems.ActionSystem;
using SunsetSystems.Combat;
using SunsetSystems.Entities;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Reload Ability", menuName = "Sunset Abilities/Weapon Reload")]
    public class ReloadWeaponAbility : BaseAbility
    {
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

        protected override async Awaitable DoExecuteAbility(IAbilityContext context, Action onCompleted)
        {
            await Awaitable.MainThreadAsync();
            var targetCombatContext = (context.TargetObject as IContextProvider<ICombatContext>).GetContext();
            var weaponManager = targetCombatContext.WeaponManager;
            var actionPerformer = context.SourceActionPerformer;
            var reloadAction = new ReloadAction(weaponManager, actionPerformer);
            await actionPerformer.PerformAction(reloadAction);
            onCompleted?.Invoke();
            
        }

        protected override RangeData GetAbilityRangeData(IAbilityContext context)
        {
            return new();
        }
    }
}
