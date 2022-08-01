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
        [field: SerializeField]
        private UI.SelectionRect _selectionRect;
        UI.SelectionRect SelectionRect
        {
            get
            {
                if (!_selectionRect)
                {
                    _selectionRect = this.FindFirstComponentWithTag<UI.SelectionRect>(TagConstants.SELECTION_RECT);
                }
                return _selectionRect;
            }
        }

        Vector2 startMousePosition;
        Vector2 mousePosition;

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
            if (context.performed)
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
            Debug.Log("selection finished call");
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