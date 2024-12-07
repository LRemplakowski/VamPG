using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities
{
    public interface IContextProvider<T>
    {
        T GetContext();
    }
}
