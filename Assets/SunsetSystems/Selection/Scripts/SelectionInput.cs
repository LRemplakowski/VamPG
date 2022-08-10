using InsaneSystems.RTSSelection.UI;
using SunsetSystems.Utils;
using SunsetSystems.Utils.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace InsaneSystems.RTSSelection
{
    /// <summary> In this class handled all player selection input. </summary>
    [RequireComponent(typeof(Selection))]
    public class SelectionInput : Singleton<SelectionInput>
    {
        [SerializeField]
        Selection selection;
        SelectionRect SelectionRect => this.FindFirstComponentWithTag<SelectionRect>(TagConstants.SELECTION_RECT);

        Vector2 startMousePosition = new();
        Vector2 mousePosition = new();

        int selectionButton;

        private void OnEnable()
        {
            PlayerInputHandler.OnPrimaryAction += OnPrimaryAction;
            PlayerInputHandler.OnPointerPosition += OnPointerPosition;
        }

        private void OnDisable()
        {
            PlayerInputHandler.OnPrimaryAction -= OnPrimaryAction;
            PlayerInputHandler.OnPointerPosition -= OnPointerPosition;
        }

        private void Start()
        {
            if (selection == null)
                selection = GetComponent<Selection>();
        }

        public void OnPrimaryAction(InputAction.CallbackContext context)
        {
            if (context.performed && !InputHelper.IsRaycastHittingUIObject(mousePosition))
            {
                HandleClick();
            }
            else if (context.canceled)
            {
                HandleClickRelease();
            }
        }

        private void HandleClick()
        {
            startMousePosition = new Vector2(mousePosition.x, mousePosition.y);
            selection.StartSelection();
            SelectionRect.EnableRect(startMousePosition);
        }

        private void HandleClickRelease()
        {
            selection.FinishSelection(startMousePosition, mousePosition);
            SelectionRect.DisableRect();
        }

        public void OnPointerPosition(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            mousePosition = context.ReadValue<Vector2>();
        }
    }
}