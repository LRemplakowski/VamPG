using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Creature : MonoBehaviour
{
    public NavMeshAgent agent;

    public int health;
    public int willpower;

    public int strength, dexterity, stamina;
    public int charisma, manipulation, composure;
    public int intelligence, wits, resolve;

    private Queue<Action> _actionQueue;
    [HideInInspector]
    public Queue<Action> ActionQueue
    {
        get => _actionQueue;
    }

    public abstract void Move(Vector3 moveTarget);

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _actionQueue = new Queue<Action>();
        _actionQueue.Enqueue(new Idle());
    }

    public void AddActionToQueue(Action action)
    {
        _actionQueue.Enqueue(action);
    }

    public void ClearAllActions()
    {
        _actionQueue.Clear();
        _actionQueue.Enqueue(new Idle());
    }

    public void Update()
    {
        if(_actionQueue.Peek().GetType() == typeof(Idle) && _actionQueue.Count > 1)
        {
            _actionQueue.Dequeue();
            _actionQueue.Peek().Begin();
        }
        if(_actionQueue.Peek().IsFinished())
        {
            Debug.Log("Action finished!");
            _actionQueue.Dequeue();
            if (_actionQueue.Count == 0) _actionQueue.Enqueue(new Idle());
            _actionQueue.Peek().Begin();
        }
    }
}
