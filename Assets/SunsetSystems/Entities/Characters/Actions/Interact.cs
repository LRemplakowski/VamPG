using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Utils.Threading;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Interact : EntityAction
    {
        private readonly IInteractable target;
        private Move moveToTarget;
        private Task moveTask;
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
                moveToTarget = new Move(Owner, target.InteractionTransform.position);
                moveToTarget.Begin();
                await Task.Run<Task>(InteractIfCloseEnough, tokenSource.Token);
            }
            else
            {
                await Owner.FaceTarget(target.InteractionTransform);
                target.TargetedBy = Owner;
                target.Interact();
            }
        }

        private async Task InteractIfCloseEnough()
        {
            while (Vector3.Distance(target.InteractionTransform.position, Owner.transform.position) <= target.InteractionDistance)
            {
                await Task.Yield();
            }
            moveToTarget.Abort();
            await Task.Run(() =>
            {
                Dispatcher.Instance.Invoke(async () =>
                {
                    await Owner.FaceTarget(target.InteractionTransform);
                });
            }, tokenSource.Token);

            target.TargetedBy = Owner;
            target.Interact();
        }
    }
}
