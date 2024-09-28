using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Economy
{
    public interface ITradeable
    {
        bool Buy(int amount);
        bool Sell(int amount);
        float GetBaseValue();
    }
}
