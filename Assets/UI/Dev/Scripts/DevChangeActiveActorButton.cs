using SunsetSystems.Entities.Characters;
using SunsetSystems.Combat;
using System.Collections.Generic;
using UnityEngine;

public class DevChangeActiveActorButton : ExposableMonobehaviour
{
    public void NextActor()
    {
        CombatManager combatManager = CombatManager.Instance;
        List<Creature> actors = combatManager.Actors;
        int currentIndex = actors.IndexOf(CombatManager.CurrentActiveActor);
        Debug.Log("current actor index: " + currentIndex + ", actors count: " + actors.Count);
        combatManager.SetCurrentActiveActor(currentIndex >= actors.Count - 1 ? 0 : currentIndex + 1);
    }

    public void PreviousActor()
    {
        CombatManager combatManager = CombatManager.Instance;
        List<Creature> actors = combatManager.Actors;
        int currentIndex = actors.IndexOf(CombatManager.CurrentActiveActor);
        Debug.Log("current actor index: " + currentIndex + ", actors count: " + actors.Count);
        combatManager.SetCurrentActiveActor(currentIndex <= 0 ? actors.Count - 1 : currentIndex - 1);
    }
}
