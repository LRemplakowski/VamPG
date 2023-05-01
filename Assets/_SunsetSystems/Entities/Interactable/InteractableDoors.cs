using SunsetSystems.Entities.Enviroment;
using System;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    [RequireComponent(typeof(DoorController))]
    public class InteractableDoors : InteractableEntity
    {
        [SerializeField]
        private DoorController _doors;

        protected override void Awake()
        {
            _doors ??= GetComponentInChildren<DoorController>();
        }

        protected override void HandleInteraction()
        {
            _doors.Open = !_doors.Open;
        }

        public override object GetPersistenceData()
        {
            DoorsPersistenceData persistenceData = new(base.GetPersistenceData() as InteractableEntityPersistenceData);
            persistenceData.DoorState = _doors.Open;
            return persistenceData;
        }

        public override void InjectPersistenceData(object data)
        {
            base.InjectPersistenceData(data);
            _doors.Open = (data as DoorsPersistenceData).DoorState;
        }

        [Serializable]
        protected class DoorsPersistenceData : InteractableEntityPersistenceData
        {
            public bool DoorState = false;

            public DoorsPersistenceData(InteractableEntityPersistenceData persistenceData)
            {
                GameObjectActive = persistenceData.GameObjectActive;
                Interactable = persistenceData.Interactable;
            }

            public DoorsPersistenceData()
            {

            }
        }
    }
}
