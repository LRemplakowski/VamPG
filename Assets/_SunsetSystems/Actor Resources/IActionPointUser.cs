using UnityEngine;

namespace SunsetSystems.ActorResources
{
    public interface IActionPointUser
    {
        int GetCurrentActionPoints();
        int GetMaxActionPoints();

        bool CanUseActionPoints();
        bool UseActionPoints(int amount);

        void AddActionPointUseBlocker(string sourceID);
        void RemoveActionPointUseBlocker(string sourceID);
    }
}
