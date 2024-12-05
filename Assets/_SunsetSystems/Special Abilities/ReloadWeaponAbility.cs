using System;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Reload Ability", menuName = "Sunset Abilities/Weapon Reload")]
    public class ReloadWeaponAbility : BaseAbility
    {
        protected override bool HasValidTarget(IAbilityContext context)
        {
            return IsTargetNotNull(context) && IsTargetWeaponUsingAmmo(context);

            static bool IsTargetNotNull(IAbilityContext context) => context.TargetObject != null;
            static bool IsTargetWeaponUsingAmmo(IAbilityContext context) => context.TargetObject.CombatContext.IsSelectedWeaponUsingAmmo;
        }

        protected override async Awaitable DoExecuteAbility(IAbilityContext context, Action onCompleted)
        {
            await Awaitable.MainThreadAsync();
            context.TargetObject.CombatContext.WeaponManager.ReloadSelectedWeapon();
            onCompleted?.Invoke();
        }

        protected override RangeData GetAbilityRangeData(IAbilityContext context)
        {
            return new();
        }
    }
}
