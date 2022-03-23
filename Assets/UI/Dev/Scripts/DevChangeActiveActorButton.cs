using Entities.Characters;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Management;
using UnityEngine;

public class DevChangeActiveActorButton : ExposableMonobehaviour
{
    public void NextActor()
    {
        TurnCombatManager turnCombatManager = References.Get<TurnCombatManager>();
        List<Creature> actors = turnCombatManager.GetCreaturesInCombat();
        int currentIndex = actors.IndexOf(turnCombatManager.CurrentActiveActor);
        Debug.Log("current actor index: " + currentIndex + ", actors count: " + actors.Count);
        turnCombatManager.SetCurrentActiveActor(currentIndex >= actors.Count - 1 ? 0 : currentIndex + 1);
    }

    public void PreviousActor()
    {
        TurnCombatManager turnCombatManager = References.Get<TurnCombatManager>();
        List<Creature> actors = turnCombatManager.GetCreaturesInCombat();
        int currentIndex = actors.IndexOf(turnCombatManager.CurrentActiveActor);
        Debug.Log("current actor index: " + currentIndex + ", actors count: " + actors.Count);
        turnCombatManager.SetCurrentActiveActor(currentIndex <= 0 ? actors.Count-1 : currentIndex - 1);
    }
}
