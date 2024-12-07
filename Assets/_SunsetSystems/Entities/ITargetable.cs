using SunsetSystems.Abilities;
using SunsetSystems.Localization;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ITargetable
    {
        bool IsValidTarget(TargetableEntityType validTargetsFlag);
    }
}
