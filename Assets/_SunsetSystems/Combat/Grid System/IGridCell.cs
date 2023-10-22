using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat.Grid
{
    public interface IGridCell
    {
        Vector3Int GridPosition { get; }
        Vector3 WorldPosition { get; }
        bool IsOccupied { get; }
        bool IsFree { get; }

        float CellSize { get; }

        bool Highlighted { get; }
    }
}
