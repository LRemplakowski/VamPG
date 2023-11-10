using SunsetSystems.Combat;
using UnityEngine;
using SunsetSystems.Entities.Interfaces;
using System.Collections;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using Sirenix.OdinInspector;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public class Attack : HostileAction
    {
        [ShowInInspector]
        private AttackModifier _attackModifier;
        private IEnumerator attackRoutine;
        [ShowInInspector]
        private FlagWrapper attackFinished;

        public Attack(ICombatant target, ICombatant attacker, AttackModifier attackModifier) : this(target, attacker)
        {
            _attackModifier = attackModifier;
        }

        public Attack(ICombatant target, ICombatant attacker) : base(target, attacker)
        {
            attackFinished = new() { Value = false };
            conditions.Add(new WaitForFlag(attackFinished));
        }

        public override void Abort()
        {
            base.Abort();
            if (attackRoutine != null)
                (Owner as MonoBehaviour).StopCoroutine(attackRoutine);
        }

        public override void Begin()
        {
            if (attackRoutine != null)
                return;
            Debug.Log(Owner.References.GameObject.name + " attacks " + Target.References.GameObject.name);
            ICombatant attacker = Owner;
            ICombatant defender = Target;
            AttackResult result = CombatCalculator.CalculateAttackResult(attacker, defender, _attackModifier);
            LogAttack(result);
            attackRoutine = PerformAttack(attacker, defender, result);
            (attacker as MonoBehaviour).StartCoroutine(attackRoutine);

            static void LogAttack(AttackResult result)
            {
                Debug.Log($"Attack hit? {result.Successful}\n" +
                    $"Attacker hit chance = {result.AttackerHitChance}\n" +
                    $"Defender dodge chance = {result.DefenderDodgeChance}\n" +
                    $"Attack roll: {result.HitRoll} vs difficulty {result.AttackerHitChance - result.DefenderDodgeChance}\n" +
                    $"Damage dealt: {result.Damage} - {result.DamageReduction} = {result.AdjustedDamage}");
            }
        }

        private IEnumerator PerformAttack(ICombatant attacker, ICombatant defender, AttackResult attackResult)
        {
            FaceTarget faceTarget = new(attacker, defender.Transform);
            faceTarget.Begin();
            while (faceTarget.EvaluateActionFinished() is false)
                yield return null;
            float waitForAttackFinish = attacker.PerformAttackAnimation();
            float waitForTakeHitFinish = defender.PerformTakeHitAnimation();
            yield return new WaitForSeconds(Mathf.Max(waitForAttackFinish, waitForTakeHitFinish));
            defender.TakeDamage(attackResult.AdjustedDamage);
            attackFinished.Value = true;
        }
    } 
}