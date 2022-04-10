using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Interactable
{
    public class Doors : InteractableEntity
    {
        [SerializeField]
        private Animator animator;

        private void Start()
        {
            if (!animator)
                animator = GetComponentInChildren<Animator>();
        }

        public override void Interact()
        {
            animator.SetTrigger("DoorSwitch");
            base.Interact();
        }
    }
}
