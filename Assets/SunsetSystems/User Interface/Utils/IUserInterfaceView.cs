using UnityEngine;

namespace SunsetSystems.UI.Utils
{
    public interface IUserInterfaceView<T>
    {
        void UpdateView(IGameDataProvider<T> dataProvider);
    }
}
