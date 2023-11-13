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
using SunsetSystems.Party;
using SunsetSystems.Animation;
using SunsetSystems.Entities.Characters.Actions;
using UltEvents;

namespace SunsetSystems.Combat
{
    [RequireComponent(typeof(Tagger))]
    public class CombatManager : SerializedMonoBehaviour
    {
        public static CombatManager Instance { get; private set; }

        [field: Title("Runtime")]
        [field: SerializeField]
        public Encounter CurrentEncounter { get; private set; }

        [field: ShowInInspector, ReadOnly]
        public List<ICombatant> Actors { get; private set; }
        public List<ICombatant> LivingActors => Actors.FindAll(a => a.IsAlive);

        [Title("Events")]
        public UltEvent<IEnumerable<ICombatant>> CombatBegin;
        public UltEvent CombatEnd;
        public UltEvent<ICombatant> CombatRoundBegin;
        public UltEvent<ICombatant> CombatRoundEnd;
        public UltEvent OnFullTurnCompleted;

        private int turnCounter;

        private ICombatant _currentActiveActor;
        public ICombatant CurrentActiveActor
        {
            get => _currentActiveActor;
            private set
            {
                ICombatant previous = _currentActiveActor;
                _currentActiveActor = value;
            }
        }
        private ICombatant FirstActor;

        private void Awake()
        {
            Instance = this;
        }

        public void SetCurrentActiveActor(int index)
        {
            ICombatant c = null;
            var livingActors = LivingActors;
            if (index < livingActors.Count)
            {
                c = livingActors[index];
            }
            CurrentActiveActor = c;
        }

        [Title("Editor")]
        [Button]
        public void NextRound()
        {
            if (CurrentActiveActor != null)
            {
                CombatRoundEnd?.Invoke(CurrentActiveActor);
                Debug.Log("Combat Manager: " + CurrentActiveActor.References.GameObject.name + " finished round " + turnCounter + "!");
                int index = Actors.IndexOf(CurrentActiveActor);
                SetCurrentActiveActor(++index < LivingActors.Count ? index : 0);
            }
            else
            {
                FirstActor = DecideFirstActor(Actors);
                CurrentActiveActor = FirstActor;
            }
            if (LivingActors.IndexOf(CurrentActiveActor) == 0)
            {
                turnCounter++;
                OnFullTurnCompleted?.Invoke();
            }
            CombatRoundBegin?.InvokeSafeDynamicFirst(CurrentActiveActor);
            Debug.Log("Combat Manager: " + CurrentActiveActor.References.GameObject.name + " begins round " + turnCounter + "!");
        }

        public async Task BeginEncounter(Encounter encounter)
        {
            CurrentEncounter = encounter;
            turnCounter = 0;
            Actors = new();
            Actors.AddRange(PartyManager.Instance.ActiveParty.Select(c => c.References.CombatBehaviour));
            Actors.AddRange(encounter.Creatures.FindAll(c => c != null).Select(c => c.References.CombatBehaviour));
            CombatBegin?.InvokeSafe(Actors);
            await MoveAllCreaturesToNearestGridPosition(Actors, CurrentEncounter);
            await new WaitForSeconds(1f);
            NextRound();
        }

        private ICombatant DecideFirstActor(List<ICombatant> creatures)
        {
            //creatures.OrderByDescending(creature => creature.References.GetComponentInChildren<StatsManager>().GetInitiative());
            return creatures[0];
        }

        public async Task EndEncounter(Encounter encounter)
        {
            await Task.Yield();
            CombatEnd?.Invoke();
            CurrentEncounter = null;
        }

        private static Task MoveAllCreaturesToNearestGridPosition(List<ICombatant> actors, Encounter currentEncounter)
        {
            List<Task> tasks = new();
            foreach (ICombatant combatant in actors)
            {
                Vector3Int gridPosition = currentEncounter.GridManager.GetNearestWalkableGridPosition(combatant.References.Transform.position);
                Debug.Log($"Nearest grid position for Combatant {combatant.References.GameObject.name} is {gridPosition}!");
                tasks.Add(combatant.PerformAction(new Move(combatant, currentEncounter.GridManager[gridPosition], currentEncounter.GridManager)));
            }
            return Task.WhenAll(tasks);
        }

        public bool IsBeforeFirstRound()
        {
            return turnCounter <= 0;
        }

        public bool IsFirstRoundOfCombat()
        {
            return turnCounter == 1;
        }

        public bool IsActiveActorPlayerControlled()
        {
            return _currentActiveActor != null ? CurrentActiveActor.IsPlayerControlled : false;
        }

        public int GetRound()
        {
            return turnCounter;
        }

        public List<ICombatant> GetCombatantsInTurnOrder()
        {
            var livingActors = LivingActors;
            if (livingActors.Count <= 0)
                return livingActors;
            int currentActorIndex;
            if (livingActors.Contains(CurrentActiveActor))
            {
                currentActorIndex = livingActors.IndexOf(CurrentActiveActor);
            }
            else
            {
                return livingActors;
            }
            int copyOffset = livingActors.Count - currentActorIndex;
            ICombatant[] offsetCopy = new ICombatant[livingActors.Count];
            Array.Copy(livingActors.ToArray(), currentActorIndex, offsetCopy, 0, copyOffset);
            for (int i = copyOffset; i < livingActors.Count; i++)
            {
                offsetCopy[^i] = livingActors[i - copyOffset];
            }
            return offsetCopy.ToList();
        }
    }
}
