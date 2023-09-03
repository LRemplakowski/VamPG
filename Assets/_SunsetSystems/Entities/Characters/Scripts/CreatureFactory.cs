using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SunsetSystems.Entities.Creatures
{
    public class CreatureFactory : MonoBehaviour
    {
        [SerializeField]
        private AssetReference creaturePrefabReference;

        public async Task<ICreature> Create(ICreatureTemplate creatureTemplate)
        {
            ICreature newInstance = await GetNewCreatureInstance();
            newInstance.InjectDataFromTemplate(creatureTemplate);
            return newInstance;
        }

        private async Task<ICreature> GetNewCreatureInstance()
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(creaturePrefabReference.RuntimeKey);
            await handle.Task;
            return handle.Result.GetComponent<ICreature>();
        }

        public void DestroyCreature(ICreature instance)
        {
            Addressables.ReleaseInstance(instance.References.GameObject);
        }
    }
}
