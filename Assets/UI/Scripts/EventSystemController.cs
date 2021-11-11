using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Singleton;

public class EventSystemController : InitializedSingleton<EventSystemController>
{
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public override void Initialize()
    {
        Player player = GameManager.GetPlayer();
        CameraControlScript cameraRig = FindObjectOfType<CameraControlScript>();
        GUIWindowsManager windowsManager = FindObjectOfType<GUIWindowsManager>();
    }
}
