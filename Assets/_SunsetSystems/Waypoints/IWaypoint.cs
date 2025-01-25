using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.LevelUtility
{
    public interface IWaypoint
    {
        string WaypointTag { get; }

        Vector3 GetPosition();
        Vector3 GetFacingDirection();
        IList<Vector3> GetPositions(int amount);
        Vector3 GetFacingDirectionFromPosition(in Vector3 position);
    }
}
