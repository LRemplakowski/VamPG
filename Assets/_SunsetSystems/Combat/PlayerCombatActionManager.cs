using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;
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
        [field: ShowInInspector, ReadOnly]
        public CombatActionType ActionType { get; private set; }

        public SelectedCombatActionData(CombatActionType ActionType)
        {
            this.ActionType = ActionType;
        }
    }

    public enum CombatActionType
    {
        Move, RangedAtk, MeleeAtk, Feed, Reload
    }
}
