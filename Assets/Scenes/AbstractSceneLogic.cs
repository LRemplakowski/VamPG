using Entities.Characters;
using UnityEngine;
using SunsetSystems.Resources;
using System.Threading.Tasks;

namespace SunsetSystems.Scenes
{
    public abstract class AbstractSceneLogic : MonoBehaviour
    {
        [SerializeField]
        protected CreatureData creaturePrefab;

        private void Reset() => creaturePrefab = ResourceLoader.GetEmptyCreaturePrefab();

        private void Start()
        {
            FindObjectOfType<ES3ReferenceMgr>().RefreshDependencies();
        }

        public abstract Task StartSceneAsync();
    }
}
