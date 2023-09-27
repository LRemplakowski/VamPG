using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.AI
{
    public class StupidAIComponent : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private IDecisionContext context;

        private void OnEnable()
        {
            _ = PerformAILogic();
        }

        private async Task PerformAILogic()
        {
            while (enabled)
            {
                if (context.IsMyTurn is false)
                    await new WaitForUpdate();
                EntityAction nextAction = DecideWhatToDo();
                if (nextAction is null)
                {
                    context.Owner.SignalEndTurn();
                }
                else
                {
                    await context.ActionPerformer.PerformAction(nextAction);
                }
            }
        }

        private EntityAction DecideWhatToDo()
        {
            if (context.CanMove)
            {
                CachedMultiLevelGrid grid = context.CurrentGrid;
                Vector3Int currentGridPosition = grid.WorldPositionToGridPosition(context.Owner.References.Transform.position);
                GridUnit target = grid.GetCellsInRange(currentGridPosition, context.Owner.MovementRange, context.Owner.References.GetComponentInChildren<NavMeshAgent>()).GetRandom();
                if (target != null)
                    return new Move(context.ActionPerformer, grid.GridPositionToWorldPosition(target.GridPosition), 0f);
                else
                    return null;
            }
            else if (context.CanAct)
            {
                ICombatant target = context.FriendlyCombatants
                    .Where(combatant => Vector3.Distance(combatant.References.Transform.position, context.Owner.References.Transform.position) <= context.Owner.CurrentWeapon.GetRangeData().maxRange)
                    .GetRandom();
                if (target != null)
                    return new Attack(target, context.Owner);
                else
                    return null;
            }
            return null;
        }
    }
}
