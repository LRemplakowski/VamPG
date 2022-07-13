using System.Threading.Tasks;

namespace SunsetSystems.Utils
{
    interface IInitialized
    {
        async Task InitializeAsync()
        {
            await new Task(Initialize);
        }

        void Initialize();
    }
}
