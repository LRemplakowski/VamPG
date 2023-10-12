using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SunsetSystems.Combat.Grid
{
    public class GridManager : SerializedMonoBehaviour
    {
        public GridUnit this[int x, int y, int z]
        {
            get => managedGrid[x, y, z];
        }

        public GridUnit this[Vector3Int position]
        {
            get => managedGrid[position];
        }

        [SerializeField]
        private CachedMultiLevelGrid managedGrid;

        private bool isDirty;
        public event Action OnGridUpdate;

        private void Update()
        {
            if (isDirty)
            {
                OnGridUpdate?.Invoke();
                isDirty = false;
            }
        }

        public Vector3 GridPositionToWorldPosition(Vector3Int gridPosition) => managedGrid.GridPositionToWorldPosition(gridPosition);
        public Vector3Int WorldPositionToGridPosition(Vector3 worldPosition) => managedGrid.WorldPositionToGridPosition(worldPosition);
    }
}
