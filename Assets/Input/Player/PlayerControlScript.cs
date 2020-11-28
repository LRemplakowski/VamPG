using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerControlScript : MonoBehaviour
{
    private const int raycastRange = 100;

    private Player player;
    private Vector2 mousePosition;
    private Collider lastHit;

    public LayerMask movementMask;



    public void OnClick(InputAction.CallbackContext context)
    {
        //Debug.Log(player);
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastRange))
        {
            if(Entity.IsInteractable(hit.collider.gameObject))
            {
                player.InteractWith(hit.collider.gameObject.GetComponent<IInteractable>());
            }
            else if (Physics.Raycast(ray, out hit, raycastRange, movementMask))
            {
                player.Move(hit.point);
            }
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, raycastRange))
        {
            //Debug.Log("Ray hit");
            if(lastHit == null)
            {
                lastHit = hit.collider;
            }
            if(lastHit != hit.collider)
            {
                if(Entity.IsInteractable(lastHit.gameObject))
                {
                    lastHit.gameObject.GetComponent<IInteractable>().IsHoveredOver = false;
                }
                lastHit = hit.collider;
            }
            if(Entity.IsInteractable(lastHit.gameObject))
            {
                lastHit.gameObject.GetComponent<IInteractable>().IsHoveredOver = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = this.GetComponent<Player>();
    }

}
