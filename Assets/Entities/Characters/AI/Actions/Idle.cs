using Apex.AI;
using UnityEngine;

namespace AI.Actions
{
    public class Idle : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            Debug.Log("Doing nothing");
        }
    }
}