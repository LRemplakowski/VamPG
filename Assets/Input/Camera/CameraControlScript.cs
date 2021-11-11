using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Singleton;

public class CameraControlScript : Singleton<CameraControlScript>
{
    private Transform target;
    public Vector3 offset;

    //Movement variables
    private const float internalMoveTargetSpeed = 8.0f;
    private const float internalMoveSpeed = 4.0f;
    private Vector3 moveTarget;
    private Vector3 moveDirection;

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

    private void Start()
    {
        target = GameManager.GetPlayer().transform;
        transform.position = target.position + offset;
        moveTarget = transform.position;
    }

    private void FixedUpdate()
    {
        moveTarget += moveDirection * Time.fixedDeltaTime * internalMoveTargetSpeed;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, moveTarget, Time.deltaTime * internalMoveSpeed);
    }
}
