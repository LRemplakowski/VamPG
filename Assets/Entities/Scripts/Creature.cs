﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Creature : Entity
{
    public NavMeshAgent agent;
    
    [ReadOnly]
    [SerializeField]
    private int health = 3;
    [ReadOnly]
    [SerializeField]
    private int willpower = 2;

    public int strength, dexterity, stamina;
    public int charisma, manipulation, composure;
    public int intelligence, wits, resolve;

    [HideInInspector]
    public Queue<Action> ActionQueue { get; private set; }

    public abstract void Move(Vector3 moveTarget);

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ActionQueue = new Queue<Action>();
        ActionQueue.Enqueue(new Idle());

        health = stamina + 3;
        willpower = composure + resolve;
    }

    public void AddActionToQueue(Action action)
    {
        ActionQueue.Enqueue(action);
    }

    public void ClearAllActions()
    {
        ActionQueue.Clear();
        ActionQueue.Enqueue(new Idle());
    }

    public void Update()
    {
        if(ActionQueue.Peek().GetType() == typeof(Idle) && ActionQueue.Count > 1)
        {
            ActionQueue.Dequeue();
            ActionQueue.Peek().Begin();
        }
        if(ActionQueue.Peek().IsFinished())
        {
            Debug.Log("Action finished!");
            ActionQueue.Dequeue();
            if (ActionQueue.Count == 0) ActionQueue.Enqueue(new Idle());
            ActionQueue.Peek().Begin();
        }
    }
}
