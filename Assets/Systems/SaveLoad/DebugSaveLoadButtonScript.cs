using SunsetSystems.GameData;
using SunsetSystems.SaveLoad;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transitions.Manager;
using UnityEngine;

public class DebugSaveLoadButtonScript : MonoBehaviour
{
    [SerializeField]
    private FadeScreenAnimator fadeScreenAnimator;

    private void Start()
    {
        if (!fadeScreenAnimator)
            fadeScreenAnimator = FindObjectOfType<FadeScreenAnimator>(true);
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
        await fadeScreenAnimator.FadeOut(.5f);
        if (StateManager.GetCurrentState().Equals(GameState.GamePaused))
            menu.ResumeGame();
        else if (StateManager.GetCurrentState().Equals(GameState.MainMenu))
            FindObjectOfType<GameStarter>().SwitchUIAndInputToGameplayMode();
        await fadeScreenAnimator.FadeIn(.5f);
        _ = SaveLoadManager.Load();
    }
}
