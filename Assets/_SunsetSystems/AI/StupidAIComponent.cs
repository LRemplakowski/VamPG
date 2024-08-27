using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Utils.Extensions;
using UnityEngine;

namespace SunsetSystems.AI
{
    public class StupidAIComponent : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private IDecisionContext context;
        [ShowInInspector, ReadOnly]
        private bool performingLogic = false;
        [ShowInInspector]
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
                        if (target != null)
                            context.Owner.MoveToGridPosition(target.GridPosition);
                        else
                            context.Owner.SignalEndTurn();
                        break;
                    case Inventory.WeaponType.Ranged:
                        target = FindRandomGridPositionInMovementRange(grid, currentGridPosition);
                        if (target != null)
                            context.Owner.MoveToGridPosition(target.GridPosition);
                        else
                            context.Owner.SignalEndTurn();
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
            else
            {
                context.Owner.SignalEndTurn();
            }
        }

        private GridUnit FindRandomGridPositionInMovementRange(GridManager grid, Vector3Int currentGridPosition)
        {
            return grid.GetCellsInRange(currentGridPosition, context.Owner.MovementRange, context.Owner.References.NavigationManager, out _).FindAll(cell => cell.IsOccupied is false).GetRandom();
        }

        private GridUnit FindGridPositionAdjacentToClosestEnemy(GridManager grid, Vector3Int currentGridPosition)
        {
            ICombatant nearestEnemy = null;
            float distance = float.MaxValue;
            foreach (ICombatant enemy in context.FriendlyCombatants)
            {
                float newDistance = Vector3.Distance(context.Owner.Transform.position, enemy.Transform.position);
                if (newDistance < distance)
                {
                    nearestEnemy = enemy;
                    distance = newDistance;
                }
            }
            GridUnit unit = null;
            distance = float.MaxValue;
            List<GridUnit> positionList = grid.GetCellsInRange(currentGridPosition, context.Owner.MovementRange, context.Owner.References.NavigationManager, out _);
            if (nearestEnemy != null)
            {
                Vector3Int enemyGridPosition = grid.WorldPositionToGridPosition(nearestEnemy.Transform.position);
                List<GridUnit> walkableCellsNearEnemy = grid.GetCellsInRange(enemyGridPosition, 1.5f, nearestEnemy.References.NavigationManager, out _);
                IEnumerable<GridUnit> commonElements = positionList.Intersect(walkableCellsNearEnemy);
                foreach (GridUnit commonUnit in commonElements)
                {
                    if (commonUnit != null && commonUnit.IsFree)
                    {
                        float distanceToCommonUnit = Vector3Int.Distance(currentGridPosition, commonUnit.GridPosition);
                        if (distanceToCommonUnit < distance)
                        {
                            unit = commonUnit;
                            distance = distanceToCommonUnit;
                        }
                    }
                }
            }
            if (unit == null)
                unit = FindRandomGridPositionInMovementRange(grid, currentGridPosition);
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
                    else
                        result = null;
                }
            }
            return result;
        }
    }
}
