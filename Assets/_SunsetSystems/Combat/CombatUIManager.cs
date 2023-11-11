using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Combat.UI
{
    public class CombatUIManager : SerializedMonoBehaviour
    {
        public UltEvent<SelectedCombatActionData> OnCombatActionSelected;

        private List<Button> childrenButtons = new();

        private void Start()
        {
            childrenButtons = GetComponentsInChildren<Button>(true).ToList();
        }

        public void OnCombatBegin()
        {
            childrenButtons.ForEach(b => b.interactable = false);
        }

        public void OnCombatRoundBegin(ICombatant combatant)
        {
            childrenButtons.ForEach(button => button.interactable = combatant.Faction is Faction.PlayerControlled);
        }

        public void SelectCombatAction(SelectedCombatActionData actionData)
        {
            OnCombatActionSelected?.InvokeSafe(actionData);
        }
    }
}
