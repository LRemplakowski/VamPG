using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCombatBehaviour : CombatBehaviour
{
    private GridController gridController;

    protected override void OnCombatStart()
    {
        gridController = TurnCombatManager.instance.GridInstance;
        GridElement nearest = gridController.GetNearestGridElement(this.gameObject.transform.position);
        HasMoved = false;
        owner.Move(nearest);
    }

    protected override void OnCombatRoundBegin(Creature newActor)
    {
        if (!newActor.Equals(owner))
            return;
        Debug.Log("PlayerCombatBehaviour >> OnCombatRoundBegin >> newActor[" + newActor + "]");
        StartCoroutine(HighlightGridAfterNextUpdate());
        HasMoved = false;
    }

    protected override void OnLookForCoverFinished(Dictionary<GridElement, List<Cover>> positionsNearCover)
    {

    }

    protected override void OnMovementStarted(Creature who)
    {
        if (!who.Equals(owner))
            return;
        HasMoved = true;
        gridController.ClearActiveElements();
    }

    protected override void OnMovementFinished(Creature who)
    {
        Debug.Log("dupa");
        if (!who.Equals(owner))
            return;
        Debug.Log("PlayerCombatBehaviour >> OnMovementFinished >> who[" + who + "]");
        //TurnCombatManager.instance.NextRound();
    }

    private IEnumerator HighlightGridAfterNextUpdate()
    {
        yield return new WaitForFixedUpdate();
        gridController.ActivateElementsInRangeOfActor(owner);
        StopCoroutine(HighlightGridAfterNextUpdate());
    }
}
