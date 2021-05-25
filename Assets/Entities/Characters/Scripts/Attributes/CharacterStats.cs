using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField]
    private TrackedAttribute health, willpower;
    // MAIN STATS
    [SerializeField]
    private Attribute strength, dexterity, stamina;
    [SerializeField]
    private Attribute charisma, manipulation, composure;
    [SerializeField]
    private Attribute intelligence, wits, resolve;
    // SKILLS


    public void Start()
    {
        health.SetBaseValue(CharacterConsts.HEALTH_BASE + stamina.GetValue());
        health.SetCurrent(health.GetValue());
        willpower.SetBaseValue(composure.GetValue() + resolve.GetValue());
        willpower.SetCurrent(willpower.GetValue());
    }

    public TrackedAttribute GetHealth()
    {
        return health;
    }

    public TrackedAttribute GetWillpower()
    {
        return willpower;
    }

    public Attribute GetAttribute(AttributeType type)
    {
        switch (type)
        {
            case AttributeType.Strength:
                return strength;
            case AttributeType.Dexterity:
                return dexterity;
            case AttributeType.Stamina:
                return stamina;
            case AttributeType.Charisma:
                return charisma;
            case AttributeType.Manipulation:
                return manipulation;
            case AttributeType.Composure:
                return composure;
            case AttributeType.Intelligence:
                return intelligence;
            case AttributeType.Wits:
                return wits;
            case AttributeType.Resolve:
                return resolve;
            default:
                break;
        }
        return null;
    }

    public enum AttributeType
    {
        Strength,
        Dexterity,
        Stamina,
        Charisma,
        Manipulation,
        Composure,
        Intelligence,
        Wits,
        Resolve
    }
}
