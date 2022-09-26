using Entities.Characters;
using NaughtyAttributes;
using SunsetSystems.Combat;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAnimator : ExposableMonobehaviour
{
    const float movementAnimationSmoothTime = 0.1f;

    [SerializeField, ReadOnly]
    private Animator animator;
    [SerializeField, ReadOnly]
    private NavMeshAgent agent;

    private void OnEnable()
    {
        CombatManager.CombatBegin += OnCombatStart;
        CombatManager.CombatEnd += OnCombatEnd;
    }

    private void OnDisable()
    {
        CombatManager.CombatBegin -= OnCombatStart;
        CombatManager.CombatEnd -= OnCombatEnd;
    }

    private void Start()
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
