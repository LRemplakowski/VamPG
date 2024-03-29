using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using System;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class InteractableEntity : PersistentEntity, IInteractable, INameplateReciever
    {
        public static readonly HashSet<IInteractable> InteractablesInScene = new();

        [field: Title("References")]
        [field: SerializeField]
        public List<IInteractionHandler> InteractionHandlers { get; set; }
        [SerializeField]
        private Collider _interactionCollider;
        [SerializeField]
        private IHighlightHandler _highlightHandler;
        [SerializeField]
        private GameObject _linkedGameObject;

        [Title("Config")]
        [SerializeField]
        protected float _interactionDistance = 2.0f;
        public float InteractionDistance
        {
            get => _interactionDistance;
            set => _interactionDistance = value;
        }
        public IActionPerformer TargetedBy { get; set; }

        private bool _interacted;
        public bool Interacted
        {
            get
            {
                return _interacted;
            }
            set
            {
                _interacted = value;
            }
        }

        [SerializeField]
        protected Transform _interactionTransform;
        public Transform InteractionTransform
        {
            get => _interactionTransform;
            set => _interactionTransform = value;
        }

        [SerializeField]
        private bool _interactable = true;
        public bool Interactable
        {
            get
            {
                return _interactable;
            }
            set
            {
                _interactable = value;
                if (_interactable)
                    InteractablesInScene.Add(this);
                else
                    InteractablesInScene.Remove(this);
                _interactionCollider.enabled = value;
            }
        }

        [SerializeField]
        private string _nameplateName;
        public string NameplateText => _nameplateName;
        [SerializeField]
        private Vector3 _nameplateOffset = new (0, 3, 0);

        public Vector3 NameplateWorldPosition => transform.position + _nameplateOffset;

        [SerializeField]
        private bool _interactableOnce = false;

        [Title("Events")]
        public UltEvent<bool> OnInteractionTriggered;

        [Title("Runtime")]
        [ShowInInspector]
        private bool _isHoveredOver;
        public bool IsHoveredOver
        {
            get => _isHoveredOver;
            set
            {
                _isHoveredOver = value;
                HandleHoverHiglight();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
            if (_interactionCollider == null)
                _interactionCollider = GetComponentInChildren<Collider>();
            if (_references == null)
                _references = GetComponent<IEntityReferences>();
            if (InteractionHandlers == null)
                InteractionHandlers = new();
            //IHH added
            if (_highlightHandler == null)
                _highlightHandler = GetComponent<IHighlightHandler>();

        }

        protected override void Awake()
        {
            base.Awake();
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
            if (Interactable)
                InteractablesInScene.Add(this);
            //Added IHH
            if (_highlightHandler == null)
                _highlightHandler = GetComponent<IHighlightHandler>();

        }

        private void OnEnable()
        {
            InteractablesInScene.Add(this);
            if (_linkedGameObject != null)
                _linkedGameObject.SetActive(true);
        }

        private void OnDisable()
        {
            InteractablesInScene.Remove(this);
            if (_linkedGameObject != null)
                _linkedGameObject.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();
            if (_interactionCollider == null)
                _interactionCollider = GetComponentInChildren<Collider>();
            _interactionCollider.enabled = Interactable;
        }

        public void ClearHandlers()
        {
            InteractionHandlers.Clear();
        }

        public void AddHandler(GameObject handler)
        {
            if (handler.TryGetComponent(out IInteractionHandler interactionHandler))
                InteractionHandlers.Add(interactionHandler);
        }

        public virtual void Interact()
        {
            if (!Interactable)
                return;
            IsHoveredOver = false;
            Debug.Log(TargetedBy + " interacted with object " + gameObject);
            Interacted = true;
            if (_interactableOnce)
                this.Interactable = false;
            bool result = false;
            foreach (IInteractionHandler handler in InteractionHandlers)
            {
                if (handler.HandleInteraction(TargetedBy))
                    result = true;
            }
            TargetedBy = null;
            OnInteractionTriggered?.Invoke(result);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(InteractionTransform.position, _interactionDistance);
        }

        private void HandleHoverHiglight()
        {
            if (_highlightHandler != null && Interactable)
                _highlightHandler.SetHighlightActive(IsHoveredOver);
        }



        public void ResetInteraction()
        {
            _interacted = false;
            Interactable = true;
        }

        public override object GetPersistenceData()
        {
            InteractableEntityPersistenceData persistenceData = new(base.GetPersistenceData() as PersistenceData);
            persistenceData.Interactable = Interactable;
            persistenceData.Interacted = _interacted;
            persistenceData.InteractableOnce = _interactableOnce;
            persistenceData.InteractionHandlers = InteractionHandlers;
            return persistenceData;
        }

        public override void InjectPersistenceData(object data)
        {
            base.InjectPersistenceData(data);
            InteractableEntityPersistenceData persistenceData = data as InteractableEntityPersistenceData;
            Interactable = persistenceData.Interactable;
            _interacted = persistenceData.Interacted;
            _interactableOnce = persistenceData.InteractableOnce;
            InteractionHandlers = persistenceData.InteractionHandlers;
            if (_linkedGameObject)
                _linkedGameObject.SetActive(persistenceData.GameObjectActive);
        }

        [Serializable]
        protected class InteractableEntityPersistenceData : PersistenceData
        {
            public bool Interactable;
            public bool Interacted;
            public bool InteractableOnce;
            public List<IInteractionHandler> InteractionHandlers;

            public InteractableEntityPersistenceData(PersistenceData persistentEntity)
            {
                GameObjectActive = persistentEntity.GameObjectActive;
            }

            public InteractableEntityPersistenceData()
            {

            }
        }
    }
}
