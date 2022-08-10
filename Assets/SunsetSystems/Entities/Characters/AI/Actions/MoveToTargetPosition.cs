namespace AI.Actions
{
    using Apex.AI;
    using System.Collections.Generic;
    using UnityEngine;

    public class MoveToTargetPosition : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            GridElement targetPosition = context.CurrentMoveTarget;
            if (targetPosition != null)
            {
                Debug.Log("Moving to " + targetPosition);
                context.Owner.Move(targetPosition);
            }
            else
            {
                Debug.LogError("targetPosition null!");
            }
        }
    }
}