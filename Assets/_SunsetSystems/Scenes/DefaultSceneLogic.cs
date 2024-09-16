using System;
using System.Threading.Tasks;
using Sirenix.Serialization;
using SunsetSystems.Audio;
using SunsetSystems.Persistence;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Core
{
    public class DefaultSceneLogic : AbstractSceneLogic
    {
        [OdinSerialize]
        private ScenePlaylistData _defaultScenePlaylists;
        [field: SerializeField]
        public UltEvent OnSceneStart { get; private set; }

        public async override Task StartSceneAsync()
        {
            AudioManager.Instance.InjectPlaylistData(_defaultScenePlaylists);
            OnSceneStart?.InvokeSafe();
            await Task.Yield();
        }

        public override void InjectSaveData(object data)
        {
            
        }

        public override object GetSaveData()
        {
            SceneLogicData saveData = new();
            return saveData;
        }

        [Serializable]
        protected class SceneLogicData : SaveData
        {

        }
    }
}
