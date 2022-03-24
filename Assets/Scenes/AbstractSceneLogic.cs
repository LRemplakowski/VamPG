using Entities.Characters;
using UnityEngine;
using SunsetSystems.Resources;
using System.Threading.Tasks;
using SunsetSystems.SaveLoad;
using CleverCrow.Fluid.UniqueIds;

namespace SunsetSystems.Scenes
{
    [RequireComponent(typeof(UniqueId))]
    public abstract class AbstractSceneLogic : MonoBehaviour, ISaveRuntimeData
    {
        [SerializeField]
        protected CreatureData creaturePrefab;
        [SerializeField]
        protected UniqueId unique;

        private void Reset() => creaturePrefab = ResourceLoader.GetEmptyCreaturePrefab();

        protected virtual void Awake()
        {
            unique = GetComponent<UniqueId>();
        }

        protected virtual void Start()
        {
            FindObjectOfType<ES3ReferenceMgr>().RefreshDependencies();
        }

        public abstract Task StartSceneAsync();

        public abstract void SaveRuntimeData();

        public abstract void LoadRuntimeData();
    }
}
