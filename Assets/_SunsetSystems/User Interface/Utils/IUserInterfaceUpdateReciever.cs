using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.UI.Utils
{
    public interface IUserInterfaceUpdateReciever<T>
    {
        void DisableViews();

        void UpdateViews(List<IGameDataProvider<T>> data);
    }
}
