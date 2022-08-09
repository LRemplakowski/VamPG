using SunsetSystems.Data;
using UnityEngine;
using SunsetSystems.Loading;
using System;
using SunsetSystems.Game;
using SunsetSystems.UI;

namespace SunsetSystems.Loading
{
    public class DebugSaveLoadButtonScript : MonoBehaviour
    {
        [SerializeField]
        private SceneLoader _sceneLoader;

        private void Start()
        {
            if (!_sceneLoader)
                _sceneLoader = FindObjectOfType<SceneLoader>();
        }

        public void DoSave()
        {
            Debug.Log("DoSave button");
            SaveLoadManager.Save();
        }

        public async void DoLoad()
        {
            Debug.Log("DoLoad button");
            PauseMenuUI menu = GetComponentInParent<PauseMenuUI>();
            Action action = null;
            if (GameManager.Instance.IsCurrentState(GameState.GamePaused))
                menu.ResumeGame();
            else if (GameManager.Instance.IsCurrentState(GameState.Menu))
                action = FindObjectOfType<GameStarter>().DisableMainMenu;
            await _sceneLoader.LoadSavedScene(action);
        }

        public void EnableLoading()
        {
            Debug.Log("ActionTest");
        }
    }
}
