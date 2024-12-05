using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public abstract class AbstractAbility : SerializedScriptableObject, IAbility
    {
        [BoxGroup("UI Data")]
        [SerializeField]
        private string _fallbackName;
        [BoxGroup("UI Data")]
        [SerializeField, MultiLineProperty]
        private string _fallbackDescription;
        [BoxGroup("UI Data")]
        [SerializeField, DictionaryDrawerSettings(IsReadOnly = true)]
        private Dictionary<IAbilityUIData.IconState, Sprite> _icons = new();
        [BoxGroup("Ability Core")]
        [SerializeField]
        private AbilityCategory _categoryMask;

        private void OnValidate()
        {
            _icons ??= new();
            foreach (IAbilityUIData.IconState iconState in Enum.GetValues(typeof(IAbilityUIData.IconState)))
            {
                if (_icons.ContainsKey(iconState) is false)
                {
                    _icons[iconState] = null;
                }
            }
        }

        public bool Execute(IAbilityContext context, Action onCompleted = null)
        {
            bool canUse = IsContextValidForExecution(context);
            if (canUse)
            {
                _ = DoExecuteAbility(context, onCompleted);
            }
            return canUse;
        }

        public async Awaitable<bool> ExecuteAsync(IAbilityContext context, Action onCompleted = null)
        {
            bool canUse = IsContextValidForExecution(context);
            if (canUse)
            {
                await DoExecuteAbility(context, onCompleted);
            }
            return canUse;
        }

        public AbilityCategory GetCategories()
        {
            return _categoryMask;
        }

        public IAbilityUIData GetAbilityUIData()
        {
            return new AbilityUIData(this);
        }

        public IAbilityCostData GetAbilityCosts(IAbilityContext context)
        {
            return new AbilityCost(context, this);
        }

        public IAbilityTargetingData GetTargetingData(IAbilityContext context)
        {
            return new AbilityTargetingData(context, this);
        }

        private string GetLocalizedName()
        {
            return _fallbackName;
        }

        private string GetLocalizedDescription()
        {
            return _fallbackDescription;
        }

        public abstract bool IsContextValidForExecution(IAbilityContext context);

        protected abstract Awaitable DoExecuteAbility(IAbilityContext context, Action onCompleted);

        protected abstract int GetMovementPointCost(IAbilityContext context);
        protected abstract int GetActionPointCost(IAbilityContext context);
        protected abstract int GetBloodPointCost(IAbilityContext context);

        protected abstract AbilityTargetingType GetAbilityTargetingType(IAbilityContext context);
        protected abstract RangeData GetAbilityRangeData(IAbilityContext context);
        protected abstract AbilityRange GetAbilityRangeType(IAbilityContext context);
        protected abstract TargetableEntityType GetValidTargetsFlag(IAbilityContext context);

        private readonly struct AbilityCost : IAbilityCostData
        {
            public int MovementCost { get; }
            public int ActionPointCost { get; }
            public int BloodCost { get; }

            public AbilityCost(IAbilityContext context, AbstractAbility ability)
            {
                MovementCost = ability.GetMovementPointCost(context);
                ActionPointCost = ability.GetActionPointCost(context);
                BloodCost = ability.GetBloodPointCost(context);
            }
        }

        private readonly struct AbilityTargetingData : IAbilityTargetingData
        {
            private readonly AbilityTargetingType _targetingType;
            private readonly RangeData _rangeData;
            private readonly AbilityRange _rangeType;
            private readonly TargetableEntityType _validTargetsFlag;

            public AbilityTargetingData(IAbilityContext context, AbstractAbility ability)
            {
                _targetingType = ability.GetAbilityTargetingType(context);
                _rangeData = ability.GetAbilityRangeData(context);
                _rangeType = ability.GetAbilityRangeType(context);
                _validTargetsFlag = ability.GetValidTargetsFlag(context);
            }

            public AbilityTargetingType GetAbilityTargetingType()
            {
                return _targetingType;
            }

            public RangeData GetRangeData()
            {
                return _rangeData;
            }

            public AbilityRange GetRangeType()
            {
                return _rangeType;
            }

            public TargetableEntityType GetValidEntityTypesFlag()
            {
                return _validTargetsFlag;
            }
        }

        private readonly struct AbilityUIData : IAbilityUIData
        {
            public readonly Dictionary<IAbilityUIData.IconState, Sprite> _icons;
            public readonly Func<string> _localizedName;
            public readonly Func<string> _localizedDescription;

            public AbilityUIData(AbstractAbility ability)
            {
                _icons = new(ability._icons);
                _localizedName = ability.GetLocalizedName;
                _localizedDescription = ability.GetLocalizedDescription;
            }

            public Sprite GetAbilityIcon(IAbilityUIData.IconState iconState)
            {
                return _icons.TryGetValue(iconState, out var icon) ? icon : null;
            }

            public string GetLocalizedName()
            {
                return _localizedName.Invoke();
            }

            public string GetLocalizedDescription()
            {
                return _localizedDescription.Invoke();
            }
        }
    }
}
