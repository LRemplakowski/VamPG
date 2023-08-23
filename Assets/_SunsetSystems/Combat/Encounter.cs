using SunsetSystems.Entities.Characters;
using System.Collections.Generic;
using UnityEngine;
using System;
using SunsetSystems.Game;
using System.Threading.Tasks;
using System.Linq;
using Redcode.Awaiting;

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
                MyGrid = GetComponent<GridController>();
            if (!combatManager)
                combatManager = this.FindFirstComponentWithTag<CombatManager>(TagConstants.COMBAT_MANAGER);
        }

        public async void Begin()
        {
            Debug.LogWarning("Begin encounter, do encounter start logic.");
            if (encounterStartLogic)
                await encounterStartLogic.Perform();
            GameManager.CurrentState = GameState.Combat;
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
            MyGrid.ClearActiveElements();
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
