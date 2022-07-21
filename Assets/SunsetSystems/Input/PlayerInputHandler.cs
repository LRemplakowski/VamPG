using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Entities.Characters;
using Entities;
using InsaneSystems.RTSSelection;
using UnityEngine.AI;
using SunsetSystems.Formation.Data;
using SunsetSystems.Formation.UI;
using SunsetSystems.Utils;

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

    private void Update()
    {
        if (EventSystem.current != null && PointerDevice != null)
            usePlayerInput = !IsRaycastHittingUIObject(PointerDevice.position.ReadValue());
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
        Pointer device = playerInput.GetDevice<Pointer>();
        if (device != null && IsRaycastHittingUIObject(device.position.ReadValue()))
            return;
        OnLeftClickEvent?.Invoke(context);
    }

    private bool IsRaycastHittingUIObject(Vector2 position)
    {
        if (m_PointerData == null)
            m_PointerData = new PointerEventData(EventSystem.current);
        m_PointerData.position = position;
        EventSystem.current.RaycastAll(m_PointerData, m_RaycastResults);
        return m_RaycastResults.Count > 0;
    }

    private PointerEventData m_PointerData;
    private List<RaycastResult> m_RaycastResults = new();

    public void OnRightClick(InputAction.CallbackContext context)
    {
        Pointer device = playerInput.GetDevice<Pointer>();
        if (device != null && IsRaycastHittingUIObject(device.position.ReadValue()))
            return;
        OnRightClickEvent?.Invoke(context);
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        OnMousePositionEvent?.Invoke(context);
    }
}
