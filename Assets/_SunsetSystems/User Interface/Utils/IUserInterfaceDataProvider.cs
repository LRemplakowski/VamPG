using System.Collections.Generic;

namespace SunsetSystems.UI.Utils
{
    public interface IUserInfertaceDataProvider<T>
    {
        T UIData { get; }
    }
}
