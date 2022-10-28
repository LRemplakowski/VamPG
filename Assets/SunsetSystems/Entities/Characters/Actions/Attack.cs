namespace SunsetSystems.Entities.Characters.Actions
{
    using SunsetSystems.Dice;
    using UnityEngine;

    public class Attack : HostileAction
    {
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

            Outcome defenseRoll = Target.GetComponent<StatsManager>().GetSkillRoll(AttributeType.Dexterity, SkillType.Athletics, Target.IsVampire);
            Outcome attackRoll = Owner.GetComponent<StatsManager>().GetAttackRoll(defenseRoll.successes, Owner.IsVampire);
            int damage = attackRoll.successes - defenseRoll.successes;
            Debug.Log("Damage from attack: " + damage
                + "\nAttacker roll: " + attackRoll.successes + ", isCritical? " + attackRoll.isCritical + ", isMessy? " + attackRoll.isMessy + ", isBestialFailure?" + attackRoll.isBestialFailure
                + "\nDefender roll: " + defenseRoll.successes + ", isCritical? " + defenseRoll.isCritical + ", isMessy? " + defenseRoll.isMessy + ", isBestialFailure?" + defenseRoll.isBestialFailure);

            if (damage > 0)
            {
                Target.GetComponent<StatsManager>().TakeDamage(attackRoll.successes - defenseRoll.successes);
            }
            if (onAttackFinished != null)
                onAttackFinished.Invoke(Target, Owner);
        }
    } 
}