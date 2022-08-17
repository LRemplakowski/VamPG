using Apex.AI;
using SunsetSystems.Combat;
using SunsetSystems.Game;
using UnityEngine;

namespace AI.Actions
{
    public class Idle : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            if (GameManager.IsCurrentState(GameState.Combat) &&
                context.Owner.Equals(CombatManager.Instance.CurrentActiveActor) &&
                !context.IsPlayerControlled)
            {
                Debug.LogWarning("idle in combat! creature: " + context.Owner);
            }
        }
    }
}