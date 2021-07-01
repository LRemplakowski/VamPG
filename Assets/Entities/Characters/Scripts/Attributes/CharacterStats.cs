using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats", menuName = "Character/Stats")]
public class CharacterStats : ScriptableObject
{
    [SerializeField, Tooltip("Czy dane statystyki s¹ generyczne, czy przeznaczone dla unikalnej postaci?")]
    private bool _isGeneric;
    /// <summary>
    /// Czy dane statystyki s¹ generyczne, czy przeznaczone dla unikalnej postaci. Generyczne statystyki powinny byæ skopiowane do nowej instacji assetu, ¿eby nie modyfikowaæ instancji bazowej.
    /// </summary>
    public bool IsGeneric 
    {
        get => _isGeneric;
        private set => _isGeneric = value; 
    }

    [SerializeField]
    protected Tracker 
        health = new Tracker(TrackerType.Health), 
        willpower = new Tracker(TrackerType.Willpower),
        humanity = new Tracker(TrackerType.Humanity),
        hunger = new Tracker(TrackerType.Hunger);

    [SerializeField]
    protected Clan clan = Clan.Invalid;

    [SerializeField, Range(1, 16)]
    protected int generation = 12;

    // MAIN STATS
    [SerializeField]
    protected Attributes attributes = new Attributes();
    [System.Serializable]
    protected class Attributes
    {
        [SerializeField]
        public Attribute
            //PHYSICAL
            strength = new Attribute(AttributeType.Strength),
            dexterity = new Attribute(AttributeType.Dexterity),
            stamina = new Attribute(AttributeType.Stamina),
            //SOCIAL
            charisma = new Attribute(AttributeType.Charisma),
            manipulation = new Attribute(AttributeType.Manipulation),
            composure = new Attribute(AttributeType.Composure),
            //MENTAL
            intelligence = new Attribute(AttributeType.Intelligence),
            wits = new Attribute(AttributeType.Wits),
            resolve = new Attribute(AttributeType.Resolve);

        public List<Attribute> GetAttributeList()
        {
            return new List<Attribute>() 
            { 
                strength, dexterity, stamina,
                charisma, manipulation, composure,
                intelligence, wits, resolve
            };
        }
    }
     
    // SKILLS
    [SerializeField]
    protected Skills skills = new Skills();
    [System.Serializable]
    protected class Skills
    {
        [SerializeField]
        public Skill 
            //PHYSICAL
            athletics = new Skill(SkillType.Atheltics),
            brawl = new Skill(SkillType.Brawl),
            craft = new Skill(SkillType.Craft),
            drive = new Skill(SkillType.Drive),
            firearms = new Skill(SkillType.Firearms),
            larceny = new Skill(SkillType.Larceny),
            melee = new Skill(SkillType.Melee),
            stealth = new Skill(SkillType.Stealth),
            survival = new Skill(SkillType.Survival),
            //SOCIAL
            animalKen = new Skill(SkillType.AnimalKen),
            etiquette = new Skill(SkillType.Etiquette),
            insight = new Skill(SkillType.Insight),
            intimidation = new Skill(SkillType.Intimidation),
            leadership = new Skill(SkillType.Leadership),
            performance = new Skill(SkillType.Performance),
            persuasion = new Skill(SkillType.Persuasion),
            streetwise = new Skill(SkillType.Streetwise),
            subterfuge = new Skill(SkillType.Subterfuge),
            //MENTAL
            academics = new Skill(SkillType.Academics),
            awarness = new Skill(SkillType.Awareness),
            finance = new Skill(SkillType.Finance),
            investigation = new Skill(SkillType.Investigation),
            medicine = new Skill(SkillType.Medicine),
            occult = new Skill(SkillType.Occult),
            politics = new Skill(SkillType.Politics),
            science = new Skill(SkillType.Science),
            technology = new Skill(SkillType.Technology);

