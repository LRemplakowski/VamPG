using Sirenix.OdinInspector;
using SunsetSystems.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat.Grid
{
    public class GridUnitObject : SerializedMonoBehaviour
    {
        [SerializeField]
        private BoxCollider cellCollider;
        [SerializeField]
        private MeshRenderer cellRenderer;
        [SerializeField]
        private Dictionary<GridCellState, IMaterialConfig> gridCellMaterialConfigs = new();
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
            if (gridCellMaterialConfigs.TryGetValue(state, out IMaterialConfig value))
                SetCellMaterialParams(value.PropertyOverrides);
        }

        private void SetCellMaterialParams(IEnumerable<MaterialPropertyData> propertyData)
        {
            Material mat = cellRenderer.material;
            foreach (MaterialPropertyData data in propertyData)
            {
                switch (data.PropertyType)
                {
                    case MaterialPropertyType.Float:
                        mat.SetFloat(data.PropertyName, data.GetValue<float>());
                        break;
                    case MaterialPropertyType.Int:
                        mat.SetInteger(data.PropertyName, data.GetValue<int>());
                        break;
                    case MaterialPropertyType.Vector:
                        mat.SetVector(data.PropertyName, data.GetValue<Vector4>());
                        break;
                    case MaterialPropertyType.Matrix:
                        mat.SetMatrix(data.PropertyName, data.GetValue<Matrix4x4>());
                        break;
                    case MaterialPropertyType.Texture:
                        mat.SetTexture(data.PropertyName, data.GetValue<Texture>());
                        break;
                    default:
                        Debug.LogError($"Invalid MaterialPropretyType {Enum.GetName(typeof(MaterialPropertyType), data.PropertyType)}!");
                        break;
                }
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
