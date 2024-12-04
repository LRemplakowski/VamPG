using System;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Abilities;
using SunsetSystems.Combat;
using SunsetSystems.DynamicLog;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    public class WeaponAbilityAction : HostileAction
    {
        [SerializeField]
        private FlagWrapper _attackFinished;
        [SerializeField]
        private FaceTarget _faceTargetSubaction;
        [SerializeField]
        private WeaponAttackAbility _weaponAbility;
        [SerializeField]
        private IAbilityContext _abilityContext;

        private IEnumerator _attackRoutine;

        public WeaponAbilityAction(WeaponAttackAbility weaponAbility, IAbilityContext context) : base(context.TargetCharacter, context.SourceCombatBehaviour)
        {
            _weaponAbility = weaponAbility;
            _abilityContext = context;
            _attackFinished = new() { Value = false };
        }

        public override void Cleanup()
        {
            base.Cleanup();
            if (_attackRoutine != null)
            {
                Attacker.CoroutineRunner.StopCoroutine(_attackRoutine);
            }
        }

        public override void Begin()
        {
            if (Attacker.WeaponManager.HasEnoughAmmoInSelectedWeapon(_weaponAbility.GetAmmoPerAttack() * _weaponAbility.GetNumberOfAttacks()) is false)
            {
                Abort();
                return;
            }
            if (_attackRoutine != null)
            {
                return;
            }
            conditions.Add(new WaitForFlag(_attackFinished));
            Debug.Log(Attacker.CombatContext.GameObject.name + " attacks " + Target.CombatContext.GameObject.name);
            _attackRoutine = PerformAttack(Attacker, Target);
            Attacker.CoroutineRunner.StartCoroutine(_attackRoutine);
        }

        private IEnumerator PerformAttack(ICombatant attacker, ITargetable defender)
        {
            var attackContext = new AttackContextFromWeaponAbilityAction(this);
            _faceTargetSubaction = new(attacker, defender.CombatContext.Transform, 180f);
            _faceTargetSubaction.Begin();
            while (_faceTargetSubaction.EvaluateAction() is false)
                yield return null;
            attacker.References.AnimationManager.PlayFireWeaponAnimation();
            for (int i = 0; i < _weaponAbility.GetNumberOfAttacks(); i++)
            {
                AttackResult result = CombatCalculator.CalculateAttackResult(attackContext);
                LogAttack(Attacker, Target, result);
                if (result.Successful && attacker.WeaponManager.UseAmmoFromSelectedWeapon(_weaponAbility.GetAmmoPerAttack()))
                {
                    defender.TakeDamage(result.AdjustedDamage);
                }
                yield return new WaitForSeconds(_weaponAbility.GetDelayBetweenAttacks());
            }
            _attackFinished.Value = true;

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

        private readonly struct AttackContextFromWeaponAbilityAction : IAttackContext
        {
            private readonly WeaponAttackAbility _ability;
            private readonly ICombatant _attacker;
            private readonly ITargetable _target;
            private readonly IAbilityContext _abilityContext;
            private readonly IAbilityTargetingData _abilityTargetingData;

            public AttackContextFromWeaponAbilityAction(WeaponAbilityAction action)
            {
                _ability = action._weaponAbility;
                _attacker = action.Attacker;
                _target = action.Target;
                _abilityContext = action._abilityContext;
                _abilityTargetingData = _ability.GetTargetingData(_abilityContext);
            }

            public readonly Vector3 GetAimingPosition(AttackParticipant entity)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _attacker.CombatContext.Transform.position,
                    AttackParticipant.Target => _target.CombatContext.Transform.position,
                    _ => Vector3.zero,
                };
            }

            public readonly int GetAttackDamage()
            {
                int damage = 0;
                var attackerContext = _attacker.CombatContext;
                float weaponDamageMod = attackerContext.IsUsingPrimaryWeapon ? 1f : 0.6f;
                damage += attackerContext.SelectedWeaponDamageBonus;
                damage += _ability.GetDamageBonus();
                damage += GetAttackType() switch
                {
                    AttackType.WeaponMelee => attackerContext.GetAttributeValue(AttributeType.Strength),
                    AttackType.WeaponRanged => attackerContext.GetAttributeValue(AttributeType.Composure),
                    _ => throw new NotImplementedException(),
                };
                return Mathf.RoundToInt(damage * weaponDamageMod);
            }

            public readonly float GetCriticalDamageMultiplier()
            {
                return 1.5f;
            }

            public readonly RangeData GetAttackRangeData()
            {
                return _abilityTargetingData.GetRangeData();
            }

            public readonly AttackType GetAttackType()
            {
                return _ability.GetAttackType();
            }

            public readonly int GetAttributeValue(AttackParticipant entity, AttributeType attributeType)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _attacker.CombatContext.GetAttributeValue(attributeType),
                    AttackParticipant.Target => _target.CombatContext.GetAttributeValue(attributeType),
                    _ => 0,
                };
            }

            public readonly AttackModifier GetBaseAttackModifier()
            {
                return new();
            }

            public readonly IEnumerable<ICover> GetCoverSources(AttackParticipant entity)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _attacker.CombatContext.CurrentCoverSources,
                    AttackParticipant.Target => _target.CombatContext.CurrentCoverSources,
                    _ => new List<ICover>(),
                };
            }

            public readonly int GetDamageReduction()
            {
                return GetAttackType() switch
                {
                    AttackType.WeaponMelee => _target.CombatContext.GetAttributeValue(AttributeType.Stamina),
                    AttackType.WeaponRanged => _target.CombatContext.GetAttributeValue(AttributeType.Dexterity),
                    AttackType.MagicMelee => throw new System.NotImplementedException(),
                    AttackType.MagicRanged => throw new System.NotImplementedException(),
                    AttackType.WeaponAOE => throw new System.NotImplementedException(),
                    AttackType.MagicAOE => throw new System.NotImplementedException(),
                    _ => int.MaxValue,
                };
            }

            public readonly AttackModifier GetHeightAttackModifier()
            {
                AttackModifier heightAttackMod = new();
                float heightDifference = _attacker.CombatContext.Transform.position.y - _target.CombatContext.Transform.position.y;
                if (heightDifference > 2f && _attacker.References.GetCachedComponentInChildren<SpellbookManager>().GetIsPowerKnown(PassivePowersHelper.Instance.HeightAttackAndDamageBonus))
                {
                    heightAttackMod.HitChanceMod += .1d;
                    heightAttackMod.DamageMod += 2;
                }
                return heightAttackMod;
            }

            public readonly Vector3 GetPosition(AttackParticipant entity)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _attacker.CombatContext.Transform.position,
                    AttackParticipant.Target => _target.CombatContext.Transform.position,
                    _ => Vector3.zero,
                };
            }

            public readonly bool IsInCover(AttackParticipant entity)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _attacker.CombatContext.IsInCover,
                    AttackParticipant.Target => _target.CombatContext.IsInCover,
                    _ => false,
                };
            }

            public readonly bool IsPlayerControlled(AttackParticipant entity)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _attacker.CombatContext.IsPlayerControlled,
                    AttackParticipant.Target => _target.CombatContext.IsPlayerControlled,
                    _ => false,
                };
            }
        }
    }
}
