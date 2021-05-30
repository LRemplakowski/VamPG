using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private GridElement gridElementPrefab;
    [SerializeField]
    private int rows, columns;
    [SerializeField, ReadOnly]
    private Vector3 bottomLeft;

    [SerializeField, ReadOnly]
    private GridElement[,] gridElements;

    [SerializeField]
    private float scale = 1f;
    [SerializeField, Range(0, 5)]
    private int numOfLevels = 0;

    private NavMeshAgent playerAgent;

    public void Start()
    {
        gridElements = new GridElement[rows, columns];
        bottomLeft = transform.position;
        playerAgent = GameManager.GetPlayer().GetComponent<NavMeshAgent>();
        GenerateGrid();
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
                        MaybeCreateGridElement(pos, i, j);
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

    private void MaybeCreateGridElement(Vector3 pos, int row, int column)
    {
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, playerAgent.radius, NavMesh.AllAreas))
        {
            GridElement g = Instantiate(gridElementPrefab, hit.position, Quaternion.identity);
            g.transform.parent = this.transform;
            g.transform.localScale = new Vector3(scale, scale, scale);
            g.GridPosition = new Vector2(row, column);
            gridElements[row, column] = g;
        }
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
