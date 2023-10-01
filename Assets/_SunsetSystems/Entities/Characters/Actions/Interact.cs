using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Interact : EntityAction
    {
        private readonly IInteractable target;
        private bool _aborted;

        public Interact(IInteractable target, IActionPerformer owner) : base(owner, true)
        {
            this.target = target;
            conditions.Add(new InteractionComplete(target));
        }

        public override void Abort()
        {
            _aborted = true;
            target.Interacted = false;
        }

        public async override void Begin()
        {
            float distance = Vector3.Distance(target.InteractionTransform.position, Owner.Transform.position);
            if (distance > target.InteractionDistance)
            {
                Task move = Owner.PerformAction(new Move(Owner, target.InteractionTransform.position, target.InteractionDistance));
                this.IsPriority = false;
                _ = Owner.PerformAction(this);
                await move;
                distance = Vector3.Distance(target.InteractionTransform.position, Owner.Transform.position);
                if (distance > target.InteractionDistance)
                    Abort();
                else
                    _ = Owner.PerformAction(this);
            }
            else
            {
                target.TargetedBy = Owner;
                target.Interact();
            }
        }
    }
}
