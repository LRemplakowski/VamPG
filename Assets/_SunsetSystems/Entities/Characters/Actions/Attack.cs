using System.Collections;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Interfaces;
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

        public Attack(ICombatant target, ICombatant attacker, AttackModifier attackModifier) : this(target, attacker)
        {
            _attackModifier = attackModifier;
        }

        public Attack(ICombatant target, ICombatant attacker) : base(target, attacker)
        {
            attackFinished = new() { Value = false };
        }

        public override void Cleanup()
        {
            base.Cleanup();
            if (attackRoutine != null)
                (Attacker as MonoBehaviour).StopCoroutine(attackRoutine);
        }

        public override void Begin()
        {
            if (attackRoutine != null)
                return;
            conditions.Add(new WaitForFlag(attackFinished));
            Debug.Log(Attacker.References.GameObject.name + " attacks " + Target.References.GameObject.name);
            ICombatant attacker = Attacker;
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
            faceTargetSubaction = new(attacker, defender.Transform, 180f);
            faceTargetSubaction.Begin();
            while (faceTargetSubaction.EvaluateAction() is false)
                yield return null;
            float waitForAttackFinish = attacker.PerformAttackAnimation();
            float waitForTakeHitFinish = defender.PerformTakeHitAnimation();
            yield return new WaitForSeconds(Mathf.Max(waitForAttackFinish, waitForTakeHitFinish));
            defender.TakeDamage(attackResult.AdjustedDamage);
            attackFinished.Value = true;
        }
    } 
}