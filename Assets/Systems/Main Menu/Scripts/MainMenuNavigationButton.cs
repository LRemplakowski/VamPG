using System;
using System.Collections;
using System.Collections.Generic;
using Transitions.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.MainMenu.UI
{
    public class MainMenuNavigationButton : MonoBehaviour
    {
        protected TransitionAnimator transitionAnimator;
        [SerializeField]
        protected GameObject currentGUIScreen;
        [SerializeField]
        protected GameObject targetGUIScreen;
        protected MainMenuNavigationButton[] navButtons;

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

        protected virtual void Start()
        {
            if (transitionAnimator == null)
                transitionAnimator = FindObjectOfType<TransitionAnimator>(true);
            navButtons = FindObjectsOfType<MainMenuNavigationButton>();
        }

        public virtual void OnClick()
        {
            Array.ForEach(Array.FindAll(navButtons, b => !b.Equals(this)), b => b.OnDisable());
            transitionAnimator.FadeOut();
        }

        protected virtual void OnFadedOut()
        {
            DoLoadTargetGUIScreen();
        }

        protected void DoLoadTargetGUIScreen()
        {
            if (targetGUIScreen)
                targetGUIScreen.SetActive(true);
            currentGUIScreen.SetActive(false);
            transitionAnimator.FadeIn();
        }
    }
}
