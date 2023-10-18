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
        private Dictionary<GridCellStateData, IMaterialConfig> gridCellMaterialConfigs = new();
        [ShowInInspector, ReadOnly]
        private GridUnit unitData = null;

        public Vector3 WorldPosition => transform.position + new Vector3(0, unitData.SurfaceY - transform.position.y, 0);

        public bool InjectUnitData(GridUnit unitData)
        {
            if (this.unitData == null)
            {
                this.unitData = unitData;
                cellCollider.size = new Vector3(unitData.CellSize, 0.1f, unitData.CellSize);
                Vector3 cellPosition = new Vector3(unitData.X, unitData.Y, unitData.Z) * unitData.CellSize;
                cellPosition.y = unitData.SurfaceY;
                transform.localPosition = cellPosition;
                cellRenderer.transform.localScale = Vector3.one * unitData.CellSize;
                UpdateCellState();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateCellState()
        {
            GridCellStateData stateData = CellStateEvaluator.EvaluateState(unitData);
            if (gridCellMaterialConfigs.TryGetValue(stateData, out IMaterialConfig value))
            {
                SetCellMaterialParams(value.PropertyOverrides);
            }
        }

        private void SetCellMaterialParams(IEnumerable<MaterialPropertyData> propertyData, bool useSharedMaterial = false)
        {
            Material mat;
            if (useSharedMaterial)
                mat = cellRenderer.sharedMaterial;
            else
                mat = cellRenderer.material;
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

        [Button]
        public void ForceInjectMaterialDataFromConfig(IMaterialConfig config)
        {
            SetCellMaterialParams(config.PropertyOverrides, true);
        }

        [Button]
        public void FillUnitStateDictionary()
        {
            foreach (GridCellBaseState baseState in Enum.GetValues(typeof(GridCellBaseState)))
            {
                foreach (GridCellSubState subState in Enum.GetValues(typeof(GridCellSubState)))
                {
                    GridCellStateData data = new()
                    {
                        BaseState = baseState,
                        SubState = subState,
                    };
                    gridCellMaterialConfigs.TryAdd(data, null);
                }
            }
        }

        public struct GridCellStateData
        {
            public GridCellBaseState BaseState;
            public GridCellSubState SubState;

            public GridCellStateData(GridCellBaseState BaseState, GridCellSubState SubState)
            {
                this.BaseState = BaseState;
                this.SubState = SubState;
            }
        }

        public enum GridCellBaseState
        {
            Default, Highlight, Walkable, Sprintable
        }

        public enum GridCellSubState
        {
            Default, Hostile, Friendly, HalfCover, FullCover
        }

        private static class CellStateEvaluator
        {
            public static GridCellStateData EvaluateState(GridUnit gridUnit)
            {
                GridCellStateData result = new();
                EvaluateBaseState(gridUnit, ref result);
                EvaluateSubState(gridUnit, ref result);
                return result;

                static void EvaluateBaseState(GridUnit gridUnit, ref GridCellStateData result)
                {
                    result.BaseState = GridCellBaseState.Default;
                    if (gridUnit.Walkable)
                    {
                        if (gridUnit.IsInSprintRange)
                            result.BaseState = GridCellBaseState.Sprintable;
                        if (gridUnit.IsInMoveRange)
                            result.BaseState = GridCellBaseState.Walkable;
                    }
                    if (gridUnit.Highlighted)
                        result.BaseState = GridCellBaseState.Highlight;
                }

                static void EvaluateSubState(GridUnit gridUnit, ref GridCellStateData result)
                {
                    result.SubState = GridCellSubState.Default;
                    if (gridUnit.AdjacentToCover)
                    {
                        switch (gridUnit.CoverQuality)
                        {
                            case CoverQuality.Half:
                                result.SubState = GridCellSubState.HalfCover;
                                break;
                            case CoverQuality.Full:
                                result.SubState = GridCellSubState.FullCover;
                                break;
                        }
                    }
                    if (gridUnit.Occupied)
                    {
                        if (gridUnit.Occupier.Faction is Faction.Hostile)
                            result.SubState = GridCellSubState.Hostile;
                        else
                            result.SubState = GridCellSubState.Friendly;
                    }
                }
            }
        }
    }
}
