using Sirenix.OdinInspector;
using SunsetSystems.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class UIInputManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private PauseMenuScreenHandler _pauseScreenHandler;
        [SerializeField]
        private GameplayScreenHandler _gameplayScreenHandler;

        private void OnEnable()
        {
            SunsetInputHandler.OnInventory += OnInventory;
            SunsetInputHandler.OnCharacterSheet += OnCharacterSheet;
            SunsetInputHandler.OnEscape += OnCancel;
            SunsetInputHandler.OnPointerPosition += OnPointerPosition;
            SunsetInputHandler.OnJournal += OnJournal;
            SunsetInputHandler.OnHighlightInteractables += OnHighlightInteractablesAction;
        }

        private void OnDisable()
        {
            SunsetInputHandler.OnInventory -= OnInventory;
            SunsetInputHandler.OnCharacterSheet -= OnCharacterSheet;
            SunsetInputHandler.OnEscape -= OnCancel;
            SunsetInputHandler.OnPointerPosition -= OnPointerPosition;
            SunsetInputHandler.OnJournal -= OnJournal;
            SunsetInputHandler.OnHighlightInteractables -= OnHighlightInteractablesAction;
        }

        private void OnSecondaryAction(InputAction.CallbackContext context)
        {
            //if (!context.performed)
            //    return;
            //if (InputHelper.IsRaycastHittingUIObject(pointerPosition, out List<RaycastResult> hits))
            //{
            //    IContextMenuTarget contextMenuTarget;
            //    if (hits.Any(h => (contextMenuTarget = h.gameObject.GetComponent<IContextMenuTarget>()) is not null))
            //    {
            //        Debug.Log("Secondary action in UI!");
            //    }
            //}
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            _pauseScreenHandler.OnCancelAction(context);
        }

        private void OnCharacterSheet(InputAction.CallbackContext context)
        {
            _pauseScreenHandler.OnCharacterSheetAction(context);
        }

        private void OnInventory(InputAction.CallbackContext context)
        {
            _pauseScreenHandler.OnInventoryAction(context);
        }

        private void OnJournal(InputAction.CallbackContext context)
        {
            _pauseScreenHandler.OnJournalAction(context);
        }

        private void OnPointerPosition(InputAction.CallbackContext context)
        {
            _gameplayScreenHandler.OnPointerPositionAction(context);
        }

        private void OnHighlightInteractablesAction(InputAction.CallbackContext context)
        {
            _gameplayScreenHandler.OnHighlightInteractablesAction(context);
        }
    }
}
