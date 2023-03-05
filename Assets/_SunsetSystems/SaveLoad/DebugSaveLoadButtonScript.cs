using UnityEngine;
using Zenject;

namespace SunsetSystems.LevelManagement
{
    public class DebugSaveLoadButtonScript : MonoBehaviour
    {
        private ILevelLoader _levelLoader;

        [Inject]
        public void InjectDependencies(ILevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
        }

        public void DoSave()
        {
            Debug.Log("DoSave button");
            SaveLoadManager.SaveObjects();
        }

        public async void DoLoad()
        {
            Debug.Log("DoLoad button");
            await _levelLoader.LoadSavedLevel();
        }
    }
}
