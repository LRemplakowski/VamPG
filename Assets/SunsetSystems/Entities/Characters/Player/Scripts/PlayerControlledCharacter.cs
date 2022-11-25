using SunsetSystems.Entities.Characters.Actions;
using InsaneSystems.RTSSelection;
using SunsetSystems.Inventory;
using UnityEngine;
using SunsetSystems.Party;

namespace SunsetSystems.Entities.Characters
{
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(SelectionEffect))]
    public class PlayerControlledCharacter : Creature
    {
        public override void Move(Vector3 moveTarget, float stoppingDistance)
        {
            ClearAllActions();
            AddActionToQueue(new Move(this, moveTarget, stoppingDistance));
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
