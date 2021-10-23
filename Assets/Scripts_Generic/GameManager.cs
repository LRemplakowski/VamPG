﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : ExposableMonobehaviour
{
    private static Player player;
    private static GridController gridController;

    #region Singleton
    private static GameManager instance;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Singleton();
    }
    private void Singleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gridController = FindObjectOfType<GridController>();
    }

    public static Player GetPlayer()
    {
        if (player == null)
            player = FindObjectOfType<Player>();
        return player;
    }

    public static GridController GetGridController()
    {
        return gridController;
    }

    public static string GetLanguage()
    {
        return "PL";
    }    
}
