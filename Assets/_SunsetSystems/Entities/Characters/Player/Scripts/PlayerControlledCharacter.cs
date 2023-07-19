using SunsetSystems.Entities.Characters.Actions;
using InsaneSystems.RTSSelection;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(SelectionEffect))]
    public class PlayerControlledCharacter : Creature
    {
        public override Move Move(Vector3 moveTarget, float stoppingDistance)
        {
            ClearAllActions();
            Move moveAction = new(this, moveTarget, stoppingDistance);
            PerformAction(moveAction);
            return moveAction;
        }

        public override Move Move(Vector3 moveTarget)
        {
            return Move(moveTarget, 0f);
        }

        public override Move Move(GridElement moveTarget)
        {
            CurrentGridPosition = moveTarget;
            return Move(moveTarget.transform.position);
        }

        public override Move MoveAndRotate(Vector3 moveTarget, Transform rotationTarget)
        {
            ClearAllActions();
            Move moveAction = new(this, moveTarget, rotationTarget);
            PerformAction(moveAction);
            return moveAction;
        }

        public void InteractWith(IInteractable target)
        {
            PerformAction(new Interact(target, this));
        }

        public override Attack Attack(Creature target)
        {
            Attack attackAction = new(target, this);
            PerformAction(attackAction);
            return attackAction;
        }
    }
}
