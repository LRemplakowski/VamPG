using Utils.Dice;
using UnityEngine;

public class Attack : HostileAction
{
    public Attack(Creature target, Creature attacker) : base(target, attacker) 
    {

    }

    public override void Abort()
    {
        base.Abort();
        if (Owner.IsOfType(typeof(Player)))
        {
            Owner.GetComponentInChildren<LineRenderer>().enabled = false;
        }
    }

    public override void Begin()
    {
        Debug.Log(Owner.gameObject.name + " attacks " + Target.gameObject.name);
        DicePool<Attribute, Skill> targetDefensePool = Target.GetComponent<StatsManager>().GetDefensePool();
        DicePool<Attribute, Skill> attackerAttackPool = Owner.GetComponent<StatsManager>().GetAttackPool();
        
        Outcome defenseRoll = Roll.d10(targetDefensePool.GetPoolSize(), 0);
        Outcome attackRoll = Roll.d10(attackerAttackPool.GetPoolSize(), 1, defenseRoll.successes);
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