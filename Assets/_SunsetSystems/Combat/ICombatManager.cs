using SunsetSystems.Entities.Characters;
using System.Collections.Generic;

namespace SunsetSystems.Combat
{
    public interface ICombatManager
    {
        List<Creature> Actors { get; }
        Creature CurrentActiveActor { get; }
        IEncounter CurrentEncounter { get; }

        void BeginEncounter(Encounter encounter);

        void EndEncounter(Encounter encounter);

        bool IsBeforeFirstRound();

        bool IsFirstRoundOfCombat();

        bool IsActiveActorPlayerControlled();

        int GetRound();

        void NextRound();
    }
}
