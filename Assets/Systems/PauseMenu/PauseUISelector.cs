using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.UI.Pause
{
    public class PauseUISelector : MonoBehaviour
    {
        [SerializeField]
        private GameObject targetGuiScreen;
        [SerializeField]
        private static GameObject currentGuiScreen;

        public void SelectGUIScreen()
        {
            if (currentGuiScreen)
                currentGuiScreen.SetActive(false);
            if (targetGuiScreen)
                targetGuiScreen.SetActive(true);
            currentGuiScreen = targetGuiScreen;
        }
    }
}
