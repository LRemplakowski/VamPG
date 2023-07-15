using Redcode.Awaiting;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Utils.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Interact : EntityAction
    {
        private readonly IInteractable target;
        private Move moveToTarget;
        private bool _aborted;

        protected override Creature Owner
        {
            get;
            set;
        }

        public Interact(IInteractable target, Creature owner)
        {
            this.target = target;
            Owner = owner;
            conditions.Add(new InteractionComplete(target));
        }

        public override void Abort()
        {
            if (moveToTarget != null)
                moveToTarget.Abort();
            _aborted = true;
            target.Interacted = false;
        }

        public async override void Begin()
        {
            float distance = Vector3.Distance(target.InteractionTransform.position, Owner.transform.position);
            if (distance > target.InteractionDistance)
            {
                await new WaitForUpdate();
                Owner.Agent.SetDestination(target.InteractionTransform.position);
                Owner.Agent.stoppingDistance = 0f;
                Owner.Agent.isStopped = false;
                NavMeshAgent agent = Owner.Agent;
                while (PathComplete() == false)
                {
                    await new WaitForSecondsRealtime(.1f);
                }
                if (_aborted || Vector3.Distance(target.InteractionTransform.position, Owner.transform.position) > target.InteractionDistance)
                    return;
                await Owner.FaceTarget((target as MonoBehaviour)?.transform);
                target.TargetedBy = Owner;
                target.Interact();
            }
            else
            {
                await Owner.FaceTarget(target.InteractionTransform);
                target.TargetedBy = Owner;
                target.Interact();
            }
        }

        private bool PathComplete()
        {
            if (Vector3.Distance(Owner.Agent.destination, Owner.Agent.transform.position) <= Owner.Agent.stoppingDistance + target.InteractionDistance - 0.01f)
            {
                if (!Owner.Agent.hasPath || Owner.Agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
