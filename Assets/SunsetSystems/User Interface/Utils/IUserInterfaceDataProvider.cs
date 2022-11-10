using System.Collections.Generic;

namespace SunsetSystems.UI.Utils
{
    public interface IGameDataProvider<T>
    {
        T Data { get; }
    }
}
