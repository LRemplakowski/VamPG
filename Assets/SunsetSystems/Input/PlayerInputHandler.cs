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

    public delegate void OnLeftClickHandler(InputAction.CallbackContext context);
    public static event OnLeftClickHandler OnLeftClickEvent;
    public delegate void OnRightClickHandler(InputAction.CallbackContext context);
    public static event OnRightClickHandler OnRightClickEvent;
    public delegate void OnMousePositionHandler(InputAction.CallbackContext context);
    public static event OnMousePositionHandler OnMousePositionEvent;

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

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        OnLeftClickEvent?.Invoke(context);
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        OnRightClickEvent?.Invoke(context);
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        OnMousePositionEvent?.Invoke(context);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
    }

    public void OnEscape(InputAction.CallbackContext context)
    {

    }

    public void OnCharacterSheet(InputAction.CallbackContext context)
    {

    }
}
