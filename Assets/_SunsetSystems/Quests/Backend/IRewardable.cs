using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public interface IRewardable
    {
        void ApplyReward(int amount);
    }
}
