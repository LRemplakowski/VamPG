using SunsetSystems.Entities.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Combat.UI
{
    [RequireComponent(typeof(Button))]
    public class EndRoundButton : MonoBehaviour
    {
        [SerializeField]
        private Button myButton;

        private CombatManager cachedCombatManager;

        private void Awake()
        {
            if (!myButton)
                myButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            CombatManager.CombatRoundBegin += OnCombatRoundBegin;
        }

        private void OnDisable()
        {
            CombatManager.CombatRoundBegin -= OnCombatRoundBegin;
        }

        private void Start()
        {
            if (!cachedCombatManager)
                cachedCombatManager = this.FindFirstComponentWithTag<CombatManager>(TagConstants.COMBAT_MANAGER);
        }

        private void OnCombatRoundBegin(Creature currentActor)
        {
            myButton.interactable = currentActor.References.GetComponentInChildren<CombatBehaviour>().IsPlayerControlled;
        }

        public void EndRound()
        {
            cachedCombatManager.NextRound();
        }
    }
}
