using SunsetSystems.Dialogue;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Interactable;
using SunsetSystems.Game;
using SunsetSystems.Inventory.UI;
using SunsetSystems.UI;
using SunsetSystems.Utils;
using SunsetSystems.Utils.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Yarn.Unity;
using Zenject;

namespace SunsetSystems.Input
{
    [RequireComponent(typeof(Tagger))]
    public class UIBehaviourController : MonoBehaviour
    {
        [Inject]
        private IGameplayUI gameplayUI;
        [Inject]
        private IGameManager gameManager;
        [SerializeField]
        private LayerMask _raycastTargetMask;

        private Vector2 pointerPosition;

        private void OnEnable()
        {
            PlayerInputHandler.OnInventory += OnInventory;
            PlayerInputHandler.OnCharacterSheet += OnCharacterSheet;
            PlayerInputHandler.OnEscape += OnEscape;
            PlayerInputHandler.OnPointerPosition += OnPointerPosition;
            PlayerInputHandler.OnJournal += OnJournal;
            PlayerInputHandler.OnHighlightInteractables += OnHighlightInteractables;
            PlayerInputHandler.OnHelp += OnShowHelp;
        }

        private void OnDisable()
        {
            PlayerInputHandler.OnInventory -= OnInventory;
            PlayerInputHandler.OnCharacterSheet -= OnCharacterSheet;
            PlayerInputHandler.OnEscape -= OnEscape;
            PlayerInputHandler.OnPointerPosition -= OnPointerPosition;
            PlayerInputHandler.OnJournal -= OnJournal;
            PlayerInputHandler.OnHighlightInteractables -= OnHighlightInteractables;
            PlayerInputHandler.OnHelp -= OnShowHelp;
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
            if (gameplayUI != null && Physics.Raycast(ray, out RaycastHit hit, 100f, _raycastTargetMask))
            {
                if (InputHelper.IsRaycastHittingUIObject(pointerPosition, out List<RaycastResult> hits))
                {
                    if (hits.Any(hit => hit.gameObject.GetComponentInParent<CanvasGroup>()?.blocksRaycasts ?? false))
                    {
                        gameplayUI.DisableNameplate();
                        return;
                    }
                }
                INameplateReciever nameplateReciever = hit.collider.GetComponent<INameplateReciever>();
                if (nameplateReciever is not null && (nameplateReciever as MonoBehaviour).enabled)
                {
                    if (nameplateReciever is IInteractable interactable && interactable.IsHoveredOver == false)
                    {
                        gameplayUI.DisableNameplate();
                    }
                    else
                    {
                        gameplayUI.ShowNameplate(nameplateReciever);
                    }
                }
                else
                {
                    gameplayUI.DisableNameplate();
                }
            }
        }

        private void OnSecondaryAction(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            if (InputHelper.IsRaycastHittingUIObject(pointerPosition, out List<RaycastResult> hits))
            {
                IContextMenuTarget contextMenuTarget;
                if (hits.Any(h => (contextMenuTarget = h.gameObject.GetComponent<IContextMenuTarget>()) is not null))
                {
                    Debug.Log("Secondary action in UI!");
                }
            }
        }

        private void OnEscape(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            if (gameplayUI.ContainerGUI.gameObject.activeInHierarchy)
            {
                gameplayUI.ContainerGUI.CloseContainerGUI();
                return;
            }
            SwitchPauseAndOpenScreen(PauseMenuScreen.Settings);
        }

        private void OnCharacterSheet(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            SwitchPauseAndOpenScreen(PauseMenuScreen.CharacterSheet);
        }

        private void OnInventory(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            SwitchPauseAndOpenScreen(PauseMenuScreen.Inventory);
        }

        private void OnJournal(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            SwitchPauseAndOpenScreen(PauseMenuScreen.Journal);
        }

        private void OnShowHelp(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                gameplayUI.HelpOverlay.SetActive(true);
            }
            else if (context.canceled)
            {
                gameplayUI.HelpOverlay.SetActive(false);
            }
        }

        private void SwitchPauseAndOpenScreen(PauseMenuScreen screen)
        {
            if (gameManager.IsCurrentState(GameState.Menu))
                return;
            PauseMenuUI pauseUI = gameplayUI.PauseMenuUI;
            if (gameManager.IsCurrentState(GameState.GamePaused) && pauseUI.CurrentActiveScreen == screen)
            {
                Debug.Log("Resuming game");
                gameManager.CurrentState = GameState.Exploration;
                pauseUI.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Pausing game");
                gameManager.CurrentState = GameState.GamePaused;
                pauseUI.gameObject.SetActive(true);
                pauseUI.OpenMenuScreen(screen);
            }
        }

        private void OnPointerPosition(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            pointerPosition = context.ReadValue<Vector2>();
        }

        private void OnHighlightInteractables(InputAction.CallbackContext context)
        {
            if (context.performed)
                InteractableEntity.InteractablesInScene.ForEach(interactable => interactable.IsHoveredOver = true);
            else if (context.canceled)
                InteractableEntity.InteractablesInScene.ForEach(interactable => interactable.IsHoveredOver = false);
        }
    }
}
