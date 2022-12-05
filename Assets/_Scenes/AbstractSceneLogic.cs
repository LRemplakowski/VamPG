using Entities.Characters;
using UnityEngine;
using SunsetSystems.Resources;
using System.Threading.Tasks;
using SunsetSystems.Loading;
using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Data;

namespace SunsetSystems.Loading
{
    [RequireComponent(typeof(UniqueId))]
    public abstract class AbstractSceneLogic : MonoBehaviour, ISaveRuntimeData
    {
        [SerializeField]
        protected UniqueId unique;

        protected virtual void OnValidate()
        {
            unique ??= GetComponent<UniqueId>();
        }

        protected virtual void Awake()
        {
            unique ??= GetComponent<UniqueId>();
        }

        public abstract Task StartSceneAsync(LevelLoadingData data);

        public abstract void SaveRuntimeData();

        public abstract void LoadRuntimeData();
    }
}
