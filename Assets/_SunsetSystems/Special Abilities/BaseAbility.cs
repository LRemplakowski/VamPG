using Sirenix.OdinInspector;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public abstract class BaseAbility : AbstractAbility
    {
        [SerializeField, BoxGroup("Ability Core")]
        protected TargetableEntityType _validTargetsFlag;
        [SerializeField, BoxGroup("Ability Core")]
        protected AbilityRange _targetingDistanceType;
        [SerializeField, BoxGroup("Ability Core")]
        protected AbilityTargetingType _abilityTargetingType;
        [SerializeField, BoxGroup("Ability Cost")]
        protected int _baseMovementCost = 0, _baseAPCost = 0, _baseBloodCost = 0;

        public override bool IsValidTarget(IAbilityContext context)
        {
            bool targetIsValidEntity = context.TargetObject.IsValidEntityType(GetValidTargetsFlag(context));
            return targetIsValidEntity;
        }

        protected override bool CanExecuteAbility(IAbilityContext context)
        {
            return HasValidContext(context) && CanAffordCost(context);
        }

        protected bool HasValidContext(IAbilityContext context) => context != null;

        protected bool CanAffordCost(IAbilityContext context)
        {
            bool result = true;
            result &= GetMovementPointCost(context) <= context.SourceCombatBehaviour.GetRemainingMovement();
            result &= GetActionPointCost(context) <= context.SourceCombatBehaviour.GetRemainingActionPoints();
            result &= GetBloodPointCost(context) <= context.SourceCombatBehaviour.GetRemainingBloodPoints();
            return result;
        }

        protected override AbilityRange GetAbilityRangeType(IAbilityContext context)
        {
            return _targetingDistanceType;
        }

        protected override AbilityTargetingType GetAbilityTargetingType(IAbilityContext context)
        {
            return _abilityTargetingType;
        }

        protected override int GetActionPointCost(IAbilityContext context)
        {
            return _baseAPCost;
        }

        protected override int GetBloodPointCost(IAbilityContext context)
        {
            return _baseBloodCost;
        }

        protected override int GetMovementPointCost(IAbilityContext context)
        {
            return _baseMovementCost;
        }

        protected override TargetableEntityType GetValidTargetsFlag(IAbilityContext context)
        {
            return _validTargetsFlag;
        }
    }
}
