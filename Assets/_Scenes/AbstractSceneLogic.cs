using UnityEngine;
using System.Threading.Tasks;
using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Data;

namespace SunsetSystems.Loading
{
    [RequireComponent(typeof(UniqueId))]
    public abstract class AbstractSceneLogic : MonoBehaviour, ISaveable
    {
        [SerializeField]
        protected UniqueId unique;

        public string DataKey => unique.Id;

        protected virtual void OnValidate()
        {
            unique ??= GetComponent<UniqueId>();
        }

        protected virtual void Awake()
        {
            unique ??= GetComponent<UniqueId>();
        }

        public abstract Task StartSceneAsync(LevelLoadingData data);

        public abstract object GetSaveData();

        public abstract void InjectSaveData(object data);
    }
}
