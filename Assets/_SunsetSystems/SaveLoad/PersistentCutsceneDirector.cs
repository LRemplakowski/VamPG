using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace SunsetSystems.Persistence
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PersistentCutsceneDirector : PersistentSceneObject
    {
        [SerializeField]
        private PlayableDirector _director;
        [SerializeField]
        private bool _playOnlyOnce;
        [SerializeField, ReadOnly]
        private PersistentDirectorComponent _persistentComponent = new();

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_director == null)
                _director = GetComponent<PlayableDirector>();
        }

        private void Start()
        {
            if (_playOnlyOnce)
            {
                _persistentComponent.PersistenceData.Director = _director;
                if (_director.playOnAwake)
                {
                    _persistentComponent.PersistenceData.StopPlayback = true;
                }
                else
                {
                    _director.played += (director) => _persistentComponent.PersistenceData.StopPlayback = true;
                }
                PersistentComponents.Add(_persistentComponent);
            }
        }

        [Serializable]
        private class PersistentDirectorComponent : IPersistentComponent
        {
            public string ComponentID => "CUTSCENE_DIRECTOR";
            public DirectorPersistenceData PersistenceData = new();


            public object GetComponentPersistenceData()
            {
                return PersistenceData;
            }

            public void InjectComponentPersistenceData(object data)
            {
                if (data is not DirectorPersistenceData persistenceData)
                    return;
                if (persistenceData.StopPlayback && persistenceData.Director != null)
                    persistenceData.Director.Stop();
            }

            [Serializable]
            public class DirectorPersistenceData
            {
                public PlayableDirector Director;
                public bool StopPlayback;
            }
        }
    }
}
