using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Creature))]
public class CombatBehaviour : ExposableMonobehaviour
{
    private CoverDetector coverDetector;

    #region Enable&Disable
    private void OnEnable()
    {
        coverDetector = GetComponentInChildren<CoverDetector>();
        TurnCombatManager.instance.onCombatStart += OnCombatStart;
        coverDetector.onLookForCoverFinished += OnLookForCoverFinished;
        TurnCombatManager.instance.onActiveActorChanged += OnActiveActorChanged;
    }

    private void OnDisable()
    {
        TurnCombatManager.instance.onCombatStart -= OnCombatStart;
        coverDetector.onLookForCoverFinished -= OnLookForCoverFinished;
        TurnCombatManager.instance.onActiveActorChanged -= OnActiveActorChanged;
    }
    #endregion

    private Creature owner;

    private void Start()
    {
        owner = GetComponent<Creature>();
    }

    private void OnCombatStart()
    {
        GridController gridController = GameManager.GetGridController();
        GridElement g = gridController.GetNearestGridElement(this.transform.position);
        owner.Move(g);
    }

    private void OnLookForCoverFinished(Dictionary<GridElement, List<Cover>> positionsNearCover)
    {
        Debug.LogWarning("Positions near cover: " + positionsNearCover.Count);
        int random = UnityEngine.Random.Range(0, positionsNearCover.Count);
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

    private void OnActiveActorChanged(Creature newActor, Creature previousActor)
    {
        if (newActor.IsOfType(typeof(Player)))
            return;
        if (newActor.Equals(owner))
        {
            Debug.LogWarning("active actor changed in combat behaviour!");
            List<GridElement> elementsInRange = TurnCombatManager.instance.GridInstance.GetElementsInRangeOfActor(owner);
            Debug.LogWarning("elements in range count: " + elementsInRange.Count);
            coverDetector.StartLookingForCover(elementsInRange);
        }
    }
}

