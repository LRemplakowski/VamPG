using System;
using System.Threading.Tasks;
using SunsetSystems.Combat;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Reload Ability", menuName = "Sunset Abilities/Weapon Reload")]
    public class ReloadWeaponAbility : BaseAbility
    {
        public override bool IsValidTarget(IAbilityContext context)
        {
            return base.IsValidTarget(context) && context.TargetCharacter.CombatContext.IsSelectedWeaponUsingAmmo;
        }

        protected override async Awaitable DoExecuteAbility(IAbilityContext abilityContext, Action onCompleted)
        {
            await Task.Yield();
        }

        protected override RangeData GetAbilityRangeData(IAbilityContext context)
        {
            return new();
        }
    }
}
