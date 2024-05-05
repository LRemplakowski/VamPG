using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class CombatActionSelectorButton : SerializedMonoBehaviour
    {
        private CombatUIManager combatUIManager;
        [SerializeField]
        private SelectedCombatActionData actionData;

        private void Start()
        {
            combatUIManager = GetComponentInParent<CombatUIManager>();
        }

        public void OnClick()
        {
            combatUIManager?.SelectCombatAction(actionData);
        }
    }
}
