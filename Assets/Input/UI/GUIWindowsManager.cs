using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GUIWindowsManager : InputHandler
{
    public PlayerInventoryUI inventoryUI;
    public CharacterSheetUI characterSheetUI;
    private List<UIWindow> windows = new List<UIWindow>();
    private int _activeWindows = 0;
    private int ActiveWindows
    {
        get => _activeWindows;
        set => _activeWindows = value;
    }

    private void Awake()
    {
        if (inventoryUI == null)
            inventoryUI = FindObjectOfType<PlayerInventoryUI>(true);
        if (characterSheetUI == null)
            characterSheetUI = FindObjectOfType<CharacterSheetUI>(true);
        windows = new List<UIWindow>(FindObjectOfType<Canvas>().GetComponentsInChildren<UIWindow>(true));
    }

    private void ToggleInventory()
    {
        bool active = inventoryUI.gameObject.activeSelf;
        Debug.Log("Inventory active: " + active);
        if (active)
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

    private void ToggleCharacterSheet()
    {
        bool active = characterSheetUI.gameObject.activeSelf;
        Debug.Log("Character sheet actvie: " + active);
        if (active)
        {
            characterSheetUI.gameObject.SetActive(false);
            --ActiveWindows;
        }
        else
        {
            characterSheetUI.gameObject.SetActive(true);
            ++ActiveWindows;
        }
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(ActiveWindows <= 0)
            {
                Debug.Log("Invoke pause menu");
            }
            else
            {
                ManageInput(ClearAllGUIWindows);
            }
        }
    }

    private void ClearAllGUIWindows()
    {
        foreach (UIWindow window in windows)
        {
            window.gameObject.SetActive(false);
        }
        ActiveWindows = 0;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ManageInput(ToggleInventory);
        }
    }

    public void OnCharacterSheet(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ManageInput(ToggleCharacterSheet);
        }
    }
}
