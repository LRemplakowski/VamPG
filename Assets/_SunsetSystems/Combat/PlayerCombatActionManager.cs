using System;
using Sirenix.OdinInspector;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Abilities;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public class PlayerCombatActionManager : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private CombatManager combatManager;

        [field: ShowInInspector, ReadOnly]
        public SelectedCombatActionData SelectedActionData { get; private set; }

        private Collider gridHit, targetableHit;

        public void OnCombatRoundBegin(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                this.SelectedActionData = new(CombatActionType.Move);
                HandleNewSelectedAction(SelectedActionData);
            }
        }

        public void OnFullTurnCompleted(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                this.SelectedActionData = new(CombatActionType.Move);
                HandleNewSelectedAction(SelectedActionData);
            }
        }

        public void OnCombatRoundEnd(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                CleanupBeforeActionChange(SelectedActionData);
            }
        }

        public void OnCombatActionSelected(SelectedCombatActionData actionData)
        {
            if (SelectedActionData.ActionType != actionData.ActionType)
            {
                CleanupBeforeActionChange(SelectedActionData);
                HandleNewSelectedAction(actionData);
            }
            this.SelectedActionData = actionData;
        }

        private void CleanupBeforeActionChange(SelectedCombatActionData action)
        {
            var actionType = action.ActionType;
            switch (actionType)
            {
                case CombatActionType when (actionType & CombatActionType.Move) != 0:
                    combatManager.CurrentEncounter.GridManager.HideCellsInMovementRange();
                    break;
                case CombatActionType when (actionType & CombatActionType.RangedAtk) != 0:
                    break;
                case CombatActionType when (actionType & CombatActionType.MeleeAtk) != 0:
                    break;
                case CombatActionType when (actionType & CombatActionType.Feed) != 0:
                    break;
                case CombatActionType when (actionType & CombatActionType.Reload) != 0:
                    break;
                case CombatActionType when (actionType & CombatActionType.UseDiscipline) != 0:
                    break;
            }
        }

        private void HandleNewSelectedAction(SelectedCombatActionData action)
        {
            var actionType = action.ActionType;
            switch (actionType)
            {
                case CombatActionType when (actionType & CombatActionType.Move) != 0:
                    combatManager.CurrentEncounter.GridManager.ShowCellsInMovementRange(combatManager.CurrentActiveActor);
                    break;
                case CombatActionType when (actionType & CombatActionType.RangedAtk) != 0:
                    break;
                case CombatActionType when (actionType & CombatActionType.MeleeAtk) != 0:
                    break;
                case CombatActionType when (actionType & CombatActionType.Feed) != 0:
                    break;
                case CombatActionType when (actionType & CombatActionType.Reload) != 0:
                    break;
                case CombatActionType when (actionType & CombatActionType.UseDiscipline) != 0:
                    break;
            }
            if (action.ExecuteImmediate)
                ExecuteAction(action);
        }

        public void ExecuteAction(SelectedCombatActionData action)
        {
            var actionFlag = action.ActionType;
            switch (actionFlag)
            {
                case CombatActionType when (actionFlag & CombatActionType.Move) != 0:
                    HandleMoveCombatAction();
                    break;
                case CombatActionType when (actionFlag & CombatActionType.RangedAtk) != 0:
                    HandleRangedAttackCombatAction();
                    break;
                case CombatActionType when (actionFlag & CombatActionType.MeleeAtk) != 0:
                    HandleMeleeAttackCombatAction();
                    break;
                case CombatActionType when (actionFlag & CombatActionType.Feed) != 0:
                    HandleFeedCombatAction();
                    break;
                case CombatActionType when (actionFlag & CombatActionType.Reload) != 0:
                    HandleReloadCombatAction();
                    break;
                case CombatActionType when (actionFlag & CombatActionType.UseDiscipline) != 0:
                    HandleUseDisciplineCombatAction();
                    break;
            }

            void HandleMoveCombatAction()
            {
                if (gridHit != null && gridHit.TryGetComponent<IGridCell>(out var gridCell))
                {
                    throw new NotImplementedException();
                    //ICombatant currentCombatant = CombatManager.Instance.CurrentActiveActor;
                    //if (gridCell.IsFree && currentCombatant.HasMoved is false)
                    //{
                    //    if (currentCombatant.MoveToGridPosition(gridCell.GridPosition))
                    //        CombatManager.Instance.CurrentEncounter.GridManager.HideCellsInMovementRange();
                    //}
                }
            }

            void HandleRangedAttackCombatAction()
            {
                if (targetableHit != null)
                {
                    throw new NotImplementedException();
                    //ICombatant attackTarget = targetableHit.gameObject.GetComponentInParent<ICreature>()?.References.CombatBehaviour;
                    //if (attackTarget != null && attackTarget.IsAlive)
                    //{
                    //    var currentActor = CombatManager.Instance.CurrentActiveActor;
                    //    if (currentActor.WeaponManager.GetSelectedWeapon().WeaponType is Inventory.AbilityRange.Ranged)
                    //    {
                    //        currentActor.AttackCreatureUsingCurrentWeapon(attackTarget);
                    //    }
                    //}
                }
            }

            void HandleMeleeAttackCombatAction()
            {
                if (targetableHit != null)
                {
                    throw new NotImplementedException();
                    //ICombatant attackTarget = targetableHit.gameObject.GetComponentInParent<ICreature>()?.References.CombatBehaviour;
                    //if (attackTarget != null && attackTarget.IsAlive)
                    //{
                    //    var currentActor = CombatManager.Instance.CurrentActiveActor;
                    //    if (currentActor.WeaponManager.GetSelectedWeapon().WeaponType is Inventory.AbilityRange.Melee)
                    //    {
                    //        currentActor.AttackCreatureUsingCurrentWeapon(attackTarget);
                    //    }
                    //}
                }
            }

            void HandleFeedCombatAction()
            {
                Debug.Log("Om non nom");
            }

            void HandleReloadCombatAction()
            {
                throw new NotImplementedException();
                //var currentActor = CombatManager.Instance.CurrentActiveActor;
                //if (currentActor.WeaponManager.GetSelectedWeapon().WeaponType is Inventory.AbilityRange.Ranged)
                //{
                //    currentActor.ReloadCurrentWeapon();
                //}
            }

            void HandleUseDisciplineCombatAction()
            {
                IAbility selectedAbility = SelectedActionData.AbilityData;
                //var abilityManager = CombatManager.Instance.CurrentActiveActor.CombatContext.AbilityUser;
                if (targetableHit != null)
                {
                    ITargetable target = targetableHit.GetComponentInChildren<ITargetable>();
                    throw new NotImplementedException();
                    //abilityManager.SetCurrentTargetObject(target);
                    //if (abilityManager.GetHasValidAbilityContext(selectedAbility))
                    //{
                    //    abilityManager.ExecuteAbility(selectedAbility);
                    //}
                }
            }
        }

        public void SetLastGridHit(Collider gridCollider)
        {
            gridHit = gridCollider;
        }

        public void SetLastTargetableHit(Collider targetableCollider)
        {
            targetableHit = targetableCollider;
        }
    }

    [Serializable]
    public struct SelectedCombatActionData
    {
        [field: SerializeField]
        public bool ExecuteImmediate { get; private set; }
        [field: SerializeField]
        public CombatActionType ActionType { get; private set; }
        [field: SerializeField]
        public IAbility AbilityData { get; private set; }

        public SelectedCombatActionData(CombatActionType ActionType) : this(ActionType, null)
        {

        }

        public SelectedCombatActionData(CombatActionType ActionType, IAbility AbilityData)
        {
            this.ActionType = ActionType;
            this.AbilityData = AbilityData;
            ExecuteImmediate = false;
        }
    }

    [Flags]
    public enum CombatActionType
    {
        None = 0,
        Move = 1 << 1, 
        RangedAtk = 1 << 2, 
        MeleeAtk = 1 << 3, 
        Feed = 1 << 4, 
        Reload = 1 << 5, 
        UseDiscipline = 1 << 6
    }
}
