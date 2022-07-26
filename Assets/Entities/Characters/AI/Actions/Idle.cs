using Apex.AI;
using SunsetSystems.Game;
using UnityEngine;

namespace AI.Actions
{
    public class Idle : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            if (GameManager.Instance.IsCurrentState(GameState.Combat) &&
                context.Owner.Equals(TurnCombatManager.Instance.CurrentActiveActor) &&
                !context.IsPlayerControlled)
            {
                Debug.LogWarning("idle in combat! creature: " + context.Owner);
            }
        }
    }
}