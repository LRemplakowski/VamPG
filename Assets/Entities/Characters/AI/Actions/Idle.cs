using Apex.AI;
using SunsetSystems.Management;
using UnityEngine;

namespace AI.Actions
{
    public class Idle : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            if (StateManager.GetCurrentState().Equals(GameState.Combat) && 
                context.Owner.Equals(ReferenceManager.GetManager<TurnCombatManager>().CurrentActiveActor) && 
                !context.IsPlayerControlled)
            {
                Debug.LogWarning("idle in combat! creature: " + context.Owner);
            }
        }
    }
}