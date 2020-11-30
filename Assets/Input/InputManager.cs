using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static PlayerInput _input;
    public static PlayerInput Input
    {
        get => _input;
        set => _input = value;
    }

    private void Awake()
    {
        Input = FindObjectOfType<PlayerInput>();
    }
}
