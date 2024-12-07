using System;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Combat;
using SunsetSystems.DynamicLog;
using SunsetSystems.Entities;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    [Obsolete]
    [Serializable]
    public class Attack : HostileAction
    {
        [SerializeField]
        private AttackModifier _attackModifier;
        [SerializeField]
        private FlagWrapper attackFinished;
        [SerializeField]
        private FaceTarget faceTargetSubaction;
        [SerializeField]
        private ICombatContext _attackerContext;
        [SerializeField]
        private ICombatContext _targetContext;
        [SerializeField]
        private IDamageable _targetDamageable;

        private IEnumerator attackRoutine;

        public Attack(ITargetable target, ICombatant attacker, AttackModifier attackModifier) : this(target, attacker)
        {
            _attackModifier = attackModifier;
        }

        public Attack(ITargetable target, ICombatant attacker) : base(target, attacker)
        {
            attackFinished = new() { Value = false };
            if (attacker is IContextProvider<ICombatContext> attackerContextSource)
                _attackerContext = attackerContextSource.GetContext();
            if (target is IContextProvider<ICombatContext> targetContextSource)
                _targetContext = targetContextSource.GetContext();
            _targetDamageable = target as IDamageable;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            if (attackRoutine != null)
                Attacker.CoroutineRunner.StopCoroutine(attackRoutine);
        }

        public override void Begin()
        {
            if (attackRoutine != null)
                return;
            conditions.Add(new WaitForFlag(attackFinished));
            //Debug.Log(Attacker.References.GameObject.name + " attacks " + TargetObject.References.GameObject.name);
            AttackResult result = CombatCalculator.CalculateAttackResult(new AttackContextFromAttackAction(this, _attackModifier));
            LogAttack(Attacker, Target, result);
            attackRoutine = PerformAttack(Attacker, Target, result);
            Attacker.CoroutineRunner.StartCoroutine(attackRoutine);

            static void LogAttack(ICombatant attacker, ITargetable target, AttackResult result)
            {
                string logMessage = LogUtility.LogMessageFromAttackResult(attacker, target, result);
                DynamicLogManager.Instance.PostLogMessage(logMessage);
                Debug.Log($"Attack hit? {result.Successful}\n" +
                    $"Attacker hit chance = {result.AttackerHitChance}\n" +
                    $"Defender dodge chance = {result.DefenderDodgeChance}\n" +
                    $"Attack roll: {result.HitRoll} vs difficulty {result.AttackerHitChance - result.DefenderDodgeChance}\n" +
                    $"Damage dealt: {result.Damage} - {result.DamageReduction} = {result.AdjustedDamage}");

            }
        }

        private IEnumerator PerformAttack(ICombatant attacker, ITargetable defender, AttackResult attackResult)
        {
            faceTargetSubaction = new(attacker, _targetContext.Transform, 180f);
            faceTargetSubaction.Begin();
            while (faceTargetSubaction.EvaluateAction() is false)
                yield return null;
            attacker.References.AnimationManager.PlayFireWeaponAnimation();
            //defender.References.AnimationManager.PlayTakeHitAnimation();
            yield return new WaitForSeconds(1f);
            if (attackResult.Successful)
                _targetDamageable.TakeDamage(attackResult.AdjustedDamage);
            attackFinished.Value = true;
        }

        private readonly struct AttackContextFromAttackAction : IAttackContext
        {
            private readonly ICombatant _attacker;
            private readonly ITargetable _target;
            private readonly AttackModifier _attackModifier;
            private readonly ICombatContext _attackerContext;
            private readonly ICombatContext _targetContext;

            public AttackContextFromAttackAction(Attack attackAction, AttackModifier attackModifier)
            {
                _attacker = attackAction.Attacker;
                _target = attackAction.Target;
                _attackModifier = attackModifier;
                _attackerContext = attackAction._attackerContext;
                _targetContext = attackAction._targetContext;
            }

            public Vector3 GetAimingPosition(AttackParticipant entity)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _attackerContext.AimingOrigin,
                    AttackParticipant.Target => _targetContext.AimingOrigin,
                    _ => Vector3.zero,
                };
            }

            public int GetAttackDamage()
            {
                int damage = 0;
                IWeapon selectedWeapon = _attackerContext.WeaponManager.GetSelectedWeapon();
                float weaponDamageMod = _attackerContext.WeaponManager.GetPrimaryWeapon().Equals(selectedWeapon) ? 1f : 0.6f;
                switch (selectedWeapon.WeaponType)
                {
                    case AbilityRange.Melee:
                        damage = Mathf.RoundToInt((_attackerContext.GetAttributeValue(AttributeType.Strength) + selectedWeapon.GetDamageData().DamageModifier) * weaponDamageMod);
                        break;
                    case AbilityRange.Ranged:
                        damage = Mathf.RoundToInt((_attackerContext.GetAttributeValue(AttributeType.Composure) + selectedWeapon.GetDamageData().DamageModifier) * weaponDamageMod);
                        break;
                }
                return damage;
            }

            public float GetCriticalDamageMultiplier()
            {
                return 1.5f;
            }

            public RangeData GetAttackRangeData()
            {
                throw new System.NotImplementedException();
            }

            public AttackType GetAttackType()
            {
                throw new System.NotImplementedException();
            }

            public int GetAttributeValue(AttackParticipant entity, AttributeType attributeType)
            {
                throw new System.NotImplementedException();
            }

            public AttackModifier GetBaseAttackModifier()
            {
                return _attackModifier;
            }

            public IEnumerable<ICover> GetCoverSources(AttackParticipant entity)
            {
                throw new System.NotImplementedException();
            }

            public int GetDamageReduction()
            {
                throw new System.NotImplementedException();
            }

            public AttackModifier GetHeightAttackModifier()
            {
                AttackModifier heightAttackMod = new();
                float heightDifference = _attacker.References.Transform.position.y - _targetContext.Transform.position.y;
                if (heightDifference > 2f && _attacker.References.GetCachedComponentInChildren<Abilities.SpellbookManager>().GetIsPowerKnown(PassivePowersHelper.Instance.HeightAttackAndDamageBonus))
                {
                    heightAttackMod.HitChanceMod += .1d;
                    heightAttackMod.DamageMod += 2;
                }
                return heightAttackMod;
            }

            public Vector3 GetPosition(AttackParticipant entity)
            {
                throw new System.NotImplementedException();
            }

            public bool IsInCover(AttackParticipant entity)
            {
                throw new System.NotImplementedException();
            }

            public bool IsPlayerControlled(AttackParticipant entity)
            {
                throw new System.NotImplementedException();
            }
        }
    } 
}