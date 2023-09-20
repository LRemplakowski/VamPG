using Redcode.Awaiting;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
        [SerializeField]
        private AssetReferenceGameObject gridObjectAsset = null;

        [Title("Instance Data")]
        [SerializeField, ReadOnly]
        private GridLevel[] levels = new GridLevel[1];

        private readonly List<GridUnitObject> gridUnitObjectInstances = new();
        private readonly Dictionary<GridUnit, GridUnitObject> gridUnitObjectDictionary = new();
        private readonly List<GridUnit> currentlyHighlitedGridUnits = new();
        private readonly HashSet<ICover> cachedCoverSourcesInGrid = new();
        public ImmutableHashSet<ICover> CachedCoverSources => new(cachedCoverSourcesInGrid);

        public Vector3 GridPositionToWorldPosition(Vector3Int gridPosition)
        {
            GridUnit unit = levels[gridPosition.y][gridPosition.x, gridPosition.z];
            return transform.position + new Vector3(unit.x * unit.cellSize, unit.surfaceY, unit.z * unit.cellSize);
        }

        public Vector3Int WorldPositionToGridPosition(Vector3 worldPosition)
        {
            return Vector3Int.zero;
        }

        private void Start()
        {
            BuildGrid();
        }

        public async Task InstantiateGrid()
        {
            await GenerateSceneObjects(levels.SelectMany(level => level.WalkableUnits));
        }

        public void CleanupGrid()
        {
            foreach (GridUnitObject gridObj in gridUnitObjectInstances)
            {
                Addressables.ReleaseInstance(gridObj.gameObject);
            }
            gridUnitObjectInstances.Clear();
            gridUnitObjectDictionary.Clear();
        }

        private async Task GenerateSceneObjects(IEnumerable<GridUnit> gridUnits)
        {
            gridUnitObjectDictionary.Clear();
            gridUnitObjectInstances.Clear();
            List<Task> tasks = new();
            foreach (GridUnit unit in gridUnits)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await new WaitForUpdate();
                    Task<GameObject> instantiation = Addressables.InstantiateAsync(gridObjectAsset, transform).Task;
                    await instantiation;
                    GridUnitObject gridObj = instantiation.Result.GetComponent<GridUnitObject>();
                    gridObj.gameObject.name = $"Pos: {unit.x};{unit.y};{unit.z}";
                    gridObj.InjectUnitData(unit);
                    if (unit.adjacentToCover)
                        gridObj.SetGridCellState(GridUnitObject.GridCellState.NearCover);
                    else
                        gridObj.SetGridCellState(GridUnitObject.GridCellState.Default);
                    gridUnitObjectDictionary.Add(unit, gridObj);
                    gridUnitObjectInstances.Add(gridObj);
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
            cachedCoverSourcesInGrid.Clear();
            levels = new GridLevel[gridHeight];
            for (int y = 0; y < gridHeight; y++)
            {
                GridLevel level = new(levelWidth, levelDepth, y, transform.position + Vector3.up * y, gridCellSize);
                level.BuildLevel(gridAreaMask, cachedCoverSourcesInGrid);
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

        public GridUnitObject GetNearestGridElement(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public void HighlightCellsInRange(Vector3Int gridPosition, int range, NavMeshAgent agent)
        {
            foreach (GridUnit unit in currentlyHighlitedGridUnits)
            {
                gridUnitObjectDictionary[unit].RestoreCachedPreviousVisualState();
            }
            currentlyHighlitedGridUnits.Clear();
            currentlyHighlitedGridUnits.AddRange(GetCellsInRange(gridPosition, range, agent));
            foreach (GridUnit unit in currentlyHighlitedGridUnits)
            {
                GridUnitObject unitObj = gridUnitObjectDictionary[unit];
                if (unitObj.CurrentCellState == GridUnitObject.GridCellState.Default)
                    unitObj.SetGridCellState(GridUnitObject.GridCellState.Walkable, true);
            }
        }

        public List<GridUnit> GetCellsInRange(Vector3Int gridPosition, int range, NavMeshAgent agent)
        {
            List<GridUnit> unitsInRange = new();
            // Calculate the path
            NavMeshPath path = new();
            // Calculate the maximum grid distance within range
            float maxGridDistance = range * gridCellSize;

            foreach (GridLevel level in levels)
            {
                foreach (GridUnit unit in level.WalkableUnits)
                {
                    // Calculate the grid distance between gridPosition and unit's position
                    int gridDistance = Mathf.Abs(gridPosition.x - unit.x) + Mathf.Abs(gridPosition.z - unit.z);

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
                                unitsInRange.Add(unit);
                        }
                        path.ClearCorners();
                    }
                }
            }
            return unitsInRange;
        }

        public void RestoreHighlightedCellsToPreviousState()
        {

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

        public void BuildLevel(AreaMask mask, HashSet<ICover> coverSourcesCache)
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
                        VerifyIfAdjactenToCoverSource(newGridUnit, coverSourcesCache);
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

            void VerifyIfAdjactenToCoverSource(GridUnit unit, HashSet<ICover> coverSourcesCache)
            {
                Vector3 gridUnitCenter = levelOrigin + new Vector3(unit.x * cellSize, cellSize / 2, unit.z * cellSize);
                if (Physics.BoxCast(gridUnitCenter, Vector3.one * cellSize / 4, Vector3.forward, out RaycastHit lastHit, Quaternion.identity, cellSize))
                {
                    if (lastHit.collider.TryGetComponent(out ICover cover))
                    {
                        unit.adjacentToCover = true;
                        coverSourcesCache.Add(cover);
                    }
                }
                if (Physics.BoxCast(gridUnitCenter, Vector3.one * cellSize / 4, Vector3.right, out lastHit, Quaternion.identity, cellSize))
                {
                    if (lastHit.collider.TryGetComponent(out ICover cover))
                    {
                        unit.adjacentToCover = true;
                        coverSourcesCache.Add(cover);
                    }
                }
                if (Physics.BoxCast(gridUnitCenter, Vector3.one * cellSize / 4, Vector3.back, out lastHit, Quaternion.identity, cellSize))
                {
                    if (lastHit.collider.TryGetComponent(out ICover cover))
                    {
                        unit.adjacentToCover = true;
                        coverSourcesCache.Add(cover);
                    }
                }
                if (Physics.BoxCast(gridUnitCenter, Vector3.one * cellSize / 4, Vector3.left, out lastHit, Quaternion.identity, cellSize))
                {
                    if (lastHit.collider.TryGetComponent(out ICover cover))
                    {
                        unit.adjacentToCover = true;
                        coverSourcesCache.Add(cover);
                    }
                }
            }
        }
    }

    [Serializable]
    public class GridUnit
    {
        public int x, y, z;
        public Vector3Int GridPosition => new(x, y, z);
        public float surfaceY;
        public bool walkable;
        public bool adjacentToCover;
        public float cellSize;
    }
}
