using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InputManager : MonoBehaviour
{
    [SerializeField]
    private static PlayerInput _input;
    [SerializeField, ExposeProperty]
    public static PlayerInput Input
    {
        get => _input;
        set => _input = value;
    }

    private void Awake()
    {
        Input = FindObjectOfType<PlayerInput>(true);
    }
}
