using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Power", menuName = "Character/Power")]
public class DisciplinePower : ScriptableObject
{
    [SerializeField, Tooltip("Drzewo dyscyplin do którego należy dyscyplina.")]
    private DisciplineType type = DisciplineType.Invalid;
    [SerializeField, Range(1, 5), Tooltip("Minimalny poziom potrzebny do wykupienia dyscypliny.")]
    private int level = 1;
    [SerializeField, Tooltip("Wymagany przy amalgamatach.")]
    private DisciplineType secondaryType = DisciplineType.Invalid;
    [SerializeField, Range(0, 5), Tooltip("Wymagany przy amalgamatach. Używany tylko jeśli Secondary Type nie jest Invalid.")]
    private int secondaryLevel;

    [SerializeField]
    private Effects[] effects = new Effects[1];
    [System.Serializable]
    private class Effects
    {
        public bool isExpanded = false;

        public EffectType effectType;
        public Target target = Target.Self;

        public AttributeEffect attributeEffect = new AttributeEffect();
        [System.Serializable]
        public class AttributeEffect
        {
            public EffectModifier propertyModifier;
            public AttributeType affectedProperty;
            public ModifierType modifierType;
            public int modifierValue;
            public DicePool disciplineRoll = new DicePool();
            public bool hasDefenseRoll = false;
            public DicePool defenseRoll = new DicePool();
        }

        public SkillEffect skillEffect = new SkillEffect();
        [System.Serializable]
        public class SkillEffect
        {
            public EffectModifier propertyModifier;
            public SkillType affectedProperty;
            public ModifierType modifierType;
            public int modifierValue;
            public DicePool disciplineRoll = new DicePool();
            public bool hasDefenseRoll = false;
            public DicePool defenseRoll = new DicePool();
        }

        public DisciplineEffect disciplineEffect = new DisciplineEffect();
        [System.Serializable]
        public class DisciplineEffect
        {
            public EffectModifier propertyModifier;
            public DisciplineType affectedProperty;
            public ModifierType modifierType;
            public int modifierValue;
            public DicePool disciplineRoll = new DicePool();
            public bool hasDefenseRoll = false;
            public DicePool defenseRoll = new DicePool();
        }

        public TrackerEffect trackerEffect = new TrackerEffect();
        [System.Serializable]
        public class TrackerEffect
        {
            public EffectModifier propertyModifier;
            public TrackerType affectedProperty;
            public AffectedValue affectedValue;
            public ModifierType modifierType;
            public int modifierValue;
            public DicePool disciplineRoll = new DicePool();
            public bool hasDefenseRoll = false;
            public DicePool defenseRoll = new DicePool();
        }

        public ScriptEffect scriptEffect = new ScriptEffect();
        [System.Serializable]
        public class ScriptEffect
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