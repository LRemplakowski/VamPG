using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAnimator : ExposableMonobehaviour
{
    const float movementAnimationSmoothTime = 0.1f;

    private Animator animator;
    private NavMeshAgent agent;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float speedPercentage = agent.velocity.magnitude;
        animator.SetFloat("Speed", speedPercentage, movementAnimationSmoothTime, Time.deltaTime);
    }
}
