using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControlScript : MonoBehaviour
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
        Debug.Log("Camera OnMove");
        if (!(context.performed || context.canceled))
            return;
        Vector2 value = context.ReadValue<Vector2>();
        Debug.Log(value.x + " " + value.y);
        //Camera is offset relative to world X & Y axis, hence we adjust direction based on input
        if(value.x == 0 && value.y == 0)
        {
            //Debug.Log("Vector 0");
            moveDirection = Vector3.zero;
        }
        else if(value.y == 1 || value.y == -1)
        {
            //Debug.Log("Vector forward or backward");
            moveDirection = new Vector3(value.y, 0, value.y);
        }
        else if(value.x == 1 || value.x == -1)
        {
            //Debug.Log("Move left or right");
            moveDirection = new Vector3(value.x, 0, -value.x);
        }
        else if(value.x == value.y)
        {
            //Debug.Log("Move diagonal up right/down left");
            moveDirection = new Vector3(value.x+value.y, 0, 0);
        }
        else
        {
            //Debug.Log("Move diagonal up left/down right");
            moveDirection = new Vector3(0, 0, -value.x + value.y);
        }
    }

    private void Start()
    {
        target = GameManager.GetPlayer().transform;
        transform.position = target.position + offset;
        moveTarget = transform.position;
    }

    private void FixedUpdate()
    {
        moveTarget -= moveDirection * Time.fixedDeltaTime * internalMoveTargetSpeed;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, moveTarget, Time.deltaTime * internalMoveSpeed);
    }
}
