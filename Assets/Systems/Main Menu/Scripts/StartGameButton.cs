using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transitions.Manager;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class StartGameButton : MonoBehaviour
    {
        private FadeScreenAnimator transitionAnimator;
        [SerializeField]
        private GameObject backgroundSelectionObject;
        [SerializeField]
        private GameObject mainMenuParent;

        private void Start()
        {
            if (transitionAnimator == null)
                transitionAnimator = FindObjectOfType<FadeScreenAnimator>(true);
        }

        public async Task StartGame()
        {
            await transitionAnimator.FadeOut(.5f);
            await DoLoadCharacterCreationPanel();
        }

        private async Task DoLoadCharacterCreationPanel()
        {
            if (backgroundSelectionObject)
                backgroundSelectionObject.SetActive(true);
            mainMenuParent.SetActive(false);
            await transitionAnimator.FadeIn(.5f);
        }
    }
}
