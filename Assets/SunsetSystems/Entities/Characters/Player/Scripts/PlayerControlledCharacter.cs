using Entities.Characters.Actions;
using InsaneSystems.RTSSelection;
using UnityEngine;

namespace Entities.Characters
{
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(SelectionEffect))]
    public class PlayerControlledCharacter : Creature
    {
        public override void Move(Vector3 moveTarget, float stoppingDistance)
        {
            ClearAllActions();
            _agent.stoppingDistance = stoppingDistance;
            AddActionToQueue(new Move(_agent, moveTarget));
        }

        public override void Move(Vector3 moveTarget)
        {
            Move(moveTarget, 0f);
        }

        public override void Move(GridElement moveTarget)
        {
            CurrentGridPosition = moveTarget;
            Move(moveTarget.transform.position);
        }

        public void InteractWith(IInteractable target)
        {
            AddActionToQueue(new Interact(target, this));
        }

        public override void Attack(Creature target)
        {
            AddActionToQueue(new Attack(target, this));
        }
    }
}
