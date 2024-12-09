using UnityEngine;

namespace SunsetSystems.ActorResources
{
    public interface IBloodPointUser
    {
        int GetCurrentBloodPoints();

        bool GetCanUseBloodPoints();

        bool UseBloodPoints(int amount);

        void AddBloodPointUseBlocker(string sourceID);
        void RemoveBloodPointUseBlocker(string sourceID);
    }
}
