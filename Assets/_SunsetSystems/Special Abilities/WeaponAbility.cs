using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Weapon Ability", menuName = "Sunset Abilities/Weapon Ability")]
    public sealed class WeaponAbility : BaseAbility
    {
        [SerializeField, BoxGroup("Ability Range"), MinValue(1)]
        private int _baseOptimalRange = 1;
        [SerializeField, BoxGroup("Ability Range"), MinValue(1)]
        private int _baseShortRange = 1;
        [BoxGroup("Weapon Ability")]
        [SerializeField]
        private int _numberOfAttacks = 1;
        [BoxGroup("Weapon Ability")]
        [SerializeField]
        private int _ammoUsed = 0;

        protected async override Awaitable DoExecuteAbility(IAbilityContext context, ITargetable target, Action onCompleted)
        {
            var actionPerformer = context.ActionPerformer;
            var shootingAction = new Attack(target, context.UserCombatBehaviour);
            await context.ActionPerformer.PerformAction(shootingAction);
            onCompleted?.Invoke();
        }

        protected override RangeData GetAbilityRangeData(IAbilityUser abilityUser)
        {
            return new RangeData(_baseShortRange, _baseOptimalRange, _baseMaxRange);
        }
    }
}
