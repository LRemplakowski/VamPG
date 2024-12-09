using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.ActionSystem
{
    public class MoveAbilityAction : EntityAction
    {
        [SerializeField]
        private INavigationManager navigationManager;
        [SerializeField, ReadOnly]
        private Vector3 destination;

        public MoveAbilityAction(MoveAbility ability, IAbilityContext context) : base(context.SourceActionPerformer)
        {
            navigationManager = context.SourceCombatBehaviour.References.NavigationManager;
            NavMesh.SamplePosition(destination, out var hit, 1f, NavMesh.AllAreas);
            conditions.Add(new Destination(navigationManager));
            this.destination = hit.position;
            var gridInstance = context.GridManager;
            var gridCell = context.TargetObject as IGridCell;
            var combatant = context.SourceCombatBehaviour;
            gridInstance.HandleCombatantMovedIntoGridCell(context.SourceCombatBehaviour, gridCell);
            context.SourceCombatBehaviour.OnChangedGridPosition += ClearOccupierFromCell;

            void ClearOccupierFromCell(ICombatant combatant)
            {
                gridInstance.ClearOccupierFromCell(gridCell);
                combatant.OnChangedGridPosition -= ClearOccupierFromCell;
            }
        }

        public override void Begin()
        {
            navigationManager.SetNavigationTarget(destination);
        }
    }
}
