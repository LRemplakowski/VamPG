using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using SunsetSystems.Formation.Data;
using SunsetSystems.Formation.UI;
using SunsetSystems.Utils;
using InsaneSystems.RTSSelection;
using SunsetSystems.Utils.UI;

[RequireComponent(typeof(Tagger))]
public class PlayerInputHandler : Singleton<PlayerInputHandler>
{
    [SerializeField]
    private PlayerInput playerInput;
    private bool usePlayerInput = false;
    private Pointer PointerDevice => playerInput.GetDevice<Pointer>();

    public static FormationData FormationData { get; set; }
    [SerializeField]
    private PredefinedFormation defaultFormation;

    private bool ongoingSelection = false;

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
    public delegate void OnEscapeHandler(InputAction.CallbackContext context);
    public static event OnEscapeHandler OnEscape;
    public delegate void OnCharacterSheetHandler(InputAction.CallbackContext conext);
    public static event OnCharacterSheetHandler OnCharacterSheet;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Selection.OnSelectionStarted += OnSelectionStarted;
        Selection.OnSelectionFinished += OnSelectionFinished;
    }

    private void OnDestroy()
    {
        Selection.OnSelectionStarted -= OnSelectionStarted;
        Selection.OnSelectionFinished -= OnSelectionFinished;
    }

    private void OnSelectionStarted()
    {
        ongoingSelection = true;
    }

    private void OnSelectionFinished()
    {
        ongoingSelection = false;
    }

    private void Update()
    {
        if (ongoingSelection)
            return;
        if (EventSystem.current != null && PointerDevice != null)
            usePlayerInput = !InputHelper.IsRaycastHittingUIObject(PointerDevice.position.ReadValue());
        if (usePlayerInput != playerInput.inputIsActive)
        {
            SetPlayerInputActive(usePlayerInput);
        }
    }

    public void SetPlayerInputActive(bool active)
    {
        if (active)
        {
            Debug.LogWarning("Enabling player input");
            playerInput.ActivateInput();
            playerInput.SwitchCurrentActionMap("Player");
        }
        else
        {
            Debug.LogWarning("Disabling player input");
            playerInput.DeactivateInput();
            playerInput.SwitchCurrentActionMap("UI");
        }
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

    public void EscapeAction(InputAction.CallbackContext context)
    {
        OnEscape?.Invoke(context);
    }

    public void CharacterSheetAction(InputAction.CallbackContext context)
    {
        OnCharacterSheet?.Invoke(context);
    }
}
