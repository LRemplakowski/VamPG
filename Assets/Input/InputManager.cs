using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private CameraControlScript cameraControl;
    private PlayerControlScript playerControl;
    private PlayerInputMapping _input;

    void Awake()
    {
        _input = new PlayerInputMapping();

        if(cameraControl == null)
        {
            cameraControl = FindObjectOfType<CameraControlScript>();

            _input.Player.CameraMove.performed += cameraControl.OnMove;
            _input.Player.CameraMove.canceled += cameraControl.OnMove;

            //_input.Player.CameraRotate.performed += cameraControl.OnRotate;
            //_input.Player.CameraRotate.canceled += cameraControl.OnRotate;

            //_input.Player.CameraRotateToggle.performed += cameraControl.OnRotateToggle;
            //_input.Player.CameraRotateToggle.canceled += cameraControl.OnRotateToggle;

            //_input.Player.CameraZoom.performed += cameraControl.OnZoom;
        }
        if(playerControl == null)
        {
            playerControl = FindObjectOfType<PlayerControlScript>();

            _input.Player.Click.performed += playerControl.OnClick;
            _input.Player.MousePosition.performed += playerControl.OnMousePosition;
        }
    }

    void OnEnable()
    {
        //Enable all actions under the Player action map
        // You can do this on an individual action level by calling _input.Player.CameraMove.Enable() instead.
        _input.Player.Enable();
    }

    void OnDisable()
    {
        //Disable all actions under the Player action map
        _input.Player.Disable();
    }
}
