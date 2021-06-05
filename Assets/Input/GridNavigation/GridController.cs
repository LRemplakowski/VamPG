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
    [SerializeField]
    private float spacing = 0f;
    [SerializeField, Range(0, 20)]
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

    #region Enable&Disable
    private void OnEnable()
    {
        Move.onMovementFinished += ActivateElementsInRangeOfActiveActor;
        Move.onMovementStarted += ClearActiveElements;
    }

    private void OnDisable()
    {
        Move.onMovementFinished -= ActivateElementsInRangeOfActiveActor;
        Move.onMovementStarted -= ClearActiveElements;
    }
    #endregion

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
                    for (int z=0; z < numOfLevels * 10; z++)
                    {
                        Vector3 pos = new Vector3(bottomLeft.x + (x * scale) + (x * spacing), bottomLeft.y + (0.1f * z), bottomLeft.z + (y * scale) + (y * spacing));
                        if (MaybeCreateGridElement(pos, x, y))
                            break;
                    }
                }
                else
                {
                    Vector3 pos = new Vector3(bottomLeft.x + (x * scale) + (x * spacing), bottomLeft.y, bottomLeft.z + (y * scale) + (x * spacing));
                    MaybeCreateGridElement(pos, x, y);
                }
            }
        }
    }

    private bool MaybeCreateGridElement(Vector3 pos, int column, int row)
    {
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, .3f, NavMesh.AllAreas))
        {
            GridElement g = Instantiate(gridElementPrefab, new Vector3(pos.x, hit.position.y, pos.z), Quaternion.identity);
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

    public GridElement GetNearestGridElement(Vector3 position)
    {
        float distance = float.MaxValue;
        GridElement nearest = null;
        foreach (GridElement g in gridElements)
        {
            if (g == null)
                continue;
            float newDistance = Vector3.Distance(position, g.transform.position);
            if (newDistance < distance)
            {
                nearest = g;
                distance = newDistance;
            }
        }
        return nearest;
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

    public void SetStatusForElements(GridElement.Status status, List<GridElement> elements)
    {
        foreach (GridElement g in elements)
        {
            g.Visited = status;
        }
    }

    private void ActivateElementsInRangeOfActiveActor()
    {
        Creature currentActiveActor = TurnCombatManager.instance.CurrentActiveActor;
        if (currentActiveActor != null)
        {
            GridElement currentGridPosition = currentActiveActor.CurrentGridPosition;
            if (currentGridPosition != null)
            {
                int actorRange = currentActiveActor.GetComponent<StatsManager>().GetCombatSpeed();
                NavMeshAgent actorAgent = currentActiveActor.GetComponent<NavMeshAgent>();
                List<GridElement> elementsInRange = FindReachableGridElements(actorAgent, currentGridPosition, actorRange);
                foreach (GridElement g in elementsInRange)
                {
                    g.gameObject.SetActive(true);
                    activeElements.Add(g);
                }
            }
        }
    }

    public List<GridElement> GetElementsInRangeOfActor(Creature c)
    {
        NavMeshAgent agent = c.GetComponent<NavMeshAgent>();
        GridElement actorPosition = c.CurrentGridPosition;
        int actorRange = c.GetComponent<StatsManager>().GetCombatSpeed();
        List<GridElement> result = FindReachableGridElements(agent, actorPosition, actorRange);
        return result;
    }

    private List<GridElement> FindReachableGridElements(NavMeshAgent agent, GridElement startElement, int movementRange)
    {
        List<GridElement> elementsInRange = new List<GridElement>();
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
                if (currentElement != null && IsGridElementWithinRange(currentElement, movementRange, agent) && currentElement.Visited != GridElement.Status.Occupied)
                {
                    elementsInRange.Add(currentElement);
                }
            }
        }
        return elementsInRange;
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

    public GridElement GetGridElement(int x, int y)
    {
        if (IsPositionInElementsArray(x, y))
        {
            return gridElements[x, y];
        }
        return null;
    }

    public GridElement[,] GetGridElements(Vector2Int bottomLeft, Vector2Int topRight)
    {
        int width = topRight.x - bottomLeft.x;
        int height = topRight.y - bottomLeft.y;
        if (IsPositionInElementsArray(bottomLeft) && IsPositionInElementsArray(topRight) && width > 0 && height > 0)
        {
            GridElement[,] result = new GridElement[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = gridElements[bottomLeft.x + x, bottomLeft.y + y];
                }
            }
            return result;
        }
        return new GridElement[0, 0];
    }

    private bool IsPositionInElementsArray(int x, int y)
    {
        return x < gridElements.GetLength(0) && x >= 0 && y < gridElements.GetLength(1) && y >= 0;
    }

    private bool IsPositionInElementsArray(Vector2Int position)
    {
        return position.x < gridElements.GetLength(0) && position.x >= 0 && position.y < gridElements.GetLength(1) && position.y >= 0;
    }

    public void Dev_SetWholeGridActive()
    {
        ClearActiveElements();
        Creature currentActiveActor = TurnCombatManager.instance.CurrentActiveActor;
        if (currentActiveActor != null)
        {
            GridElement currentGridPosition = currentActiveActor.CurrentGridPosition;
            if (currentGridPosition != null)
            {
                int actorRange = currentActiveActor.GetComponent<StatsManager>().GetCombatSpeed();
                NavMeshAgent actorAgent = currentActiveActor.GetComponent<NavMeshAgent>();
                currentGridPosition.Visited = GridElement.Status.Occupied;
                activeElements.Add(currentGridPosition);
                FindReachableGridElements(actorAgent, currentGridPosition, gridElements.Length * 2);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Gizmos.DrawWireCube(new Vector3(transform.position.x + j*scale + j*spacing, transform.position.y + 0.5f * scale, transform.position.z + i*scale + i*spacing), new Vector3(1*scale, 1*scale, 1*scale));
            }
        }
    }
}
