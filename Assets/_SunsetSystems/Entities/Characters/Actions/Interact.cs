using System.Collections;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Utils.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private IEnumerator delayedInteractionCoroutine;

        public Interact(IActionPerformer owner, IInteractable target) : base(owner, false)
        {
            this.target = target;
            conditions.Add(new InteractionComplete(target));
            this.navMeshAgent = owner.References.NavMeshAgent;
            NavMesh.SamplePosition(target.InteractionTransform.position, out var hit, 1f, NavMesh.AllAreas);
            this.destination = hit.position;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            target.Interacted = false;
            navMeshAgent.isStopped = true;
            if (delayedInteractionCoroutine != null)
                Owner.CoroutineRunner.StopCoroutine(delayedInteractionCoroutine);
        }

        public override void Begin()
        {
            float distance = Vector3.Distance(target.InteractionTransform.position, Owner.References.BodyTransform.position);
            if (distance > target.InteractionDistance)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.ResetPath();
                if (navMeshAgent.SetDestination(destination))
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
            while (Vector3.Distance(target.InteractionTransform.position, Owner.References.BodyTransform.position) > target.InteractionDistance)
                yield return null;
            navMeshAgent.isStopped = true;
            target.TargetedBy = Owner;
            target.Interact();
        }
    }
}
