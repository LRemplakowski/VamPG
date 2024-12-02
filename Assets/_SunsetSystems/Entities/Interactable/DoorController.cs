using System;
using Sirenix.OdinInspector;
using SunsetSystems.ActionSystem;
using SunsetSystems.Persistence;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class DoorController : SerializedMonoBehaviour, IInteractionHandler, IPersistentComponent
    {
        private const string DOOR_STATE = "Open";
        private const string ANIMATION_REVERSE = "Reversed";
        private const string COMPONENT_ID = "DoorControllerComponent";

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private bool _reverseAnimation = false;

        private bool _doorState = false;
        public bool Open
        {
            get => _doorState;
            set
            {
                _doorState = value;
                if (_animator != null || gameObject.TryGetComponent(out _animator))
                    _animator.SetBool(DOOR_STATE, value);
            }
        }

        public string ComponentID => COMPONENT_ID;

        private void Start()
        {
            if (_animator != null)
                _animator = GetComponentInChildren<Animator>();
            _animator?.SetBool(ANIMATION_REVERSE, _reverseAnimation);
        }

        [Button]
        private void TriggerStateChange()
        {
            Open = !Open;
        }

        public bool HandleInteraction(IActionPerformer interactee)
        {
            TriggerStateChange();
            return true;
        }

        public object GetComponentPersistenceData()
        {
            return new DoorsPersistenceData(this);
        }

        public void InjectComponentPersistenceData(object data)
        {
            if (data is not DoorsPersistenceData saveData)
                return;
            Open = saveData.DoorState;
        }

        [Serializable]
        public class DoorsPersistenceData
        {
            public bool DoorState = false;

            public DoorsPersistenceData(DoorController doorController)
            {
                DoorState = doorController.Open;
            }

            public DoorsPersistenceData()
            {

            }
        }
    }
}
