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

        protected override void Start()
        {
            base.Start();
            if (!_animator)
                _animator = GetComponentInChildren<Animator>();
        }

        protected override void HandleInteraction()
        {
            Debug.Log("Switching door animation state!");
            _animator?.SetTrigger("DoorSwitch");
        }
    }
}
