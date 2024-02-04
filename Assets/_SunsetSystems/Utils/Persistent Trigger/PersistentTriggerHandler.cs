using System;
using Sirenix.OdinInspector;
using SunsetSystems.Entities;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Utils.Triggers
{
    public class PersistentTriggerHandler : PersistentEntity, ITriggerHandler
    {
        [Title("Config")]
        [SerializeField]
        private bool triggerOnlyOnce;
        [SerializeField]
        private bool triggeredOnlyBySpecificObject;
        [SerializeField, ShowIf("triggeredOnlyBySpecificObject")]
        private Collider triggeringObject;

        [Title("Events")]
        public UltEvent<Collider> TriggerEnter = new();
        public UltEvent<Collider> TriggerStay = new();
        public UltEvent<Collider> TriggerExit = new();

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private bool enterTriggeredOnce = false;
        [ShowInInspector, ReadOnly]
        private bool stayTriggeredOnce = false;
        [ShowInInspector, ReadOnly]
        private bool exitTriggeredOnce = false;

        [SerializeField, Title("Editor")]
        private bool visualizeTrigger = true;
        [SerializeField, ShowIf("visualizeTrigger")]
        private Color gizmoColor = Color.green;
        [SerializeField, HideInInspector]
        private Collider myCollider;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (myCollider == null)
                myCollider = GetComponent<Collider>();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (triggerOnlyOnce && enterTriggeredOnce)
                return;
            if (triggeredOnlyBySpecificObject && triggeringObject != other)
                return;
            TriggerEnter?.InvokeSafe(other);
            enterTriggeredOnce = true;
        }

        public void OnTriggerExit(Collider other)
        {
            if (triggerOnlyOnce && exitTriggeredOnce)
                return;
            if (triggeredOnlyBySpecificObject && triggeringObject != other)
                return;
            TriggerExit?.InvokeSafe(other);
            exitTriggeredOnce = true;
        }

        public void OnTriggerStay(Collider other)
        {
            if (triggerOnlyOnce && stayTriggeredOnce)
                return;
            if (triggeredOnlyBySpecificObject && triggeringObject != other)
                return;
            TriggerStay?.InvokeSafe(other);
            stayTriggeredOnce = true;
        }

        private void OnDrawGizmos()
        {
            if (visualizeTrigger && myCollider != null)
            {
                Gizmos.color = gizmoColor;
                if (myCollider is BoxCollider box)
                {
                    Matrix4x4 rotationMatrix = Matrix4x4.TRS(box.transform.position, box.transform.rotation, box.transform.lossyScale);
                    Gizmos.matrix = rotationMatrix;
                    Gizmos.DrawCube(box.center, box.size);
                }
            }
        }

        public override object GetPersistenceData()
        {
            return new PersistentTriggerData(this, base.GetPersistenceData() as PersistenceData);
        }

        public override void InjectPersistenceData(object data)
        {
            if (data is not PersistentTriggerData triggerData)
                return;
            base.InjectPersistenceData(data);
            enterTriggeredOnce = triggerData.EnterTriggered;
            exitTriggeredOnce = triggerData.ExitTriggered;
            stayTriggeredOnce = triggerData.StayTriggered;
        }

        [Serializable]
        protected class PersistentTriggerData : PersistenceData
        {
            public bool EnterTriggered, ExitTriggered, StayTriggered;

            public PersistentTriggerData(PersistentTriggerHandler triggerHandler, PersistenceData baseData)
            {
                GameObjectActive = baseData.GameObjectActive;
                PersistentComponentData = baseData.PersistentComponentData;
                EnterTriggered = triggerHandler.enterTriggeredOnce;
                ExitTriggered = triggerHandler.enterTriggeredOnce;
                StayTriggered = triggerHandler.exitTriggeredOnce;
            }
        }
    }

    public interface ITriggerHandler
    {
        void OnTriggerEnter(Collider other);

        void OnTriggerStay(Collider other);

        void OnTriggerExit(Collider other);
    }
}
