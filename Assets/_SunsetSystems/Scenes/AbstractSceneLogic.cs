using UnityEngine;
using System.Threading.Tasks;
using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Data;
using Sirenix.OdinInspector;

namespace SunsetSystems.Persistence
{
    [RequireComponent(typeof(UniqueId))]
    public abstract class AbstractSceneLogic : SerializedMonoBehaviour, ISaveable
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

        protected async virtual void Start()
        {
            await StartSceneAsync();
        }

        public abstract Task StartSceneAsync();

        public abstract object GetSaveData();

        public abstract void InjectSaveData(object data);
    }
}
