using Entities.Characters;
using UnityEngine;
using SunsetSystems.Resources;

namespace SunsetSystems.Scenes
{
    public abstract class AbstractSceneLogic : MonoBehaviour
    {
        [SerializeField]
        protected CreatureData creaturePrefab;
        [SerializeField, ReadOnly]
        protected bool isPartyInitialized;

        private void Reset() => creaturePrefab = ResourceLoader.GetEmptyCreaturePrefab();

        private void Start()
        {
            FindObjectOfType<ES3ReferenceMgr>().RefreshDependencies();
        }

        public abstract void StartScene();
    }
}
