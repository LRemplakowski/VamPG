using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public PlayerInventoryUI inventoryUI;
    private List<UIWindow> windows = new List<UIWindow>();
    private int _activeWindows = 0;
    private int ActiveWindows
    {
        get => _activeWindows;
        set
        {
            _activeWindows = value;
            ToggleActionMap();
        }
    }
    private void Awake()
    {
        if (inventoryUI == null)
            inventoryUI = FindObjectOfType<PlayerInventoryUI>();
        windows =  new List<UIWindow>(FindObjectOfType<Canvas>().GetComponentsInChildren<UIWindow>(true));
    }

    public void ToggleInventory()
    {
        bool active = inventoryUI.gameObject.activeSelf;
        Debug.Log("Inventory active: " + active);
        if(active)
        {
            inventoryUI.gameObject.SetActive(false);
            --ActiveWindows;
        }
        else
        {
            inventoryUI.gameObject.SetActive(true);
            ++ActiveWindows;
        }    
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            foreach (UIWindow window in windows)
            {
                window.gameObject.SetActive(false);
            }
            ActiveWindows = 0;
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            ToggleInventory();
        }
    }

    private void ToggleActionMap()
    {
        Debug.Log("Action map toggle! Active windows: " + ActiveWindows);
        if(ActiveWindows <= 0)
        {
            InputManager.Input.SwitchCurrentActionMap("Player");
        }
    }
}
