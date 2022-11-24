using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Interactable
{
    public class Doors : InteractableEntity
    {
        [SerializeField]
        private Animator animator;

        protected void Start()
        {
            if (!animator)
                animator = GetComponentInChildren<Animator>();
        }

        protected override void HandleInteraction()
        {
            animator?.SetTrigger("DoorSwitch");
        }
    }
}
