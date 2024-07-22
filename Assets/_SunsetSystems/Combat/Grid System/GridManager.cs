using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using System.Linq;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Entities.Characters.Navigation;

namespace SunsetSystems.Combat.Grid
{
    public class GridManager : SerializedMonoBehaviour
    {
        public GridUnit this[int x, int y, int z]
        {
            get => managedGrid[x, y, z];
        }

        public GridUnit this[Vector3Int position]
        {
            get => managedGrid[position];
        }

        [SerializeField]
        private CachedMultiLevelGrid managedGrid;

        private GridUnit currentlyHighlightedCell;
        private readonly List<GridUnit> currentlyHighlitedGridUnits = new();


        public Vector3 GridPositionToWorldPosition(Vector3Int gridPosition) => managedGrid.GridPositionToWorldPosition(gridPosition);
        public Vector3Int WorldPositionToGridPosition(Vector3 worldPosition) => managedGrid.WorldPositionToGridPosition(worldPosition);

        public async void EnableGrid()
        {
            await managedGrid.EnableGrid();
        }

        public void DisableGrid()
        {
            managedGrid.DisableGrid();
        }

        public void HighlightCell(IGridCell gridCell)
        {
            ClearHighlightedCell();
            if (gridCell == null)
                return;
            GridUnit cellData = this[gridCell.GridPosition];
            cellData.Highlighted = true;
            currentlyHighlightedCell = cellData;
            managedGrid.MarkCellDirty(currentlyHighlightedCell);
        }

        public void HandleCombatantMovedIntoGridCell(ICombatant combatant, IGridCell cell)
        {
            GridUnit cellObject = this[cell.GridPosition];
            cellObject.Occupier = combatant;
            managedGrid.MarkCellDirty(cellObject);
        }

        public void ClearOccupierFromCell(IGridCell cell)
        {
            this[cell.GridPosition].Occupier = null;
            managedGrid.MarkCellDirty(cell);
        }

        public void ClearHighlightedCell()
        {
            if (currentlyHighlightedCell == null)
                return;
            currentlyHighlightedCell.Highlighted = false;
            managedGrid.MarkCellDirty(currentlyHighlightedCell);
            currentlyHighlightedCell = null;
        }

        public List<GridUnit> GetCellsInRange(Vector3Int gridPosition, float range, INavigationManager agent, out Dictionary<GridUnit, float> distanceToUnitDictionary)
        {
            distanceToUnitDictionary = new();
            List<GridUnit> unitsInRange = new();
            // Calculate the path
            NavMeshPath path = new();
            // Calculate the maximum grid distance within range
            float maxGridDistance = range * managedGrid.GridCellSize;

            foreach (GridUnit unit in managedGrid.GetAllWalkableGridUnits())
            {
                // Calculate the grid distance between gridPosition and unit's position
                float gridDistance = (new Vector2(gridPosition.x, gridPosition.z) - new Vector2(unit.GridPosition.x, unit.GridPosition.z)).magnitude;

                if (gridDistance <= maxGridDistance)
                {
                    Vector3 unitWorldPosition = GridPositionToWorldPosition(unit.GridPosition);
                    if (agent.CalculatePath(unitWorldPosition, path))
                    {
                        // Calculate path length
                        float pathLength = 0;
                        for (int i = 1; i < path.corners.Length; i++)
                        {
                            pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                        }
                        if (pathLength <= maxGridDistance)
                        {
                            unitsInRange.Add(unit);
                            distanceToUnitDictionary[unit] = pathLength;
                        }
                    }
                    path.ClearCorners();
                }
            }
            return unitsInRange;
        }

        public void ShowCellsInMovementRange(ICombatant combatant)
        {
            HideCellsInMovementRange();
            Vector3Int gridPosition = WorldPositionToGridPosition(combatant.References.Transform.position);
            var navigationManager = combatant.References.NavigationManager;
            currentlyHighlitedGridUnits.Clear();
            currentlyHighlitedGridUnits.AddRange(GetCellsInRange(gridPosition, combatant.SprintRange + (managedGrid.GridCellSize / 2), navigationManager, out Dictionary<GridUnit, float> distanceToUnitDictionary));
            foreach (GridUnit unit in currentlyHighlitedGridUnits)
            {
                float distanceToUnit = distanceToUnitDictionary[unit];
                if (distanceToUnit <= combatant.MovementRange + (managedGrid.GridCellSize / 2) && !combatant.HasMoved)
                {
                    unit.IsInMoveRange = true;
                    managedGrid.MarkCellDirty(unit);
                }
                if (distanceToUnit <= combatant.SprintRange + (managedGrid.GridCellSize / 2) && !combatant.HasMoved && !combatant.HasActed)
                {
                    unit.IsInSprintRange = true;
                    managedGrid.MarkCellDirty(unit);
                }
            }
        }

        public void HideCellsInMovementRange()
        {
            foreach (GridUnit unit in currentlyHighlitedGridUnits)
            {
                unit.IsInSprintRange = false;
                unit.IsInMoveRange = false;
                managedGrid.MarkCellDirty(unit);
            }
        }

        public GridUnitObject GetNearestWalkableGridCell(Vector3 position)
        {
            Vector3Int gridPosition = GetNearestWalkableGridPosition(position);
            return managedGrid.GetCellGameObject(gridPosition);
        }

        public Vector3Int GetNearestWalkableGridPosition(Vector3 position)
        {
            Vector3Int bestGridPos = WorldPositionToGridPosition(position);
            GridUnit unit = this[bestGridPos.x, bestGridPos.y, bestGridPos.z];
            if (unit.Walkable)
            {
                return bestGridPos;
            }
            else
            {
                unit = CrawlForNearestWalkablePosition(unit, managedGrid);
                return unit.GridPosition;
            }

            static GridUnit CrawlForNearestWalkablePosition(GridUnit relativeTo, CachedMultiLevelGrid managedGrid)
            {
                IEnumerable<GridUnit> allWalkableUnits = managedGrid.GetAllWalkableGridUnits();
                GridUnit result = allWalkableUnits.FirstOrDefault();
                float gridDistance = float.MaxValue;
                foreach (GridUnit gridUnit in allWalkableUnits)
                {
                    float newDistance = Mathf.Abs((gridUnit.GridPosition - relativeTo.GridPosition).magnitude);
                    if (newDistance < gridDistance)
                    {
                        gridDistance = newDistance;
                        result = gridUnit;
                    }
                }
                return result;
            }
        }
    }
}
