using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace InsaneSystems.RTSSelection
{
    /// <summary> In this class handled all player selection input. </summary>
    [RequireComponent(typeof(Selection))]
    public class SelectionInput : InitializedSingleton<SelectionInput>
    {
        [SerializeField] Selection selection;
        [SerializeField] UI.SelectionRect selectionRect;

        Vector3 startMousePosition;
        Vector2 mousePosition;

        int selectionButton;

        private void Start()
        {
            Initialize();
        }

        public override void Initialize()
        {
            if (selectionRect == null)
                selectionRect = FindObjectOfType<UI.SelectionRect>(true);
            if (selection == null)
                selection = GetComponent<Selection>();
        }

        public void OnLeftClick(InputAction.CallbackContext context)
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
            startMousePosition = new Vector3(mousePosition.x, mousePosition.y);
            selection.StartSelection();
            selectionRect.EnableRect(startMousePosition);
        }

        private void HandleClickRelease()
        {
            selection.FinishSelection(startMousePosition, mousePosition);
            selectionRect.DisableRect();
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            mousePosition = context.ReadValue<Vector2>();
        }
    }
}