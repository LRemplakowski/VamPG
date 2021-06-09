using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public class NPCombatBehaviour : CombatBehaviour
{
    protected override void OnCombatRoundBegin(Creature newActor)
    {
        if (newActor.Equals(owner))
        {
            Debug.LogWarning("active actor changed in combat behaviour!");
            List<GridElement> elementsInRange = TurnCombatManager.instance.GridInstance.GetElementsInRangeOfActor(owner);
            Debug.LogWarning("elements in range count: " + elementsInRange.Count);
            HasMoved = false;
            coverDetector.StartLookingForCover(elementsInRange);
        }
    }

    protected override void OnCombatStart()
    {
        GridController gridController = GameManager.GetGridController();
        GridElement g = gridController.GetNearestGridElement(this.transform.position);
        owner.Move(g);
    }

    protected override void OnLookForCoverFinished(Dictionary<GridElement, List<Cover>> positionsNearCover)
    {
        Debug.LogWarning("Positions near cover: " + positionsNearCover.Count);
        int random = Random.Range(0, positionsNearCover.Count);
        int index = 0;
        foreach (KeyValuePair<GridElement, List<Cover>> pair in positionsNearCover)
        {
            if (index == random)
            {
                owner.Move(pair.Key);
                break;
            }
            index++;
        }
    }

    protected override void OnMovementFinished(Creature who)
    {
        if (!who.Equals(owner))
            return;
        if (who.Equals(TurnCombatManager.instance.CurrentActiveActor))
            TurnCombatManager.instance.NextRound();
    }

    protected override void OnMovementStarted(Creature who)
    {
        if (!who.Equals(owner))
            return;
        HasMoved = true;
    }
}
