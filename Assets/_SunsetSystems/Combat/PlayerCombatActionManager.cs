using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public class PlayerCombatActionManager : SerializedMonoBehaviour
    {
        [field: ShowInInspector, ReadOnly]
        public SelectedCombatActionData SelectedActionData { get; private set; }

        public void OnCombatActionSelected(SelectedCombatActionData actionData)
        {
            this.SelectedActionData = actionData;
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
