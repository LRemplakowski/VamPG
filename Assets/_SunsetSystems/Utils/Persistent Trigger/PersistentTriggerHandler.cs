using System;
using Sirenix.OdinInspector;
using SunsetSystems.Entities;
using SunsetSystems.Party;
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
        [SerializeField]
        private bool _stopPlayerPartyOnTrigger = false;

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
            if (_stopPlayerPartyOnTrigger)
                PartyManager.Instance.StopTheParty();
        }

        public void OnTriggerExit(Collider other)
        {
            if (triggerOnlyOnce && exitTriggeredOnce)
                return;
            if (triggeredOnlyBySpecificObject && triggeringObject != other)
                return;
            TriggerExit?.InvokeSafe(other);
            exitTriggeredOnce = true;
            if (_stopPlayerPartyOnTrigger)
                PartyManager.Instance.StopTheParty();
        }

        public void OnTriggerStay(Collider other)
        {
            if (triggerOnlyOnce && stayTriggeredOnce)
                return;
            if (triggeredOnlyBySpecificObject && triggeringObject != other)
                return;
            TriggerStay?.InvokeSafe(other);
            stayTriggeredOnce = true;
            if (_stopPlayerPartyOnTrigger)
                PartyManager.Instance.StopTheParty();
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
            return new PersistentTriggerData(this);
        }

        public override void InjectPersistenceData(object data)
        {
            base.InjectPersistenceData(data);
            if (data is not PersistentTriggerData triggerData)
                return;
            enterTriggeredOnce = triggerData.EnterTriggered;
            exitTriggeredOnce = triggerData.ExitTriggered;
            stayTriggeredOnce = triggerData.StayTriggered;
        }

        [Serializable]
        protected class PersistentTriggerData : PersistenceData
        {
            public bool EnterTriggered, ExitTriggered, StayTriggered;

            public PersistentTriggerData(PersistentTriggerHandler triggerHandler) : base(triggerHandler)
            {
                EnterTriggered = triggerHandler.enterTriggeredOnce;
                ExitTriggered = triggerHandler.enterTriggeredOnce;
                StayTriggered = triggerHandler.exitTriggeredOnce;
            }

            public PersistentTriggerData() : base()
            {

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
