using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using SunsetSystems.Utils;
using UnityEngine.InputSystem.UI;
using SunsetSystems.Game;

[RequireComponent(typeof(Tagger))]
public class PlayerInputHandler : InitializedSingleton<PlayerInputHandler>
{
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private GameManager gameManager;

    // Mouse input
    public delegate void OnLeftClickHandler(InputAction.CallbackContext context);
    public static event OnLeftClickHandler OnPrimaryAction;
    public delegate void OnRightClickHandler(InputAction.CallbackContext context);
    public static event OnRightClickHandler OnSecondaryAction;
    public delegate void OnMousePositionHandler(InputAction.CallbackContext context);
    public static event OnMousePositionHandler OnPointerPosition;
    // Keyboard input
    public delegate void OnInventoryHandler(InputAction.CallbackContext context);
    public static event OnInventoryHandler OnInventory;
    public delegate void OnJournalHandler(InputAction.CallbackContext context);
    public static event OnJournalHandler OnJournal;
    public delegate void OnEscapeHandler(InputAction.CallbackContext context);
    public static event OnEscapeHandler OnEscape;
    public delegate void OnCharacterSheetHandler(InputAction.CallbackContext conext);
    public static event OnCharacterSheetHandler OnCharacterSheet;
    public delegate void OnSkipDialogueHandler(InputAction.CallbackContext context);
    public static event OnSkipDialogueHandler OnSkipDialogue;
    public delegate void OnHighlightInteractablesHandler(InputAction.CallbackContext context);
    public static event OnHighlightInteractablesHandler OnHighlightInteractables;
    public delegate void OnHelpHandler(InputAction.CallbackContext context);
    public static event OnHelpHandler OnHelp;

    protected override void Awake()
    {
        base.Awake();
    }

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
