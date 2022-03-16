using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Canvas pauseMenuCanvas;
    [SerializeField]
    private GUIWindowsManager windowsManager;
    public static bool IsGamePaused { get; private set; }

    private void Awake()
    {
        if (pauseMenuCanvas == null)
            pauseMenuCanvas = GetComponent<Canvas>();
        if (windowsManager == null)
            windowsManager = FindObjectOfType<GUIWindowsManager>();
    }
    public void PauseGame()
    {
        if (windowsManager.ActiveWindows <= 0)
        {
            Time.timeScale = 0;
            IsGamePaused = true;
            gameObject.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        IsGamePaused = false;
        gameObject.SetActive(false);
    }
}
