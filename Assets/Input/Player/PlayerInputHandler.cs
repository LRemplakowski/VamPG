using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Entities.Characters;
using Entities;
using InsaneSystems.RTSSelection;
using UnityEngine.AI;
using SunsetSystems.Formation.Data;
using SunsetSystems.Formation.UI;
using SunsetSystems.Utils;

[RequireComponent(typeof(Tagger))]
public class PlayerInputHandler : Singleton<PlayerInputHandler>
{
    private const int raycastRange = 100;

    private PlayerControlledCharacter player;
    private Vector2 mousePosition;
    private Collider lastHit;
    [SerializeField]
    private LineRenderer lineOrigin;
    [SerializeField]
    private LayerMask defaultRaycastMask;
    [SerializeField]
    private PlayerInput playerInput;

    private TurnCombatManager turnCombatManager;

    public static FormationData FormationData { get; set; }
    [SerializeField]
    private PredefinedFormation defaultFormation;
    [SerializeField]
    private float _followerStoppingDistance = 1.0f;

    public delegate void OnLeftClickHandler();
    public event OnLeftClickHandler OnLeftClickEvent;
    public delegate void OnRightClickHandler();
    public event OnRightClickHandler OnRightClickEvent;
    public delegate void OnMousePositionHandler(Vector2 mousePosition);
    public event OnMousePositionHandler OnMousePositionEvent;

    protected override void Awake()
    {
        base.Awake();
        if (FormationData == null)
            FormationData = defaultFormation.GetData();
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerControlledCharacter>();
        if (player == null)
            return;
        if (lineOrigin == null)
            lineOrigin = player.GetComponentInChildren<LineRenderer>(true);
        turnCombatManager = FindObjectOfType<TurnCombatManager>();
    }

    public void SetPlayerInputActive(bool active)
    {
        if (active)
        {
            playerInput.ActivateInput();
            playerInput.SwitchCurrentActionMap("Player");
        }
        else
        {
            playerInput.DeactivateInput();
            playerInput.SwitchCurrentActionMap("UI");
        }
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        Pointer device = playerInput.GetDevice<Pointer>();
        if (device != null && IsRaycastHittingUIObject(device.position.ReadValue()))
            return;
        OnLeftClickEvent?.Invoke();
    }

    private bool IsRaycastHittingUIObject(Vector2 position)
    {
        if (m_PointerData == null)
            m_PointerData = new PointerEventData(EventSystem.current);
        m_PointerData.position = position;
        EventSystem.current.RaycastAll(m_PointerData, m_RaycastResults);
        return m_RaycastResults.Count > 0;
    }

    private PointerEventData m_PointerData;
    private List<RaycastResult> m_RaycastResults = new();

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        Pointer device = playerInput.GetDevice<Pointer>();
        if (device != null && IsRaycastHittingUIObject(device.position.ReadValue()))
            return;
        OnRightClickEvent?.Invoke();
        HandleWorldClick();
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
                        List<ISelectable> selectables = Selection.Instance.GetAllSelected();
                        PlayerControlledCharacter currentLead;
                        if (selectables.Count > 0)
                        {
                            currentLead = selectables[0].GetCreature() as PlayerControlledCharacter;
                        }
                        else
                        {
                            break;
                        }
                        if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                        {
                            currentLead.ClearAllActions();
                            currentLead.InteractWith(interactable);
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
                if (!turnCombatManager.IsActiveActorPlayerControlled() || player.GetComponent<CombatBehaviour>().HasActed)
                    return;
                NPC enemy = hit.collider.GetComponent<NPC>();
                if (enemy)
                {
                    if (enemy.Data.Faction.Equals(Faction.Hostile) &&
                        Vector3.Distance(player.transform.position, enemy.transform.position) <= player.GetComponent<StatsManager>().GetWeaponMaxRange())
                    {
                        turnCombatManager.CurrentActiveActor.Attack(enemy);
                    }
                }
                break;
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        mousePosition = context.ReadValue<Vector2>();
        OnMousePositionEvent?.Invoke(mousePosition);
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask, QueryTriggerInteraction.Ignore))
        {
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
        List<ISelectable> allSelected = Selection.Instance.GetAllSelected();
        float stoppingDistance = 0f;
        for (int i = 0; i < allSelected.Count; i++)
        {
            samplingPoint = hit.point;
            NavMesh.SamplePosition(samplingPoint, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas);
            stoppingDistance += (i % 2) * _followerStoppingDistance;
            MoveSelectableToPosition(allSelected[i], navHit.position, stoppingDistance);
        }
    }

    private void MoveSelectableToPosition(ISelectable selectable, Vector3 position, float stoppingDistance)
    {
        Creature creature = selectable.GetCreature();
        creature.Move(position, stoppingDistance);
    }

    public static List<Vector3> GetPositionsFromPoint(Vector3 point)
    {
        List<Vector3> positions = new();
        for (int i = 0; i < 6; i++)
        {
            Vector3 positionOffset = FormationData.positions[i];
            Vector3 position = point + positionOffset;
            positions.Add(position);
        }
        return positions;
    }
}
