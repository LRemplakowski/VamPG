using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.ActionSystem;
using SunsetSystems.ActorResources;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Input;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public class AbilityManager : SerializedMonoBehaviour, IAbilityUser
    {
        [SerializeField]
        private ICreatureReferences _references;
        [SerializeField]
        private IMovementPointUser _movementPointUser;
        [SerializeField]
        private IActionPointUser _actionPointUser;
        [SerializeField]
        private IBloodPointUser _bloodPointUser;
        [SerializeField]
        private List<IAbility> _defaultAbilities = new();

        private AbilityContext _abilityContext;

        private ITargetable _characterTarget;

        private void Awake()
        {
            _abilityContext = new(this);
            CombatInputHandler.OnTargetingDataUpdate += UpdatePlayerTargetingData;
        }

        private void OnDestroy()
        {
            CombatInputHandler.OnTargetingDataUpdate -= UpdatePlayerTargetingData;
        }

        private void UpdatePlayerTargetingData(ITargetable target)
        {
            if (_references.CreatureData.Faction is not Faction.PlayerControlled)
                return;
            SetCurrentTargetObject(target);
        }

        public void SetCurrentTargetObject(ITargetable targetable)
        {
            _characterTarget = targetable;
        }

        public bool ExecuteAbility(IAbility ability, Action onCompleted = null)
        {
            if (GetCanAffordAbility(ability) && ConsumeAbilityCost(ability))
            {
                return ability.Execute(GetCurrentAbilityContext(), onCompleted);
            }
            return false;
        }

        public async Awaitable<bool> ExecuteAbilityAsync(IAbility ability)
        {
            if (GetCanAffordAbility(ability) && ConsumeAbilityCost(ability))
            {
                return await ability.ExecuteAsync(GetCurrentAbilityContext());
            }
            return false;
        }

        private bool ConsumeAbilityCost(IAbility ability)
        {
            var cost = ability.GetAbilityCosts(GetCurrentAbilityContext());
            bool result = true;
            result &= _movementPointUser.UseMovementPoints(cost.MovementCost);
            result &= _actionPointUser.UseActionPoints(cost.ActionPointCost);
            result &= _bloodPointUser.UseBloodPoints(cost.BloodCost);
            return result;
        }

        public IAbilityContext GetCurrentAbilityContext()
        {
            return _abilityContext;
        }

        public bool GetCanAffordAbility(IAbility ability)
        {
            var cost = ability.GetAbilityCosts(GetCurrentAbilityContext());
            bool result = true;
            result &= _movementPointUser.GetCurrentMovementPoints() >= cost.MovementCost;
            result &= _actionPointUser.GetCurrentActionPoints() >= cost.ActionPointCost;
            result &= _bloodPointUser.GetCurrentBloodPoints() >= cost.BloodCost;
            return result;
        }

        public bool GetHasValidAbilityContext(IAbility ability)
        {
            return ability.IsContextValidForExecution(GetCurrentAbilityContext());
        }

        public IEnumerable<IAbility> GetAllAbilities()
        {
            return GetCoreAbilities().Union(GetAbilitiesFromDisciplines());
        }

        public IEnumerable<IAbility> GetCoreAbilities()
        {
            return _defaultAbilities.Union(GetAbilitiesFromEquipment());
        }

        private IEnumerable<IAbility> GetAbilitiesFromEquipment()
        {
            return _references.EquipmentManager.EquippedItems.OfType<IAbilitySource>().SelectMany(item => item.GetAbilities());
        }

        private IEnumerable<IAbility> GetAbilitiesFromDisciplines()
        {
            return new List<IAbility>();
        }

        private ITargetable GetCurrentTargetObject()
        {
            return _characterTarget;
        }

        private class AbilityContext : IAbilityContext
        {
            private readonly AbilityManager _abilityManager;
            private readonly Func<ITargetable> _targetCharacter;

            public IActionPerformer SourceActionPerformer => SourceCombatBehaviour;
            public ICombatant SourceCombatBehaviour => _abilityManager._references.CombatBehaviour;
            public ITargetable TargetObject => _targetCharacter.Invoke();
            public GridManager GridManager => CombatManager.Instance.CurrentEncounter.GridManager;

            public AbilityContext(AbilityManager abilityManager)
            {
                _abilityManager = abilityManager;
                _targetCharacter = abilityManager.GetCurrentTargetObject;
            }
        }
    }
}
