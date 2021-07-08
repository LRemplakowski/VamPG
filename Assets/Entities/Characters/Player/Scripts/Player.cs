using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayableCharacterCombatBehaviour)), RequireComponent(typeof(PlayerControlScript))]
public class Player : Creature
{
    public override void Move(Vector3 moveTarget)
    {
        ClearAllActions();
        AddActionToQueue(new Move(GetComponent<NavMeshAgent>(), moveTarget));
    }

    public override void Move(GridElement moveTarget)
    {
        ClearAllActions();
        CurrentGridPosition = moveTarget;
        AddActionToQueue(new Move(GetComponent<NavMeshAgent>(), moveTarget.transform.position));
    }

    public void InteractWith(IInteractable target)
    {
        ClearAllActions();
        AddActionToQueue(new Interact(target, this));
    }

    public override void Attack(Creature target)
    {
        ClearAllActions();
        AddActionToQueue(new Attack(target, this));
    }
}
