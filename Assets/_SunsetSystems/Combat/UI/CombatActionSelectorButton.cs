using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class CombatActionSelectorButton : SerializedMonoBehaviour
    {
        [SerializeField]
        private CombatActionType actionType;

        private CombatUIManager combatUIManager;

        private SelectedCombatActionData actionData;

        private void Start()
        {
            combatUIManager = GetComponentInParent<CombatUIManager>();

            actionData = new(actionType);
        }

        public void OnClick()
        {
            combatUIManager?.SelectCombatAction(actionData);
        }
    }
}
