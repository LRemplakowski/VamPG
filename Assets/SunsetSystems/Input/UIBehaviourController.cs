using SunsetSystems.Game;
using SunsetSystems.Inventory.UI;
using SunsetSystems.UI;
using SunsetSystems.Utils;
using SunsetSystems.Utils.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    [RequireComponent(typeof(Tagger))]
    public class UIBehaviourController : MonoBehaviour, IInitialized
    {
        [SerializeField]
        private GameplayUIManager gameplayUIParent;
        [SerializeField]
        private GameManager gameManager;
        private Vector2 pointerPosition;

        private void OnEnable()
        {
            PlayerInputHandler.OnInventory += OnInventory;
            PlayerInputHandler.OnCharacterSheet += OnCharacterSheet;
            PlayerInputHandler.OnEscape += OnEscape;
            PlayerInputHandler.OnPointerPosition += OnPointerPosition;
        }

        private void OnDisable()
        {
            PlayerInputHandler.OnInventory -= OnInventory;
            PlayerInputHandler.OnCharacterSheet -= OnCharacterSheet;
            PlayerInputHandler.OnEscape -= OnEscape;
            PlayerInputHandler.OnPointerPosition -= OnPointerPosition;
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (!gameplayUIParent)
                gameplayUIParent = this.FindFirstComponentWithTag<GameplayUIManager>(TagConstants.GAMEPLAY_UI);
            if (!gameManager)
                gameManager = this.FindFirstComponentWithTag<GameManager>(TagConstants.GAME_MANAGER);
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
            PauseMenuUI pauseUI = gameplayUIParent.PauseMenuUI;
            if (GameManager.IsCurrentState(GameState.GamePaused))
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
                pauseUI.OpenSettingsScreen();
            }
        }

        private void OnCharacterSheet(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            GameManager.CurrentState = GameState.GamePaused;
            PauseMenuUI pauseUI = gameplayUIParent.PauseMenuUI;
            pauseUI.gameObject.SetActive(true);
            pauseUI.OpenCharacterSheetScreen();
        }

        private void OnInventory(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            GameManager.CurrentState = GameState.GamePaused;
            PauseMenuUI pauseUI = gameplayUIParent.PauseMenuUI;
            pauseUI.gameObject.SetActive(true);
            pauseUI.OpenInventoryScreen();
        }

        private void OnPointerPosition(InputAction.CallbackContext context)
        {
            pointerPosition = context.ReadValue<Vector2>();
        }
    }
}
