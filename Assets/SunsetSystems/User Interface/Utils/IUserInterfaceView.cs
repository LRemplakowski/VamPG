using UnityEngine;

namespace SunsetSystems.UI.Utils
{
    public interface IUserInterfaceView<T,U> where U : MonoBehaviour, IUserInterfaceView<T,U> where T : IGameDataProvider<T>
    {
        void UpdateView(IGameDataProvider<T> dataProvider);
    }
}
