using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Interactable;
using SunsetSystems.Utils.Input;
using SunsetSystems.Utils.ObjectPooling;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SunsetSystems.UI
{
    public class NameplateManager : AbstractObjectPool<HoverNameplate>
    {
        public static NameplateManager Instance { get; private set; }

        [Title("Nameplate Config")]
        [SerializeField, Required]
        private Canvas _parentCanvas;
        [SerializeField, Required]
        private RectTransform _rectTransform;
        [SerializeField]
        private LayerMask _raycastTargetMask;
        [SerializeField, Required]
        private HoverNameplate _hoverNameplate;

        private Vector2 _pointerPosition;

        private Dictionary<INameplateReciever, HoverNameplate> _activeNameplates = new();

        protected override void Awake()
        {
            base.Awake();
            _activeNameplates = new();
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void OnPointerPosition(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _pointerPosition = context.ReadValue<Vector2>();
            }
        }

        private void Update()
        {
            foreach (var nameplate in _activeNameplates)
            {
                if (WorldToUISpace(nameplate.Key.NameplateWorldPosition, out var canvasPoint))
                {
                    nameplate.Value.transform.localPosition = canvasPoint;
                }
            }
        }

        public void HandleNameplateHover(INameplateReciever nameplateReciever)
        {
            if (string.IsNullOrEmpty(nameplateReciever.NameplateText))
            {
                DisableNameplate();
                return;
            }
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(nameplateReciever.NameplateWorldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform, screenPoint, Camera.main, out Vector2 nameplatePosition);
            (_hoverNameplate.transform as RectTransform).localPosition = nameplatePosition;
            _hoverNameplate.SetNameplateText(nameplateReciever.NameplateText);
            _hoverNameplate.gameObject.SetActive(true);
        }

        public void DisableNameplate()
        {
            _hoverNameplate.gameObject.SetActive(false);
        }

        public void OnHighlightInteractables(InputAction.CallbackContext context)
        {
            if (context.performed)
                InteractableEntity.InteractablesInScene.ForEach(interactable => interactable.IsHoveredOver = true);
            else if (context.canceled)
                InteractableEntity.InteractablesInScene.ForEach(interactable => interactable.IsHoveredOver = false);
        }

        public void AddNameplateSource(INameplateReciever nameplateReciever)
        {
            if (string.IsNullOrWhiteSpace(nameplateReciever.NameplateText))
                return;
            if (WorldToUISpace(nameplateReciever.NameplateWorldPosition, out Vector2 canvasPoint))
            {
                var nameplate = GetPooledObject();
                nameplate.transform.localPosition = canvasPoint;
                nameplate.SetNameplateText(nameplateReciever.NameplateText);
                if (_activeNameplates.TryGetValue(nameplateReciever, out var oldNameplate))
                    ReturnObject(oldNameplate);
                _activeNameplates[nameplateReciever] = nameplate;
            }
        }

        private bool WorldToUISpace(Vector3 worldPos, out Vector2 canvasPoint)
        {
            var camera = Camera.main;
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPos);
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentCanvas.transform as RectTransform, screenPosition, null, out canvasPoint);
        }

        public void RemoveNameplateSource(INameplateReciever nameplateReciever)
        {
            if (_activeNameplates.TryGetValue(nameplateReciever, out var nameplate))
            {
                ReturnObject(nameplate);
                _activeNameplates.Remove(nameplateReciever);
            }
        }
    }
}
