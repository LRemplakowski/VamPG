using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities;
using SunsetSystems.Combat;
using SunsetSystems.Resources;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private GridElement gridElementPrefab;
    [SerializeField]
    private int rows, columns;
    [SerializeField]
    private Vector3 bottomLeft;

    [SerializeField]
    private GridElement[,] gridElements;

    private readonly List<GridElement> activeElements = new();

    [SerializeField]
    private float scale = 1f;
    [SerializeField]
    private float spacing = 0f;

    [SerializeField]
    private LayerMask coverMask;
    [SerializeField]
    private NavMeshAgent gridAgentHelper;

    public List<Cover> CoverSourcesInGrid { get; private set; }

    public void Start()
    {
        gridElements = new GridElement[columns, rows];
        transform.DestroyChildren();
        GenerateGrid();
        CoverSourcesInGrid = FindCoverSourcesInGrid();
        if (!gridAgentHelper)
            gridAgentHelper = Instantiate(ResourceLoader.GetGridHelperAgentPrefab(), transform.position, Quaternion.identity, transform);
    }

    [ContextMenu("Populate grid")]
    public void PopulateGrid()
    {
        this.transform.DestroyChildrenImmediate();
        gridElements = new GridElement[columns, rows];
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 pos = new(bottomLeft.x + (x * scale) + (x * spacing), bottomLeft.y, bottomLeft.z + (y * scale) + (y * spacing));
                MaybeCreateGridElement(pos, x, y);
            }
        }
    }

    private void MaybeCreateGridElement(Vector3 pos, int column, int row)
    {
        bool foundNavMeshHit = NavMesh.SamplePosition(transform.TransformPoint(pos), out NavMeshHit hit, float.MaxValue, NavMesh.AllAreas);
        if (foundNavMeshHit)
        {
            GridElement g = Instantiate(gridElementPrefab);
            g.name = column + ";" + row;
            g.transform.parent = this.transform;
            g.transform.position = hit.position;
            g.SetScale(scale);
            g.GridPosition = new Vector2Int(column, row);
            gridElements[column, row] = g;
            g.gameObject.SetActive(false);
            g.Visited = GridElement.Status.NotVisited;
        }
        else
        {
            Debug.LogError("Creating grid element failed! " + column + ";" + row);
        }
    }

    public List<GridElement> GetAdjacentGridElements(GridElement element)
    {
        List<GridElement> adjacents = new();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i != 0 && j != 0)
                {
                    try
                    {
                        int x = element.GridPosition.x + i;
                        int y = element.GridPosition.y + j;
                        GridElement g = gridElements[x, y];
                        adjacents.Add(g);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
        return adjacents;
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
        Debug.Log("clearing active elements");
        foreach (GridElement g in activeElements)
        {
            g.gameObject.SetActive(false);
            if (g.Visited == GridElement.Status.Occupied)
                continue;
            g.Visited = GridElement.Status.NotVisited;
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

    public void ActivateElementsInRangeOfActor(Creature actor)
    {
        GridElement currentGridPosition = actor.CurrentGridPosition;
        Debug.Log(currentGridPosition);
        if (currentGridPosition != null)
        {
            gridAgentHelper.transform.position = actor.transform.position;
            currentGridPosition.Visited = GridElement.Status.Occupied;
            //Debug.Log($"Grid Controll: Getting actor combat speed! Value = {actor.StatsManager.GetCombatSpeed()}");
            //List<GridElement> elementsInRange = FindReachableGridElements(currentGridPosition, actor.StatsManager.GetCombatSpeed());
            //Debug.Log("Elements in range: " + elementsInRange.Count);
            //foreach (GridElement g in elementsInRange)
            //{
              //  g.gameObject.SetActive(true);
              //  activeElements.Add(g);
            //}
        }
    }

    //public List<GridElement> GetElementsInRangeOfActor(Creature c)
    //{
      //  gridAgentHelper.transform.position = c.transform.position;
      //  GridElement actorPosition = c.CurrentGridPosition;
       // List<GridElement> result = FindReachableGridElements(actorPosition, c.StatsManager.GetCombatSpeed());
       // return result;
    //}

    //public List<Vector3> GetGridPositionsInRangeOfActor(Creature actor)
   // {
        //List<GridElement> elements = GetElementsInRangeOfActor(actor);
        //List<Vector3> result = new();
       // elements.ForEach(e => result.Add(e.transform.position));
       // return result;
    //}

    private List<GridElement> FindReachableGridElements(GridElement startElement, int movementRange)
    {
        List<GridElement> elementsInRange = new();
        for (int x = startElement.GridPosition.x - (movementRange + 1); x < startElement.GridPosition.x + (movementRange + 1); x++)
        {
            if (x < 0)
                continue;
            if (x >= gridElements.GetLength(0))
                break;
            for (int y = startElement.GridPosition.y - (movementRange + 1); y < startElement.GridPosition.y + (movementRange + 1); y++)
            {
                if (y < 0)
                    continue;
                if (y >= gridElements.GetLength(1))
                    break;
                GridElement currentElement = gridElements[x, y];
                if (currentElement != null && IsGridElementWithinRange(currentElement, movementRange, startElement.transform.position) && currentElement.Visited != GridElement.Status.Occupied)
                {
                    elementsInRange.Add(currentElement);
                }
            }
        }
        return elementsInRange;
    }

    private bool IsGridElementWithinRange(GridElement endPoint, int movementRange, Vector3 fromPosition)
    {
        if (endPoint == null)
            return false;
        double scaleAdjustedMovementRange = (movementRange * scale) + 0.1d;
        NavMeshPath pathToElement = new();
        gridAgentHelper.transform.position = fromPosition;
        gridAgentHelper.CalculatePath(endPoint.transform.position, pathToElement);
        if (pathToElement.status == NavMeshPathStatus.PathComplete)
        {
            return CalculatePathLength(pathToElement.corners) <= scaleAdjustedMovementRange;
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
            Vector3 previous = pathNavpoints[i - 1];
            Vector3 current = pathNavpoints[i];
            previous.y = 0;
            current.y = 0;
            pathLength += Vector3.Distance(previous, current);
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
        Creature currentActiveActor = CombatManager.CurrentActiveActor;
        if (currentActiveActor != null)
        {
            GridElement currentGridPosition = currentActiveActor.CurrentGridPosition;
            if (currentGridPosition != null)
            {
                gridAgentHelper.transform.position = currentActiveActor.transform.position;
                currentGridPosition.Visited = GridElement.Status.Occupied;
                activeElements.Add(currentGridPosition);
                List<GridElement> reachable = FindReachableGridElements(currentGridPosition, gridElements.Length);
                foreach (GridElement g in reachable)
                {
                    g.gameObject.SetActive(true);
                    activeElements.Add(g);
                }
            }
        }
    }

    private List<Cover> FindCoverSourcesInGrid()
    {
        Collider[] covers = Physics.OverlapBox(transform.position, new Vector3(rows * scale, 100f, columns * scale), Quaternion.identity, coverMask);
        List<Cover> result = new();
        foreach (Collider c in covers)
        {
            result.Add(c.GetComponent<Cover>());
        }
        return result;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Gizmos.DrawWireCube(new Vector3(transform.position.x + bottomLeft.x + j * scale + j * spacing,
                    transform.position.y + bottomLeft.y + 0.5f * scale,
                    transform.position.z + bottomLeft.z + i * scale + i * spacing),
                    new Vector3(1 * scale, 1 * scale, 1 * scale));
            }
        }
    }
}
