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
    [SerializeField, Range(0, 5)]
    private int numOfLevels = 0;

    private NavMeshAgent playerAgent;

    private bool _highlightGrid;
    [SerializeField, ExposeProperty]
    public bool HighlightGrid
    {
        get => _highlightGrid;
        set
        {
            _highlightGrid = value;
            if (_highlightGrid)
            {
                SetElementsInRangeActive();
                HighlightGrid = false;
            }
        }
    }

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
        gridElements = new GridElement[rows, columns];
        bottomLeft = transform.position;
        playerAgent = GameManager.GetPlayer().GetComponent<NavMeshAgent>();
        GenerateGrid();
        TempMovePlayerToNearestGridElement();
    }

    private void GenerateGrid()
    {
        for (int i=0; i < rows; i++)
        {
            for (int j=0; j < columns; j++)
            {
                if (numOfLevels > 0)
                {
                    for (int y=0; y < numOfLevels/scale; y++)
                    {
                        Vector3 pos = new Vector3(bottomLeft.x + (j * scale), bottomLeft.y+ (y * scale), bottomLeft.z + (i * scale));
                        if (MaybeCreateGridElement(pos, i, j))
                            break;
                    }
                }
                else
                {
                    Vector3 pos = new Vector3(bottomLeft.x + (j * scale), bottomLeft.y, bottomLeft.z + (i * scale));
                    MaybeCreateGridElement(pos, i, j);
                }
            }
        }
    }

    private bool MaybeCreateGridElement(Vector3 pos, int row, int column)
    {
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, playerAgent.radius, NavMesh.AllAreas))
        {
            GridElement g = Instantiate(gridElementPrefab, hit.position, Quaternion.identity);
            g.transform.parent = this.transform;
            g.transform.localScale = new Vector3(scale, scale, scale);
            g.GridPosition = new Vector2(row, column);
            gridElements[row, column] = g;
            g.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    private void TempMovePlayerToNearestGridElement()
    {
        float distance = float.MaxValue;
        GridElement nearest = null;
        Player player = GameManager.GetPlayer();
        foreach (GridElement g in gridElements)
        {
            if (g == null)
                continue;
            float newDistance = Vector3.Distance(player.transform.position, g.transform.position);
            if (newDistance < distance)
            {
                nearest = g;
                distance = newDistance;
            }
        }
        if (nearest != null)
            player.Move(nearest);
    }

    public void ClearActiveElements()
    {
        foreach (GridElement g in activeElements)
        {
            g.gameObject.SetActive(false);
        }
        activeElements.Clear();
    }

    private void SetElementsInRangeActive()
    {
        GameObject currentActiveActor = GameManager.GetCurrentActiveActor().gameObject;
        GridElement currentGridPosition = currentActiveActor.GetComponent<Creature>().CurrentGridPosition;
        float actorRange = currentActiveActor.GetComponent<StatsManager>().GetCombatSpeed();
        NavMeshAgent agent = currentActiveActor.GetComponent<NavMeshAgent>();
        foreach (GridElement g in gridElements)
        {
            if (g == null)
                continue;
            if (Vector3.Distance(currentActiveActor.transform.position, g.transform.position) <= actorRange)
            {
                NavMeshPath path = new NavMeshPath();
                agent.SetDestination(g.transform.position);
                agent.CalculatePath(g.transform.position, path);
                agent.SetPath(path);
                if (agent.remainingDistance <= actorRange)
                {
                    g.gameObject.SetActive(true);
                    activeElements.Add(g);
                }
                agent.ResetPath();
            }
        }
        currentGridPosition.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Gizmos.DrawWireCube(new Vector3(transform.position.x + j*scale, transform.position.y+0.5f, transform.position.z + i*scale), new Vector3(1*scale, 1*scale, 1*scale));
            }
        }
    }
}
