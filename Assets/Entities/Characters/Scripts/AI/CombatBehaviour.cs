using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Creature))]
public abstract class CombatBehaviour : ExposableMonobehaviour
{
    protected CoverDetector coverDetector;

    public bool HasMoved { get; set; }

    #region Enable&Disable
    protected void OnEnable()
    {
        Debug.Log("fired for " + this.gameObject.name);
        coverDetector = GetComponentInChildren<CoverDetector>();
        TurnCombatManager.instance.onCombatStart += OnCombatStart;
        coverDetector.onLookForCoverFinished += OnLookForCoverFinished;
        TurnCombatManager.instance.onCombatRoundBegin += OnCombatRoundBegin;
        Move.onMovementStarted += OnMovementStarted;
        Move.onMovementFinished += OnMovementFinished;
    }

    protected void OnDisable()
    {
        TurnCombatManager.instance.onCombatStart -= OnCombatStart;
        coverDetector.onLookForCoverFinished -= OnLookForCoverFinished;
        TurnCombatManager.instance.onCombatRoundBegin -= OnCombatRoundBegin;
        Move.onMovementFinished -= OnMovementFinished;
        Move.onMovementStarted -= OnMovementStarted;
    }
    #endregion

    protected Creature owner;

    protected void Start()
    {
        owner = GetComponent<Creature>();
    }

    protected abstract void OnCombatStart();

    protected abstract void OnLookForCoverFinished(Dictionary<GridElement, List<Cover>> positionsNearCover);

    protected abstract void OnCombatRoundBegin(Creature newActor);

    protected abstract void OnMovementStarted(Creature who);

    protected abstract void OnMovementFinished(Creature who);
}

