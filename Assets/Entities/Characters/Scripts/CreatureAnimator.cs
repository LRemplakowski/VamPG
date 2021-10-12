using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAnimator : ExposableMonobehaviour
{
    const float movementAnimationSmoothTime = 0.1f;

    private Animator animator;
    private NavMeshAgent agent;

    private void OnEnable()
    {
        TurnCombatManager.onCombatStart += OnCombatStart;
        TurnCombatManager.onCombatEnd += OnCombatEnd;
    }

    private void OnDisable()
    {
        TurnCombatManager.onCombatStart -= OnCombatStart;
        TurnCombatManager.onCombatEnd -= OnCombatEnd;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnCombatStart(List<Creature> creaturesInCombat)
    {
        animator.SetBool("IsCombat", true);
    }

    private void OnCombatEnd()
    {
        animator.SetBool("IsCombat", false);
    }

    private void Update()
    {
        float speedPercentage = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", speedPercentage);
    }
}
