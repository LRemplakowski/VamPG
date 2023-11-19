using Sirenix.OdinInspector;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Resources;
using SunsetSystems.UI;
using SunsetSystems.UI.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Spellbook
{
    [System.Serializable, CreateAssetMenu(fileName = "New Power", menuName = "Character/Power")]
    public class DisciplinePower : ScriptableObject, IGameDataProvider<DisciplinePower>
    {
        [field: SerializeField]
        public string PowerName { get; private set; }
        [field: SerializeField, MultiLineProperty]
        public string PowerDescription { get; private set; }
        [field: SerializeField]
        public int BloodCost { get; private set; }
        [field: SerializeField]
        public int Cooldown { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [SerializeField, ReadOnly]
        private string _id;
        public string ID => _id;
        [SerializeField]
        private DisciplineType type = DisciplineType.Invalid;
        public DisciplineType Type { get => type; }
        [SerializeField, Range(1, 5)]
        private int level = 1;
        public int Level { get => level; }
        [SerializeField]
        private DisciplineType secondaryType = DisciplineType.Invalid;
        public DisciplineType SecondaryType { get => secondaryType; }
        [SerializeField, Range(0, 5)]
        private int secondaryLevel;
        public int SecondaryLevel { get => secondaryLevel; }

        [SerializeField]
        private Target _target = Target.Self;
        public Target Target { get => _target; }
        [SerializeField]
        private Range range = Range.Ranged;
        public Range Range { get => range; }
        [SerializeField]
        private bool hasDiciplinePool = false;
        public bool HasDisciplinePool { get => hasDiciplinePool; }
        [SerializeField, ShowIf("HasDisciplinePool")]
        private DicePool disciplinePool = new DicePool();
        public DicePool DisciplinePool { get => disciplinePool; }
        [SerializeField]
        private int disciplineRollDifficulty = 0;
        public int DisciplineRollDifficulty { get => disciplineRollDifficulty; }
        [SerializeField]
        private bool hasAttackPool = false;
        public bool HasAttackPool { get => hasAttackPool; }
        [SerializeField, ShowIf("HasAttackPool")]
        private DicePool attackPool = new DicePool();
        public DicePool AttackPool { get => attackPool; }
        [SerializeField]
        private bool _hasDefensePool = false;
        public bool HasDefensePool { get => _hasDefensePool; }
        [SerializeField, ShowIf("HasDefensePool")]
        private DicePool _defensePool = new DicePool();
        public DicePool DefensePool { get => _defensePool; }
        [SerializeField]
        private TargetableCreatureType _targetableCreatureType = TargetableCreatureType.Any;
        public TargetableCreatureType TargetableCreatureType { get => _targetableCreatureType; }

        private TooltipContent _powerTooltip;
        public TooltipContent Tooltip => _powerTooltip;
        public DisciplinePower Data => this;

        private void OnValidate()
        {
            _powerTooltip._title = this.PowerName;
            _powerTooltip._description = this.PowerDescription;
        }

        private void Awake()
        {
            if (string.IsNullOrEmpty(_id))
                _id = Guid.NewGuid().ToString();
        }

        private void OnEnable()
        {
            Icon ??= ResourceLoader.GetFallbackIcon();
        }

        public List<EffectWrapper> GetEffects()
        {
            return effects;
        }

        [SerializeField]
        private List<EffectWrapper> effects = new();
        [System.Serializable]
        public class EffectWrapper
        {
            private const string effectTypeTooltip = "Typ efektu. Na jaką właściwość ma wpływać lub czy ma być obsługiwany osobnym skryptem.";
            private const string affectedCreatureTooltip = "Na kogo ma wpłynąć efekt. Nie ma znaczenia jeśli cel mocy jest Self.";

            //Discipline variables
            [SerializeField, Tooltip(effectTypeTooltip)]
            private EffectType effectType;
            public EffectType EffectType { get => effectType; }
            [SerializeField, Tooltip(affectedCreatureTooltip)]
            private AffectedCreature _affectedCreature = AffectedCreature.Self;
            public AffectedCreature AffectedCreature { get => _affectedCreature; }
            [SerializeField]
            private Duration duration = Duration.Immediate;
            public Duration Duration { get => duration; }
            [SerializeField]
            private int rounds = 0;
            public int Rounds { get => rounds; }

            public abstract class Effect 
            {
            }

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
            
            [ShowIf("effectType", EffectType.Attribute)]
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
            }

            [ShowIf("effectType", EffectType.Skill)]
            public SkillEffect skillEffect = new();
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
            }

            public ScriptEffect scriptEffect = new();
            [System.Serializable]
            public class ScriptEffect : Effect
            {
                [SerializeReference, RequireInterface(typeof(IDisciplineScript))]
                private UnityEngine.Object _script;
                public IDisciplineScript Script { get => _script as IDisciplineScript; }
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

        public enum AffectedCreature
        {
            Self,
            Target
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
}