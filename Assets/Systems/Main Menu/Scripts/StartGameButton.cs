using System;
using System.Collections;
using System.Collections.Generic;
using Transitions.Manager;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class StartGameButton : MonoBehaviour
    {
        private TransitionAnimator transitionAnimator;
        [SerializeField]
        private GameObject backgroundSelectionObject;
        [SerializeField]
        private GameObject mainMenuParent;

        private void Start()
        {
            if (transitionAnimator == null)
                transitionAnimator = FindObjectOfType<TransitionAnimator>(true);
        }

        #region Enable&Disable
        private void OnEnable()
        {
            TransitionAnimator.OnFadedOut += OnFadedOut;
        }

        private void OnDisable()
        {
            TransitionAnimator.OnFadedOut -= OnFadedOut;
        }
        #endregion

        private void OnFadedOut()
        {
            DoLoadCharacterCreationPanel();
        }

        public void StartGame()
        {
            transitionAnimator.FadeOut();
        }

        private void DoLoadCharacterCreationPanel()
        {
            if (backgroundSelectionObject)
                backgroundSelectionObject.SetActive(true);
            mainMenuParent.SetActive(false);
            transitionAnimator.FadeIn();
        }
    }
}
