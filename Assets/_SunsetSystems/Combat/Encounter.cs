using SunsetSystems.Entities.Characters;
using System.Collections.Generic;
using UnityEngine;
using System;
using SunsetSystems.Game;
using SunsetSystems.Combat.Grid;

namespace SunsetSystems.Combat
{
    public class Encounter : MonoBehaviour, IEncounter
    {
        [field: SerializeField]
        public CachedMultiLevelGrid MyGrid { get; private set; }
        private CombatManager combatManager;

        [field: SerializeField, Tooltip("Creatures taking part in this encounter.")]
        public List<Creature> Creatures { get; private set; }

        [SerializeField]
        private EncounterEndTrigger _encounterEndTrigger = EncounterEndTrigger.Automatic;

        [Header("Optional")]
        [SerializeField, Tooltip("(Optional) Custom logic run before the start of the encounter.")]
        private AbstractEncounterLogic encounterStartLogic;
        [SerializeField, Tooltip("(Optional) Custom logic run after the end of the encounter.")]
        private AbstractEncounterLogic encounterEndLogic;

        private int _creatureCounter = 0;

        private void Start()
        {
            if (!MyGrid)
                MyGrid = GetComponent<CachedMultiLevelGrid>();
            if (!combatManager)
                combatManager = this.FindFirstComponentWithTag<CombatManager>(TagConstants.COMBAT_MANAGER);
        }

        public async void Begin()
        {
            Debug.LogWarning("Begin encounter, do encounter start logic.");
            if (encounterStartLogic)
                await encounterStartLogic.Perform();
            GameManager.CurrentState = GameState.Combat;
            await MyGrid.InstantiateGrid();
            _creatureCounter = Creatures.Count;
            await combatManager.BeginEncounter(this);
            if (_encounterEndTrigger == EncounterEndTrigger.Automatic)
            {
                Creatures.ForEach(c => c.References.GetComponentInChildren<StatsManager>().OnCreatureDied += DecrementCounterAndCheckForEncounterEnd);
            }
        }

        private void DecrementCounterAndCheckForEncounterEnd(Creature creature)
        {
            _creatureCounter -= 1;
            creature.References.GetComponentInChildren<StatsManager>().OnCreatureDied -= DecrementCounterAndCheckForEncounterEnd;
            if (_creatureCounter <= 0)
                End();
        }

        public async void End()
        {
            Debug.LogWarning("End encounter, do encounter end logic.");
            MyGrid.CleanupGrid();
            await combatManager.EndEncounter(this);
            GameManager.CurrentState = GameState.Exploration;
            if (encounterEndLogic)
                await encounterEndLogic.Perform();
        }

        private enum EncounterEndTrigger
        {
            Automatic, Manual
        }
    }
}
