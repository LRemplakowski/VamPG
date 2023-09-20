using SunsetSystems.Entities.Characters;
using System.Collections.Generic;
using UnityEngine;
using System;
using SunsetSystems.Game;
using SunsetSystems.Combat.Grid;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;

namespace SunsetSystems.Combat
{
    public class Encounter : MonoBehaviour, IEncounter
    {
        [field: SerializeField]
        public CachedMultiLevelGrid MyGrid { get; private set; }
        private CombatManager combatManager;

        [field: SerializeField, Tooltip("Creatures taking part in this encounter.")]
        public List<ICombatant> Creatures { get; private set; }

        [SerializeField]
        private EncounterEndTrigger _encounterEndTrigger = EncounterEndTrigger.Automatic;

        [Title("Optional")]
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

        [Title("Editor Utility")]
        [Button("Begin Encounter")]
        public async void Begin()
        {
            Debug.LogWarning("Begin encounter, do encounter start logic.");
            if (encounterStartLogic)
                await encounterStartLogic.Perform();
            GameManager.CurrentState = GameState.Combat;
            MyGrid.EnableGrid();
            _creatureCounter = Creatures.Count;
            //await combatManager.BeginEncounter(this);
            //if (_encounterEndTrigger == EncounterEndTrigger.Automatic)
            //{
            //    Creatures.ForEach(c => c.References.StatsManager.OnCreatureDied += DecrementCounterAndCheckForEncounterEnd);
            //}
        }

        private void DecrementCounterAndCheckForEncounterEnd(Creature creature)
        {
            _creatureCounter -= 1;
            creature.References.StatsManager.OnCreatureDied -= DecrementCounterAndCheckForEncounterEnd;
            if (_creatureCounter <= 0)
                End();
        }

        [Button("End Encounter")]
        public async void End()
        {
            Debug.LogWarning("End encounter, do encounter end logic.");
            MyGrid.DisableGrid();
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
