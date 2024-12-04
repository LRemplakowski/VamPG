using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.ActionSystem;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public class AbilityManager : SerializedMonoBehaviour, IAbilityUser
    {
        [SerializeField]
        private ICreatureReferences _references;
        [SerializeField]
        private List<IAbility> _defaultAbilities = new();

        private AbilityContext _abilityContext;

        private void Awake()
        {
            _abilityContext = new(this);
        }

        public bool ExecuteAbility(IAbility ability, ITargetable target, Action onCompleted = null)
        {
            return ability.Execute(GetAbilityContext(target), onCompleted);
        }

        public async Awaitable<bool> ExecuteAbilityAsync(IAbility ability, ITargetable target)
        {
            return await ability.ExecuteAsync(GetAbilityContext(target));
        }

        public IAbilityContext GetAbilityContext(ITargetable target)
        {
            return _abilityContext;
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

        private ITargetable GetCurrentTargetCharacter()
        {
            return null;
        }

        private IGridCell GetCurrentTargetPosition()
        {
            return null;
        }

        private class AbilityContext : IAbilityContext
        {
            private readonly AbilityManager _abilityManager;
            private readonly Func<ITargetable> _targetCharacter;
            private readonly Func<IGridCell> _targetPosition;

            public IActionPerformer SourceActionPerformer => SourceCombatBehaviour;
            public ICombatant SourceCombatBehaviour => _abilityManager._references.CombatBehaviour;
            public ITargetable TargetCharacter => _targetCharacter.Invoke();
            public IGridCell TargetPosition => _targetPosition.Invoke();
            public GridManager GridManager => CombatManager.Instance.CurrentEncounter.GridManager;

            public AbilityContext(AbilityManager abilityManager)
            {
                _abilityManager = abilityManager;
                _targetCharacter = abilityManager.GetCurrentTargetCharacter;
                _targetPosition = abilityManager.GetCurrentTargetPosition;
            }
        }
    }
}
