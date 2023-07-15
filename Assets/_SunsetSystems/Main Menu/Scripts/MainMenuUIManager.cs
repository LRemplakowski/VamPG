using SunsetSystems.Constants;
using SunsetSystems.Data;
using SunsetSystems.Persistence;
using SunsetSystems.Utils;
using UnityEngine;

namespace SunsetSystems.MainMenu
{
    [RequireComponent(typeof(Tagger))]
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject debugUi;

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
