using Apex.AI;
using SunsetSystems.Entities.Characters.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Actions
{

    public class MoveToTargetPosition : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            GridElement targetPosition = context.CurrentMoveTarget;
            if (targetPosition != null)
            {
                Debug.Log("Moving to " + targetPosition);
                context.ActionPerformer.PerformAction(new Move(context.ActionPerformer, targetPosition.WorldPosition, 0f));
                context.CurrentMoveTarget = null;
            }
            else
            {
                Debug.LogError("targetPosition null!");
            }
        }
    }
}