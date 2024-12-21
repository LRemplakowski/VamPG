using System;
using UnityEngine;

namespace SunsetSystems.ActorResources
{
    public interface IActionPointUser
    {
        event Action<int> OnActionPointUpdate;

        int GetCurrentActionPoints();
        int GetMaxActionPoints();

        bool CanUseActionPoints();
        bool UseActionPoints(int amount);

        void AddActionPointUseBlocker(string sourceID);
        void RemoveActionPointUseBlocker(string sourceID);
    }
}
