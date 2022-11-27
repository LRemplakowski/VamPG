﻿using SunsetSystems.Entities.Characters;
using SunsetSystems.Resources;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SunsetSystems.Entities.Interactable
{
    public abstract class InteractableEntity : Entity, IInteractable, INameplateReciever
    {
        public static readonly List<IInteractable> InteractablesInScene = new();

        [SerializeField]
        private Collider _interactionCollider;
        [field: SerializeField]
        public GameObject HoverHighlight { get; set; }

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

        [SerializeField]
        protected float _interactionDistance = 2.0f;
        public float InteractionDistance
        {
            get => _interactionDistance;
            set => _interactionDistance = value;
        }
        public Creature TargetedBy { get; set; }

        private bool _interacted;
        public bool Interacted
        {
            get
            {
                return _interacted;
            }
            set
            {
                if (_interactableOnce || _interacted)
                    return;
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
                this.enabled = value;
                _interactionCollider.enabled = value;
            }
        }

        [SerializeField]
        private string _nameplateName;
        public string NameplateText => _nameplateName;

        public Vector3 NameplateWorldPosition => transform.position + Vector3.up;

        [SerializeField]
        private bool _interactableOnce = false;

        public UnityEvent OnInteractionTriggered;

        protected virtual void OnValidate()
        {
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
            if (_interactionCollider == null)
                _interactionCollider = GetComponentInChildren<Collider>();
        }

        protected virtual void Awake()
        {
            if (InteractionTransform == null)
            {
                InteractionTransform = this.transform;
            }
        }

        private void OnEnable()
        {
            InteractablesInScene.Add(this);
        }

        protected virtual void Start()
        {
            enabled = Interactable;
            if (_interactionCollider == null)
                _interactionCollider = GetComponentInChildren<Collider>();
            _interactionCollider.enabled = Interactable;
        }

        protected virtual void OnDisable()
        {
            InteractablesInScene.Remove(this);
        }

        public void Interact()
        {
            IsHoveredOver = false;
            if (!Interactable || (_interactableOnce && Interacted))
                return;
            Debug.Log(TargetedBy + " interacted with object " + gameObject);
            HandleInteraction();
            OnInteractionTriggered?.Invoke();
            Interacted = true;
            TargetedBy = null;
            if (_interactableOnce)
                this.enabled = false;
        }

        protected abstract void HandleInteraction();

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(InteractionTransform.position, _interactionDistance);
        }

        private void HandleHoverHiglight()
        {
            if (HoverHighlight != null && Interactable)
                HoverHighlight.SetActive(IsHoveredOver);
        }
    }
}
