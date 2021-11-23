using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Singleton;

[System.Serializable]
public class InputManager : ExposableMonobehaviour
{
    [SerializeField]
    private static PlayerInput _input;
    [SerializeField, ExposeProperty]
    public static PlayerInput Input
    {
        get => _input;
        set => _input = value;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        Input = FindObjectOfType<PlayerInput>(true);
    }
}
