using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems
{
    public class GridObject
    {

        private GridSystem gridSystem;
        private GridPosition gridPosition;
        private List<Unit> unitList;

        public GridObject(GridSystem gridSystem, GridPosition gridPosition)
        {
            this.gridSystem = gridSystem;
            this.gridPosition = gridPosition;
            unitList = new List<Unit>();
        }

        public void AddUnit(Unit unit)
        {
            unitList.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            unitList.Remove(unit);
        }

        public List<Unit> GetUnitList()
        {
            return unitList;
        }

        public bool HasAnyUnit()
        {
            return unitList.Count > 0;
        }

        public Unit GetUnit()
        {
            if (HasAnyUnit())
            {
                return unitList[0];
            } else
            {
                return null;
            }
        }

    }
}
