using SunsetSystems.Entities.Characters;
using UnityEngine;
using UnityEngine.Events;

namespace SunsetSystems.Entities.Interactable
{
    public abstract class InteractableEntity : Entity, IInteractable
    {
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

        public UnityEvent OnInteractionTriggered;

        protected virtual void OnValidate()
        {
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
        }

        protected virtual void Awake()
        {
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
        }

        public void Interact()
        {
            Debug.Log(TargetedBy + " interacted with object " + gameObject);
            HandleInteraction();
            OnInteractionTriggered?.Invoke();
            Interacted = true;
            TargetedBy = null;
        }

        protected abstract void HandleInteraction();

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(InteractionTransform.position, _interactionDistance);
        }
    }
}
