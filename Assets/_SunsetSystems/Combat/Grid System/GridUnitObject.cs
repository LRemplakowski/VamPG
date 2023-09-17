using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Combat.Grid
{
    public class GridUnitObject : SerializedMonoBehaviour
    {
        [SerializeField]
        private BoxCollider cellCollider;
        [SerializeField]
        private SpriteRenderer cellRenderer;
        [ShowInInspector, ReadOnly]
        private GridUnit unitData = null;

        private GridCellState previousState = GridCellState.Default;
        private GridCellState currentState = GridCellState.Default;
        public GridCellState CurrentCellState => currentState;

        public Vector3 WorldPosition => transform.position + new Vector3(0, unitData.surfaceY - transform.position.y, 0);

        public bool InjectUnitData(GridUnit unitData)
        {
            if (this.unitData == null)
            {
                this.unitData = unitData;
                cellCollider.size = new Vector3(unitData.cellSize, 0.1f, unitData.cellSize);
                Vector3 worldPosition = transform.TransformPoint(unitData.x, unitData.y, unitData.z);
                worldPosition.y = unitData.surfaceY;
                transform.position = worldPosition;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetGridCellState(GridCellState state, bool cachePrevious = false)
        {
            if (cachePrevious)
                previousState = currentState;
            currentState = state;
            switch (state)
            {
                case GridCellState.Default:
                    break;
                case GridCellState.Hostile:
                    break;
                case GridCellState.Walkable:
                    break;
                case GridCellState.Sprintable:
                    break;
                case GridCellState.Danger:
                    break;
            }
        }

        public void RestoreCachedPreviousVisualState()
        {
            SetGridCellState(previousState);
        }

        public enum GridCellState
        {
            Default, Hostile, Walkable, Sprintable, Danger 
        }
    }
}
