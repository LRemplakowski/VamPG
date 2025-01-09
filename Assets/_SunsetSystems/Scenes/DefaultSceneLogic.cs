using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Audio;
using SunsetSystems.Persistence;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Core
{
    public class DefaultSceneLogic : AbstractSceneLogic
    {
        [Title("Audio")]
        [OdinSerialize, InlineProperty]
        private ScenePlaylistData _defaultScenePlaylists;
        [field: Title("Events")]
        [field: SerializeField]
        public UltEvent OnSceneStart { get; private set; }

        public async override Task StartSceneAsync()
        {
            AudioManager.Instance.InjectPlaylistData(_defaultScenePlaylists);
            OnSceneStart?.InvokeSafe();
            await Task.Yield();
        }

        public override bool InjectSaveData(object data)
        {
            return true;
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
