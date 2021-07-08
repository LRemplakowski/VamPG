using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayableCharacterCombatBehaviour : CombatBehaviour
{
    private GridController gridController;

    protected override bool EvaluateEndTurnCondition()
    {
        return HasMoved && HasActed;
    }

    protected override void OnCombatStartImpl()
    {
        gridController = TurnCombatManager.instance.GridInstance;
        GridElement nearest = gridController.GetNearestGridElement(this.gameObject.transform.position);
        HasMoved = false;
        owner.Move(nearest);
    }

    protected override void OnCombatRoundBeginImpl(Creature newActor)
    {
        StartCoroutine(HighlightGridAfterNextUpdate());
        HasMoved = false;
        HasActed = false;
    }

    protected override void OnCombatRoundEndImpl(Creature currentActor)
    {
        Debug.Log("Player-controlled character round ends!");
    }

    protected override void OnLookForCoverFinishedImpl(Dictionary<GridElement, List<Cover>> positionsNearCover)
    {

    }

    protected override void OnMovementStartedImpl(Creature who)
    {
        Debug.Log("PlayableCharacterCombatBehaviour >> OnMovementStarted >> who[" + who + "]");
        gridController.ClearActiveElements();   
    }

    protected override void OnMovementFinishedImpl(Creature who)
    {
        HasMoved = true;
        Debug.Log("PlayableCharacterCombatBehaviour >> OnMovementFinished >> who[" + who + "]");
        //TurnCombatManager.instance.NextRound();
    }

    private IEnumerator HighlightGridAfterNextUpdate()
    {
        yield return new WaitForFixedUpdate();
        gridController.ActivateElementsInRangeOfActor(owner);
        StopCoroutine(HighlightGridAfterNextUpdate());
    }
}
