using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Data
{
    [CreateAssetMenu(fileName = "New Stats", menuName = "Character/Stats")]
    public class StatsConfig : ScriptableObject
    {
        public Trackers trackers = Trackers.Initialize();
        public Clan clan = Clan.Invalid;
        [Range(1, 16)]
        public int generation = 12;
        public Attributes attributes = Attributes.Initialize();
        public Skills skills = Skills.Initialize();
        public Disciplines disciplines = Disciplines.Initialize();

        public virtual Tracker GetTracker(TrackerType type)
        {
            return trackers.GetTracker(type);
        }

        public CreatureAttribute GetAttribute(AttributeType type)
        {
            return attributes.GetAttribute(type);
        }

        public Skill GetSkill(SkillType type)
        {
            return skills.GetSkill(type);
        }

        public Discipline GetDiscipline(DisciplineType type)
        {
            return disciplines.GetDiscipline(type);
        }

        public DisciplinePower GetDisciplinePower(DisciplineType disciplineType, int powerIndex)
        {
            return GetDiscipline(disciplineType).GetPower(powerIndex);
        }

        public DisciplinePower GetDisciplinePower(string scriptName)
        {
            foreach (Discipline d in disciplines.GetDisciplines())
            {
                foreach (DisciplinePower p in d.GetKnownPowers())
                {
                    if (p.ScriptName.Equals(scriptName))
                        return p;
                }
            }
            return CreateInstance<DisciplinePower>();
        }
        public List<CreatureAttribute> GetAttributes()
        {
            return attributes.GetAttributeList();
        }       
    }

    [Serializable]
    public struct Trackers
    {
        [SerializeField]
        private Tracker health, willpower, hunger, humanity;

        public static Trackers Initialize()
        {
            Trackers result = new();
            result.health = new(TrackerType.Health);
            result.willpower = new(TrackerType.Willpower);
            result.hunger = new(TrackerType.Hunger);
            result.humanity = new(TrackerType.Humanity);
            return result;
        }

        public Tracker GetTracker(TrackerType type)
        {
            return type switch
            {
                TrackerType.Health => health,
                TrackerType.Willpower => willpower,
                TrackerType.Humanity => humanity,
                TrackerType.Hunger => hunger,
                _ => new Tracker(TrackerType.Invalid),
            };
        }
    }

    [Serializable]
    public struct Attributes
    {
        [SerializeField]
        private CreatureAttribute
            //PHYSICAL
            strength,
            dexterity,
            stamina,
            //SOCIAL
            charisma,
            manipulation,
            composure,
            //MENTAL
            intelligence,
            wits,
            resolve;

        public static Attributes Initialize()
        {
            Attributes result = new();
            //PHYSICAL
            result.strength = new CreatureAttribute(AttributeType.Strength);
            result.dexterity = new CreatureAttribute(AttributeType.Dexterity);
            result.stamina = new CreatureAttribute(AttributeType.Stamina);
            //SOCIAL
            result.charisma = new CreatureAttribute(AttributeType.Charisma);
            result.manipulation = new CreatureAttribute(AttributeType.Manipulation);
            result.composure = new CreatureAttribute(AttributeType.Composure);
            //MENTAL
            result.intelligence = new CreatureAttribute(AttributeType.Intelligence);
            result.wits = new CreatureAttribute(AttributeType.Wits);
            result.resolve = new CreatureAttribute(AttributeType.Resolve);
            return result;
        }

        public List<CreatureAttribute> GetAttributeList()
        {
            return new List<CreatureAttribute>()
            {
                strength, dexterity, stamina,
                charisma, manipulation, composure,
                intelligence, wits, resolve
            };
        }

        public CreatureAttribute GetAttribute(AttributeType attributeType)
        {
            return attributeType switch
            {
                AttributeType.Strength => strength,
                AttributeType.Dexterity => dexterity,
                AttributeType.Stamina => stamina,
                AttributeType.Charisma => charisma,
                AttributeType.Manipulation => manipulation,
                AttributeType.Composure => composure,
                AttributeType.Intelligence => intelligence,
                AttributeType.Wits => wits,
                AttributeType.Resolve => resolve,
                _ => new(AttributeType.Invalid),
            };
        }
    }

    [Serializable]
    public struct Skills
    {
        [SerializeField]
        private Skill
            //PHYSICAL
            athletics,
            brawl,
            craft,
            drive,
            firearms,
            larceny,
            melee,
            stealth,
            survival,
            //SOCIAL
            animalKen,
            etiquette,
            insight,
            intimidation,
            leadership,
            performance,
            persuasion,
            streetwise,
            subterfuge,
            //MENTAL
            academics,
            awarness,
            finance,
            investigation,
            medicine,
            occult,
            politics,
            science,
            technology;

        public static Skills Initialize()
        {
            Skills result = new();
            //PHYSICAL
            result.athletics = new Skill(SkillType.Athletics);
            result.brawl = new Skill(SkillType.Brawl);
            result.craft = new Skill(SkillType.Craft);
            result.drive = new Skill(SkillType.Drive);
            result.firearms = new Skill(SkillType.Firearms);
            result.larceny = new Skill(SkillType.Larceny);
            result.melee = new Skill(SkillType.Melee);
            result.stealth = new Skill(SkillType.Stealth);
            result.survival = new Skill(SkillType.Survival);
            //SOCIAL
            result.animalKen = new Skill(SkillType.AnimalKen);
            result.etiquette = new Skill(SkillType.Etiquette);
            result.insight = new Skill(SkillType.Insight);
            result.intimidation = new Skill(SkillType.Intimidation);
            result.leadership = new Skill(SkillType.Leadership);
            result.performance = new Skill(SkillType.Performance);
            result.persuasion = new Skill(SkillType.Persuasion);
            result.streetwise = new Skill(SkillType.Streetwise);
            result.subterfuge = new Skill(SkillType.Subterfuge);
            //MENTAL
            result.academics = new Skill(SkillType.Academics);
            result.awarness = new Skill(SkillType.Awareness);
            result.finance = new Skill(SkillType.Finance);
            result.investigation = new Skill(SkillType.Investigation);
            result.medicine = new Skill(SkillType.Medicine);
            result.occult = new Skill(SkillType.Occult);
            result.politics = new Skill(SkillType.Politics);
            result.science = new Skill(SkillType.Science);
            result.technology = new Skill(SkillType.Technology);
            return result;
        }

        public Skill GetSkill(SkillType type)
        {
            return type switch
            {
                SkillType.Athletics => athletics,
                SkillType.Brawl => brawl,
                SkillType.Craft => craft,
                SkillType.Drive => drive,
                SkillType.Firearms => firearms,
                SkillType.Larceny => larceny,
                SkillType.Melee => melee,
                SkillType.Stealth => stealth,
                SkillType.Survival => survival,
                SkillType.AnimalKen => animalKen,
                SkillType.Etiquette => etiquette,
                SkillType.Insight => insight,
                SkillType.Intimidation => intimidation,
                SkillType.Leadership => leadership,
                SkillType.Performance => performance,
                SkillType.Persuasion => persuasion,
                SkillType.Streetwise => streetwise,
                SkillType.Subterfuge => subterfuge,
                SkillType.Academics => academics,
                SkillType.Awareness => awarness,
                SkillType.Finance => finance,
                SkillType.Investigation => investigation,
                SkillType.Medicine => medicine,
                SkillType.Occult => occult,
                SkillType.Politics => politics,
                SkillType.Science => science,
                SkillType.Technology => technology,
                _ => new Skill(SkillType.Invalid),
            };
        }

        public List<Skill> GetSkillList()
        {
            return new List<Skill>()
            {
                athletics, brawl, craft, drive, firearms, melee, stealth, survival,
                animalKen, etiquette, insight, intimidation, leadership, performance, persuasion, streetwise, subterfuge,
                academics, awarness, finance, investigation, medicine, occult, politics, science, technology
            };
        }
    }

    [Serializable]
    public struct Disciplines
    {
        [SerializeField]
        private Discipline
            animalism,
            auspex,
            bloodSorcery,
            celerity,
            dominate,
            fortitude,
            obfuscate,
            oblivion,
            potence,
            presence,
            protean;

        public static Disciplines Initialize()
        {
            Disciplines result = new();
            result.animalism = new(DisciplineType.Animalism);
            result.auspex = new(DisciplineType.Auspex);
            result.bloodSorcery = new(DisciplineType.BloodSorcery);
            result.celerity = new(DisciplineType.Celerity);
            result.dominate = new(DisciplineType.Dominate);
            result.fortitude = new(DisciplineType.Fortitude);
            result.obfuscate = new(DisciplineType.Obfuscate);
            result.oblivion = new(DisciplineType.Oblivion);
            result.potence = new(DisciplineType.Potence);
            result.presence = new(DisciplineType.Presence);
            result.protean = new(DisciplineType.Protean);
            return result;
        }

        public Discipline GetDiscipline(DisciplineType type)
        {
            return type switch
            {
                DisciplineType.Animalism => animalism,
                DisciplineType.Auspex => auspex,
                DisciplineType.BloodSorcery => bloodSorcery,
                DisciplineType.Celerity => celerity,
                DisciplineType.Dominate => dominate,
                DisciplineType.Fortitude => fortitude,
                DisciplineType.Obfuscate => obfuscate,
                DisciplineType.Oblivion => oblivion,
                DisciplineType.Potence => potence,
                DisciplineType.Presence => presence,
                DisciplineType.Protean => protean,
                _ => new(DisciplineType.Invalid),
            };
        }

        public List<Discipline> GetDisciplines() => new()
        {
            animalism,
            auspex,
            bloodSorcery,
            celerity,
            dominate,
            fortitude,
            obfuscate,
            oblivion,
            potence,
            presence,
            protean
        };
    }
}
