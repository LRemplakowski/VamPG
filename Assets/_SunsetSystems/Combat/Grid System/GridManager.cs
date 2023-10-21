using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

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

        public void HighlightCell(GridUnitObject gridUnitObject, bool highlight)
        {

        }

        public List<GridUnit> GetCellsInRange(Vector3Int gridPosition, float range, NavMeshAgent agent, out Dictionary<GridUnit, float> distanceToUnitDictionary)
        {
            return managedGrid.GetCellsInRange(gridPosition, range, agent, out distanceToUnitDictionary);
        }

        internal void HighlightCellsInRange(Vector3Int vector3Int, CombatBehaviour combatBehaviour, NavMeshAgent navMeshAgent)
        {
            throw new NotImplementedException();
        }

        internal void RestoreHighlightedCellsToPreviousState()
        {
            throw new NotImplementedException();
        }

        internal Vector3Int GetNearestWalkableGridPosition(Vector3 position)
        {
            throw new NotImplementedException();
        }
    }
}
