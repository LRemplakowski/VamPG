using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : ExposableMonobehaviour
{
    public CharacterStats characterStats;

    [SerializeField, ReadOnly]
    private Creature owner;

    [SerializeField, ReadOnly]
    private int currentHealth, currentWillpower;

    public void Start()
    {
        owner = GetComponentInParent<Creature>();
        currentHealth = characterStats.GetTracker(TrackerType.Health).GetValue();
        currentWillpower = characterStats.GetTracker(TrackerType.Willpower).GetValue();
    }

    public void TakeDamage(int damage)
    {
        Tracker health = characterStats.GetTracker(TrackerType.Health);
        int newHealth = currentHealth;
        newHealth -= damage;
        Debug.Log(owner.gameObject.name + " takes " + damage + " damage!" + "\nCurrent health: " + currentHealth + "\nHealth after attack: " + newHealth);
        currentHealth = newHealth < 0 ? 0 : newHealth;
        if (currentHealth <= 0)
            Die();
    }

    public virtual void Die()
    {
        Debug.Log("Character died!");
    }

    public int GetCombatSpeed()
    {
        Attribute dexterity = characterStats.GetAttribute(AttributeType.Dexterity);
        Skill athletics = characterStats.GetSkill(SkillType.Atheltics);
        if (dexterity.GetAttributeType() != AttributeType.Invalid && athletics.GetSkillType() != SkillType.Invalid)
            return (dexterity.GetValue() + athletics.GetValue());
        else
            return 0;
    }

    public float GetAttackRange()
    {
        return 10.0f;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public int GetDefensePool()
    {
        return characterStats.GetAttribute(AttributeType.Dexterity).GetValue() + characterStats.GetSkill(SkillType.Atheltics).GetValue();
    }

    public int GetAttackPool()
    {
        return characterStats.GetAttribute(GetWeaponAttribute()).GetValue() + characterStats.GetSkill(GetWeaponSkill()).GetValue();
    }

    private AttributeType GetWeaponAttribute()
    {
        return AttributeType.Composure;
    }

    private SkillType GetWeaponSkill()
    {
        return SkillType.Firearms;
    }
}
