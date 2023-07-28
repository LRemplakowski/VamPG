using SunsetSystems.Core.SceneLoading.UI;
using UnityEngine;

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
