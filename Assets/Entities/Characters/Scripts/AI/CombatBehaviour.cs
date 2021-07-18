using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatsManager))]
public class CombatBehaviour : ExposableMonobehaviour
{
    protected CoverDetector coverDetector;

    public bool HasMoved { get; set; }
    public bool HasActed { get; set; }

    #region Enable&Disable
    private void OnEnable()
    {
        coverDetector = GetComponentInChildren<CoverDetector>();
        if (!coverDetector)
            coverDetector = GenerateDefaultCoverDetector();
        Move.onMovementFinished += OnMovementFinished;
    }

    private void OnDisable()
    {
        Move.onMovementFinished -= OnMovementFinished;
    }

    private CoverDetector GenerateDefaultCoverDetector()
    {
        GameObject cd = new GameObject("Default Cover Detector");
        cd.transform.parent = this.transform;
        cd.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        SphereCollider sc = cd.AddComponent<SphereCollider>();
        sc.radius = 1;
        sc.center = new Vector3(0, 1, 0);
        sc.isTrigger = true;
        Rigidbody rb = cd.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        return cd.AddComponent<CoverDetector>();
    }
    #endregion

    protected Creature owner;
    protected StatsManager stats;
    [SerializeField]
    private bool _playerControlled = false;
    public bool PlayerControlled { get => _playerControlled; set => _playerControlled = value; }

    protected void Start()
    {
        owner = GetComponent<Creature>();
        stats = GetComponent<StatsManager>();
    }

    private void OnMovementFinished(Creature who)
    {
        if (who.Equals(owner))
        {
            HasMoved = true;
        }
    }
}

