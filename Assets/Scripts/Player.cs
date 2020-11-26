using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : Vampire
{
    public override void Move(Vector3 moveTarget)
    {
        agent.SetDestination(moveTarget);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
