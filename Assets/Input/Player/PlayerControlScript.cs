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
    [SerializeField]
    private LineRenderer lineOrigin;

    public LayerMask defaultRaycastMask;

    public void OnClick(InputAction.CallbackContext context)
    {
        //Debug.Log(player);
        if (context.phase != InputActionPhase.Performed)
            return;
        if (EventSystem.current.IsPointerOverGameObject())
        {
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
                        HandleCombatMouseClick(hit);
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

    private void HandleCombatMouseClick(RaycastHit hit)
    {
        ActionBarUI.SelectedBarAction selectedBarAction = ActionBarUI.instance.GetSelectedBarAction();
        switch (selectedBarAction.actionType)
        {
            case BarAction.MOVE:
                Debug.Log("Mouse clicked in move mode!");
                if (!TurnCombatManager.instance.IsActiveActorPlayerControlled() && !DevMoveActorToPosition.InputOverride)
                    return;
                else if (hit.collider.GetComponent<GridElement>())
                {
                    if (hit.collider.gameObject.GetComponent<GridElement>().Visited != GridElement.Status.Occupied)
                    {
                        TurnCombatManager.instance.CurrentActiveActor.Move(hit.collider.gameObject.GetComponent<GridElement>());
                    }
                }
                break;
            case BarAction.ATTACK:
                Debug.Log("Mouse clicked in attack mode!");
                if (!TurnCombatManager.instance.IsActiveActorPlayerControlled() || player.GetComponent<CombatBehaviour>().HasActed)
                    return;
                NPC enemy = hit.collider.GetComponent<NPC>();
                if (enemy)
                {
                    if (enemy.Faction.Equals(Faction.Hostile) && Vector3.Distance(this.transform.position, enemy.transform.position) <= GetComponent<StatsManager>().GetAttackRange())
                    {
                        TurnCombatManager.instance.CurrentActiveActor.Attack(enemy);
                    }
                }
                break;
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;
        mousePosition = context.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask, QueryTriggerInteraction.Ignore))
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
                            if (Entity.IsInteractable(lastHit.gameObject))
                            {
                                lastHit.gameObject.GetComponent<IInteractable>().IsHoveredOver = false;
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
                        HandleCombatMousePosition(hit);
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

    private void HandleCombatMousePosition(RaycastHit hit)
    {
        ActionBarUI.SelectedBarAction selectedBarAction = ActionBarUI.instance.GetSelectedBarAction();
        switch (selectedBarAction.actionType)
        {
            case BarAction.MOVE:
                if (!TurnCombatManager.instance.IsActiveActorPlayerControlled() && !DevMoveActorToPosition.InputOverride)
                    return;
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
            case BarAction.ATTACK:
                if (!TurnCombatManager.instance.IsActiveActorPlayerControlled() || player.GetComponent<CombatBehaviour>().HasActed)
                    return;
                if (lastHit != hit.collider)
                {
                    lineOrigin.enabled = false;
                    lastHit = hit.collider;
                }
                NPC creature = lastHit.GetComponent<NPC>();
                if (creature)
                {
                    Vector3 origin = lineOrigin.transform.position;
                    Vector3 end = creature.LineTarget.position;
                    lineOrigin.positionCount = 2;
                    lineOrigin.SetPosition(0, lineOrigin.transform.position);
                    lineOrigin.SetPosition(1, creature.LineTarget.position);
                    //Debug.LogError((GetComponent<StatsManager>().GetAttackRange() >= Vector3.Distance(origin, end)) + "; range == " + GetComponent<StatsManager>().GetAttackRange() + "; distance = " + Vector3.Distance(origin, end));
                    Color color = GetComponent<StatsManager>().GetAttackRange() >= Vector3.Distance(origin, end) ? Color.green : Color.red;
                    lineOrigin.startColor = color;
                    lineOrigin.endColor = color;
                    lineOrigin.enabled = true;
                }
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = this.GetComponent<Player>();
        if (lineOrigin == null)
            lineOrigin = GetComponentInChildren<LineRenderer>(true);
    }

}
