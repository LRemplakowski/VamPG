using SunsetSystems.Utils.Threading;
using System.Threading.Tasks;

namespace SunsetSystems.Utils
{
    interface IInitialized
    {
        async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                Dispatcher.Instance.Invoke(() =>
                {
                    Initialize();
                });
            });
        }

        void Initialize();
    }
}
