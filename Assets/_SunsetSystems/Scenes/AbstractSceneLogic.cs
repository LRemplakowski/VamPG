using UnityEngine;
using System.Threading.Tasks;
using CleverCrow.Fluid.UniqueIds;
using Sirenix.OdinInspector;
using SunsetSystems.Persistence;

namespace SunsetSystems.Core
{
    [RequireComponent(typeof(UniqueId))]
    public abstract class AbstractSceneLogic : SerializedMonoBehaviour, ISaveable
    {
        [SerializeField]
        protected UniqueId _unique;

        public string DataKey => _unique.Id;

        protected virtual void OnValidate()
        {
            _unique ??= GetComponent<UniqueId>();
        }

        protected virtual void Awake()
        {
            _unique ??= GetComponent<UniqueId>();
        }

        protected virtual void Start()
        {

        }

        public async void StartScene()
        {
            await StartSceneAsync();
        }

        public abstract Task StartSceneAsync();

        public abstract object GetSaveData();

        public abstract void InjectSaveData(object data);
    }
}
