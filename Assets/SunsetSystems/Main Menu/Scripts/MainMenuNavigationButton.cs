using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunsetSystems.Loading;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.MainMenu.UI
{
    public class MainMenuNavigationButton : MonoBehaviour
    {
        protected FadeScreenAnimator transitionAnimator;
        [SerializeField]
        protected GameObject currentGUIScreen;
        [SerializeField]
        protected GameObject targetGUIScreen;
        protected MainMenuNavigationButton[] navButtons;

        protected virtual void Start()
        {
            if (transitionAnimator == null)
                transitionAnimator = FindObjectOfType<FadeScreenAnimator>(true);
            navButtons = FindObjectsOfType<MainMenuNavigationButton>();
        }

        public async virtual void OnClick()
        {
            await transitionAnimator.FadeOut(.5f);
            await DoLoadTargetGUIScreen();
        }

        protected async Task DoLoadTargetGUIScreen()
        {
            if (targetGUIScreen)
                targetGUIScreen.SetActive(true);
            currentGUIScreen.SetActive(false);
            await transitionAnimator.FadeIn(.5f);
        }
    }
}
