using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Interactable
{
    public class Doors : InteractableEntity
    {
        [SerializeField]
        private Animator _animator;
        [field: SerializeField]
        public bool ReverseAnimation { get; private set; }


        protected override void Start()
        {
            base.Start();
            if (!_animator)
                _animator = GetComponentInChildren<Animator>();
        }

        protected override void HandleInteraction()
        {
            Debug.Log("Switching door animation state!");
            if (ReverseAnimation)
                _animator?.SetTrigger("DoorSwitchReverse");
            else
                _animator?.SetTrigger("DoorSwitch");
        }
    }
}
