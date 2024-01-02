using CleverCrow.Fluid.UniqueIds;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Persistence;
using System;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    [RequireComponent(typeof(IPersistentObject))]
    public class DoorController : SerializedMonoBehaviour, IInteractionHandler, IPersistentComponent
    {
        private const string DOOR_STATE = "Open";
        private const string ANIMATION_REVERSE = "Reversed";

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
                _animator?.SetBool(DOOR_STATE, value);
            }
        }

        [SerializeField, HideInInspector]
        private UniqueId _unique;
        public string ComponentID => _unique.Id;

        private void Awake()
        {
            _animator ??= GetComponentInChildren<Animator>();
            _animator?.SetBool(ANIMATION_REVERSE, _reverseAnimation);
            if (_unique == null)
                _unique = GetComponent<UniqueId>();
        }

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
            DoorsPersistenceData persistenceData = new DoorsPersistenceData()
            {
                DoorState = Open,
            };
            return persistenceData;
        }

        public void InjectComponentPersistenceData(object data)
        {
            if (data is not DoorsPersistenceData saveData)
                return;
            Open = saveData.DoorState;
        }

        [Serializable]
        protected class DoorsPersistenceData
        {
            public bool DoorState = false;

            public DoorsPersistenceData()
            {

            }
        }
    }
}
