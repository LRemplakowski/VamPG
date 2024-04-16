using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Spellbook;
using System.Linq;

namespace SunsetSystems.Entities.Data
{
    [CreateAssetMenu(fileName = "New Stats", menuName = "Character/Stats")]
    public class StatsConfig : ScriptableObject
    {
        public Trackers Trackers = Trackers.Initialize();
        public Clan Clan = Clan.Invalid;
        [Range(1, 16)]
        public int Generation = 12;
        public int BloodPotency = 1;
        public Attributes Attributes = Attributes.Initialize();
        public Skills Skills = Skills.Initialize();  
    }

    [Serializable]
    public class Trackers
    {
        [SerializeField]
        private Tracker health, willpower, hunger, humanity;

        public static Trackers Initialize()
        {
            Trackers result = new()
            {
                health = new(TrackerType.Health),
                willpower = new(TrackerType.Willpower),
                hunger = new(TrackerType.Hunger),
                humanity = new(TrackerType.Humanity)
            };
            result.hunger.SetValue(0);
            return result;
        }

        public static Trackers DeepCopy(Trackers existing)
        {
            if (existing == null)
                return Initialize();
            Trackers result = new()
            {
                health = new(existing.health),
                willpower = new(existing.willpower),
                hunger = new(existing.hunger),
                humanity = new(existing.humanity)
            };
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
    public class Attributes
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
            resolve,
            speed;

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
            result.speed = new CreatureAttribute(AttributeType.Speed);
            result.speed.SetValue(5);
            return result;
        }

        public static Attributes DeepCopy(Attributes existing)
        {
            if (existing == null)
                return Initialize();
            Attributes result = new();
            result.strength = new(existing.strength);
            result.dexterity = new(existing.dexterity);
            result.stamina = new(existing.stamina);
            result.charisma = new(existing.charisma);
            result.manipulation = new(existing.manipulation);
            result.composure = new(existing.composure);
            result.intelligence = new(existing.intelligence);
            result.wits = new(existing.wits);
            result.resolve = new(existing.resolve);
            result.speed = new(existing.speed);
            return result;
        }

        public List<CreatureAttribute> GetAttributeList()
        {
            return new List<CreatureAttribute>()
            {
                strength, dexterity, stamina,
                charisma, manipulation, composure,
                intelligence, wits, resolve, speed
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
                AttributeType.Speed => speed,
                _ => new(AttributeType.Invalid),
            };
        }
    }

    [Serializable]
    public class Skills
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

        public static Skills DeepCopy(Skills existing)
        {
            if (existing == null)
                return Initialize();
            Skills result = new();
            //PHYSICAL
            result.athletics = new Skill(existing.athletics);
            result.brawl = new Skill(existing.brawl);
            result.craft = new Skill(existing.craft);
            result.drive = new Skill(existing.drive);
            result.firearms = new Skill(existing.firearms);
            result.larceny = new Skill(existing.larceny);
            result.melee = new Skill(existing.melee);
            result.stealth = new Skill(existing.stealth);
            result.survival = new Skill(existing.survival);
            //SOCIAL
            result.animalKen = new Skill(existing.animalKen);
            result.etiquette = new Skill(existing.etiquette);
            result.insight = new Skill(existing.insight);
            result.intimidation = new Skill(existing.intimidation);
            result.leadership = new Skill(existing.leadership);
            result.performance = new Skill(existing.performance);
            result.persuasion = new Skill(existing.persuasion);
            result.streetwise = new Skill(existing.streetwise);
            result.subterfuge = new Skill(existing.subterfuge);
            //MENTAL
            result.academics = new Skill(existing.academics);
            result.awarness = new Skill(existing.awarness);
            result.finance = new Skill(existing.finance);
            result.investigation = new Skill(existing.investigation);
            result.medicine = new Skill(existing.medicine);
            result.occult = new Skill(existing.occult);
            result.politics = new Skill(existing.politics);
            result.science = new Skill(existing.science);
            result.technology = new Skill(existing.technology);
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
                athletics, brawl, craft, drive, firearms, larceny, melee, stealth, survival,
                animalKen, etiquette, insight, intimidation, leadership, performance, persuasion, streetwise, subterfuge,
                academics, awarness, finance, investigation, medicine, occult, politics, science, technology
            };
        }
    }

    [Serializable]
    public class Disciplines
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
            protean,
            athletics,
            firearms,
            melee,
            streetwise,
            insight,
            medicine,
            larceny,
            intimidation;

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
            result.athletics = new(DisciplineType.Athletics);
            result.firearms = new(DisciplineType.Firearms);
            result.melee = new(DisciplineType.Melee);
            result.streetwise = new(DisciplineType.Streetwise);
            result.insight = new(DisciplineType.Insight);
            result.medicine = new(DisciplineType.Medicine);
            result.larceny = new(DisciplineType.Larceny);
            result.intimidation = new(DisciplineType.Intimidation);
            return result;
        }

        public static Disciplines DeepCopy(Disciplines existing)
        {
            if (existing == null)
                return Initialize();
            Disciplines result = new();
            result.animalism = new(existing.animalism);
            result.auspex = new(existing.auspex);
            result.bloodSorcery = new(existing.bloodSorcery);
            result.celerity = new(existing.celerity);
            result.dominate = new(existing.dominate);
            result.fortitude = new(existing.fortitude);
            result.obfuscate = new(existing.obfuscate);
            result.oblivion = new(existing.oblivion);
            result.potence = new(existing.potence);
            result.presence = new(existing.presence);
            result.protean = new(existing.protean);
            result.athletics = new(existing.athletics);
            result.firearms = new(existing.firearms);
            result.melee = new(existing.melee);
            result.streetwise = new(existing.streetwise);
            result.insight = new(existing.insight);
            result.medicine = new(existing.medicine);
            result.larceny = new(existing.larceny);
            result.intimidation = new(existing.intimidation);
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
                DisciplineType.Athletics => athletics,
                DisciplineType.Firearms => firearms,
                DisciplineType.Melee => melee,
                DisciplineType.Streetwise => streetwise,
                DisciplineType.Insight => insight,
                DisciplineType.Medicine => medicine,
                DisciplineType.Larceny => larceny,
                DisciplineType.Intimidation => intimidation,
                _ => throw new NotImplementedException(),
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
            protean,
            //athletics,
            //firearms,
            //melee,
            //streetwise,
            //insight,
            //medicine,
            //larceny,
            //intimidation
        };
    }
}
