using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using SunsetSystems.Utils;
using UnityEngine.InputSystem.UI;
using SunsetSystems.Game;
using System;

[RequireComponent(typeof(Tagger))]
public class PlayerInputHandler : InitializedSingleton<PlayerInputHandler>
{
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private GameManager gameManager;

    // Mouse input
    public static event Action<InputAction.CallbackContext> OnPrimaryAction;
    public static event Action<InputAction.CallbackContext> OnSecondaryAction;
    public static event Action<InputAction.CallbackContext> OnPointerPosition;
    // Keyboard input
    public static event Action<InputAction.CallbackContext> OnInventory;
    public static event Action<InputAction.CallbackContext> OnJournal;
    public static event Action<InputAction.CallbackContext> OnEscape;
    public static event Action<InputAction.CallbackContext> OnCharacterSheet;
    public static event Action<InputAction.CallbackContext> OnSkipDialogue;
    public static event Action<InputAction.CallbackContext> OnHighlightInteractables;
    public static event Action<InputAction.CallbackContext> OnHelp;

    private void Start()
    {
        if (!gameManager)
            this.FindFirstComponentWithTag<GameManager>(TagConstants.GAME_MANAGER);
        if (!playerInput)
            playerInput = GetComponent<PlayerInput>();
    }

    public override void Initialize()
    {
        playerInput.uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
    }

    public override void LateInitialize()
    {
        
    }

    public void PrimaryAction(InputAction.CallbackContext context)
    {
        OnPrimaryAction?.Invoke(context);
    }

    public void SecondaryAction(InputAction.CallbackContext context)
    {
        OnSecondaryAction?.Invoke(context);
    }

    public void PointerPosition(InputAction.CallbackContext context)
    {
        OnPointerPosition?.Invoke(context);
    }

    public void InventoryAction(InputAction.CallbackContext context)
    {
        OnInventory?.Invoke(context);
    }

    public void JournalAction(InputAction.CallbackContext context)
    {
        OnJournal?.Invoke(context);
    }

    public void EscapeAction(InputAction.CallbackContext context)
    {
        OnEscape?.Invoke(context);
    }

    public void CharacterSheetAction(InputAction.CallbackContext context)
    {
        OnCharacterSheet?.Invoke(context);
    }

    public void SkipDialogueAction(InputAction.CallbackContext context)
    {
        OnSkipDialogue?.Invoke(context);
    }

    public void HighlightInteractablesAction(InputAction.CallbackContext context)
    {
        OnHighlightInteractables?.Invoke(context);
    }

    public void Help(InputAction.CallbackContext context)
    {
        OnHelp?.Invoke(context);
    }
}
