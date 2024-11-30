using System;
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
        [SerializeField]
        private Sprite _icon;
        [BoxGroup("Ability Core")]
        [SerializeField]
        private AbilityCategory _categoryMask;

        public bool Execute(IAbilityContext context, ITargetable target, Action onCompleted = null)
        {
            bool canUse = CanExecuteAbility(context, target);
            if (canUse)
            {
                _ = DoExecuteAbility(context, target, onCompleted);
            }
            return canUse;
        }

        public async Awaitable<bool> ExecuteAsync(IAbilityContext context, ITargetable target, Action onCompleted = null)
        {
            bool canUse = CanExecuteAbility(context, target);
            if (canUse)
            {
                await DoExecuteAbility(context, target, onCompleted);
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

        public IAbilityCostData GetAbilityCosts(IAbilityUser abilityUser, ITargetable target)
        {
            return new AbilityCost(abilityUser, target, this);
        }

        public IAbilityTargetingData GetTargetingData(IAbilityUser abilityUser)
        {
            return new AbilityTargetingData(abilityUser, this);
        }

        private string GetLocalizedName()
        {
            return _fallbackName;
        }

        private string GetLocalizedDescription()
        {
            return _fallbackDescription;
        }

        public abstract bool IsValidTarget(IAbilityUser abilityUser, ITargetable target);

        protected abstract Awaitable DoExecuteAbility(IAbilityContext abilityUser, ITargetable target, Action onCompleted);
        protected abstract bool CanExecuteAbility(IAbilityContext abilityUser, ITargetable target);

        protected abstract int GetMovementPointCost(IAbilityUser abilityUser, ITargetable target);
        protected abstract int GetActionPointCost(IAbilityUser abilityUser, ITargetable target);
        protected abstract int GetBloodPointCost(IAbilityUser abilityUser, ITargetable target);

        protected abstract AbilityTargetingType GetAbilityTargetingType(IAbilityUser abilityUser);
        protected abstract RangeData GetAbilityRangeData(IAbilityUser abilityUser);
        protected abstract AbilityRange GetAbilityRangeType(IAbilityUser abilityUser);
        protected abstract TargetableEntityType GetValidTargetsFlag(IAbilityUser abilityUser);

        private readonly struct AbilityCost : IAbilityCostData
        {
            public int MovementCost { get; }
            public int ActionPointCost { get; }
            public int BloodCost { get; }

            public AbilityCost(IAbilityUser abilityUser, ITargetable target, AbstractAbility ability)
            {
                MovementCost = ability.GetMovementPointCost(abilityUser, target);
                ActionPointCost = ability.GetActionPointCost(abilityUser, target);
                BloodCost = ability.GetBloodPointCost(abilityUser, target);
            }
        }

        private readonly struct AbilityTargetingData : IAbilityTargetingData
        {
            private readonly AbilityTargetingType _targetingType;
            private readonly RangeData _rangeData;
            private readonly AbilityRange _rangeType;
            private readonly TargetableEntityType _validTargetsFlag;

            public AbilityTargetingData(IAbilityUser abilityUser, AbstractAbility ability)
            {
                _targetingType = ability.GetAbilityTargetingType(abilityUser);
                _rangeData = ability.GetAbilityRangeData(abilityUser);
                _rangeType = ability.GetAbilityRangeType(abilityUser);
                _validTargetsFlag = ability.GetValidTargetsFlag(abilityUser);
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
            public readonly Sprite _icon;
            public readonly Func<string> _localizedName;
            public readonly Func<string> _localizedDescription;

            public AbilityUIData(AbstractAbility ability)
            {
                _icon = ability._icon;
                _localizedName = ability.GetLocalizedName;
                _localizedDescription = ability.GetLocalizedDescription;
            }

            public Sprite GetAbilityIcon()
            {
                return _icon;
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
