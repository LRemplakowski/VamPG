using SunsetSystems.Constants;
using SunsetSystems.Data;
using SunsetSystems.LevelManagement;
using SunsetSystems.Utils;
using UnityEngine;

namespace SunsetSystems.MainMenu
{
    [RequireComponent(typeof(Tagger))]
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject debugUi;
        [SerializeField]
        private GameStarter gameStarter;

        private void OnEnable()
        {
            LevelLoader.OnBeforeLevelLoad += OnBeforeLevelLoad;
        }

        private void OnDisable()
        {
            LevelLoader.OnBeforeLevelLoad -= OnBeforeLevelLoad;
        }

        // Start is called before the first frame update
        private void Start()
        {
            if (debugUi)
                debugUi.SetActive(GameConstants.DEBUG_MODE);
        }

        private void OnBeforeLevelLoad(LevelLoadingEventData data)
        {
            gameObject.SetActive(false);
        }

        public void StartGameDebug()
        {
            gameStarter.InitializeGameDebug();
        }

        public void StartGameJam()
        {
            gameStarter.InitializeGameJam();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
