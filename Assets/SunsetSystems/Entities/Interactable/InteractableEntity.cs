using Entities.Characters;
using UnityEngine;

namespace Entities.Interactable
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
                HoverHighlight.SetActive(IsHoveredOver);
            }
        }

        [SerializeField]
        protected float _interactionDistance = 1.0f;
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

        public void OnValidate()
        {
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
        }

        private void Awake()
        {
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
        }

        ///<summary>
        ///If overriden, base should be called always, after any override logic.
        /// </summary>
        public virtual void Interact()
        {
            Debug.Log(TargetedBy + " interacted with object " + gameObject);
            Interacted = true;
            TargetedBy = null;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(InteractionTransform.position, _interactionDistance);
        }
    }
}
