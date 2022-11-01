using SunsetSystems.Entities.Characters.Actions;
using InsaneSystems.RTSSelection;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(SelectionEffect))]
    public class PlayerControlledCharacter : Creature
    {
        protected override void Start()
        {
            base.Start();
            bool equipmentAdded = InventoryManager.TryAddCoterieMemberEquipment(this);
            if (equipmentAdded)
                Debug.Log("Successfully added equipment of " + gameObject.name + " to InventoryManager!");
            else
                Debug.LogError("Equipment entry of " + gameObject.name + " already exists in InventoryManager!");
        }

        public override void Move(Vector3 moveTarget, float stoppingDistance)
        {
            ClearAllActions();
            Agent.stoppingDistance = stoppingDistance;
            AddActionToQueue(new Move(this, moveTarget));
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