        public Skill GetSkill(SkillType type)
        {
            return type switch
            {
                SkillType.Atheltics => athletics,
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

    [SerializeField]
    protected Disciplines disciplines = new Disciplines();
    [System.Serializable]
    protected class Disciplines
    {
        [SerializeField]
        public Discipline
            animalism = new Discipline(DisciplineType.Animalism),
            auspex = new Discipline(DisciplineType.Auspex),
            bloodSorcery = new Discipline(DisciplineType.BloodSorcery),
            celerity = new Discipline(DisciplineType.Celerity),
            dominate = new Discipline(DisciplineType.Dominate),
            fortitude = new Discipline(DisciplineType.Fortitude),
            obfuscate = new Discipline(DisciplineType.Obfuscate),
            oblivion = new Discipline(DisciplineType.Oblivion),
            potence = new Discipline(DisciplineType.Potence),
            presence = new Discipline(DisciplineType.Presence),
            protean = new Discipline(DisciplineType.Protean);

        public List<Discipline> GetDisciplines()
        {
            return new List<Discipline>()
            {
                animalism, auspex, bloodSorcery, celerity, dominate, fortitude, obfuscate, oblivion, potence, presence, protean
            };
        }
    }

    public virtual Tracker GetTracker(TrackerType type)
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

    public Attribute GetAttribute(AttributeType type)
    {
        return type switch
        {
            AttributeType.Strength => attributes.strength,
            AttributeType.Dexterity => attributes.dexterity,
            AttributeType.Stamina => attributes.stamina,
            AttributeType.Charisma => attributes.charisma,
            AttributeType.Manipulation => attributes.manipulation,
            AttributeType.Composure => attributes.composure,
            AttributeType.Intelligence => attributes.intelligence,
            AttributeType.Wits => attributes.wits,
            AttributeType.Resolve => attributes.resolve,
            _ => new Attribute(AttributeType.Invalid),
        };
    }

    public Skill GetSkill(SkillType type)
    {
        return type switch
        {
            SkillType.Atheltics => skills.athletics,
            SkillType.Brawl => skills.brawl,
            SkillType.Craft => skills.craft,
            SkillType.Drive => skills.drive,
            SkillType.Firearms => skills.firearms,
            SkillType.Larceny => skills.larceny,
            SkillType.Melee => skills.melee,
            SkillType.Stealth => skills.stealth,
            SkillType.Survival => skills.survival,
            SkillType.AnimalKen => skills.animalKen,
            SkillType.Etiquette => skills.etiquette,
            SkillType.Insight => skills.insight,
            SkillType.Intimidation => skills.intimidation,
            SkillType.Leadership => skills.leadership,
            SkillType.Performance => skills.performance,
            SkillType.Persuasion => skills.persuasion,
            SkillType.Streetwise => skills.streetwise,
            SkillType.Subterfuge => skills.subterfuge,
            SkillType.Academics => skills.academics,
            SkillType.Awareness => skills.awarness,
            SkillType.Finance => skills.finance,
            SkillType.Investigation => skills.investigation,
            SkillType.Medicine => skills.medicine,
            SkillType.Occult => skills.occult,
            SkillType.Politics => skills.politics,
            SkillType.Science => skills.science,
            SkillType.Technology => skills.technology,
            _ => new Skill(SkillType.Invalid),
        };
    }

    public Discipline GetDiscipline(DisciplineType type)
    {
        return type switch
        {
            DisciplineType.Animalism => disciplines.animalism,
            DisciplineType.Auspex => disciplines.auspex,
            DisciplineType.BloodSorcery => disciplines.bloodSorcery,
            DisciplineType.Celerity => disciplines.celerity,
            DisciplineType.Dominate => disciplines.dominate,
            DisciplineType.Fortitude => disciplines.fortitude,
            DisciplineType.Obfuscate => disciplines.obfuscate,
            DisciplineType.Oblivion => disciplines.oblivion,
            DisciplineType.Potence => disciplines.potence,
            DisciplineType.Presence => disciplines.presence,
            DisciplineType.Protean => disciplines.protean,
            _ => new Discipline(DisciplineType.Invalid),
        };
    }

    /// <summary>
    /// Kopiuje pola z przekazanego assetu do nowej instancji tego samego typu.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="asset"></param>
    /// <returns>Now¹ instancjê assetu.</returns>
    public virtual T CopyAssetInstance<T>(T asset) where T : CharacterStats
    {
        T newInstance = CreateInstance(typeof(T)) as T;
        newInstance.IsGeneric = asset.IsGeneric;

        newInstance.health.SetValue(asset.health.GetValue(false));
        newInstance.health.CurrentValue = asset.health.CurrentValue;
        newInstance.health.AddModifiers(asset.health.GetModifiers());

        newInstance.willpower.SetValue(asset.willpower.GetValue(false));
        newInstance.willpower.CurrentValue = asset.health.CurrentValue;
        newInstance.willpower.AddModifiers(asset.willpower.GetModifiers());

        newInstance.hunger.SetValue(asset.hunger.GetValue(false));
        newInstance.hunger.CurrentValue = asset.hunger.CurrentValue;
        newInstance.health.AddModifiers(asset.hunger.GetModifiers());

        newInstance.humanity.SetValue(asset.humanity.GetValue(false));
        newInstance.humanity.CurrentValue = asset.humanity.CurrentValue;
        newInstance.humanity.AddModifiers(asset.humanity.GetModifiers());

        foreach (Attribute a in newInstance.attributes.GetAttributeList())
        {
            Attribute other = asset.GetAttribute(a.GetAttributeType());
            a.SetValue(other.GetValue(false));
            a.AddModifiers(other.GetModifiers());
        }

        foreach (Skill s in newInstance.skills.GetSkillList())
        {
            Skill other = asset.GetSkill(s.GetSkillType());
            s.SetValue(other.GetValue(false));
            s.AddModifiers(other.GetModifiers());
        }

        return newInstance;
    }
}
