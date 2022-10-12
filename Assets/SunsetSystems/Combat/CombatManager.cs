using Entities.Characters;
using SunsetSystems.Data;
using SunsetSystems.Game;
using SunsetSystems.Party;
using SunsetSystems.Utils;
using SunsetSystems.Utils.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Combat
{
    [RequireComponent(typeof(Tagger))]
    public class CombatManager : Singleton<CombatManager>
    {
        public delegate void CombatBeginHandler(List<Creature> creaturesInCombat);
        public static event CombatBeginHandler CombatBegin;
        public delegate void CombatEndHandler();
        public static event CombatEndHandler CombatEnd;
        public delegate void ActiveActorChangedHandler(Creature newActor, Creature previousActor);
        public static event ActiveActorChangedHandler ActiveActorChanged;
        public delegate void CombatRoundBeginHandler(Creature currentActor);
        public static event CombatRoundBeginHandler CombatRoundBegin;
        public delegate void CombatRoundEndHandler(Creature currentActor);
        public static event CombatRoundEndHandler CombatRoundEnd;

        private int roundCounter;

        private static Creature _currentActiveActor;
        public static Creature CurrentActiveActor
        {
            get => _currentActiveActor;
            private set
            {
                Creature previous = _currentActiveActor;
                _currentActiveActor = value;
                if (ActiveActorChanged != null)
                    ActiveActorChanged.Invoke(value, previous);
            }
        }

        [field: SerializeField]
        public Encounter CurrentEncounter { get; private set; }

        [field: SerializeField]
        public List<Creature> Actors { get; private set; }

        [field: SerializeField]
        private GameRuntimeData RuntimeData { get; set; }

        private void Start()
        {
            if (!RuntimeData)
                RuntimeData = this.FindFirstComponentWithTag<GameRuntimeData>(TagConstants.GAME_RUNTIME_DATA);
        }

        private void Update()
        {
            if (GameManager.IsCurrentState(GameState.Combat))
            {
                if (!CurrentEncounter)
                    return;
                foreach (Creature c in Actors)
                {
                    if (c.IsAlive && c.Data.faction.Equals(Faction.Hostile))
                    {
                        break;
                    }
                    else
                    {
                        CombatEnd?.Invoke();
                    }
                }
            }
        }

        public void SetCurrentActiveActor(int index)
        {
            Creature c = null;
            if (index < Actors.Count)
                c = Actors[index];
            CurrentActiveActor = c;
        }

        public void NextRound()
        {
            if (CurrentActiveActor)
            {
                CombatRoundEnd?.Invoke(CurrentActiveActor);
                Debug.Log("Combat Manager: " + CurrentActiveActor.gameObject.name + " finished round " + roundCounter + "!");
            }
            roundCounter++;
            if (!CurrentActiveActor)
                CurrentActiveActor = DecideFirstActor(Actors);
            int index = Actors.IndexOf(CurrentActiveActor);
            SetCurrentActiveActor(++index < Actors.Count ? index : 0);
            CombatRoundBegin.Invoke(CurrentActiveActor);
            Debug.Log("Combat Manager: " + CurrentActiveActor.gameObject.name + " begins round " + roundCounter + "!");
        }

        public async Task BeginEncounter(Encounter encounter)
        {
            CurrentEncounter = encounter;
            roundCounter = 0;
            Actors = new();
            Actors.AddRange(encounter.Creatures);
            Actors.AddRange(PartyManager.ActiveParty);
            CombatBegin?.Invoke(Actors);
            await MoveAllCreaturesToNearestGridPosition(Actors, CurrentEncounter);
            NextRound();
        }

        private Creature DecideFirstActor(List<Creature> creatures)
        {
            creatures.OrderByDescending(creature => creature.StatsManager.GetInitiative());
            return creatures[0];
        }

        public void EndEncounter(Encounter encounter)
        {
            CombatEnd?.Invoke();
        }

        private static async Task MoveAllCreaturesToNearestGridPosition(List<Creature> actors, Encounter currentEncounter)
        {
            List<Task> moveTasks = new();
            foreach (Creature c in actors)
            {
                moveTasks.Add(Task.Run(async () =>
                {
                    Dispatcher.Instance.Invoke(() =>
                    {
                        c.Move(currentEncounter.MyGrid.GetNearestGridElement(c.transform.position));
                    });
                    while (!c.CombatBehaviour.HasMoved)
                    {
                        Dispatcher.Instance.Invoke(() =>
                        {
                            Debug.Log("Waiting for creature " + c.gameObject.name + " to move!");
                        });
                        await Task.Delay(1000);
                    }
                }));
            }
            await Task.WhenAll(moveTasks);
        }

        public bool IsBeforeFirstRound()
        {
            return roundCounter <= 0;
        }

        public bool IsFirstRoundOfCombat()
        {
            return roundCounter == 1;
        }

        public static bool IsActiveActorPlayerControlled()
        {
            return _currentActiveActor ? CurrentActiveActor.CombatBehaviour.IsPlayerControlled : false;
        }

        public int GetRound()
        {
            return roundCounter;
        }

        public class CombatEventData
        {

        }
    }
}
