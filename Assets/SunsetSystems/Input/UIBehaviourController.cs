using SunsetSystems.Game;
using SunsetSystems.UI;
using SunsetSystems.Utils;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    [RequireComponent(typeof(Tagger))]
    public class UIBehaviourController : MonoBehaviour
    {
        [SerializeField]
        private GameplayUIManager gameplayUIParent;
        [SerializeField]
        private GameManager gameManager;

        private void OnEnable()
        {
            PlayerInputHandler.OnInventory += OnInventory;
            PlayerInputHandler.OnCharacterSheet += OnCharacterSheet;
            PlayerInputHandler.OnEscape += OnEscape;
        }

        private void OnDisable()
        {
            PlayerInputHandler.OnInventory -= OnInventory;
            PlayerInputHandler.OnCharacterSheet -= OnCharacterSheet;
            PlayerInputHandler.OnEscape -= OnEscape;
        }

        private void Start()
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
            PauseMenuUI pauseUI = gameplayUIParent.PauseMenuUI;
            if (pauseUI.gameObject.activeSelf)
            {
                pauseUI.gameObject.SetActive(false);
            }
            else
            {
                pauseUI.gameObject.SetActive(true);
                pauseUI.OpenSettingsScreen();
            }
        }

        private void OnCharacterSheet(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            PauseMenuUI pauseUI = gameplayUIParent.PauseMenuUI;
            pauseUI.gameObject.SetActive(true);
            pauseUI.OpenCharacterSheetScreen();
        }

        private void OnInventory(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            PauseMenuUI pauseUI = gameplayUIParent.PauseMenuUI;
            pauseUI.gameObject.SetActive(true);
            pauseUI.OpenInventoryScreen();
        }
    }
}
