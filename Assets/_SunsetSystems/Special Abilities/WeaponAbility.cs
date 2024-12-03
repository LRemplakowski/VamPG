using System;
using Sirenix.OdinInspector;
using SunsetSystems.ActionSystem;
using SunsetSystems.Combat;
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
        [SerializeField, PropertyRange(0, 5)]
        private float _delayBetweenAttacks = .2f;
        [BoxGroup("Weapon Ability")]
        [SerializeField]
        private int _ammoPerAttack = 0;
        [BoxGroup("Weapon Ability")]
        [SerializeField]
        private int _damageBonus = 0;
        [BoxGroup("Weapon Ability")]
        [SerializeField]
        private AttackType _attackType = AttackType.WeaponMelee;
        [BoxGroup("Weapon Ability")]
        [SerializeField]
        private DamageType _damageType = DamageType.Piercing;

        protected async override Awaitable DoExecuteAbility(IAbilityContext context, ITargetable target, Action onCompleted)
        {
            var actionPerformer = context.ActionPerformer;
            var shootingAction = new WeaponAbilityAction(this, context.User, context.UserCombatBehaviour, target);
            await context.ActionPerformer.PerformAction(shootingAction);
            onCompleted?.Invoke();
        }

        protected override RangeData GetAbilityRangeData(IAbilityUser abilityUser)
        {
            return new RangeData(_baseShortRange, _baseOptimalRange, _baseMaxRange);
        }

        public int GetAmmoPerAttack() => _ammoPerAttack;
        public int GetNumberOfAttacks() => _numberOfAttacks;
        public int GetDamageBonus() => _damageBonus;
        public float GetDelayBetweenAttacks() => _delayBetweenAttacks;
        public AttackType GetAttackType() => _attackType;
        public DamageType GetDamageType() => _damageType;
    }
}
