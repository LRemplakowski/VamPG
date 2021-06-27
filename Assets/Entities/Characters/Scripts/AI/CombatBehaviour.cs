using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Creature)), RequireComponent(typeof(StatsManager))]
public abstract class CombatBehaviour : ExposableMonobehaviour
{
    protected CoverDetector coverDetector;

    public bool HasMoved { get; set; }
    public bool HasActed { get; set; }

    #region Enable&Disable
    protected void OnEnable()
    {
        Debug.Log("fired for " + this.gameObject.name);
        coverDetector = GetComponentInChildren<CoverDetector>();
        TurnCombatManager.instance.onCombatStart += OnCombatStart;
        coverDetector.onLookForCoverFinished += OnLookForCoverFinished;
        TurnCombatManager.instance.onCombatRoundBegin += OnCombatRoundBegin;
        TurnCombatManager.instance.onCombatRoundEnd += OnCombatRoundEnd;
        Move.onMovementStarted += OnMovementStarted;
        Move.onMovementFinished += OnMovementFinished;
    }

    protected void OnDisable()
    {
        TurnCombatManager.instance.onCombatStart -= OnCombatStart;
        coverDetector.onLookForCoverFinished -= OnLookForCoverFinished;
        TurnCombatManager.instance.onCombatRoundBegin -= OnCombatRoundBegin;
        TurnCombatManager.instance.onCombatRoundEnd -= OnCombatRoundEnd;
        Move.onMovementFinished -= OnMovementFinished;
        Move.onMovementStarted -= OnMovementStarted;
    }
    #endregion

    protected Creature owner;
    protected StatsManager stats;

    protected void Start()
    {
        owner = GetComponent<Creature>();
        stats = GetComponent<StatsManager>();
    }

    private bool ShouldIgnoreEvent(Creature caller)
    {
        return !(caller.Equals(owner) && StateManager.instance.GetCurrentState().Equals(GameState.Combat));
    }

    private void OnCombatStart()
    {
        OnCombatStartImpl();
    }

    /// <summary>
    /// Wywoływane przed pierwszą turą walki.
    /// </summary>
    protected abstract void OnCombatStartImpl();

    private void OnLookForCoverFinished(Dictionary<GridElement, List<Cover>> positionsNearCover)
    {
        OnLookForCoverFinishedImpl(positionsNearCover);
    }

    /// <summary>
    /// Wywoływane kiedy <see cref="CoverDetector"/> przypisany do GameObjectu skończy szukać pól mogących dawać osłonę.
    /// </summary>
    protected abstract void OnLookForCoverFinishedImpl(Dictionary<GridElement, List<Cover>> positionsNearCover);

    private void OnCombatRoundBegin(Creature newActor)
    {
        if (ShouldIgnoreEvent(newActor))
            return;
        if (stats.IsDead())
        {
            TurnCombatManager.instance.NextRound();
            return;
        }
        Debug.Log("CombatBehaviour >> OnCombatRoundBegin >> newActor[" + newActor + "]");
        OnCombatRoundBeginImpl(newActor);
        StartCoroutine(MaybeEndTurn());
    }

    /// <summary>
    /// Wywoływane na początku każdej tury GameObjectu do którego przypisany jest ten skrypt.
    /// </summary>
    protected abstract void OnCombatRoundBeginImpl(Creature newActor);

    private void OnCombatRoundEnd(Creature currentActor)
    {
        if (ShouldIgnoreEvent(currentActor))
            return;
        OnCombatRoundEndImpl(currentActor);
        HasMoved = false;
        HasActed = false;
    }

    /// <summary>
    /// Wywoływane na końcu każdej tury GameObjectu do którego przypisany jest ten skrypt.
    /// </summary>
    protected abstract void OnCombatRoundEndImpl(Creature currentActor);

    private void OnMovementStarted(Creature mover)
    {
        if (ShouldIgnoreEvent(mover))
            return;
        OnMovementStartedImpl(mover);
    }

    /// <summary>
    /// Wywoływane kiedy GameObject do którego przypisany jest ten skrypt ropzpocznie akcję <see cref="Move"/>.
    /// </summary>
    protected abstract void OnMovementStartedImpl(Creature who);

    private void OnMovementFinished(Creature mover)
    {
        if (ShouldIgnoreEvent(mover))
            return;
        OnMovementFinishedImpl(mover);
    }

    /// <summary>
    /// Wywoływane kiedy GameObject do którego przypisany jest ten skrypt zakończy akcję <see cref="Move"/>.
    /// </summary>
    protected abstract void OnMovementFinishedImpl(Creature who);

    private IEnumerator MaybeEndTurn()
    {
        yield return new WaitUntil(() => EvaluateEndTurnCondition() && !TurnCombatManager.instance.IsBeforeFirstRound());
        Debug.Log("Ending turn for " + owner);
        TurnCombatManager.instance.NextRound();
        StopCoroutine(this.MaybeEndTurn());
    }

    /// <summary>
    /// Warunek sprawdzany co klatkę, po wykonaniu funkcji Update.
    /// Kiedy ta funkcja zwróci TRUE runda GameObjectu do którego przypisany jest ten skrypt dobiegnie końca.
    /// </summary>
    protected abstract bool EvaluateEndTurnCondition();
}

