using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Singleton;

[System.Serializable]
public class InputManager : InitializedSingleton<InputManager>
{
    [SerializeField]
    private static PlayerInput _input;
    [SerializeField, ExposeProperty]
    public static PlayerInput Input
    {
        get => _input;
        set => _input = value;
    }

    public override void Awake()
    {
        base.Awake();
        Input = FindObjectOfType<PlayerInput>(true);
    }

    public override void Initialize()
    {
        Input = FindObjectOfType<PlayerInput>(true);
    }
}
