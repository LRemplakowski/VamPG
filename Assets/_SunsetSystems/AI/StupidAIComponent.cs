using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;
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
        [ShowInInspector, ReadOnly]
        private bool performingLogic = false;

        private EntityAction performedAction;

        private void OnEnable()
        {
            performingLogic = false;
            CombatManager.Instance.CombatBegin += OnCombatBegin;
            CombatManager.Instance.CombatEnd += OnCombatEnd;
        }

        private void OnDisable()
        {
            CombatManager.Instance.CombatBegin -= OnCombatBegin;
            CombatManager.Instance.CombatEnd -= OnCombatEnd;
            performingLogic = false;
        }

        private void Update()
        {
            if (performingLogic is false || context.IsMyTurn is false)
                return;
            if (context.Owner.HasActionsQueued is false)
                DecideWhatToDo();
        }

        private void OnCombatBegin(IEnumerable<ICombatant> combatants)
        {
            if (combatants.Contains(context.Owner))
            {
                performingLogic = true;
            }
        }
        
        private void OnCombatEnd()
        {
            performingLogic = false;
        }

        private void DecideWhatToDo()
        {
            if (context.CanMove)
            {
                GridManager grid = context.GridManager;
                Vector3Int currentGridPosition = grid.WorldPositionToGridPosition(context.Owner.References.Transform.position);
                GridUnit target = grid.GetCellsInRange(currentGridPosition, context.Owner.MovementRange, context.Owner.References.GetComponentInChildren<NavMeshAgent>(), out _).GetRandom();
                if (target != null)
                    context.Owner.MoveToGridPosition(target.GridPosition);
            }
            else if (context.CanAct)
            {
                ICombatant target = context.FriendlyCombatants
                    .Where(combatant => Vector3.Distance(combatant.References.Transform.position, context.Owner.References.Transform.position) <= context.Owner.CurrentWeapon?.GetRangeData().maxRange)
                    .GetRandom();
                if (target != null)
                    context.Owner.AttackCreatureUsingCurrentWeapon(target);
                else
                    context.Owner.SignalEndTurn();
            }
        }
    }
}
