using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Creature
{
    [SerializeField]
    private Transform _lineTarget;

    public Transform LineTarget
    {
        get => _lineTarget;
        private set => _lineTarget = value;
    }

    public override void Move(Vector3 moveTarget)
    {
        ClearAllActions();
        AddActionToQueue(new Move(agent, moveTarget));
    }

    public override void Move(GridElement moveTarget)
    {
        ClearAllActions();
        CurrentGridPosition = moveTarget;
        AddActionToQueue(new Move(agent, moveTarget.transform.position));
    }

    public override void Attack(Creature target)
    {
        ClearAllActions();
        AddActionToQueue(new Attack(target, this));
    }

    protected override void Start()
    {
        base.Start();
        if (!LineTarget)
            LineTarget = CreateDefaultLineTarget();
    }

    private Transform CreateDefaultLineTarget()
    {
        GameObject lt = new GameObject("Default Line Target");
        lt.transform.parent = this.transform;
        lt.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z);
        return lt.transform;
    }
}
