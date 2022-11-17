namespace SunsetSystems.Entities.Characters.Actions
{
    using SunsetSystems.Combat;
    using SunsetSystems.Dice;
    using UnityEngine;

    public class Attack : HostileAction
    {
        private AttackModifier _attackModifier;

        public Attack(Creature target, Creature attacker, AttackModifier attackModifier) : this(target, attacker)
        {
            _attackModifier = attackModifier;
        }

        public Attack(Creature target, Creature attacker) : base(target, attacker)
        {

        }

        public override void Abort()
        {
            base.Abort();
            if (Owner is PlayerControlledCharacter)
            {
                Owner.GetComponentInChildren<LineRenderer>().enabled = false;
            }
        }

        public override void Begin()
        {
            Debug.Log(Owner.gameObject.name + " attacks " + Target.gameObject.name);
            AttackResult result = CombatCalculator.CalculateAttackResult(Owner, Target, _attackModifier);
            Debug.Log($"Attack hit? {result.Successful}\n" +
                $"Attacker hit chance = {result.AttackerHitChance}\n" +
                $"Defender dodge chance = {result.DefenderDodgeChance}\n" +
                $"Attack roll: {result.HitRoll} vs difficulty {result.AttackerHitChance - result.DefenderDodgeChance}\n" +
                $"Damage dealt: {result.Damage} - {result.DamageReduction} = {result.AdjustedDamage}");
            Target.StatsManager.TakeDamage(result.AdjustedDamage);
            if (onAttackFinished != null)
                onAttackFinished.Invoke(Target, Owner);
        }
    } 
}