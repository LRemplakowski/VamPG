using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public CharacterStats characterStats;

    [SerializeField, ReadOnly]
    private Creature owner;

    public void Start()
    {
        owner = GetComponentInParent<Creature>();
    }

    public void OnEnable()
    {
        characterStats.GetTracker(TrackerType.Health).onTrackerNegativeOrZero += Die;
    }

    public void OnDisable()
    {
        characterStats.GetTracker(TrackerType.Health).onTrackerNegativeOrZero -= Die;
    }

    public void TakeDamage(int damage)
    {
        Tracker health = characterStats.GetTracker(TrackerType.Health);
        int newHealth = health.GetCurrentValue();
        newHealth -= damage;
        health.SetCurrentValue(newHealth);
    }

    public virtual void Die()
    {
        Debug.Log("Character died!");
    }

    public float GetCombatSpeed()
    {
        Attribute dexterity = characterStats.GetAttribute(AttributeType.Dexterity);
        Skill athletics = characterStats.GetSkill(SkillType.Atheltics);
        if (dexterity.GetAttributeType() != AttributeType.Invalid && athletics.GetSkillType() != SkillType.Invalid)
            return (dexterity.GetValue() + athletics.GetValue());
        else
            return 0f;
    }
}
