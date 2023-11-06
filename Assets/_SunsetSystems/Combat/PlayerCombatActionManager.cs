using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public class PlayerCombatActionManager : SerializedMonoBehaviour
    {
        public SelectedCombatActionData SelectedActionData { get; private set; }

        public void OnCombatActionSelected(SelectedCombatActionData actionData)
        {
            this.SelectedActionData = actionData;
        }
    }

    public struct SelectedCombatActionData
    {
        public readonly CombatActionType ActionType;

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
