using Sirenix.OdinInspector;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Interfaces;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public class Move : EntityAction
    {
        [SerializeField]
        private NavMeshAgent navMeshAgent;
        [SerializeField, ReadOnly]
        private Vector3 destination;
        public static event Action<IActionPerformer> OnMovementFinished;
        public static event Action<IActionPerformer> OnMovementStarted;
        //private Task rotationTask;

        public Move(IActionPerformer owner, Vector3 destination/*, float stoppingDistance = .1f*/) : base(owner, false)
        {
            this.navMeshAgent = owner.References.NavMeshAgent;
            NavMesh.SamplePosition(destination, out var hit, 1f, NavMesh.AllAreas);
            conditions.Add(new Destination(navMeshAgent));
            this.destination = hit.position;
        }

        public Move(ICombatant owner, IGridCell gridCell, GridManager gridInstance) : this(owner, gridInstance.GridPositionToWorldPosition(gridCell.GridPosition))
        {
            gridInstance.HandleCombatantMovedIntoGridCell(owner, gridCell);
            owner.OnChangedGridPosition += ClearOccupierFromCell;

            void ClearOccupierFromCell(ICombatant combatant)
            {
                gridInstance.ClearOccupierFromCell(gridCell);
                owner.OnChangedGridPosition -= ClearOccupierFromCell;
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            navMeshAgent.isStopped = true;
            if (OnMovementFinished != null)
                OnMovementFinished.Invoke(this.Owner);
        }

        public override void Begin()
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.ResetPath();
            if (navMeshAgent.SetDestination(destination)) 
            {
                if (OnMovementStarted != null)
                    OnMovementStarted.Invoke(this.Owner);
            }
            else
            {
                Cleanup();
            }
        }
    }
}
