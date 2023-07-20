using SunsetSystems.Combat;
using UnityEngine;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interfaces;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Attack : HostileAction
    {
        private AttackModifier _attackModifier;

        public Attack(ICombatant target, ICombatant attacker, AttackModifier attackModifier) : this(target, attacker)
        {
            _attackModifier = attackModifier;
        }

        public Attack(ICombatant target, ICombatant attacker) : base(target, attacker)
        {

        }

        public override void Abort()
        {
            base.Abort();
            if (Owner.Faction is Faction.PlayerControlled)
            {
                Owner.References.GetComponentInChildren<LineRenderer>().enabled = false;
            }
        }

        public override void Begin()
        {
            Debug.Log(Owner.References.GameObject.name + " attacks " + Target.References.GameObject.name);
            ICombatant attacker = Owner;
            ICombatant defender = Target;
            AttackResult result = CombatCalculator.CalculateAttackResult(attacker, defender, _attackModifier);
            Debug.Log($"Attack hit? {result.Successful}\n" +
                $"Attacker hit chance = {result.AttackerHitChance}\n" +
                $"Defender dodge chance = {result.DefenderDodgeChance}\n" +
                $"Attack roll: {result.HitRoll} vs difficulty {result.AttackerHitChance - result.DefenderDodgeChance}\n" +
                $"Damage dealt: {result.Damage} - {result.DamageReduction} = {result.AdjustedDamage}");
            defender.TakeDamage(result.AdjustedDamage);
            Finish(Target, Owner);
        }
    } 
}