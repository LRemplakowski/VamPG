using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Interactable;
using SunsetSystems.Utils.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SunsetSystems.UI
{
    public class NameplateManager : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _raycastTargetMask;
        [SerializeField]
        private HoverNameplate _hoverNameplate;

        private Vector2 _pointerPosition;

        public void OnPointerPosition(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _pointerPosition = context.ReadValue<Vector2>();
            }

        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(_pointerPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _raycastTargetMask))
            {
                if (InputHelper.IsRaycastHittingUIObject(_pointerPosition, out List<RaycastResult> hits))
                {
                    if (hits.Any(hit => hit.gameObject.GetComponentInParent<CanvasGroup>()?.blocksRaycasts ?? false))
                    {
                        DisableNameplate();
                        return;
                    }
                }
                INameplateReciever nameplateReciever = hit.collider.GetComponent<INameplateReciever>();
                if (nameplateReciever is not null && (nameplateReciever as MonoBehaviour).enabled)
                {
                    if (nameplateReciever is IInteractable interactable && interactable.IsHoveredOver == false)
                    {
                        DisableNameplate();
                    }
                    else
                    {
                        HandleNameplateHover(nameplateReciever);
                    }
                }
                else
                {
                    DisableNameplate();
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
            _hoverNameplate.transform.position = screenPoint;
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
    }
}
