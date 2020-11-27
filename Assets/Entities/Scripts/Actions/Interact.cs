using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Interact : Action
{
    private IInteractable target;
    private Move moveToTarget;

    public Interact(IInteractable target, Creature owner)
    {
        this.target = target;
        this.owner = owner;
        conditions.Add(new InteractionComplete(target));
    }

    public override void Begin()
    {
        float distance = Vector3.Distance(target.GetTransform().position, owner.transform.position);
        if(distance > target.InteractionDistance)
        {
            moveToTarget = new Move(owner.gameObject.GetComponent<NavMeshAgent>(), target.GetTransform().position);
            moveToTarget.Begin();
            owner.StartCoroutine(InteractIfCloseEnough());
        }
        else
        {
            target.TargetedBy = owner.gameObject;
            target.Interact();
        }
    }

    private IEnumerator InteractIfCloseEnough()
    {
        yield return new WaitUntil(() => moveToTarget.IsFinished());

        target.TargetedBy = owner.gameObject;
        target.Interact();
        owner.StopCoroutine(InteractIfCloseEnough());

    }
}
