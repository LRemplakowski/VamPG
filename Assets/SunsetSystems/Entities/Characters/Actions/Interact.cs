using Redcode.Awaiting;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Utils.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Interact : EntityAction
    {
        private readonly IInteractable target;
        private Move moveToTarget;
        private readonly CancellationTokenSource tokenSource = new();

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
            tokenSource.Cancel();
            if (moveToTarget != null)
                moveToTarget.Abort();
            target.Interacted = false;
        }

        public async override void Begin()
        {
            float distance = Vector3.Distance(target.InteractionTransform.position, Owner.transform.position);
            if (distance > target.InteractionDistance)
            {
                await new WaitForUpdate();
                moveToTarget = new Move(Owner, target.InteractionTransform.position, target.InteractionDistance);
                moveToTarget.Begin();
                while (!moveToTarget.IsFinished())
                {
                    if (tokenSource.Token.IsCancellationRequested)
                    {
                        Debug.Log("Cancelling interaction task!!");
                        return;
                    }
                    await new WaitForUpdate();
                }
                moveToTarget = null;
                await Owner.FaceTarget(target.InteractionTransform);
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
    }
}
