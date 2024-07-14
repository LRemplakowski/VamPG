using System.Collections;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public class Interact : EntityAction
    {
        private readonly IInteractable target;
        private readonly INavigationManager navMeshAgent;
        [ShowInInspector, ReadOnly]
        private Vector3 destination;
        private IEnumerator delayedInteractionCoroutine;

        public Interact(IActionPerformer owner, IInteractable target) : base(owner, false)
        {
            this.target = target;
            conditions.Add(new InteractionComplete(target));
            this.navMeshAgent = owner.References.NavigationManager;
            NavMesh.SamplePosition(target.InteractionTransform.position, out var hit, 1f, NavMesh.AllAreas);
            this.destination = hit.position;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            target.Interacted = false;
            if (delayedInteractionCoroutine != null)
                Owner.CoroutineRunner.StopCoroutine(delayedInteractionCoroutine);
        }

        public override void Begin()
        {
            float distance = Vector3.Distance(target.InteractionTransform.position, Owner.References.Transform.position);
            if (distance > target.InteractionDistance)
            {
                if (navMeshAgent.SetNavigationTarget(destination))
                {
                    conditions.Add(new Destination(navMeshAgent));
                    delayedInteractionCoroutine = InteractWhenCloseEnough();
                    Owner.CoroutineRunner.StartCoroutine(delayedInteractionCoroutine);
                }
                else
                {
                    Cleanup();
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
            while (Vector3.Distance(target.InteractionTransform.position, Owner.References.Transform.position) > target.InteractionDistance)
                yield return null;
            target.TargetedBy = Owner;
            target.Interact();
        }
    }
}
