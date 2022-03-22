using SunsetSystems.Data;
using SunsetSystems.SaveLoad;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transitions.Manager;
using UnityEngine;
using SunsetSystems.Scenes;

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
        PauseMenu menu = GetComponentInParent<PauseMenu>();
        if (StateManager.GetCurrentState().Equals(GameState.GamePaused))
            menu.ResumeGame();
        else if (StateManager.GetCurrentState().Equals(GameState.Menu))
            FindObjectOfType<GameStarter>().SwitchUiToGameplayMode();
        await _sceneLoader.LoadSavedScene();
    }

    public void EnableLoading()
    {
        Debug.Log("ActionTest");
    }    
}
