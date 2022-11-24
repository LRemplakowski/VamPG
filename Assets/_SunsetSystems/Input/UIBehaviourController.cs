using SunsetSystems.Dialogue;
using SunsetSystems.Entities;
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

namespace SunsetSystems.Input
{
    [RequireComponent(typeof(Tagger))]
    public class UIBehaviourController : MonoBehaviour, IInitialized
    {
        [SerializeField]
        private GameplayUIManager gameplayUIParent;
        [SerializeField]
        private GameManager gameManager;
        [SerializeField]
        private LayerMask defaultRaycastMask;

        private Vector2 pointerPosition;

        private void OnEnable()
        {
            PlayerInputHandler.OnInventory += OnInventory;
            PlayerInputHandler.OnCharacterSheet += OnCharacterSheet;
            PlayerInputHandler.OnEscape += OnEscape;
            PlayerInputHandler.OnPointerPosition += OnPointerPosition;
            PlayerInputHandler.OnJournal += OnJournal;
            PlayerInputHandler.OnSkipDialogue += OnSkipDialogue;
        }

        private void OnDisable()
        {
            PlayerInputHandler.OnInventory -= OnInventory;
            PlayerInputHandler.OnCharacterSheet -= OnCharacterSheet;
            PlayerInputHandler.OnEscape -= OnEscape;
            PlayerInputHandler.OnPointerPosition -= OnPointerPosition;
            PlayerInputHandler.OnJournal -= OnJournal;
            PlayerInputHandler.OnSkipDialogue -= OnSkipDialogue;
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
            if (gameplayUIParent != null && Physics.Raycast(ray, out RaycastHit hit, 100f, defaultRaycastMask))
            {
                INameplateReciever nameplateReciever = hit.collider.GetComponent<INameplateReciever>();
                if (nameplateReciever is not null)
                {
                    gameplayUIParent.HandleNameplateHover(nameplateReciever);
                }
                else
                {
                    gameplayUIParent.DisableNameplate();
                }
            }
        }

        public void Initialize()
        {
            if (!gameplayUIParent)
                gameplayUIParent = this.FindFirstComponentWithTag<GameplayUIManager>(TagConstants.GAMEPLAY_UI);
            if (!gameManager)
                gameManager = this.FindFirstComponentWithTag<GameManager>(TagConstants.GAME_MANAGER);
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
            if (gameplayUIParent.ContainerGUI.gameObject.activeInHierarchy)
            {
                gameplayUIParent.ContainerGUI.CloseContainerGUI();
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

        private void SwitchPauseAndOpenScreen(PauseMenuScreen screen)
        {
            PauseMenuUI pauseUI = gameplayUIParent.PauseMenuUI;
            if (GameManager.IsCurrentState(GameState.GamePaused) && pauseUI.CurrentActiveScreen == screen)
            {
                Debug.Log("Resuming game");
                GameManager.CurrentState = GameState.Exploration;
                pauseUI.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Pausing game");
                GameManager.CurrentState = GameState.GamePaused;
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

        private void OnSkipDialogue(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            DialogueManager.InterruptCurrentLine();
        }
    }
}
