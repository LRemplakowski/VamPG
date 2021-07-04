using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Power", menuName = "Character/Power")]
public class DisciplinePower : ScriptableObject
{
    private const string typeTooltip = "Drzewo dyscyplin do którego należy dyscyplina.";
    private const string levelTooltip = "Minimalny poziom potrzebny do wykupienia dyscypliny. Ma znaczenie tylko dla rozwoju postaci.";
    private const string secondaryTypeTooltip = "Wymagany przy amalgamatach.";
    private const string secondaryLevelTooltip = "Wymagany przy amalgamatach. Używany tylko jeśli Secondary Type nie jest Invalid. Ma znaczenie tylko przy rozwoju postaci.";
    private const string _targetTooltip = "Cel na jaki wpływa dyscyplina.";
    private const string rangeTooltip = "Zasięg w jakim może być użyta dyscyplina.";
    private const string hasDisciplinePoolString = "Czy moc posiada dice pool niezbędny do skutecznej aktywacji.";
    private const string disciplineRollDifficultyTooltip = "ST jakie musi przerzucić użytkownik by skutecznie aktywować dyscyplinę.";
    private const string hasAttackPoolTooltip = "Czy moc posiada odrębną pule kości dla ataku. Ma znaczenie tylko dla mocy o celu innym niż Self.";
    private const string defensePoolTooltip = "Czy moc posiada pulę kości dla broniącego się celu. " 
        + "Ma znaczenie tylko dla mocy o celu innym niż Self. " 
        + "Ta pula kości będzie sporna z Attack Pool lub, jeśli moc go nie posiada, Discipline Pool. " 
        + "Właściwość zostanie zignorowana jeśli dyscyplina nie ma żadnej z tych puli kości.";
    private const string targetableCreatureTypeTooltip = "Rodzaj istot na jakie może wpływać moc. Ma znaczenie tylko dla dyscyplin o celu innym niż Self.";

    [SerializeField]
    private string _scriptName;
    public string ScriptName { get; }
    [SerializeField, Tooltip(typeTooltip)]
    private DisciplineType type = DisciplineType.Invalid;
    public DisciplineType Type { get => type; }
    [SerializeField, Range(1, 5), Tooltip("Minimalny poziom potrzebny do wykupienia dyscypliny.")]
    private int level = 1;
    public int Level { get => level; }
    [SerializeField, Tooltip("Wymagany przy amalgamatach.")]
    private DisciplineType secondaryType = DisciplineType.Invalid;
    public DisciplineType SecondaryType { get => secondaryType; }
    [SerializeField, Range(0, 5), Tooltip("Wymagany przy amalgamatach. Używany tylko jeśli Secondary Type nie jest Invalid.")]
    private int secondaryLevel;
    public int SecondaryLevel { get => secondaryLevel; }

    public Effects[] GetEffects()
    {
        return effects;
    }

    [SerializeField]
    private Effects[] effects = new Effects[1];
    [System.Serializable]
    public class Effects
    {
        //Editor variables
        [SerializeField]
        private bool isExpanded = false;

        //Discipline variables
        [SerializeField]
        private EffectType effectType;
        public EffectType EffectType { get => effectType; }
        [SerializeField]
        private Target target = Target.Self;
        public Target Target { get => target; }
        [SerializeField]
        private Range range = Range.Ranged;
        public Range Range { get => range; }
        [SerializeField]
        private bool hasAttackPool = false;
        public bool HasAttackPool { get => hasAttackPool; }
        [SerializeField]
        private DicePool attackPool = new DicePool();
        public DicePool AttackPool { get => attackPool; }
        [SerializeField]
        private bool hasDiciplinePool = false;
        public bool HasDisciplinePool { get => hasDiciplinePool; }
        [SerializeField]
        private DicePool disciplinePool = new DicePool();
        public DicePool DisciplinePool { get => disciplinePool; }
        [SerializeField]
        private int disciplineRollDifficulty = 0;
        public int DisciplineRollDifficulty { get => disciplineRollDifficulty; }
        [SerializeField]
        private bool hasDefensePool = false;
        public bool HasDefensePool { get => hasDefensePool; }
        [SerializeField]
        private DicePool defensePool = new DicePool();
        public DicePool DefensePool { get => defensePool; }
        [SerializeField]
        private TargetableCreatureType targetableCreatureType = TargetableCreatureType.Any;
        public TargetableCreatureType TargetableCreatureType { get => targetableCreatureType; }
        [SerializeField]
        private Duration duration = Duration.Immediate;
        public Duration Duration { get => duration; }
        [SerializeField]
        private int rounds = 0;
        public int Rounds { get => rounds; }

        public abstract class Effect { }

        public Effect GetEffect()
        {
            return effectType switch
            {
                EffectType.Attribute => attributeEffect,
                EffectType.Skill => skillEffect,
                EffectType.Discipline => disciplineEffect,
                EffectType.Tracker => trackerEffect,
                EffectType.ScriptDriven => scriptEffect,
                _ => scriptEffect,
            };
        }

