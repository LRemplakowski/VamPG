using SunsetSystems.Entities.Characters;

namespace SunsetSystems.Combat
{
    public interface ICombatManager
    {
        Creature CurrentActiveActor { get; }

        void BeginEncounter(Encounter encounter);

        void EndEncounter(Encounter encounter);

        bool IsBeforeFirstRound();

        bool IsFirstRoundOfCombat();

        bool IsActiveActorPlayerControlled();

        int GetRound();

        void NextRound();
    }
}
