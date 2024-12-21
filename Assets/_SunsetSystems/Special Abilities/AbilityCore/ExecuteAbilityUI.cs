using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SunsetSystems.Abilities
{
    public class ExecuteAbilityUI : SerializedMonoBehaviour, IExecutionConfirmationUI
    {
        [SerializeField]
        private CanvasGroup _uiCanvasGroup;
        [SerializeField]
        private Button _executeAbilityButton;

        private Func<bool> _shouldEnableButton;

        private void Start()
        {
            SetActive(false);
        }

        public void RegisterConfirmationCallback(UnityAction callback)
        {
            _executeAbilityButton.onClick.AddListener(callback);
        }

        public void SetActive(bool active)
        {
            float alpha = active ? 1f : 0f;
            _uiCanvasGroup.alpha = alpha;
            _uiCanvasGroup.interactable = active;
            _uiCanvasGroup.blocksRaycasts = active;
            _executeAbilityButton.interactable = active;
        }

        public void SetExectuionValidationDelegate(Func<bool> validationDelegate)
        {
            _shouldEnableButton = validationDelegate;
        }

        public void UnregisterConfirmationCallback(UnityAction callback)
        {
            _executeAbilityButton.onClick.RemoveListener(callback);
        }
    }
}
