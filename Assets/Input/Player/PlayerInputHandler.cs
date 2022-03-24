using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using SunsetSystems.Management;
using Entities.Characters;
using Entities;
using SunsetSystems.Formation;
using InsaneSystems.RTSSelection;
using UnityEngine.AI;
using SunsetSystems.Formation.Data;
using SunsetSystems.Formation.UI;

public class PlayerInputHandler : InputHandler
{
    private const int raycastRange = 100;

    private PlayerControlledCharacter player;
    private Vector2 mousePosition;
    private Collider lastHit;
    [SerializeField]
    private LineRenderer lineOrigin;

    public LayerMask defaultRaycastMask;

    private TurnCombatManager turnCombatManager;

    public static FormationData FormationData { get; set; }
    [SerializeField]
    private PredefinedFormation defaultFormation;

    private void Awake()
    {
        if (FormationData == null)
            FormationData = defaultFormation.GetData();
    }

    // Start is called before the first frame update
    private void Start()
    {
        player = FindObjectOfType<PlayerControlledCharacter>();
        if (player == null)
            return;
        if (lineOrigin == null)
            lineOrigin = player.GetComponentInChildren<LineRenderer>(true);
        turnCombatManager = FindObjectOfType<TurnCombatManager>();
    }

    public void OnRightClick(InputAction.CallbackContext context)
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
            switch (StateManager.GetCurrentState())
            {
                case GameState.Combat:
                    {
                        HandleCombatMouseClick(hit);
                        break;
                    }
                case GameState.Exploration:
                    {
                        PlayerControlledCharacter currentLead = References
                            .Get<Selection>()
                            .GetAllSelected()[0]
                            .GetCreature() as PlayerControlledCharacter;
                        if (Entity.IsInteractable(hit.collider.gameObject))
                        {
                            currentLead.ClearAllActions();
                            currentLead.InteractWith(hit.collider.gameObject.GetComponent<IInteractable>());
                        }
                        else
                        {
                            MoveCurrentSelectionToPositions(hit);
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
                if (!turnCombatManager.IsActiveActorPlayerControlled() && !DevMoveActorToPosition.InputOverride)
                    return;
                else if (hit.collider.GetComponent<GridElement>())
                {
                    if (hit.collider.gameObject.GetComponent<GridElement>().Visited != GridElement.Status.Occupied)
                    {
                        turnCombatManager.CurrentActiveActor.Move(hit.collider.gameObject.GetComponent<GridElement>());
                    }
                }
                break;
            case BarAction.ATTACK:
                Debug.Log("Mouse clicked in attack mode!");
                if (!turnCombatManager.IsActiveActorPlayerControlled() || player.GetComponent<CombatBehaviour>().HasActed)
                    return;
                NPC enemy = hit.collider.GetComponent<NPC>();
                if (enemy)
                {
                    if (enemy.Data.Faction.Equals(Faction.Hostile) && Vector3.Distance(player.transform.position, enemy.transform.position) <= player.GetComponent<StatsManager>().GetWeaponMaxRange())
                    {
                        turnCombatManager.CurrentActiveActor.Attack(enemy);
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
            switch (StateManager.GetCurrentState())
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
                if (!turnCombatManager.IsActiveActorPlayerControlled() && !DevMoveActorToPosition.InputOverride)
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
                if (!turnCombatManager.IsActiveActorPlayerControlled() || player.GetComponent<CombatBehaviour>().HasActed)
                    return;
                if (lastHit != hit.collider)
                {
                    lineOrigin.enabled = false;
                    lastHit = hit.collider;
                }
                NPC creature = lastHit.GetComponent<NPC>();
                if (creature)
                {
                    lineOrigin.positionCount = 2;
                    lineOrigin.SetPosition(0, lineOrigin.transform.position);
                    lineOrigin.SetPosition(1, creature.LineTarget.position);
                    Color color = player.GetComponent<StatsManager>()
                        .GetWeaponMaxRange() >= Vector3.Distance(player.CurrentGridPosition.transform.position, creature.CurrentGridPosition.transform.position) 
                        ? Color.green 
                        : Color.red;
                    lineOrigin.startColor = color;
                    lineOrigin.endColor = color;
                    lineOrigin.enabled = true;
                }
                break;
        }
    }

    private void MoveCurrentSelectionToPositions(RaycastHit hit)
    {
        Vector3 samplingPoint;
        List<ISelectable> allSelected = References.Get<Selection>().GetAllSelected();
        for (int i = 0; i < allSelected.Count; i++)
        {
            Vector3 positionOffset = FormationData.positions[i];
            samplingPoint = hit.point + positionOffset;
            NavMesh.SamplePosition(samplingPoint, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas);
            MoveSelectableToPosition(allSelected[i], navHit.position);
        }
    }

    private void MoveSelectableToPosition(ISelectable selectable, Vector3 position)
    {
        Creature creature = selectable.GetCreature();
        creature.ClearAllActions();
        creature.Move(position);
    }

    public static List<Vector3> GetPositionsFromPoint(Vector3 point)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < 6; i++)
        {
            Vector3 positionOffset = FormationData.positions[i];
            Vector3 position = point + positionOffset;
            positions.Add(position);
        }
        return positions;
    }
}
