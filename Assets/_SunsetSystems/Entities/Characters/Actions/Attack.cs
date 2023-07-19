using SunsetSystems.Combat;
using SunsetSystems.Dice;
using UnityEngine;
using SunsetSystems.Entities.Characters.Interfaces;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Attack : HostileAction
    {
        private AttackModifier _attackModifier;

        public Attack(ICreature target, ICreature attacker, AttackModifier attackModifier) : this(target, attacker)
        {
            _attackModifier = attackModifier;
        }

        public Attack(ICreature target, ICreature attacker) : base(target, attacker)
        {

        }

        public override void Abort()
        {
            base.Abort();
            if (Owner is PlayerControlledCharacter)
            {
                Owner.References.GetComponentInChildren<LineRenderer>().enabled = false;
            }
        }

        public override void Begin()
        {
            Debug.Log(Owner.References.GameObject.name + " attacks " + Target.References.GameObject.name);
            AttackResult result = CombatCalculator.CalculateAttackResult(Owner, Target, _attackModifier);
            Debug.Log($"Attack hit? {result.Successful}\n" +
                $"Attacker hit chance = {result.AttackerHitChance}\n" +
                $"Defender dodge chance = {result.DefenderDodgeChance}\n" +
                $"Attack roll: {result.HitRoll} vs difficulty {result.AttackerHitChance - result.DefenderDodgeChance}\n" +
                $"Damage dealt: {result.Damage} - {result.DamageReduction} = {result.AdjustedDamage}");
            Target.TakeDamage(result.AdjustedDamage);
            Finish(Target, Owner);
        }
    } 
}