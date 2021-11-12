using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utils.Singleton;

public class Player : Creature
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public override void Move(Vector3 moveTarget)
    {
        AddActionToQueue(new Move(GetComponent<NavMeshAgent>(), moveTarget));
    }

    public override void Move(GridElement moveTarget)
    {
        CurrentGridPosition = moveTarget;
        AddActionToQueue(new Move(GetComponent<NavMeshAgent>(), moveTarget.transform.position));
    }

    public void InteractWith(IInteractable target)
    {
        AddActionToQueue(new Interact(target, this));
    }

    public override void Attack(Creature target)
    {
        AddActionToQueue(new Attack(target, this));
    }
}
