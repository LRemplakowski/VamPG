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
        private WeaponAbility _weaponAbility;
        [SerializeField]
        private IAbilityUser _abilityUser;

        private IEnumerator _attackRoutine;

        public WeaponAbilityAction(WeaponAbility weaponAbility, IAbilityUser user, ICombatant attacker, ITargetable target) : base(target, attacker)
        {
            _weaponAbility = weaponAbility;
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
            if (_attackRoutine != null)
                return;
            conditions.Add(new WaitForFlag(_attackFinished));
            //Debug.Log(Attacker.References.GameObject.name + " attacks " + Target.References.GameObject.name);
            AttackResult result = CombatCalculator.CalculateAttackResult(new AttackContextFromWeaponAbilityAction(this));
            LogAttack(Attacker, Target, result);
            _attackRoutine = PerformAttack(Attacker, Target, result);
            Attacker.CoroutineRunner.StartCoroutine(_attackRoutine);

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
            _faceTargetSubaction = new(attacker, defender.CombatContext.Transform, 180f);
            _faceTargetSubaction.Begin();
            while (_faceTargetSubaction.EvaluateAction() is false)
                yield return null;
            attacker.References.AnimationManager.PlayFireWeaponAnimation();
            //defender.References.AnimationManager.PlayTakeHitAnimation();
            if (attackResult.Successful)
                defender.TakeDamage(attackResult.AdjustedDamage);
            _attackFinished.Value = true;
        }

        private readonly struct AttackContextFromWeaponAbilityAction : IAttackContext
        {
            private readonly WeaponAbilityAction _action;
            private readonly IAbilityTargetingData _abilityTargetingData;

            public AttackContextFromWeaponAbilityAction(WeaponAbilityAction action)
            {
                _action = action;
                _abilityTargetingData = _action._weaponAbility.GetTargetingData(_action._abilityUser);
            }

            public readonly Vector3 GetAimingPosition(AttackParticipant entity)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _action.Attacker.CombatContext.Transform.position,
                    AttackParticipant.Target => _action.Target.CombatContext.Transform.position,
                    _ => Vector3.zero,
                };
            }

            public readonly int GetAttackDamage()
            {
                int damage = 0;
                IWeapon selectedWeapon = _action.Attacker.WeaponManager.GetSelectedWeapon();
                float weaponDamageMod = _action.Attacker.WeaponManager.GetPrimaryWeapon().Equals(selectedWeapon) ? 1f : 0.6f;
                switch (GetAttackType())
                {
                    case AttackType.WeaponMelee:
                        damage = Mathf.RoundToInt((_action.Attacker.CombatContext.GetAttributeValue(AttributeType.Strength) + selectedWeapon.GetDamageData().DamageModifier) * weaponDamageMod);
                        break;
                    case AttackType.WeaponRanged:
                        damage = Mathf.RoundToInt((_action.Attacker.CombatContext.GetAttributeValue(AttributeType.Composure) + selectedWeapon.GetDamageData().DamageModifier) * weaponDamageMod);
                        break;
                    case AttackType.MagicMelee:
                        break;
                    case AttackType.MagicRanged:
                        break;
                    case AttackType.WeaponAOE:
                        break;
                    case AttackType.MagicAOE:
                        break;
                }
                return damage;
            }

            public readonly RangeData GetAttackRangeData()
            {
                return _abilityTargetingData.GetRangeData();
            }

            public readonly AttackType GetAttackType()
            {
                return _action._weaponAbility.GetAttackType();
            }

            public readonly int GetAttributeValue(AttackParticipant entity, AttributeType attributeType)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _action.Attacker.CombatContext.GetAttributeValue(attributeType),
                    AttackParticipant.Target => _action.Target.CombatContext.GetAttributeValue(attributeType),
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
                    AttackParticipant.Attacker => _action.Attacker.CombatContext.CurrentCoverSources,
                    AttackParticipant.Target => _action.Target.CombatContext.CurrentCoverSources,
                    _ => new List<ICover>(),
                };
            }

            public readonly int GetDamageReduction()
            {
                return GetAttackType() switch
                {
                    AttackType.WeaponMelee => _action.Target.CombatContext.GetAttributeValue(AttributeType.Stamina),
                    AttackType.WeaponRanged => _action.Target.CombatContext.GetAttributeValue(AttributeType.Dexterity),
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
                float heightDifference = _action.Attacker.CombatContext.Transform.position.y - _action.Target.CombatContext.Transform.position.y;
                if (heightDifference > 2f && _action.Attacker.References.GetCachedComponentInChildren<SpellbookManager>().GetIsPowerKnown(PassivePowersHelper.Instance.HeightAttackAndDamageBonus))
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
                    AttackParticipant.Attacker => _action.Attacker.CombatContext.Transform.position,
                    AttackParticipant.Target => _action.Target.CombatContext.Transform.position,
                    _ => Vector3.zero,
                };
            }

            public readonly bool IsInCover(AttackParticipant entity)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _action.Attacker.CombatContext.IsInCover,
                    AttackParticipant.Target => _action.Target.CombatContext.IsInCover,
                    _ => false,
                };
            }

            public readonly bool IsPlayerControlled(AttackParticipant entity)
            {
                return entity switch
                {
                    AttackParticipant.Attacker => _action.Attacker.CombatContext.IsPlayerControlled,
                    AttackParticipant.Target => _action.Target.CombatContext.IsPlayerControlled,
                    _ => false,
                };
            }
        }
    }
}
