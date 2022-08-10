using SunsetSystems.Game;
using SunsetSystems.UI;
using SunsetSystems.Utils;
using UnityEngine;
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
            Debug.Log(context.phase.ToString());
            if (!context.performed)
                return;
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
    }
}
