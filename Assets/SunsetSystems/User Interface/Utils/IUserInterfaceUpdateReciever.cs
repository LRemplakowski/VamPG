using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.UI.Utils
{
    public interface IUserInterfaceUpdateReciever<T,U> where U : MonoBehaviour, IUserInterfaceView<T,U> where T : IGameDataProvider<T>
    {
        void DisableViews();

        void UpdateViews(IList<IGameDataProvider<T>> data);
    }
}
