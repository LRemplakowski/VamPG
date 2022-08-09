using SunsetSystems.UI;
using SunsetSystems.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    [RequireComponent(typeof(Tagger))]
    public class UIBehaviourController : MonoBehaviour
    {
        [SerializeField]
        private GameplayUIManager gameplayUIParent;

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
            if (!gameplayUIParent)
                gameplayUIParent = this.FindFirstComponentWithTag<GameplayUIManager>(TagConstants.GAMEPLAY_UI);
        }

        private void OnEscape(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        private void OnCharacterSheet(InputAction.CallbackContext conext)
        {
            throw new NotImplementedException();
        }

        private void OnInventory(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}
