using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apex.AI;
using Apex.AI.Components;

[RequireComponent(typeof(StatsManager))]
public class CombatBehaviour : ExposableMonobehaviour, IContextProvider
{
    private CreatureContext _context;

    protected CoverDetector CoverDetector => GetComponentInChildren<CoverDetector>();

    public Creature Owner => GetComponent<Creature>();

    public bool HasActed => _context.HasActed;

    public bool HasMoved => _context.HasMoved;

    public bool IsPlayerControlled => _context.IsPlayerControlled;


    private void Awake()
    {
        _context = new CreatureContext(Owner);
    }

    #region Enable&Disable
    private void OnEnable()
    {
        if (!CoverDetector)
            GenerateDefaultCoverDetector();
        Move.onMovementFinished += OnMovementFinished;
        TurnCombatManager.onCombatStart += OnCombatStart;
        TurnCombatManager.onCombatRoundBegin += OnCombatRoundBegin;
    }

    private void OnDisable()
    {
        Move.onMovementFinished -= OnMovementFinished;
        TurnCombatManager.onCombatStart -= OnCombatStart;
        TurnCombatManager.onCombatRoundBegin -= OnCombatRoundBegin;
    }

    private CoverDetector GenerateDefaultCoverDetector()
    {
        GameObject coverDetector = new GameObject("Default Cover Detector");
        coverDetector.transform.parent = this.transform;
        coverDetector.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        SphereCollider sphereCollider = coverDetector.AddComponent<SphereCollider>();
        sphereCollider.radius = 1;
        sphereCollider.center = Vector3.up;
        sphereCollider.isTrigger = true;
        Rigidbody rigidbody = coverDetector.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation.None;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        return coverDetector.AddComponent<CoverDetector>();
    }
    #endregion


    private void OnMovementFinished(Creature who)
    {
        if (who.Equals(Owner))
        {
            _context.HasMoved = true;
        }
    }

    private void OnCombatStart(List<Creature> creaturesInCombat)
    {
        _context.HasMoved = false;
        _context.HasActed = false;
    }

    private void OnCombatRoundBegin(Creature currentActor)
    {
        if (currentActor.Equals(Owner))
        {
            _context.HasMoved = false;
            _context.HasActed = false;
        }
    }

    public IAIContext GetContext(Guid aiId)
    {
        return _context;
    }
}

