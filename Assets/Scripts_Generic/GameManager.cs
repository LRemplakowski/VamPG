using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static Player player;
    private static Creature currentActiveActor;
    private static GridController gridController;

    #region Singleton
    public static GameManager instance;
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
        player = FindObjectOfType<Player>();
        gridController = FindObjectOfType<GridController>();
        currentActiveActor = player;
    }

    public static Player GetPlayer()
    {
        return player;
    }

    public static Creature GetCurrentActiveActor()
    {
        return currentActiveActor;
    }

    public static GridController GetGridController()
    {
        return gridController;
    }
}
