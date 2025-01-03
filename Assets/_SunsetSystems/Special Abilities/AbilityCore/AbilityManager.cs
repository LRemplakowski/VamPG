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
using SunsetSystems.Inventory;
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
        private List<IAbilityConfig> _defaultAbilities = new();

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

        public bool ExecuteAbility(IAbilityConfig ability, Action onCompleted = null)
        {
            if (GetCanAffordAbility(ability) && ConsumeAbilityCost(ability) && ability.IsContextValidForExecution(GetCurrentAbilityContext()))
            {
                ability.GetExecutionStrategy().BeginExecute(GetCurrentAbilityContext(), onCompleted);
                return true;
            }
            return false;
        }

        public async Awaitable<bool> ExecuteAbilityAsync(IAbilityConfig ability)
        {
            if (GetCanAffordAbility(ability) && ConsumeAbilityCost(ability) && ability.IsContextValidForExecution(GetCurrentAbilityContext()))
            {
                await ability.GetExecutionStrategy().BeginExecute(GetCurrentAbilityContext(), null);
                return true;
            }
            return false;
        }

        private bool ConsumeAbilityCost(IAbilityConfig ability)
        {
            var cost = ability.GetAbilityCosts(GetCurrentAbilityContext());
            bool result = true;
            result &= _movementPointUser.UseMovementPoints(cost.MovementCost);
            result &= _actionPointUser.UseActionPoints(cost.ActionPointCost);
            result &= _bloodPointUser.UseBloodPoints(cost.BloodCost);
            if (ability is IAmmoAbility ammoAbility)
                result &= ConsumeAmmo(ammoAbility);
            return result;
        }

        public IAbilityContext GetCurrentAbilityContext()
        {
            return _abilityContext;
        }

        public bool GetCanAffordAbility(IAbilityConfig ability)
        {
            if (ability == null)
                return false;
            var cost = ability.GetAbilityCosts(GetCurrentAbilityContext());
            bool result = true;
            result &= _movementPointUser.GetCurrentMovementPoints() >= cost.MovementCost;
            result &= _actionPointUser.GetCurrentActionPoints() >= cost.ActionPointCost;
            result &= _bloodPointUser.GetCurrentBloodPoints() >= cost.BloodCost;
            if (ability is IAmmoAbility ammoAbility)
                result &= HasEnoughAmmo(ammoAbility);
            return result;
        }

        public bool GetHasValidAbilityContext(IAbilityConfig ability)
        {
            return ability.IsContextValidForExecution(GetCurrentAbilityContext());
        }

        public IEnumerable<IAbilityConfig> GetAllAbilities()
        {
            return GetCoreAbilities().Union(GetAbilitiesFromDisciplines());
        }

        public IEnumerable<IAbilityConfig> GetCoreAbilities()
        {
            return _defaultAbilities.Union(GetAbilitiesFromEquipment());
        }

        private IEnumerable<IAbilityConfig> GetAbilitiesFromEquipment()
        {
            var selectedWeapon = _references.WeaponManager.GetSelectedWeapon();
            return _references.EquipmentManager.EquippedItems.OfType<IAbilitySource>()
                                                             .Where(item => item is not IWeapon weapon || weapon == selectedWeapon)
                                                             .SelectMany(item => item.GetAbilities());
        }

        private IEnumerable<IAbilityConfig> GetAbilitiesFromDisciplines()
        {
            return new List<IAbilityConfig>();
        }

        private ITargetable GetCurrentTargetObject()
        {
            return _characterTarget;
        }

        private bool HasEnoughAmmo(IAmmoAbility ammoAbility)
        {
            return _references.WeaponManager.GetSelectedWeaponAmmoData().CurrentAmmo >= ammoAbility.GetAmmoPerUse() * ammoAbility.GetUsesPerExecution();
        }

        private bool ConsumeAmmo(IAmmoAbility ammoAbility)
        {
            var ammoCost = ammoAbility.GetAmmoPerUse() * ammoAbility.GetUsesPerExecution();
            return _references.WeaponManager.UseAmmoFromSelectedWeapon(ammoCost);
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
