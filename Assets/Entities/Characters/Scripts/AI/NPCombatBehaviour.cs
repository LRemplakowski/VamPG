using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public class NPCombatBehaviour : CombatBehaviour
{
    protected override bool EvaluateEndTurnCondition()
    {
        return HasMoved && HasActed;
    }

    protected override void OnCombatRoundBeginImpl(Creature newActor)
    {
        HasActed = false;
        HasMoved = false;
        Debug.LogWarning("active actor changed in combat behaviour!");
        List<GridElement> elementsInRange = TurnCombatManager.instance.GridInstance.GetElementsInRangeOfActor(owner);
        Debug.LogWarning("elements in range count: " + elementsInRange.Count);
        coverDetector.StartLookingForCover(elementsInRange);
    }

    protected override void OnCombatRoundEndImpl(Creature currentActor)
    {
        Debug.Log("combat round ended for " + owner);
    }

    protected override void OnCombatStartImpl()
    {
        GridController gridController = GameManager.GetGridController();
        GridElement g = gridController.GetNearestGridElement(this.transform.position);
        owner.Move(g);
    }

    protected override void OnLookForCoverFinishedImpl(Dictionary<GridElement, List<Cover>> positionsNearCover)
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

    protected override void OnMovementFinishedImpl(Creature who)
    {
        HasMoved = true;
        HasActed = true;
    }

    protected override void OnMovementStartedImpl(Creature who)
    {

    }
}
