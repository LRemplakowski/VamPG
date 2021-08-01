using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Dice;

public class StatsManager : ExposableMonobehaviour
{
    [SerializeField]
    protected CharacterStats characterStats;
    public CharacterStats Stats { get => characterStats; }

    [SerializeField, ReadOnly]
    protected Creature owner;

    [SerializeField, ReadOnly]
    private int _health;
    public int Health { get => _health; private set => _health = value; }
    [SerializeField, ReadOnly]
    private int _willpower;
    public int Willpower { get => _willpower; private set => _willpower = value; }
    [SerializeField, ReadOnly]
    private int _humanity;
    public int Humanity { get => _humanity; private set => _humanity = value; }
    [SerializeField, ReadOnly]
    private int _hunger;
    public int Hunger { get => _hunger; private set => _hunger = value; }

    public virtual void Start()
    {
        owner = GetComponentInParent<Creature>();
        if (!characterStats)
            characterStats = ScriptableObject.CreateInstance(typeof(CharacterStats)) as CharacterStats;
        if (characterStats.IsGeneric)
            characterStats = CharacterStats.CopyAssetInstance(characterStats);
        Health = characterStats.GetTracker(TrackerType.Health).GetValue();
        Willpower = characterStats.GetTracker(TrackerType.Willpower).GetValue();
        Humanity = characterStats.GetTracker(TrackerType.Humanity).GetValue();
        Hunger = characterStats.GetTracker(TrackerType.Hunger).GetValue();
    }

    public void TakeDamage(int damage)
    {
        Tracker health = characterStats.GetTracker(TrackerType.Health);
        int newHealth = Health;
        newHealth -= damage;
        Debug.Log(owner.gameObject.name + " takes " + damage + " damage!" + "\nCurrent health: " + Health + "\nHealth after attack: " + newHealth);
        Health = newHealth < 0 ? 0 : newHealth;
        if (Health <= 0)
            Die();
    }

    public virtual void Die()
    {
        Debug.Log("Character died!");
    }

    public int GetCombatSpeed()
    {
        Attribute dexterity = characterStats.GetAttribute(AttributeType.Dexterity);
        Skill athletics = characterStats.GetSkill(SkillType.Athletics);
        if (dexterity.GetAttributeType() != AttributeType.Invalid && athletics.GetSkillType() != SkillType.Invalid)
            return (dexterity.GetValue() + athletics.GetValue());
        else
            return 0;
    }

    public float GetWeaponMaxRange()
    {
        return 10.0f;
    }

    public float GetWeaponEffectiveRange()
    {
        return 5.0f;
    }

    public bool IsAlive()
    {
        return Health > 0;
    }  

    public AttributeSkillPool GetDefensePool()
    {
        return new AttributeSkillPool(characterStats.GetAttribute(AttributeType.Dexterity), characterStats.GetSkill(SkillType.Athletics));
    }

    public AttributeSkillPool GetAttackPool()
    {
        return new AttributeSkillPool(characterStats.GetAttribute(GetWeaponAttribute()), characterStats.GetSkill(GetWeaponSkill()));
    }

    private AttributeType GetWeaponAttribute()
    {
        return AttributeType.Composure;
    }

    private SkillType GetWeaponSkill()
    {
        return SkillType.Firearms;
    }

    public DisciplinePower GetDisciplinePower(string scriptName)
    {
        return characterStats.GetDisciplinePower(scriptName);
    }

    public Outcome GetSkillRoll(AttributeType attribute, SkillType skill)
    {
        Attribute a = characterStats.GetAttribute(attribute);
        Skill s = characterStats.GetSkill(skill);
        int normalDice = a.GetValue() + s.GetValue();
        int hungerDice = 0;
        if (owner.GetCreatureType().Equals(CreatureType.Vampire))
        {
            hungerDice = Hunger;
            normalDice = hungerDice <= normalDice ? normalDice - hungerDice : 0;
        }

        return Roll.d10(normalDice, hungerDice);
    }

    public Outcome GetSkillRoll(AttributeType attribute, SkillType skill, int dc)
    {
        Attribute a = characterStats.GetAttribute(attribute);
        Skill s = characterStats.GetSkill(skill);
        int normalDice = a.GetValue() + s.GetValue();
        int hungerDice = 0;
        if (owner.GetCreatureType().Equals(CreatureType.Vampire))
        {
            hungerDice = Hunger;
            normalDice = hungerDice <= normalDice ? normalDice - hungerDice : 0;
        }

        return Roll.d10(normalDice, hungerDice, dc);
    }

    public List<Attribute> GetAttributes()
    {
        return characterStats.GetAttributes();
    }
}
