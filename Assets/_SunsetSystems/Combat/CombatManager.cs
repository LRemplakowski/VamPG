using SunsetSystems.Entities.Characters;
using SunsetSystems.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Redcode.Awaiting;
using System;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;

namespace SunsetSystems.Combat
{
    [RequireComponent(typeof(Tagger))]
    public class CombatManager : Singleton<CombatManager>
    {
        public delegate void CombatBeginHandler(List<ICombatant> creaturesInCombat);
        public static event CombatBeginHandler CombatBegin;
        public delegate void CombatEndHandler();
        public static event CombatEndHandler CombatEnd;
        public delegate void ActiveActorChangedHandler(ICombatant newActor, ICombatant previousActor);
        public static event ActiveActorChangedHandler ActiveActorChanged;
        public delegate void CombatRoundBeginHandler(ICombatant currentActor);
        public static event CombatRoundBeginHandler CombatRoundBegin;
        public delegate void CombatRoundEndHandler(ICombatant currentActor);
        public static event CombatRoundEndHandler CombatRoundEnd;
        public static event Action OnFullTurnCompleted;

        private int turnCounter;

        private static ICombatant _currentActiveActor;
        public static ICombatant CurrentActiveActor
        {
            get => _currentActiveActor;
            private set
            {
                ICombatant previous = _currentActiveActor;
                _currentActiveActor = value;
                ActiveActorChanged?.Invoke(value, previous);
            }
        }
        private ICombatant FirstActor;

        [field: SerializeField]
        public Encounter CurrentEncounter { get; private set; }

        [field: ShowInInspector, ReadOnly]
        public List<ICombatant> Actors { get; private set; }

        public void SetCurrentActiveActor(int index)
        {
            ICombatant c = null;
            if (index < Actors.Count)
                c = Actors[index];
            CurrentActiveActor = c;
        }

        public void NextRound()
        {
            if (CurrentActiveActor != null)
            {
                CombatRoundEnd?.Invoke(CurrentActiveActor);
                Debug.Log("Combat Manager: " + CurrentActiveActor.References.GameObject.name + " finished round " + turnCounter + "!");
                int index = Actors.IndexOf(CurrentActiveActor);
                SetCurrentActiveActor(++index < Actors.Count ? index : 0);
            }
            else
            {
                FirstActor = DecideFirstActor(Actors);
                CurrentActiveActor = FirstActor;
            }
            if (CurrentActiveActor == FirstActor)
            {
                turnCounter++;
                OnFullTurnCompleted?.Invoke();
            }
            CombatRoundBegin?.Invoke(CurrentActiveActor);
            Debug.Log("Combat Manager: " + CurrentActiveActor.References.GameObject.name + " begins round " + turnCounter + "!");
        }

        public async Task BeginEncounter(Encounter encounter)
        {
            CurrentEncounter = encounter;
            turnCounter = 0;
            Actors = new();
            Actors.AddRange(encounter.Creatures);
            //Actors.AddRange(PartyManager.ActiveParty);
            throw new NotImplementedException();
            CombatBegin?.Invoke(Actors);
            //Actors.ForEach(c => c.GetComponent<CreatureAnimationController>().SetCombatAnimationsActive(true));
            await MoveAllCreaturesToNearestGridPosition(Actors, CurrentEncounter);
            NextRound();
        }

        private ICombatant DecideFirstActor(List<ICombatant> creatures)
        {
            creatures.OrderByDescending(creature => creature.References.GetComponentInChildren<StatsManager>().GetInitiative());
            return creatures[0];
        }

        public async Task EndEncounter(Encounter encounter)
        {
            await Task.Yield();
            CombatEnd?.Invoke();
            CurrentEncounter = null;
        }

        private static async Task MoveAllCreaturesToNearestGridPosition(List<ICombatant> actors, Encounter currentEncounter)
        {
            List<Task> moveTasks = new();
            foreach (ICombatant c in actors)
            {
                moveTasks.Add(Task.Run(async () =>
                {
                await new WaitForUpdate();
                Vector3Int gridPosition = currentEncounter.MyGrid.GetNearestGridPosition(c.References.Transform.position);
                c.MoveToGridPosition(gridPosition);
                }));
            }
            await Task.WhenAll(moveTasks);
        }

        public bool IsBeforeFirstRound()
        {
            return turnCounter <= 0;
        }

        public bool IsFirstRoundOfCombat()
        {
            return turnCounter == 1;
        }

        public static bool IsActiveActorPlayerControlled()
        {
            return _currentActiveActor != null ? CurrentActiveActor.IsPlayerControlled : false;
        }

        public int GetRound()
        {
            return turnCounter;
        }

        public class CombatEventData
        {

        }
    }
}
