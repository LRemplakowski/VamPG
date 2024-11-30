using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public class BaseAbility : AbstractAbility
    {
        [SerializeField, BoxGroup("Ability Core")]
        protected TargetableEntityType _validTargetsFlag;
        [SerializeField, BoxGroup("Ability Core")]
        protected AbilityRange _targetingDistanceType;
        [SerializeField, BoxGroup("Ability Core")]
        protected AbilityTargetingType _abilityTargetingType;
        [SerializeField, BoxGroup("Ability Cost")]
        protected int _baseMovementCost = 0, _baseAPCost = 0, _baseBloodCost = 0;
        [SerializeField, BoxGroup("Ability Range"), MinValue(1)]
        protected int _baseMaxRange = 1;

        public override bool IsValidTarget(IAbilityUser abilityUser, ITargetable target)
        {
            bool targetIsValidEntity = target.IsValidEntityType(GetValidTargetsFlag(abilityUser));
            return targetIsValidEntity;
        }

        protected override bool CanExecuteAbility(IAbilityContext abilityContext, ITargetable target)
        {
            return abilityContext != null && target != null;
        }

        protected override async Awaitable DoExecuteAbility(IAbilityContext abilityContext, ITargetable target, Action onCompleted)
        {
            await Task.Yield();
            Debug.Log($"{abilityContext.User} used {name} against {target}!");
            onCompleted?.Invoke();
        }

        protected override RangeData GetAbilityRangeData(IAbilityUser abilityUser)
        {
            return new()
            {
                ShortRange = _baseMaxRange,
                MaxRange = _baseMaxRange,
                OptimalRange = _baseMaxRange
            };
        }

        protected override AbilityRange GetAbilityRangeType(IAbilityUser abilityUser)
        {
            return _targetingDistanceType;
        }

        protected override AbilityTargetingType GetAbilityTargetingType(IAbilityUser abilityUser)
        {
            return _abilityTargetingType;
        }

        protected override int GetActionPointCost(IAbilityUser abilityUser, ITargetable target)
        {
            return _baseAPCost;
        }

        protected override int GetBloodPointCost(IAbilityUser abilityUser, ITargetable target)
        {
            return _baseBloodCost;
        }

        protected override int GetMovementPointCost(IAbilityUser abilityUser, ITargetable target)
        {
            return _baseMovementCost;
        }

        protected override TargetableEntityType GetValidTargetsFlag(IAbilityUser abilityUser)
        {
            return _validTargetsFlag;
        }
    }
}
