using Redcode.Awaiting;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Core.AddressableManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

namespace SunsetSystems.Combat.Grid
{
    public class CachedMultiLevelGrid : SerializedMonoBehaviour
    {
        [Title("Config")]
        [SerializeField, Min(1)]
        private int levelWidth = 10;
        [SerializeField, Min(1)]
        private int levelDepth = 10;
        [SerializeField, Min(1)]
        private int gridHeight;
        [SerializeField, Min(1)]
        private float gridCellSize = 1f;
        [SerializeField]
        private NavMeshAreas gridAreaMask = NavMeshAreas.All;

        [Title("References")]
        [OdinSerialize]
        private AssetReferenceT<GridUnitObject> gridObjectAsset = null;

        [Title("Instance Data")]
        [SerializeField, ReadOnly]
        private GridLevel[] levels = new GridLevel[1];
        private Dictionary<GridUnit, GridUnitObject> gridUnitGameObjects = new();

        public Vector3 GridPositionToWorldPosition(Vector3Int gridPosition)
        {
            GridUnit unit = levels[gridPosition.y][gridPosition.x, gridPosition.z];
            return transform.position + new Vector3(unit.x * unit.cellSize, unit.surfaceY, unit.z * unit.cellSize);
        }

        private void Start()
        {
            gridUnitGameObjects.Clear();
            BuildGrid();
            _ = GenerateSceneObjects(levels.SelectMany(level => level.WalkableUnits));
        }

        private async Task GenerateSceneObjects(IEnumerable<GridUnit> gridUnits)
        {
            List<Task> tasks = new();
            foreach (GridUnit unit in gridUnits)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await new WaitForUpdate();
                    Task<GameObject> instantiation = Addressables.InstantiateAsync(gridObjectAsset, transform).Task;
                    await instantiation;
                    GridUnitObject gridObj = instantiation.Result.GetComponent<GridUnitObject>();
                    gridObj.InjectUnitData(unit);
                    gridUnitGameObjects.Add(unit, gridObj);
                }));
            }
            await Task.WhenAll(tasks);
        }
        private void OnValidate()
        {
            BuildGrid();
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
                            continue;
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

        private GridUnit[,] gridCells = new GridUnit[10, 10];
        public GridUnit[,] GridCells => gridCells;
        private readonly HashSet<GridUnit> walkableUnits = new();
        public HashSet<GridUnit> WalkableUnits => walkableUnits;
        private readonly HashSet<GridUnit> coverAdjacentUnits = new();
        public HashSet<GridUnit> CoverAdjacentUnits => coverAdjacentUnits;

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
            gridCells = new GridUnit[width, depth];
        }

        public void BuildLevel(AreaMask mask)
        {
            gridCells = new GridUnit[width, depth];
            walkableUnits.Clear();
            coverAdjacentUnits.Clear();
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    GridUnit newGridUnit = new();
                    newGridUnit.x = x;
                    newGridUnit.y = yPosition;
                    newGridUnit.z = z;
                    newGridUnit.cellSize = cellSize;
                    VerifyIfIsWalkable(newGridUnit);
                    if (newGridUnit.walkable)
                    {
                        walkableUnits.Add(newGridUnit);
                        VerifyIfAdjactenToCoverSource(newGridUnit);
                        if (newGridUnit.adjacentToCover)
                            coverAdjacentUnits.Add(newGridUnit);
                    }

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
        public float cellSize;
    }
}
