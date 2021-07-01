using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Power", menuName = "Character/Power")]
public class DisciplinePower : ScriptableObject
{
    [SerializeField, Tooltip("Drzewo dyscyplin do którego należy dyscyplina.")]
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
            public EffectModifier propertyModifier;
            public AttributeType affectedProperty;
            public ModifierType modifierType;
            public int modifierValue;
            public bool hasDefenseRoll = false;
            public DicePool defenseRoll = new DicePool();
        }

        public SkillEffect skillEffect = new SkillEffect();
        [System.Serializable]
        public class SkillEffect : Effect
        {
            public EffectModifier propertyModifier;
            public SkillType affectedProperty;
            public ModifierType modifierType;
            public int modifierValue;
            public bool hasDefenseRoll = false;
            public DicePool defenseRoll = new DicePool();
        }

        public DisciplineEffect disciplineEffect = new DisciplineEffect();
        [System.Serializable]
        public class DisciplineEffect : Effect
        {
            public EffectModifier propertyModifier;
            public DisciplineType affectedProperty;
            public ModifierType modifierType;
            public int modifierValue;
            public bool hasDefenseRoll = false;
            public DicePool defenseRoll = new DicePool();
        }

        public TrackerEffect trackerEffect = new TrackerEffect();
        [System.Serializable]
        public class TrackerEffect : Effect
        {
            public EffectModifier propertyModifier;
            public TrackerType affectedProperty;
            public AffectedValue affectedValue;
            public ModifierType modifierType;
            public int modifierValue;
            public bool hasDefenseRoll = false;
            public DicePool defenseRoll = new DicePool();
        }

        public ScriptEffect scriptEffect = new ScriptEffect();
        [System.Serializable]
        public class ScriptEffect : Effect
        {
            public DisciplineScript script;
        }

        [System.Serializable]
        public class DicePool
        {
            public FieldType firstPool, secondPool;
            public AttributeType attribute, secondAttribute;
            public SkillType skill, secondSkill;
            public DisciplineType discipline, secondDiscipline;
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