using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class CombatUIManager : SerializedMonoBehaviour
    {
        public UltEvent<SelectedCombatActionData> OnCombatActionSelected;


        public void SelectCombatAction(SelectedCombatActionData actionData)
        {
            OnCombatActionSelected?.InvokeSafe(actionData);
        }
    }
}
