using SunsetSystems.Data;
using System.Threading.Tasks;

namespace SunsetSystems.LevelManagement
{
    public interface ILevelLoader
    {
        Task LoadGameLevel(LevelLoadingData data);

        Task LoadSavedLevel();
    }
}
