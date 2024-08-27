using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.UI;
using SunsetSystems.UI.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Spellbook
{
    [CreateAssetMenu(fileName = "New Power", menuName = "Character/Power")]
    public class DisciplinePower : SerializedScriptableObject, IUserInfertaceDataProvider<DisciplinePower>
    {
        [field: SerializeField]
        public string PowerName { get; private set; }
        [field: SerializeField]
        public AssetReferenceSprite PowerIcon { get; private set; }
        [field: SerializeField]
        public AssetReference PowerGUIButtonAsset { get; private set; }
        [field: SerializeField, MultiLineProperty]
        public string PowerDescription { get; private set; }
        [field: SerializeField]
        public int BloodCost { get; private set; }
        [field: SerializeField]
        public int Cooldown { get; private set; }
        [SerializeField, ReadOnly]
        private string _id = Guid.NewGuid().ToString();
        public string ID => _id;
        [SerializeField]
        private DisciplineType type;
        public DisciplineType Type { get => type; }
        [SerializeField, Range(1, 5)]
        private int level = 1;
        public int Level { get => level; }
        [SerializeField]
        private DisciplineType secondaryType;
        public DisciplineType SecondaryType { get => secondaryType; }
        [SerializeField, Range(0, 5)]
        private int secondaryLevel;
        public int SecondaryLevel { get => secondaryLevel; }

        [SerializeField]
        private Target _target = Target.Self;
        public Target Target { get => _target; }
        [SerializeField]
        private Duration _duration = Duration.Immediate;
        public Duration Duration => _duration;
        [SerializeField]
        private Range range = Range.Ranged;
        public Range Range { get => range; }
        [SerializeField]
        private TargetableCreatureType _targetableCreatureType = TargetableCreatureType.Any;
        public TargetableCreatureType TargetableCreatureType { get => _targetableCreatureType; }

        private TooltipContent _powerTooltip;
        public TooltipContent Tooltip => _powerTooltip;
        public DisciplinePower UIData => this;

        [SerializeField]
        private List<IEffect> effects = new();

        private void OnValidate()
        {
            _powerTooltip._title = this.PowerName;
            _powerTooltip._description = this.PowerDescription;
            if (string.IsNullOrWhiteSpace(_id))
                _id = Guid.NewGuid().ToString();
        }

        public IEnumerable<IEffect> GetEffects()
        {
            return effects;
        }


        public bool IsValidTarget(ITargetable target, IMagicUser caster)
        {
            return _target switch
            {
                Target.Self => target.IsMe(caster.References.CombatBehaviour),
                Target.Friendly => target.IsFriendlyTowards(caster.References.CombatBehaviour),
                Target.Hostile => target.IsHostileTowards(caster.References.CombatBehaviour),
                Target.AOE_Friendly => throw new NotImplementedException(),
                Target.AOE_Hostile => throw new NotImplementedException(),
                Target.Invalid => false,
                _ => false,
            };
        }
    }
}