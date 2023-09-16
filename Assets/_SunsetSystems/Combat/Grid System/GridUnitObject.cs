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
    }
}
