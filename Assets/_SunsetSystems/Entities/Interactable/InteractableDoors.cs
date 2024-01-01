using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Enviroment;
using System;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    [RequireComponent(typeof(DoorController))]
    public class InteractableDoors : PersistentEntity, IInteractionHandler
    {
        [SerializeField]
        private DoorController _doors;

        protected override void Awake()
        {
            base.Awake();
            _doors ??= GetComponentInChildren<DoorController>();
        }

        public bool HandleInteraction(IActionPerformer interactee)
        {
            _doors.Open = !_doors.Open;
            return true;
        }

        public override object GetPersistenceData()
        {
            DoorsPersistenceData persistenceData = new(base.GetPersistenceData() as PersistenceData);
            persistenceData.DoorState = _doors.Open;
            return persistenceData;
        }

        public override void InjectPersistenceData(object data)
        {
            base.InjectPersistenceData(data);
            _doors.Open = (data as DoorsPersistenceData).DoorState;
        }

        [Serializable]
        protected class DoorsPersistenceData : PersistenceData
        {
            public bool DoorState = false;

            public DoorsPersistenceData(PersistenceData persistenceData)
            {
                GameObjectActive = persistenceData.GameObjectActive;
            }

            public DoorsPersistenceData()
            {

            }
        }
    }
}
