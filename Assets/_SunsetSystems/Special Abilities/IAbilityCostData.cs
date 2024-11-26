using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityCostData
    {
        public int MovementCost { get; }
        public int ActionPointCost { get; }
        public int BloodCost { get; }
    }
}
