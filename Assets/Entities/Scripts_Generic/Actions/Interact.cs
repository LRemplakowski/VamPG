using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Interact : EntityAction
{
    private readonly IInteractable target;
    private Move moveToTarget;

    protected override Creature Owner 
    { 
        get; 
        set; 
    }

    public Interact(IInteractable target, Creature owner)
    {
        this.target = target;
        Owner = owner;
        conditions.Add(new InteractionComplete(target));
    }

    public override void Abort()
    {
        Owner.StopCoroutine(InteractIfCloseEnough());
        Owner.StopCoroutine(Owner.FaceTarget(target.InteractionTransform));
        if (moveToTarget != null)
            moveToTarget.Abort();
        target.Interacted = false;
    }

    public override void Begin()
    {
        float distance = Vector3.Distance(target.InteractionTransform.position, Owner.transform.position);
        if (distance > target.InteractionDistance)
        {
            moveToTarget = new Move(Owner.gameObject.GetComponent<NavMeshAgent>(), target.InteractionTransform.position);
            moveToTarget.Begin();
            Owner.StartCoroutine(InteractIfCloseEnough());
        }
        else
        {
            Owner.StartCoroutine(Owner.FaceTarget(target.InteractionTransform));
            target.TargetedBy = Owner;
            target.Interact();
        }
    }

    private IEnumerator InteractIfCloseEnough()
    {
        yield return new WaitUntil(() => Vector3.Distance(target.InteractionTransform.position, Owner.transform.position) <= target.InteractionDistance);
        moveToTarget.Abort();
        yield return new WaitUntil(() => Owner.RotateTowardsTarget(target.InteractionTransform));

        target.TargetedBy = Owner;
        target.Interact();
        Owner.StopCoroutine(InteractIfCloseEnough());
    }
}
