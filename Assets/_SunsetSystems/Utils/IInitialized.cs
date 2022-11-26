using Redcode.Awaiting;
using SunsetSystems.Utils.Threading;
using System.Threading.Tasks;

namespace SunsetSystems.Utils
{
    interface IInitialized
    {
        async Task InitializeAsync()
        {
            await Task.Run(async () =>
            {
                await new WaitForUpdate();
                Initialize();
            });
        }

        void Initialize();
    }
}
