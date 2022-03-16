using Entities.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControlScript : ExposableMonobehaviour
{
    private Transform target;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Transform rotationTarget;
    public Vector3 offset;

    //Movement variables
    private const float internalMoveTargetSpeed = 8.0f;
    [SerializeField]
    private float cameraMoveSpeed = 4.0f, cameraRotationSpeed = 15f;
    private Vector3 moveTarget;
    private Vector3 moveDirection;
    private float rotationDirection;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!(context.performed || context.canceled))
            return;
        Vector2 value = context.ReadValue<Vector2>();
        if(value.x == 0 && value.y == 0)
        {
            moveDirection = Vector3.zero;
        }
        moveDirection = new Vector3(value.x, 0, value.y);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!(context.performed || context.canceled))
            return;
        rotationDirection = -context.ReadValue<Vector2>().x;
    }

    private void OnEnable()
    {
        MainCharacter.onMainCharacterInitialized += Initialize;
    }

    private void OnDisable()
    {
        MainCharacter.onMainCharacterInitialized -= Initialize;
    }

    public void Initialize()
    {
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;
        target = GameManager.GetMainCharacter().transform;
        if (target)
        {
            transform.position = target.position;
            moveTarget = transform.position;
            cameraTransform.localPosition = offset;
            cameraTransform.LookAt(rotationTarget);
        }
    }

    private void FixedUpdate()
    {
        if (moveDirection.z != 0)
            moveTarget += transform.forward * moveDirection.z * Time.fixedDeltaTime * internalMoveTargetSpeed;
        if (moveDirection.x != 0)
            moveTarget += transform.right * moveDirection.x * Time.fixedDeltaTime * internalMoveTargetSpeed;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, moveTarget, Time.deltaTime * cameraMoveSpeed);
        transform.Rotate(Vector3.up * Time.deltaTime * cameraRotationSpeed * rotationDirection);
    }
}
