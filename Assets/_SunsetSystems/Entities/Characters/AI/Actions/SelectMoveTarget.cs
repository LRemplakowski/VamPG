/*using Apex.AI;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Actions
{
    public class SelectMoveTarget : ActionWithOptions<GridElement>
    {
        public override void Execute(IAIContext context)
        {
            CreatureContext c = context as CreatureContext;
            List<GridElement> elementsInMoveRange = c.PositionsInRange;

            GridElement moveTarget = this.GetBest(c, elementsInMoveRange);

            if (moveTarget != null)
            {
                Debug.Log($"Selected move target: {moveTarget}");
                c.CurrentMoveTarget = moveTarget;
            }
            else
            {
                Debug.LogError("Selected move target null!");
                c.Behaviour.HasMoved = true;
            }
        }
    }
}*/