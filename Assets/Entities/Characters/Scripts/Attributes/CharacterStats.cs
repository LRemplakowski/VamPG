using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats", menuName = "Character/Stats/Mortal")]
public class CharacterStats : ScriptableObject
{
    [SerializeField]
    private Tracker 
        health = new Tracker(TrackerType.Health), 
        willpower = new Tracker(TrackerType.Willpower);
    // MAIN STATS
    [SerializeField]
    private Attribute[] attributes = new Attribute[9] {
        new Attribute(AttributeType.Strength),
        new Attribute(AttributeType.Dexterity),
        new Attribute(AttributeType.Stamina),
        new Attribute(AttributeType.Charisma),
        new Attribute(AttributeType.Manipulation),
        new Attribute(AttributeType.Composure),
        new Attribute(AttributeType.Intelligence),
        new Attribute(AttributeType.Wits),
        new Attribute(AttributeType.Resolve)
        };

    // SKILLS
    [SerializeField]
    private Skill[] skills = new Skill[27] {
        /*--PHYSICAL--*/
        new Skill(SkillType.Atheltics),
        new Skill(SkillType.Brawl),
        new Skill(SkillType.Craft),
        new Skill(SkillType.Drive),
        new Skill(SkillType.Firearms),
        new Skill(SkillType.Larceny),
        new Skill(SkillType.Melee),
        new Skill(SkillType.Stealth),
        new Skill(SkillType.Survival),
        /*--SOCIAL--*/
        new Skill(SkillType.AnimalKen),
        new Skill(SkillType.Etiquette),
        new Skill(SkillType.Insight),
        new Skill(SkillType.Intimidation),
        new Skill(SkillType.Leadership),
        new Skill(SkillType.Performance),
        new Skill(SkillType.Persuasion),
        new Skill(SkillType.Streetwise),
        new Skill(SkillType.Subterfuge),
        /*--MENTAL--*/
        new Skill(SkillType.Academics),
        new Skill(SkillType.Awareness),
        new Skill(SkillType.Finance),
        new Skill(SkillType.Investigation),
        new Skill(SkillType.Medicine),
        new Skill(SkillType.Occult),
        new Skill(SkillType.Politics),
        new Skill(SkillType.Science),
        new Skill(SkillType.Technology)
        };

    public virtual Tracker GetTracker(TrackerType type)
    {
        switch(type)
        {
            case TrackerType.Health:
                return health;
            case TrackerType.Willpower:
                return willpower;
            default:
                return new Tracker(TrackerType.Invalid);
        }
    }

    public Attribute GetAttribute(AttributeType type)
    {
        foreach (Attribute a in attributes)
        {
            if (a.GetAttributeType().Equals(type))
            {
                return a;
            }
        }
        return new Attribute(AttributeType.Invalid);
    }

    public Skill GetSkill(SkillType type)
    {
        foreach (Skill s in skills)
        {
            if (s.GetSkillType().Equals(type))
            {
                return s;
            }
        }
        return new Skill(SkillType.Invalid);
    }
}
