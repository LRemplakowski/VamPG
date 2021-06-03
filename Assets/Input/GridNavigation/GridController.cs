using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class GridController : ExposableMonobehaviour
{
    [SerializeField]
    private GridElement gridElementPrefab;
    [SerializeField]
    private int rows, columns;
    [SerializeField, ReadOnly]
    private Vector3 bottomLeft;

    [SerializeField, ReadOnly]
    private GridElement[,] gridElements;

    private List<GridElement> activeElements = new List<GridElement>();

    [SerializeField]
    private float scale = 1f;
    [SerializeField, Range(0, 10)]
    private int numOfLevels = 0;

    private NavMeshAgent playerAgent;

    //private bool _highlightGrid;
    //[SerializeField, ExposeProperty]
    //public bool HighlightGrid
    //{
    //    get => _highlightGrid;
    //    set
    //    {
    //        _highlightGrid = value;
    //        if (_highlightGrid)
    //        {
    //            SetElementsInRangeActive();
    //            HighlightGrid = false;
    //        }
    //    }
    //}

    private void OnEnable()
    {
        Move.onMovementFinished += SetElementsInRangeActive;
        Move.onMovementStarted += ClearActiveElements;
    }

    private void OnDisable()
    {
        Move.onMovementFinished -= SetElementsInRangeActive;
        Move.onMovementStarted -= ClearActiveElements;
    }

    public void Start()
    {
        gridElements = new GridElement[columns, rows];
        bottomLeft = transform.position;
        playerAgent = GameManager.GetPlayer().GetComponent<NavMeshAgent>();
        GenerateGrid();
        //TempMovePlayerToNearestGridElement();
    }

    private void GenerateGrid()
    {
        for (int x=0; x < columns; x++)
        {
            for (int y=0; y < rows; y++)
            {
                if (numOfLevels > 0)
                {
                    for (int z=0; z < numOfLevels/scale * 2; z++)
                    {
                        Vector3 pos = new Vector3(bottomLeft.x + (x * scale), bottomLeft.y + (z * scale / 2), bottomLeft.z + (y * scale));
                        if (MaybeCreateGridElement(pos, x, y))
                            break;
                    }
                }
                else
                {
                    Vector3 pos = new Vector3(bottomLeft.x + (x * scale), bottomLeft.y, bottomLeft.z + (y * scale));
                    MaybeCreateGridElement(pos, x, y);
                }
            }
        }
    }

    private bool MaybeCreateGridElement(Vector3 pos, int column, int row)
    {
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, playerAgent.radius/2, NavMesh.AllAreas))
        {
            GridElement g = Instantiate(gridElementPrefab, hit.position, Quaternion.identity);
            g.transform.parent = this.transform;
            g.transform.localScale = new Vector3(scale, scale, scale);
            g.GridPosition = new Vector2Int(column, row);
            gridElements[column, row] = g;
            g.gameObject.SetActive(false);
            g.Visited = GridElement.Status.NotVisited;
            return true;
        }
        return false;
    }

    public void MoveCreatureToNearestGridElement(Creature creature)
    {
        float distance = float.MaxValue;
        GridElement nearest = null;
        foreach (GridElement g in gridElements)
        {
            if (g == null)
                continue;
            float newDistance = Vector3.Distance(creature.transform.position, g.transform.position);
            if (newDistance < distance)
            {
                nearest = g;
                distance = newDistance;
            }
        }
        if (nearest != null)
            creature.Move(nearest);
    }

    public void ClearActiveElements()
    {
        foreach (GridElement g in activeElements)
        {
            g.Visited = GridElement.Status.NotVisited;
            g.gameObject.SetActive(false);
        }
        activeElements.Clear();
    }

    private void SetElementsInRangeActive()
    {
        Creature currentActiveActor = TurnCombatManager.instance.CurrentActiveActor;
        if (currentActiveActor != null)
        {
            GridElement currentGridPosition = currentActiveActor.CurrentGridPosition;
            if (currentGridPosition != null)
            {
                int actorRange = currentActiveActor.GetComponent<StatsManager>().GetCombatSpeed();
                NavMeshAgent actorAgent = currentActiveActor.GetComponent<NavMeshAgent>();
                FindReachableGridElements(actorAgent, currentGridPosition, actorRange);
            }
        }
    }

    private void FindReachableGridElements(NavMeshAgent agent, GridElement startElement, int movementRange)
    {
        startElement.Visited = GridElement.Status.StartingPoint;
        activeElements.Add(startElement);
        for (int x = startElement.GridPosition.x - ((int)(movementRange / scale) + 1); x < startElement.GridPosition.x + ((int)(movementRange / scale) + 1); x++)
        {
            if (x < 0)
                continue;
            if (x >= gridElements.GetLength(0))
                break;
            for (int y = startElement.GridPosition.y - ((int)(movementRange / scale) + 1); y < startElement.GridPosition.y + ((int)(movementRange / scale) + 1); y++)
            {
                if (y < 0)
                    continue;
                if (y >= gridElements.GetLength(1))
                    break;
                GridElement currentElement = gridElements[x, y];
                if (currentElement != null && IsGridElementWithinRange(currentElement, movementRange, agent) && currentElement.Visited == GridElement.Status.NotVisited)
                {
                    currentElement.gameObject.SetActive(true);
                    currentElement.Visited = GridElement.Status.Visited;
                    activeElements.Add(currentElement);
                }
            }
        }
    }

    private bool IsGridElementWithinRange(GridElement endPoint, int movementRange, NavMeshAgent agent)
    {
        if (endPoint == null)
            return false;
        NavMeshPath pathToElement = new NavMeshPath();
        agent.CalculatePath(endPoint.transform.position, pathToElement);
        if (pathToElement.status == NavMeshPathStatus.PathComplete)
        {
            return CalculatePathLength(pathToElement.corners) <= movementRange + ActionConsts.COMPLETION_MARGIN;
        }
        else
        {
            return false;
        }
    }

    private float CalculatePathLength(Vector3[] pathNavpoints)
    {
        float pathLength = 0;
        for (int i = 1; i < pathNavpoints.Length; i++)
        {
            pathLength += Vector3.Distance(pathNavpoints[i - 1], pathNavpoints[i]);
        }
        return pathLength;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Gizmos.DrawWireCube(new Vector3(transform.position.x + j*scale, transform.position.y + 0.5f * scale, transform.position.z + i*scale), new Vector3(1*scale, 1*scale, 1*scale));
            }
        }
    }
}
