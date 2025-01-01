using SunsetSystems.Core;
using UnityEngine;

namespace SunsetSystems.MainMenu
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject debugUi;

        // Start is called before the first frame update
        private void Start()
        {
            if (debugUi)
                debugUi.SetActive(GameConstants.DEBUG_MODE);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
