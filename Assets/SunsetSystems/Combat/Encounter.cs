using SunsetSystems.Entities.Characters;
using System.Collections.Generic;
using UnityEngine;
using System;
using SunsetSystems.Game;

namespace SunsetSystems.Combat
{
    [RequireComponent(typeof(GridController))]
    public class Encounter : MonoBehaviour, IEncounter
    {
        [field: SerializeField]
        public GridController MyGrid { get; private set; }
        private CombatManager combatManager;

        [field: SerializeField, Tooltip("Creatures taking part in this encounter.")]
        public List<Creature> Creatures { get; private set; }

        [Header("Optional")]
        [SerializeField, Tooltip("(Optional) Custom logic run before the start of the encounter.")]
        private AbstractEncounterLogic encounterStartLogic;
        [SerializeField, Tooltip("(Optional) Custom logic run after the end of the encounter.")]
        private AbstractEncounterLogic encounterEndLogic;

        private void Start()
        {
            if (!MyGrid)
                MyGrid = GetComponent<GridController>();
            if (!combatManager)
                combatManager = this.FindFirstComponentWithTag<CombatManager>(TagConstants.COMBAT_MANAGER);
        }

        public async void Begin()
        {
            Debug.LogWarning("Begin encounter, do encounter start logic.");
            if (encounterStartLogic)
                encounterStartLogic.Perform();
            GameManager.CurrentState = GameState.Combat;
            await combatManager.BeginEncounter(this);
        }

        public void End()
        {
            Debug.LogWarning("End encounter, do encounter end logic.");
            combatManager.EndEncounter(this);
            GameManager.CurrentState = GameState.Exploration;
            if (encounterEndLogic)
                encounterEndLogic.Perform();
        }
    }
}
