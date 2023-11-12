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
            GridManager grid = context.GridManager;
            Vector3Int currentGridPosition = grid.WorldPositionToGridPosition(context.Owner.References.Transform.position);
            if (context.CanMove)
            {
                GridUnit target = null;
                switch (context.CurrentWeaponType)
                {
                    case Inventory.WeaponType.Melee:
                        target = FindGridPositionAdjacentToClosestEnemy(grid, currentGridPosition);
                        target ??= FindRandomGridPositionInMovementRange(grid, currentGridPosition);
                        if (target != null)
                        {
                            context.Owner.MoveToGridPosition(target.GridPosition);
                        }
                        break;
                    case Inventory.WeaponType.Ranged:
                        target = FindRandomGridPositionInMovementRange(grid, currentGridPosition);
                        if (target != null)
                            context.Owner.MoveToGridPosition(target.GridPosition);
                        break;
                }
            }
            else if (context.CanAct)
            {
                ICombatant target = null;
                switch (context.CurrentWeaponType)
                {
                    case Inventory.WeaponType.Melee:
                        target = FindTargetInAdjacentSquares(grid, currentGridPosition);
                        break;
                    case Inventory.WeaponType.Ranged:
                        target = context.FriendlyCombatants
                                        .Where(combatant => Vector3.Distance(combatant.References.Transform.position, context.Owner.References.Transform.position) <= context.Owner.CurrentWeapon?.GetRangeData().maxRange)
                                        .GetRandom();
                        break;
                }
                if (target != null)
                    context.Owner.AttackCreatureUsingCurrentWeapon(target);
                else
                    context.Owner.SignalEndTurn();
            }
        }

        private GridUnit FindRandomGridPositionInMovementRange(GridManager grid, Vector3Int currentGridPosition)
        {
            return grid.GetCellsInRange(currentGridPosition, context.Owner.MovementRange, context.Owner.References.GetComponentInChildren<NavMeshAgent>(), out _).FindAll(cell => cell.IsOccupied is false).GetRandom();
        }

        private GridUnit FindGridPositionAdjacentToClosestEnemy(GridManager grid, Vector3Int currentGridPosition)
        {
            ICombatant nearestEnemy = null;
            float distance = float.MaxValue;
            foreach (ICombatant enemy in context.FriendlyCombatants)
            {
                if (Vector3.Distance(context.Owner.Transform.position, enemy.Transform.position) < distance)
                    nearestEnemy = enemy;
            }
            GridUnit unit = null;
            distance = float.MaxValue;
            if (nearestEnemy != null)
            {
                Vector3Int enemyGridPosition = grid.WorldPositionToGridPosition(nearestEnemy.Transform.position);
                for (int x = -1; x < 1; x++)
                {
                    for (int z = -1; z < 1; z++)
                    {
                        GridUnit newUnit = grid[enemyGridPosition + new Vector3Int(x, 0, z)];
                        if (unit.IsFree)
                        {
                            if (Vector3Int.Distance(newUnit.GridPosition, currentGridPosition) < distance)
                                unit = newUnit;
                        }
                    }
                }
            }
            return unit;
        }

        private ICombatant FindTargetInAdjacentSquares(GridManager grid, Vector3Int currentGridPosition)
        {
            ICombatant result = null;
            for (int x = -1; x < 1; x++)
            {
                for (int z = -1; z < 1; z++)
                {
                    result = grid[currentGridPosition + new Vector3Int(x, 0, z)].Occupier;
                    if (result != null && (result.Faction == Faction.PlayerControlled || result.Faction == Faction.Friendly))
                        return result;
                }
            }
            return result;
        }
    }
}
