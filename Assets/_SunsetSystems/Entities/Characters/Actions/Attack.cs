﻿using System.Collections;
using SunsetSystems.Combat;
using SunsetSystems.DynamicLog;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public class Attack : HostileAction
    {
        [SerializeField]
        private AttackModifier _attackModifier;
        [SerializeField]
        private FlagWrapper attackFinished;
        [SerializeField]
        private FaceTarget faceTargetSubaction;

        private IEnumerator attackRoutine;

        public Attack(ITargetable target, ICombatant attacker, AttackModifier attackModifier) : this(target, attacker)
        {
            _attackModifier = attackModifier;
        }

        public Attack(ITargetable target, ICombatant attacker) : base(target, attacker)
        {
            attackFinished = new() { Value = false };
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
            //Debug.Log(Attacker.References.GameObject.name + " attacks " + Target.References.GameObject.name);
            AttackResult result = CombatCalculator.CalculateAttackResult(Attacker, Target, _attackModifier);
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
            faceTargetSubaction = new(attacker, defender.CombatContext.Transform, 180f);
            faceTargetSubaction.Begin();
            while (faceTargetSubaction.EvaluateAction() is false)
                yield return null;
            attacker.References.AnimationManager.PlayFireWeaponAnimation();
            //defender.References.AnimationManager.PlayTakeHitAnimation();
            yield return new WaitForSeconds(1f);
            if (attackResult.Successful)
                defender.TakeDamage(attackResult.AdjustedDamage);
            attackFinished.Value = true;
        }
    } 
}