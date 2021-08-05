using Apex.AI;
using UnityEngine;

namespace AI.Actions
{
    public class Idle : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            if (StateManager.instance.GetCurrentState().Equals(GameState.Combat) && context.Owner.Equals(TurnCombatManager.instance.CurrentActiveActor) && !context.IsPlayerControlled)
            {
                Debug.LogWarning("idle in combat! creature: " + context.Owner);
            }
        }
    }
}