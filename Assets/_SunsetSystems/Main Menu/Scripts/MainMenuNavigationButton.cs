using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunsetSystems.Persistence;
using SunsetSystems.UI;
using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.MainMenu.UI
{
    public class MainMenuNavigationButton : MonoBehaviour
    {
        [SerializeField]
        protected FadeScreenAnimator fadeUI;
        [SerializeField]
        protected GameObject currentGUIScreen;
        [SerializeField]
        protected GameObject targetGUIScreen;

        protected virtual void Start()
        {

        }

        public async virtual void OnClick()
        {
            await fadeUI.FadeOut(.5f);
            DoLoadTargetGUIScreen();
            await fadeUI.FadeIn(.5f);
        }

        protected void DoLoadTargetGUIScreen()
        {
            if (targetGUIScreen)
                targetGUIScreen.SetActive(true);
            currentGUIScreen.SetActive(false);
        }
    }
}
