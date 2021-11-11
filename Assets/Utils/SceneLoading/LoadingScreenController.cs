using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Transitions.Fade;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField]
    private Slider loadingBar;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private TextMeshProUGUI loadingBarText;
    [SerializeField]
    private Button continueButton;

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
        //loadingBar.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
        loadingBar.onValueChanged.AddListener(MaybeReplaceBarWithButton);
    }

    private void OnDisable()
    {
        loadingBar.onValueChanged.RemoveListener(MaybeReplaceBarWithButton);
    }

    public void SetLoadingProgress(float value)
    {
        loadingBar.value = value;
        loadingBarText.text = value * 100f + " %";
    }

    private void MaybeReplaceBarWithButton(float value)
    {
        if (value >= 1.0f)
        {
            //loadingBar.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
        }
    }

    public void OnContinue()
    {
        TransitionManager.Instance.PerformTransition(null);
    }

}
