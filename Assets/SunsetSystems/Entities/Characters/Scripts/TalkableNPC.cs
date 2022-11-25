using Apex;
using SunsetSystems.Dialogue;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Entities.Characters
{
    public class TalkableNPC : AbstractNPC, IInteractable
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

        [SerializeField]
        private YarnProject dialogueProject;
        [SerializeField]
        private string _startNode;

        public void OnValidate()
        {
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
            DialogueManager.StartDialogue(_startNode, dialogueProject);
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
