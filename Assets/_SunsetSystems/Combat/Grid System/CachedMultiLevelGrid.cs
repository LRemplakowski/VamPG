using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Combat.Grid
{
    public class CachedMultiLevelGrid : SerializedMonoBehaviour
    {
        [SerializeField, Min(1)]
        private int levelWidth = 10, levelDepth = 10;
        [SerializeField, Min(1)]
        private int gridHeight;
        [SerializeField, Min(1)]
        private float gridCellSize = 1f;
        [SerializeField]
        private NavMeshAreas gridAreaMask = NavMeshAreas.All;

        [SerializeField]
        private GridLevel[] levels = new GridLevel[1];

        public Vector3 GridPositionToWorldPosition(Vector3 gridPosition)
        {
            return transform.position + gridPosition * gridCellSize;
        }

        [Button]
        public void BuildGrid()
        {
            levels = new GridLevel[gridHeight];
            for (int y = 0; y < gridHeight; y++)
            {
                GridLevel level = new(levelWidth, levelDepth, y, transform.position + Vector3.up * y, gridCellSize);
                level.BuildLevel(gridAreaMask);
                levels[y] = level;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            foreach (GridLevel level in levels)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    for (int z = 0; z < levelDepth; z++)
                    {
                        GridUnit unit = level[x, z];
                        if (unit == null)
                            return;
                        if (unit.walkable)
                        {
                            if (unit.adjacentToCover)
                                Gizmos.color = Color.yellow;
                            else
                                Gizmos.color = Color.blue;
                            Gizmos.DrawWireCube(transform.position + new Vector3(x, unit.surfaceY + .05f, z) * gridCellSize, new Vector3(gridCellSize, .1f, gridCellSize));
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class GridLevel
    {
        [SerializeField, MinValue(1)]
        private int width = 10, depth = 10;
        [SerializeField, MinValue(0)]
        private int yPosition = 0;
        [SerializeField]
        private Vector3 levelOrigin = Vector3.zero;
        [SerializeField]
        private float cellSize = 1f;
        public int YPosition => yPosition;

        [SerializeField]
        private GridUnit[,] gridCells = new GridUnit[10, 10];
        public GridUnit[,] GridCells => gridCells;

        public GridUnit this[int x, int z]
        {
            get => gridCells[x, z];
            set => gridCells[x, z] = value;
        }

        public GridLevel(int width, int depth, int yPosition, Vector3 levelOrigin, float cellSize)
        {
            this.width = width;
            this.yPosition = yPosition;
            this.depth = depth;
            this.levelOrigin = levelOrigin;
            this.cellSize = cellSize;
            gridCells = new GridUnit[width, depth];
        }

        public GridLevel()
        {
            gridCells = new GridUnit[10, 10];
        }

        public void BuildLevel(AreaMask mask)
        {
            gridCells = new GridUnit[width, depth];
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    GridUnit newGridUnit = new();
                    newGridUnit.x = x;
                    newGridUnit.y = yPosition;
                    newGridUnit.z = z;
                    VerifyIfIsWalkable(newGridUnit);
                    VerifyIfAdjactenToCoverSource(newGridUnit);
                    gridCells[x, z] = newGridUnit;
                }
            }

            void VerifyIfIsWalkable(GridUnit unit)
            {
                for (int i = 0; i < cellSize * 10; i++)
                {
                    if (NavMesh.SamplePosition(levelOrigin + new Vector3(unit.x * cellSize, .1f * i, unit.z * cellSize), out NavMeshHit hit, .05f, mask))
                    {
                        unit.surfaceY = hit.position.y;
                        unit.walkable = true;
                    }
                }
            }

            void VerifyIfAdjactenToCoverSource(GridUnit unit)
            {
                Vector3 gridUnitCenter = levelOrigin + new Vector3(unit.x * cellSize, cellSize / 2, unit.z * cellSize);
                if (Physics.BoxCast(gridUnitCenter, Vector3.one * cellSize / 4, Vector3.forward, out RaycastHit lastHit, Quaternion.identity, cellSize) ||
                    Physics.BoxCast(gridUnitCenter, Vector3.one * cellSize / 4, Vector3.right, out lastHit, Quaternion.identity, cellSize) ||
                    Physics.BoxCast(gridUnitCenter, Vector3.one * cellSize / 4, Vector3.back, out lastHit, Quaternion.identity, cellSize) ||
                    Physics.BoxCast(gridUnitCenter, Vector3.one * cellSize / 4, Vector3.left, out lastHit, Quaternion.identity, cellSize))
                {
                    if (lastHit.collider.TryGetComponent<ICover>(out _))
                        unit.adjacentToCover = true;
                }
            }
        }
    }

    [Serializable]
    public class GridUnit
    {
        public int x, y, z;
        public float surfaceY;
        public bool walkable;
        public bool adjacentToCover;
    }
}
