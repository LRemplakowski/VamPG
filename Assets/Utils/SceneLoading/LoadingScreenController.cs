using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Transitions.Manager;
using SunsetSystems.Management;
using System;
using System.Threading.Tasks;

public class LoadingScreenController : MonoBehaviour
{
    private const string LOADING_SCREEN_TAG = "LoadingScreen";

    [SerializeField]
    private Slider loadingBar;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private TextMeshProUGUI loadingBarText;
    [SerializeField]
    private Button continueButton;

    private FadeScreenAnimator transitionAnimator;

    private void OnEnable()
    {
        if (loadingBar == null)
            loadingBar = GetComponentInChildren<Slider>();
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        if (loadingBarText == null)
            loadingBarText = GetComponentInChildren<TextMeshProUGUI>();
        if (continueButton == null)
            continueButton = GetComponentInChildren<Button>();
        loadingBar.value = 0f;
        continueButton.gameObject.SetActive(false);
        loadingBar.onValueChanged.AddListener(MaybeReplaceBarWithButton);
        StateManager.SetCurrentState(GameState.GamePaused);
    }

    private void OnDisable()
    {
        loadingBar.onValueChanged.RemoveListener(MaybeReplaceBarWithButton);
    }

    private void Start()
    {
        transitionAnimator = FindObjectOfType<FadeScreenAnimator>(true);
    }

    public void SetUnloadingProgress(float value)
    {
        loadingBar.value = value / 2;
        loadingBarText.text = value * 50f + " %";
    }

    public void SetLoadingProgress(float value)
    {
        loadingBar.value = (value / 2) + .5f;
        loadingBarText.text = ((value * 50f) + 50f) + " %";
    }

    private void MaybeReplaceBarWithButton(float value)
    {
        if (value >= 1.0f)
        {
            continueButton.gameObject.SetActive(true);
        }
    }

    public async void OnContinue()
    {
        await transitionAnimator.FadeOut(.5f);
        await DisableLoadingScreen();
    }

    private async Task DisableLoadingScreen()
    {
        this.gameObject.SetActive(false);
        await transitionAnimator.FadeIn(.5f);
        StateManager.SetCurrentState(GameState.Exploration);
    }
}
