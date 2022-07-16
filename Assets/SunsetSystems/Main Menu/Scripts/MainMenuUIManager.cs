using SunsetSystems.Constants;
using SunsetSystems.Data;
using UnityEngine;

namespace SunsetSystems.MainMenu
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject debugUi;

        // Start is called before the first frame update
        void Start()
        {
            if (debugUi)
                debugUi.SetActive(GameConstants.DEBUG_MODE);
        }

        public void StartGameDebug()
        {
            GameStarter.Instance.InitializeGameDebug();
        }
    }
}
