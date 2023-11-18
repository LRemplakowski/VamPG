using Sirenix.OdinInspector;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interfaces;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public class Move : EntityAction
    {
        private readonly NavMeshAgent navMeshAgent;
        private readonly NavMeshObstacle navMeshObstacle;
        [ShowInInspector, ReadOnly]
        private Vector3 destination;
        public static event Action<IActionPerformer> OnMovementFinished;
        public static Action<IActionPerformer> OnMovementStarted;
        private float stoppingDistance;
        //private Task rotationTask;

        public Move(IActionPerformer owner, Vector3 destination, float stoppingDistance = .1f) : base(owner, true)
        {
            this.navMeshAgent = owner.GetComponent<NavMeshAgent>();
            this.navMeshObstacle = owner.GetComponent<NavMeshObstacle>();
            NavMesh.SamplePosition(destination, out var hit, 1f, NavMesh.AllAreas);
            conditions.Add(new Destination(navMeshAgent));
            this.destination = hit.position;
            this.stoppingDistance = stoppingDistance;
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

        public override void Abort()
        {
            base.Abort();
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
                navMeshAgent.stoppingDistance = stoppingDistance;
                if (OnMovementStarted != null)
                    OnMovementStarted.Invoke(this.Owner);
            }
            else
            {
                Abort();
            }
        }
    }
}
