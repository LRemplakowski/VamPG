using Entities.Characters;
using UnityEngine;

namespace Entities.Interactable
{
    public abstract class InteractableEntity : Entity, IInteractable
    {
        [HideInInspector, SerializeField]
        private GameObject _hoverHighlight;
        [ExposeProperty]
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

        [HideInInspector, SerializeField]
        private float _interactionDistance = 1.0f;
        [ExposeProperty]
        public float InteractionDistance
        {
            get => _interactionDistance;
            set => _interactionDistance = value;
        }
        public Creature TargetedBy { get; set; }
        public bool Interacted { get; set; }

        [HideInInspector, SerializeField]
        private Transform _interactionTransform;
        [ExposeProperty]
        public Transform InteractionTransform
        {
            get => _interactionTransform;
            set => _interactionTransform = value;
        }

        public void Awake()
        {
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
        }

        //If overriden, base should be called always, after any override logic.
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
