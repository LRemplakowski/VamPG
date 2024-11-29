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
        private TargetableEntityType _validTargetsFlag;
        [SerializeField, BoxGroup("Ability Core")]
        private AbilityRange _targetingDistanceType;
        [SerializeField, BoxGroup("Ability Core")]
        private AbilityTargetingType _abilityTargetingType;
        [SerializeField, BoxGroup("Ability Cost")]
        private int _baseMovementCost = 0, _baseAPCost = 0, _baseBloodCost = 0;
        [SerializeField, BoxGroup("Ability Range"), MinValue(1)]
        private int _baseMaxRange = 1;

        public override bool IsValidTarget(IAbilityUser abilityUser, ITargetable target)
        {
            bool targetIsValidEntity = target.IsValidEntityType(GetValidTargetsFlag(abilityUser));
            return targetIsValidEntity;
        }

        protected override bool CanExecuteAbility(IAbilityUser abilityUser, ITargetable target)
        {
            return abilityUser != null && target != null;
        }

        protected override async Awaitable ExecuteAbilityAsync(IAbilityUser abilityUser, ITargetable target, Action onCompleted)
        {
            await Task.Yield();
            Debug.Log($"{abilityUser} used {name} against {target}!");
            onCompleted?.Invoke();
        }

        protected override RangeData GetAbilityRangeData(IAbilityUser abilityUser)
        {
            return new()
            {
                ShortRange = _baseMaxRange,
                MaxRange = _baseMaxRange,
                OptimalRange = _baseMaxRange,
                RangeFalloff = _baseMaxRange
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
