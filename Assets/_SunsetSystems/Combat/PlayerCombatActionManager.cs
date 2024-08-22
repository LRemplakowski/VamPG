using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Spellbook;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public class PlayerCombatActionManager : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private CombatManager combatManager;

        [field: ShowInInspector, ReadOnly]
        public SelectedCombatActionData SelectedActionData { get; private set; }

        public void OnCombatRoundBegin(ICombatant actor)
        {
            if (actor.Faction == Faction.PlayerControlled)
            {
                this.SelectedActionData = new(CombatActionType.Move);
                HandleNewSelectedAction(SelectedActionData.ActionType);
            }
        }

        public void OnFullTurnCompleted(ICombatant actor)
        {
            if (actor.Faction == Faction.PlayerControlled)
            {
                this.SelectedActionData = new(CombatActionType.Move);
                HandleNewSelectedAction(SelectedActionData.ActionType);
            }
        }

        public void OnCombatRoundEnd(ICombatant actor)
        {
            if (actor.Faction == Faction.PlayerControlled)
            {
                CleanupBeforeActionChange(SelectedActionData.ActionType);
            }
        }

        public void OnCombatActionSelected(SelectedCombatActionData actionData)
        {
            if (SelectedActionData.ActionType != actionData.ActionType)
            {
                CleanupBeforeActionChange(SelectedActionData.ActionType);
                HandleNewSelectedAction(actionData.ActionType);
            }
            this.SelectedActionData = actionData;
        }

        private void CleanupBeforeActionChange(CombatActionType actionType)
        {
            switch (actionType)
            {
                case CombatActionType.Move:
                    combatManager.CurrentEncounter.GridManager.HideCellsInMovementRange();
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

        private void HandleNewSelectedAction(CombatActionType actionType)
        {
            switch (actionType)
            {
                case CombatActionType.Move:
                    combatManager.CurrentEncounter.GridManager.ShowCellsInMovementRange(combatManager.CurrentActiveActor);
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

    [Serializable]
    public struct SelectedCombatActionData
    {
        [field: SerializeField]
        public bool ExecuteImmediate { get; private set; }
        [field: SerializeField]
        public CombatActionType ActionType { get; private set; }
        [field: SerializeField, ShowIf("@this.ActionType == CombatActionType.UseDiscipline")]
        public DisciplinePower DisciplinePowerData { get; private set; }

        public SelectedCombatActionData(CombatActionType ActionType) : this(ActionType, null)
        {

        }

        public SelectedCombatActionData(CombatActionType ActionType, DisciplinePower DisciplinePowerData)
        {
            this.ActionType = ActionType;
            this.DisciplinePowerData = DisciplinePowerData;
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
