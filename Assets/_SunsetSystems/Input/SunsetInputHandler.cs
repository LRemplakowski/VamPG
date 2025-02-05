﻿using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SunsetInputHandler : SerializedMonoBehaviour
{
    [SerializeField]
    private PlayerInput _playerInput;

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

    [Button]
    private void Awake()
    {
        _playerInput.actions.actionMaps.ForEach(map => map.Enable());
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
