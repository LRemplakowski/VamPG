using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Player))]
public class PlayerControlScript : InputHandler
{
    private const int raycastRange = 100;

    private Player player;
    private Vector2 mousePosition;
    private Collider lastHit;

    public LayerMask defaultRaycastMask;



    public void OnClick(InputAction.CallbackContext context)
    {
        //Debug.Log(player);
        if (context.phase != InputActionPhase.Performed)
            return;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Pointer over game object");
            return;
        }
        ManageInput(HandleWorldClick);
    }

    private void HandleWorldClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask))
        {
            switch (StateManager.instance.GetCurrentState())
            {
                case GameState.Combat:
                    {
                        if (GridElement.IsInstance(hit.collider.gameObject))
                        {
                            player.Move(hit.collider.gameObject.transform.position);
                        }
                        break;
                    }
                case GameState.Exploration:
                    {
                        if (Entity.IsInteractable(hit.collider.gameObject))
                        {
                            player.InteractWith(hit.collider.gameObject.GetComponent<IInteractable>());
                        }
                        else
                        {
                            player.Move(hit.point);
                        }
                        break;
                    }
                case GameState.Conversation:
                    {
                        break;
                    }
                default:
                    break;
            }
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;
        mousePosition = context.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask))
        {
            //Debug.Log("Ray hit");
            if (lastHit == null)
            {
                lastHit = hit.collider;
            }
            switch (StateManager.instance.GetCurrentState())
            {
                case GameState.Exploration:
                    {
                        if (lastHit != hit.collider)
                        {
                            if (GridElement.IsInstance(lastHit.gameObject))
                            {
                                lastHit.gameObject.GetComponent<GridElement>().MouseOver = false;
                            }
                            lastHit = hit.collider;
                        }
                        if (Entity.IsInteractable(lastHit.gameObject))
                        {
                            lastHit.gameObject.GetComponent<IInteractable>().IsHoveredOver = true;
                        }
                        break;
                    }  
                case GameState.Combat:
                    {
                        if (lastHit != hit.collider)
                        {
                            if (GridElement.IsInstance(lastHit.gameObject))
                            {
                                lastHit.gameObject.GetComponent<GridElement>().MouseOver = false;
                            }
                            lastHit = hit.collider;
                        }
                        if (GridElement.IsInstance(lastHit.gameObject))
                        {
                            lastHit.gameObject.GetComponent<GridElement>().MouseOver = true;
                        }
                        break;
                    }
                case GameState.Conversation:
                    {
                        break;
                    }
                default:
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = this.GetComponent<Player>();
    }

}
