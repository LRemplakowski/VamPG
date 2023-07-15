using SunsetSystems.Dialogue;
using System;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    public class DialogueNPC : AbstractNPC, IInteractable
    {
        public override string NameplateText { get => ShowNameplate && Interactable ? Data.FullName : string.Empty; }
        [SerializeField]
        protected GameObject _hoverHighlight;
        public GameObject HoverHighlight
        {
            get => _hoverHighlight;
            set => _hoverHighlight = value;
        }

        private bool _isHoveredOver;
        public bool IsHoveredOver
        {
            get => _isHoveredOver;
            set
            {
                _isHoveredOver = value;
                if (HoverHighlight != null)
                    HoverHighlight.SetActive(IsHoveredOver);
            }
        }

        [SerializeField]
        protected float _interactionDistance = 2.0f;
        public float InteractionDistance
        {
            get => _interactionDistance;
            set => _interactionDistance = value;
        }
        public Creature TargetedBy { get; set; }
        public bool Interacted { get; set; }

        [SerializeField]
        protected Transform _interactionTransform;
        public Transform InteractionTransform
        {
            get => _interactionTransform;
            set => _interactionTransform = value;
        }

        [SerializeField]
        protected bool _interactable;
        public bool Interactable 
        { 
            get
            {
                return _interactable;
            } 
            set
            {
                _interactable = value;
            }
        }

        [SerializeField]
        private string _startNode;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
        }

        public void Interact()
        {
            if (!Interactable)
                return;
            //DialogueManager.Instance.StartDialogue(_startNode, dialogueProject);
            Debug.Log(TargetedBy + " interacted with object " + gameObject);
            Interacted = true;
            TargetedBy = null;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(InteractionTransform.position, _interactionDistance);
        }

        public override object GetPersistenceData()
        {
            TalkableNPCPersistenceData data = new(base.GetPersistenceData() as PersistenceData);
            data.Interactable = Interactable;
            return data;
        }

        public override void InjectPersistenceData(object data)
        {
            base.InjectPersistenceData(data);
            TalkableNPCPersistenceData persistenceData = data as TalkableNPCPersistenceData;
            Interactable = persistenceData.Interactable;
        }

        [Serializable]
        protected class TalkableNPCPersistenceData : PersistenceData
        {
            public bool Interactable;

            public TalkableNPCPersistenceData(PersistenceData persistenceData)
            {
                GameObjectActive = persistenceData.GameObjectActive;
            }

            public TalkableNPCPersistenceData()
            {

            }
        }
    }
}
