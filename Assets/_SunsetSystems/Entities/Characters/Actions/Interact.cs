using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public class Interact : EntityAction
    {
        private readonly IInteractable target;
        private readonly NavMeshAgent navMeshAgent;
        [ShowInInspector, ReadOnly]
        private Vector3 destination;
        private float stoppingDistance;
        private IEnumerator delayedInteractionCoroutine;

        public Interact(IInteractable target, IActionPerformer owner) : base(owner, true)
        {
            this.target = target;
            conditions.Add(new InteractionComplete(target));
            this.navMeshAgent = owner.GetComponent<NavMeshAgent>();
            NavMesh.SamplePosition(target.InteractionTransform.position, out var hit, 1f, NavMesh.AllAreas);
            this.destination = hit.position;
            this.stoppingDistance = target.InteractionDistance - 0.01f;
        }

        public override void Abort()
        {
            base.Abort();
            target.Interacted = false;
            navMeshAgent.isStopped = true;
            if (delayedInteractionCoroutine != null)
                Owner.CoroutineRunner.StopCoroutine(delayedInteractionCoroutine);
        }

        public override void Begin()
        {
            float distance = Vector3.Distance(target.InteractionTransform.position, Owner.Transform.position);
            if (distance > target.InteractionDistance)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.ResetPath();
                if (navMeshAgent.SetDestination(destination))
                {
                    conditions.Add(new Destination(navMeshAgent));
                    navMeshAgent.stoppingDistance = stoppingDistance;
                    delayedInteractionCoroutine = InteractWhenCloseEnough();
                    Owner.CoroutineRunner.StartCoroutine(delayedInteractionCoroutine);
                }
                else
                {
                    Abort();
                }
            }
            else
            {
                target.TargetedBy = Owner;
                target.Interact();
            }
        }

        private IEnumerator InteractWhenCloseEnough()
        {
            while (Vector3.Distance(target.InteractionTransform.position, Owner.Transform.position) > target.InteractionDistance)
                yield return null;
            navMeshAgent.isStopped = true;
            target.TargetedBy = Owner;
            target.Interact();
        }
    }
}
