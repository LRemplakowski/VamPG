using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerControlScript : MonoBehaviour
{
    private Player player;
    private Vector2 mousePosition;
    private Collider lastHit;

    public LayerMask movementMask;



    public void OnClick(InputAction.CallbackContext context)
    {
        //Debug.Log(player);
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, movementMask))
        {
            Debug.Log("Move hit");
            player.Move(hit.point);
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100))
        {
            Debug.Log("Ray hit");
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
