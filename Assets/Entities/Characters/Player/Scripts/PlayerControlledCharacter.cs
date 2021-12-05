using Entities.Characters.Actions;
using InsaneSystems.RTSSelection;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Characters
{
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(SelectionEffect))]
    public class PlayerControlledCharacter : Creature
    {
        public override void Move(Vector3 moveTarget)
        {
            AddActionToQueue(new Move(GetComponent<NavMeshAgent>(), moveTarget));
        }

        public override void Move(GridElement moveTarget)
        {
            CurrentGridPosition = moveTarget;
            AddActionToQueue(new Move(GetComponent<NavMeshAgent>(), moveTarget.transform.position));
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
