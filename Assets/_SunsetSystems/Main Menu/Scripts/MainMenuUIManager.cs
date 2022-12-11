using SunsetSystems.Audio;
using SunsetSystems.Constants;
using SunsetSystems.Data;
using SunsetSystems.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.MainMenu
{
    [RequireComponent(typeof(Tagger))]
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

        public void StartGameJam()
        {
            GameStarter.Instance.InitializeGameJam();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
