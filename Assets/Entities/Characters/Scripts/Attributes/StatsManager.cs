using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : ExposableMonobehaviour
{
    [SerializeField]
    protected CharacterStats characterStats;

    [SerializeField, ReadOnly]
    protected Creature owner;

    public virtual void Start()
    {
        owner = GetComponentInParent<Creature>();
        if (!characterStats)
            characterStats = ScriptableObject.CreateInstance(typeof(CharacterStats)) as CharacterStats;
        if (characterStats.IsGeneric)
            characterStats = characterStats.CopyAssetInstance(characterStats);
    }

    public void TakeDamage(int damage)
    {
        Tracker health = characterStats.GetTracker(TrackerType.Health);
        int newHealth = health.CurrentValue;
        newHealth -= damage;
        Debug.Log(owner.gameObject.name + " takes " + damage + " damage!" + "\nCurrent health: " + health.CurrentValue + "\nHealth after attack: " + newHealth);
        health.CurrentValue = newHealth < 0 ? 0 : newHealth;
        if (health.CurrentValue <= 0)
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
        bool isDead = characterStats.GetTracker(TrackerType.Health).CurrentValue <= 0;
        return isDead;
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
