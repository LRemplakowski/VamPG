using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunsetSystems.Loading;
using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.MainMenu.UI
{
    public class MainMenuNavigationButton : MonoBehaviour
    {
        protected SceneLoadingUIManager fadeUI;
        [SerializeField]
        protected GameObject currentGUIScreen;
        [SerializeField]
        protected GameObject targetGUIScreen;

        protected virtual void Start()
        {
            if (fadeUI == null)
                if (this.TryFindFirstGameObjectWithTag(TagConstants.SCENE_LOADING_UI, out GameObject fadeUIObject))
                    fadeUI = fadeUIObject.GetComponent<SceneLoadingUIManager>();
        }

        public async virtual void OnClick()
        {
            await fadeUI.DoFadeOutAsync(.5f);
            DoLoadTargetGUIScreen();
            await fadeUI.DoFadeInAsync(.5f);
        }

        protected void DoLoadTargetGUIScreen()
        {
            if (targetGUIScreen)
                targetGUIScreen.SetActive(true);
            currentGUIScreen.SetActive(false);
        }
    }
}
