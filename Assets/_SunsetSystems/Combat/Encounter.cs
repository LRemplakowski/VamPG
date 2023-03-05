using SunsetSystems.Entities.Characters;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Game;
using Zenject;

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

        private IGameManager gameManager;

        [Inject]
        public void InjectDependencies(IGameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        private void Start()
        {
            if (!MyGrid)
                MyGrid = GetComponent<GridController>();
            if (!combatManager)
                combatManager = this.FindFirstComponentWithTag<CombatManager>(TagConstants.COMBAT_MANAGER);
        }

        public void Begin()
        {
            Debug.LogWarning("Begin encounter, do encounter start logic.");
            if (encounterStartLogic)
                encounterStartLogic.Perform();
            gameManager.CurrentState = GameState.Combat;
            _creatureCounter = Creatures.Count;
            combatManager.BeginEncounter(this);
            if (_encounterEndTrigger == EncounterEndTrigger.Automatic)
            {
                Creatures.ForEach(c => c.StatsManager.OnCreatureDied += DecrementCounterAndCheckForEncounterEnd);
            }
        }

        private void DecrementCounterAndCheckForEncounterEnd(Creature creature)
        {
            _creatureCounter -= 1;
            creature.StatsManager.OnCreatureDied -= DecrementCounterAndCheckForEncounterEnd;
            if (_creatureCounter <= 0)
                End();
        }

        public void End()
        {
            Debug.LogWarning("End encounter, do encounter end logic.");
            MyGrid.ClearActiveElements();
            combatManager.EndEncounter(this);
            gameManager.CurrentState = GameState.Exploration;
            if (encounterEndLogic)
                encounterEndLogic.Perform();
        }

        private enum EncounterEndTrigger
        {
            Automatic, Manual
        }
    }
}
