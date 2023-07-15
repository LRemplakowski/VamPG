using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems
{
    public class GridSystem
    {
        private int width;
        private int height;
        private float cellSize;
        private GridObject[,] gridObjectArray;
        private LayerMask coverLayer = LayerMask.GetMask("Cover");

        public GridSystem(int width, int height, float cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            gridObjectArray = new GridObject[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    gridObjectArray[x, z] = new GridObject(this, gridPosition);
                }
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
        }

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return new GridPosition(
                Mathf.RoundToInt(worldPosition.x / cellSize),
                Mathf.RoundToInt(worldPosition.z / cellSize)
            );
        }

        public void CreateDebugObjects(Transform debugPrefab)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);

                    Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                }
            }
        }

        public GridObject GetGridObject(GridPosition gridPosition)
        {
            return gridObjectArray[gridPosition.x, gridPosition.z];
        }

        public bool IsValidGridPosition(GridPosition gridPosition)
        {        
            Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
            if(gridPosition.x >= 0 && 
                gridPosition.z >= 0 && 
                gridPosition.x < width && 
                gridPosition.z < height && 
                !Physics.CheckBox(worldPosition, Vector3.one/2 * cellSize, 
                Quaternion.identity, coverLayer)){
                    return true;
                }
                else{
                    return false;
                }
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }
    }
}
