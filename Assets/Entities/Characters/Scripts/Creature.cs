using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Inventory))]
public abstract class Creature : Entity
{
    private const int healthBase = 3;
    private const float lookTowardsRotationSpeed = 5.0f;

    public NavMeshAgent agent;
    public Inventory inventory;

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
    public Queue<EntityAction> ActionQueue { get; private set; }

    public abstract void Move(Vector3 moveTarget);

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        agent = GetComponent<NavMeshAgent>();
        ActionQueue = new Queue<EntityAction>();
        ActionQueue.Enqueue(new Idle());

        health = stamina + healthBase;
        willpower = composure + resolve;
    }

    public void AddActionToQueue(EntityAction action)
    {
        ActionQueue.Enqueue(action);
    }

    public void ClearAllActions()
    {
        ActionQueue.Peek().Abort();
        ActionQueue.Clear();
        ActionQueue.Enqueue(new Idle());
    }

    public bool RotateTowardsTarget(Transform target)
    {
        if (target == null)
            return true;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookTowardsRotationSpeed);
        float dot = Quaternion.Dot(transform.rotation, lookRotation);
        return dot >= 0.999f || dot <= -0.999f;
    }

    public IEnumerator FaceTarget(Transform target)
    {
        yield return new WaitUntil(() => RotateTowardsTarget(target));
        StopCoroutine(FaceTarget(target));
    }

    public void Update()
    {
        if (ActionQueue.Peek().GetType() == typeof(Idle) && ActionQueue.Count > 1)
        {
            ActionQueue.Dequeue();
            ActionQueue.Peek().Begin();
        }
        if (ActionQueue.Peek().IsFinished())
        {
            Debug.Log("Action finished!");
            ActionQueue.Dequeue();
            if (ActionQueue.Count == 0) 
                ActionQueue.Enqueue(new Idle());
            ActionQueue.Peek().Begin();
        }
    }
}