        public AttributeEffect attributeEffect = new AttributeEffect();
        [System.Serializable]
        public class AttributeEffect : Effect
        {
            [SerializeField]
            private EffectModifier propertyModifier;
            public EffectModifier PropertyModifier { get => propertyModifier; }
            [SerializeField]
            private AttributeType affectedProperty;
            public AttributeType AffectedProperty { get => affectedProperty; }
            [SerializeField]
            private ModifierType modifierType;
            public ModifierType ModifierType { get => modifierType; }
            [SerializeField]
            private int modifierValue;
            public int ModifierValue { get => modifierValue; }
            [SerializeField]
            private bool hasDefenseRoll = false;
            public bool HasDefenseRoll { get => hasDefenseRoll; }
            [SerializeField]
            private DicePool defenseRoll = new DicePool();
            public DicePool DefenseRoll { get => defenseRoll; }
        }

        public SkillEffect skillEffect = new SkillEffect();
        [System.Serializable]
        public class SkillEffect : Effect
        {
            [SerializeField]
            private EffectModifier propertyModifier;
            public EffectModifier PropertyModifier { get => propertyModifier; }
            [SerializeField]
            private SkillType affectedProperty;
            public SkillType AffectedProperty { get => affectedProperty; }
            [SerializeField]
            private ModifierType modifierType;
            public ModifierType ModifierType { get => modifierType; }
            [SerializeField]
            private int modifierValue;
            public int ModifierValue { get => modifierValue; }
            [SerializeField]
            private bool hasDefenseRoll = false;
            public bool HasDefenseRoll { get => hasDefenseRoll; }
            [SerializeField]
            private DicePool defenseRoll = new DicePool();
            public DicePool DefenseRoll { get => defenseRoll; }
        }

        public DisciplineEffect disciplineEffect = new DisciplineEffect();
        [System.Serializable]
        public class DisciplineEffect : Effect
        {
            [SerializeField]
            private EffectModifier propertyModifier;
            public EffectModifier PropertyModifier { get => propertyModifier; }
            [SerializeField]
            private DisciplineType affectedProperty;
            public DisciplineType AffectedProperty { get => affectedProperty; }
            [SerializeField]
            private ModifierType modifierType;
            public ModifierType ModifierType { get => modifierType; }
            [SerializeField]
            private int modifierValue;
            public int ModifierValue { get => modifierValue; }
            [SerializeField]
            private bool hasDefenseRoll = false;
            public bool HasDefenseRoll { get => hasDefenseRoll; }
            [SerializeField]
            private DicePool defenseRoll = new DicePool();
            public DicePool DefenseRoll { get => defenseRoll; }
        }

        public TrackerEffect trackerEffect = new TrackerEffect();
        [System.Serializable]
        public class TrackerEffect : Effect
        {
            [SerializeField]
            private EffectModifier propertyModifier;
            public EffectModifier PropertyModifier { get => propertyModifier; }
            [SerializeField]
            private TrackerType affectedProperty;
            public TrackerType AffectedProperty { get => affectedProperty; }
            [SerializeField]
            private AffectedValue affectedValue;
            public AffectedValue AffectedValue { get => affectedValue; }
            [SerializeField]
            private ModifierType modifierType;
            public ModifierType ModifierType { get => modifierType; }
            [SerializeField]
            private int modifierValue;
            public int ModifierValue { get => modifierValue; }
            [SerializeField]
            private bool hasDefenseRoll = false;
            public bool HasDefenseRoll { get => hasDefenseRoll; }
            [SerializeField]
            private DicePool defenseRoll = new DicePool();
            public DicePool DefenseRoll { get => defenseRoll; }

        }

        public ScriptEffect scriptEffect = new ScriptEffect();
        [System.Serializable]
        public class ScriptEffect : Effect
        {
            [SerializeField]
            private DisciplineScript script;
            public DisciplineScript Script { get => script; }
        }

        [System.Serializable]
        public class DicePool
        {
            [SerializeField]
            private FieldType firstPool, secondPool;
            public FieldType FirstPool { get => firstPool; }
            public FieldType SecondPool { get => secondPool; }
            [SerializeField]
            private AttributeType attribute, secondAttribute;
            public AttributeType Attribute { get => attribute; }
            public AttributeType SecondAttribute { get => secondAttribute; }
            [SerializeField]
            private SkillType skill, secondSkill;
            public SkillType Skill { get => skill; }
            public SkillType SecondSkill { get => secondSkill; }
            [SerializeField]
            private DisciplineType discipline, secondDiscipline;
            public DisciplineType Discipline { get => discipline; }
            public DisciplineType SecondDiscipline { get => secondDiscipline; }
        }

    }

    public enum AffectedValue
    {
        MaxValue,
        CurrentValue
    }

    public enum EffectModifier
    {
        StaticValue,
        LevelBased,
        RollBased
    }

    public enum FieldType
    {
        Attribute,
        Skill,
        Discipline
    }
}