using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GUIWindowsManager : InputHandler
{
    public PlayerInventoryUI inventoryUI;
    public CharacterSheetUI characterSheetUI;
    public DialogueWindowUI dialogueUI;
    private List<UIWindow> windows = new List<UIWindow>();
    private int _activeWindows = 0;
    public int ActiveWindows
    {
        get => _activeWindows;
        private set => _activeWindows = value;
    }

    #region Enable&Disable
    private void OnEnable()
    {
        DialogueManager.onDialogueBegin += ToggleDialogue;
        DialogueManager.onDialogueEnd += ToggleDialogue;
    }

    private void OnDisable()
    {
        DialogueManager.onDialogueBegin -= ToggleDialogue;
        DialogueManager.onDialogueEnd -= ToggleDialogue;
    }
    #endregion

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (inventoryUI == null)
            inventoryUI = canvas.GetComponentInChildren<PlayerInventoryUI>(true);
        if (characterSheetUI == null)
            characterSheetUI = canvas.GetComponentInChildren<CharacterSheetUI>(true);
        if (dialogueUI == null)
            dialogueUI = canvas.GetComponentInChildren<DialogueWindowUI>(true);
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

    private void ToggleDialogue()
    {
        bool active = dialogueUI.gameObject.activeSelf;
        Debug.Log("Dialogue window active: " + active);
        if (active)
        {
            dialogueUI.gameObject.SetActive(false);
            --ActiveWindows;
        }
        else
        {
            dialogueUI.gameObject.SetActive(true);
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
