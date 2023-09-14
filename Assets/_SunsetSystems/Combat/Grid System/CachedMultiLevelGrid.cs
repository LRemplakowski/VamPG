using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat.Grid
{
    public class CachedMultiLevelGrid : SerializedMonoBehaviour
    {
        [SerializeField, Min(1)]
        private int levelWidth = 10, levelDepth = 10;
        [SerializeField, Min(1)]
        private int gridHeight;
        [SerializeField, Min(1)]
        public float gridCellSize = 1f;

        [SerializeField]
        private GridLevel[] levels = new GridLevel[1];

        [Button]
        private void BuildGrid()
        {
            levels = new GridLevel[gridHeight];
            for (int y = 0; y < gridHeight; y++)
            {
                GridLevel level = new(levelWidth, levelDepth, y);
                level.BuildLevel();
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
                        Gizmos.DrawWireCube(transform.position + new Vector3(x, level.YPosition, z) * gridCellSize, new Vector3(gridCellSize, 1f, gridCellSize));
                    }
                }
            }
        }

        [Serializable]
        private class GridLevel
        {
            [SerializeField, MinValue(1)]
            private int width = 10, depth = 10;
            [SerializeField, MinValue(0)]
            private int yPosition = 0;
            public int YPosition => yPosition;
            [SerializeField]
            private GridUnit[,] gridCells = new GridUnit[10, 10];

            public GridLevel(int width, int depth, int yPosition)
            {
                this.width = width;
                this.yPosition = yPosition;
                this.depth = depth;
            }

            public GridUnit GetCell(int x, int z) => gridCells[x, z];

            public void BuildLevel()
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
                        gridCells[x, z] = newGridUnit;
                    }
                }
            }
        }

        [Serializable]
        private class GridUnit
        {
            public int x, y, z;
        }
    }
}
