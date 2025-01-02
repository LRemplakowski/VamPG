using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redcode.Awaiting;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
        private float levelHeight = 2f;
        [SerializeField, Min(1), PreviouslySerializedAs("gridHeight")]
        private int gridLevelsCount;
        [SerializeField, Min(1)]
        private float gridCellSize = 1f;
        public float GridCellSize => gridCellSize;
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

        private readonly Queue<GridUnit> dirtyUnits = new();
        private readonly HashSet<ICover> cachedCoverSourcesInGrid = new();
        public HashSet<ICover> CachedCoverSources => cachedCoverSourcesInGrid;

        // Grid initialization
        private bool gridFinished = false;

        public GridUnit this[int x, int y, int z]
        {
            get
            {
                try
                {
                    return levels[y][x, z];
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.LogException(e);
                    return levels[0][0, 0];
                }
            }
            private set 
            {
                try
                {
                    levels[y][x, z] = value;
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public GridUnit this[Vector3Int position]
        {
            get => this[position.x, position.y, position.z];
            private set => this[position.x, position.y, position.z] = value;
        }

        [Title("Editor Utility")]
        [SerializeField]
        private bool showGizmosWhenNotSelected = false;

        private void Start()
        {
            BuildGrid();
            gridFinished = false;
            dirtyUnits.Clear();
            _ = GenerateSceneObjects(levels.SelectMany(level => level.WalkableUnits));
        }

        private void Update()
        {
            while (dirtyUnits.Count > 0)
            {
                GridUnit unit = dirtyUnits.Dequeue();
                if (gridUnitObjectDictionary.TryGetValue(unit, out GridUnitObject gridGameObject))
                    gridGameObject.UpdateCellState();
            }
        }

        private void OnValidate()
        {
            BuildGrid();
        }

        public Vector3 GridPositionToWorldPosition(Vector3Int gridPosition)
        {
            GridUnit unit = levels[gridPosition.y][gridPosition.x, gridPosition.z];
            return transform.position + new Vector3(unit.GridPosition.x * unit.CellSize, unit.SurfaceY, unit.GridPosition.z * unit.CellSize);
        }

        public Vector3Int WorldPositionToGridPosition(Vector3 worldPosition)
        {
            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
            Vector3Int gridPosition = Vector3Int.zero;
            localPosition.x = Mathf.Clamp(localPosition.x, 0, levelWidth * gridCellSize);
            localPosition.y = Mathf.Clamp(localPosition.y, 0, gridLevelsCount * gridCellSize);
            localPosition.z = Mathf.Clamp(localPosition.z, 0, levelDepth * gridCellSize);
            gridPosition.x = Mathf.RoundToInt(localPosition.x / gridCellSize);
            gridPosition.x = Mathf.Clamp(gridPosition.x, 0, levelWidth - 1);
            gridPosition.y = Mathf.RoundToInt(localPosition.y / gridCellSize);
            gridPosition.y = Mathf.Clamp(gridPosition.y, 0, gridLevelsCount - 1);
            gridPosition.z = Mathf.RoundToInt(localPosition.z / gridCellSize);
            gridPosition.z = Mathf.Clamp(gridPosition.z, 0, levelDepth - 1);
            return gridPosition;
        }

        public GridUnitObject GetCellGameObject(GridUnit unit)
        {
            return gridUnitObjectDictionary[unit];
        }

        public GridUnitObject GetCellGameObject(Vector3Int cellPosition)
        {
            return gridUnitObjectDictionary[this[cellPosition]];
        }

        public IEnumerable<GridUnit> GetAllWalkableGridUnits()
        {
            return levels.SelectMany(level => level.WalkableUnits);
        }

        public void MarkCellDirty(IGridCell cell)
        {
            if (cell is GridUnit gridUnit)
            {
                dirtyUnits.Enqueue(gridUnit);
            }
            else
            {
                dirtyUnits.Enqueue(this[cell.GridPosition]);
            }
        }

        #region Grid Initialization
        public async Task EnableGrid()
        {
            await new WaitUntil(() => gridFinished);
            gridUnitObjectInstances.ForEach(o => o.gameObject.SetActive(true));
        }

        public void DisableGrid()
        {
            gridUnitObjectInstances.ForEach(o => o.gameObject.SetActive(false));
        }

        private async Task GenerateSceneObjects(IEnumerable<GridUnit> gridUnits)
        {
            gridUnitObjectDictionary.Clear();
            gridUnitObjectInstances.Clear();
            Dictionary<GridUnit, Task<GameObject>> tasks = new();
            foreach (GridUnit unit in gridUnits)
            {
                tasks.Add(unit, Addressables.InstantiateAsync(gridObjectAsset, transform).Task);
            }
            await Task.WhenAll(tasks.Values);
            foreach (GridUnit unit in tasks.Keys)
            {
                GridUnitObject gridObj = tasks[unit].Result.GetComponent<GridUnitObject>();
                gridObj.gameObject.name = $"Pos: {unit.GridPosition.x};{unit.GridPosition.y};{unit.GridPosition.z}";
                gridObj.InjectUnitData(unit);
                gridUnitObjectDictionary.Add(unit, gridObj);
                gridUnitObjectInstances.Add(gridObj);
                gridObj.gameObject.SetActive(false);
            }
            gridFinished = true;
        }

        [Button]
        public void BuildGrid()
        {
            cachedCoverSourcesInGrid.Clear();
            levels = new GridLevel[gridLevelsCount];
            for (int y = 0; y < gridLevelsCount; y++)
            {
                GridLevel level = new(levelWidth, levelDepth, levelHeight, y, transform.position + Vector3.up * y, gridCellSize);
                level.BuildLevel(gridAreaMask, cachedCoverSourcesInGrid);
                levels[y] = level;
            }
        }
        #endregion

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
                        if (unit.Walkable)
                        {
                            if (unit.AdjacentToCover)
                                Gizmos.color = Color.yellow;
                            else
                                Gizmos.color = Color.blue;
                            Gizmos.DrawWireCube(transform.position + new Vector3(x, level.YPosition + .05f, z) * gridCellSize, new Vector3(gridCellSize, .1f, gridCellSize));
                            Gizmos.DrawWireSphere(transform.position + new Vector3(x, level.YPosition + .05f, z) * gridCellSize, .1f);
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (showGizmosWhenNotSelected)
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
                            if (unit.Walkable)
                            {
                                if (unit.AdjacentToCover)
                                    Gizmos.color = Color.yellow;
                                else
                                    Gizmos.color = Color.blue;
                                Gizmos.DrawWireCube(transform.position + new Vector3(x, level.YPosition + .05f, z) * gridCellSize, new Vector3(gridCellSize, .1f, gridCellSize));
                                Gizmos.DrawWireSphere(transform.position + new Vector3(x, level.YPosition + .05f, z) * gridCellSize, .1f);
                            }
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
        [SerializeField, MinValue(1f)]
        private float height = 2f;
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

        public GridLevel(int width, int depth, float height, int yPosition, Vector3 levelOrigin, float cellSize)
        {
            this.width = width;
            this.yPosition = yPosition;
            this.depth = depth;
            this.levelOrigin = levelOrigin;
            this.cellSize = cellSize;
            this.height = height;
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
                    Vector3Int gridPos = new(x, yPosition, z);
                    Vector3 worldPos = levelOrigin + new Vector3(gridPos.x * cellSize, gridPos.y * height, gridPos.z * cellSize);
                    GridUnit newGridUnit = new(gridPos, worldPos)
                    {
                        CellSize = cellSize
                    };
                    VerifyIfIsWalkable(newGridUnit);
                    if (newGridUnit.Walkable)
                    {
                        walkableUnits.Add(newGridUnit);
                        VerifyIfAdjactenToCoverSource(newGridUnit, coverSourcesCache);
                        if (newGridUnit.AdjacentToCover)
                            coverAdjacentUnits.Add(newGridUnit);
                    }
                    gridCells[x, z] = newGridUnit;
                }
            }

            void VerifyIfIsWalkable(GridUnit unit)
            {
                Vector3 originOffset;
                for (float h = height; h > -1f; h -= .1f)
                {
                    originOffset = new Vector3(unit.GridPosition.x * cellSize, h, unit.GridPosition.z * cellSize);
                    if (NavMesh.SamplePosition(levelOrigin + originOffset, out NavMeshHit hit, .11f, mask))
                    {
                        unit.SurfaceY = hit.position.y;
                        unit.Walkable = true;
                    }
                }
            }

            void VerifyIfAdjactenToCoverSource(GridUnit unit, HashSet<ICover> coverSourcesCache)
            {
                Vector3 gridUnitCenter = levelOrigin + new Vector3(unit.GridPosition.x * cellSize, height / 2, unit.GridPosition.z * cellSize);
                CoverQuality coverQuality = CoverQuality.None;
                FindCover(unit, coverSourcesCache, gridUnitCenter + Vector3.forward, ref coverQuality);
                FindCover(unit, coverSourcesCache, gridUnitCenter + Vector3.right, ref coverQuality);
                FindCover(unit, coverSourcesCache, gridUnitCenter + Vector3.back, ref coverQuality);
                FindCover(unit, coverSourcesCache, gridUnitCenter + Vector3.left, ref coverQuality);
                unit.CoverQuality = coverQuality;

                static void FindCover(GridUnit unit, HashSet<ICover> coverSourcesCache, Vector3 cellPosition, ref CoverQuality coverQuality)
                {
                    Collider[] overlap = new Collider[8];
                    int overlapCount = Physics.OverlapBoxNonAlloc(cellPosition, new Vector3(unit.CellSize / 3f, unit.CellSize / 3f, unit.CellSize / 3f), overlap);
                    if (overlapCount > 0)
                    {
                        for (int i = 0; i < overlapCount; i++)
                        {
                            if (overlap[i].TryGetComponent(out ICover cover))
                            {
                                unit.AdjacentToCover = true;
                                unit.AdjacentCoverSources.Add(cover);
                                coverQuality = cover.Quality > coverQuality ? cover.Quality : coverQuality;
                                coverSourcesCache.Add(cover);
                            }
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class GridUnit : IGridCell
    {
        public Vector3Int GridPosition { get; }
        public float SurfaceY;
        public bool Walkable;
        public bool IsInMoveRange;
        public bool IsInSprintRange;
        public bool AdjacentToCover;
        public List<ICover> AdjacentCoverSources = new();
        public CoverQuality CoverQuality;
        public ICombatant Occupier;
        public bool IsOccupied => Occupier != null;
        public bool IsFree => !IsOccupied;
        private Vector3 _worldPositionCenter;
        public Vector3 WorldPosition => new(_worldPositionCenter.x, SurfaceY, _worldPositionCenter.z);
        public float CellSize { get; set; }

        public bool Highlighted { get; set; }

        public GridUnit(Vector3Int GridPosition, Vector3 WorldPosition)
        {
            this.GridPosition = GridPosition;
            this._worldPositionCenter = WorldPosition;
        }
    }
}
