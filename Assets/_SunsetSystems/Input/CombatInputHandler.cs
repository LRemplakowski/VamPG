using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Creatures.Interfaces;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Spellbook;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class CombatInputHandler : SerializedMonoBehaviour, IGameplayInputHandler
    {
        [Title("References")]
        [SerializeField, Required]
        private PlayerCombatActionManager selectedActionManager;
        [Title("Config")]
        [SerializeField]
        private LayerMask gridLayerMask;
        [SerializeField]
        private LayerMask targetableLayerMask;
        private const int raycastRange = 100;
        private Vector2 mousePosition;

        private Collider gridHit;
        private Collider targetableHit;

        public void HandlePointerPosition(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            mousePosition = context.ReadValue<Vector2>();
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            CombatManager.Instance.CurrentEncounter.GridManager.ClearHighlightedCell();
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastRange, gridLayerMask))
            {
                if (gridHit == null)
                    gridHit = hit.collider;
                HandleGridCellPointerPosition();
                gridHit = hit.collider;
            }
            if (Physics.Raycast(ray, out hit, raycastRange, targetableLayerMask))
            {
                if (targetableHit == null)
                    targetableHit = hit.collider;
                HandleTargetablePointerPosition();
                targetableHit = hit.collider;
            }

            void HandleGridCellPointerPosition()
            {
                IGridCell gridCell = hit.collider.gameObject.GetComponent<IGridCell>();
                if (gridCell != null)
                {
                    CombatManager.Instance.CurrentEncounter.GridManager.HighlightCell(gridCell);
                }
            }

            void HandleTargetablePointerPosition()
            {
                ICombatant combatant = hit.collider.gameObject.GetComponent<ICreature>()?.References.CombatBehaviour;
                if (combatant != null)
                {
                    // do some target highlight
                }
            }
        }

        public void HandlePrimaryAction(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
            switch (selectedActionManager.SelectedActionData.ActionType)
            {
                case CombatActionType.Move:
                    HandleMoveCombatAction();
                    break;
                case CombatActionType.RangedAtk:
                    HandleRangedAttackCombatAction();
                    break;
                case CombatActionType.MeleeAtk:
                    HandleMeleeAttackCombatAction();
                    break;
                case CombatActionType.Feed:
                    HandleFeedCombatAction();
                    break;
                case CombatActionType.Reload:
                    HandleReloadCombatAction();
                    break;
                case CombatActionType.UseDiscipline:
                    HandleUseDisciplineCombatAction();
                    break;
            }

            void HandleMoveCombatAction()
            {
                if (gridHit != null)
                {
                    IGridCell gridCell = gridHit.gameObject.GetComponent<IGridCell>();
                    if (gridCell != null)
                    {
                        ICombatant currentCombatant = CombatManager.Instance.CurrentActiveActor;
                        if (gridCell.IsFree && currentCombatant.HasMoved is false)
                        {
                            if (currentCombatant.MoveToGridPosition(gridCell.GridPosition))
                                CombatManager.Instance.CurrentEncounter.GridManager.HideCellsInMovementRange();
                        }
                    }
                }
            }

            void HandleRangedAttackCombatAction()
            {
                if (targetableHit != null)
                {
                    ICombatant attackTarget = targetableHit.gameObject.GetComponentInParent<ICreature>()?.References.CombatBehaviour;
                    if (attackTarget != null && attackTarget.IsAlive)
                    {
                        var currentActor = CombatManager.Instance.CurrentActiveActor;
                        if (currentActor.CurrentWeapon.WeaponType is Inventory.WeaponType.Ranged)
                        {
                            currentActor.AttackCreatureUsingCurrentWeapon(attackTarget);
                        }
                    }
                }
            }

            void HandleMeleeAttackCombatAction()
            {
                if (targetableHit != null)
                {
                    ICombatant attackTarget = targetableHit.gameObject.GetComponentInParent<ICreature>()?.References.CombatBehaviour;
                    if (attackTarget != null && attackTarget.IsAlive)
                    {
                        var currentActor = CombatManager.Instance.CurrentActiveActor;
                        if (currentActor.CurrentWeapon.WeaponType is Inventory.WeaponType.Melee)
                        {
                            currentActor.AttackCreatureUsingCurrentWeapon(attackTarget);
                        }
                    }
                }
            }

            void HandleFeedCombatAction()
            {
                Debug.Log("Om non nom");
            }

            void HandleReloadCombatAction()
            {
                var currentActor = CombatManager.Instance.CurrentActiveActor;
                if (currentActor.CurrentWeapon.WeaponType is Inventory.WeaponType.Ranged)
                {
                    currentActor.ReloadCurrentWeapon();
                }
            }

            void HandleUseDisciplineCombatAction()
            {
                DisciplinePower selectedDisciplinePower = selectedActionManager.SelectedActionData.DisciplinePowerData;
                IMagicUser currentActorSpellcaster = CombatManager.Instance.CurrentActiveActor.MagicUser;
                if (targetableHit != null)
                {
                    ITargetable target = targetableHit.GetComponentInChildren<ITargetable>();
                    if (target != null && selectedDisciplinePower.IsValidTarget(target, currentActorSpellcaster))
                    {
                        currentActorSpellcaster.UsePower(selectedDisciplinePower, target);
                    }
                }
            }
        }

        public void HandleSecondaryAction(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
            switch (selectedActionManager.SelectedActionData.ActionType)
            {
                case CombatActionType.Move:
                    break;
                case CombatActionType.RangedAtk:
                    break;
                case CombatActionType.MeleeAtk:
                    break;
                case CombatActionType.Feed:
                    break;
                case CombatActionType.Reload:
                    break;
            }
        }
    }
}
