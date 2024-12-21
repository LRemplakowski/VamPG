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
            UpdateShowInterface(false, () => false);
        }

        public void RegisterConfirmationCallback(UnityAction callback)
        {
            _executeAbilityButton.onClick.AddListener(callback);
        }

        public void UpdateShowInterface(bool visible, Func<bool> interactableDelegate)
        {
            float alpha = visible ? 1f : 0f;
            bool interactable = interactableDelegate?.Invoke() ?? false;
            _uiCanvasGroup.alpha = alpha;
            _uiCanvasGroup.interactable = interactable;
            _uiCanvasGroup.blocksRaycasts = visible;
            _executeAbilityButton.interactable = interactable;
        }

        public void UnregisterConfirmationCallback(UnityAction callback)
        {
            _executeAbilityButton.onClick.RemoveListener(callback);
        }
    }
}
