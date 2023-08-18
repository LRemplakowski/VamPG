using SunsetSystems.Core.AddressableManagement;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace SunsetSystems.Entities.Instantiation
{
    public class CreatureFactory : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceT<Creature> creaturePrefabReference;

        public async Task<ICreature> Create(ICreatureTemplate creatureTemplate)
        {
            ICreature newInstance = await GetNewCreatureInstance();
            throw new System.NotImplementedException();
        }

        private async Task<Creature> GetNewCreatureInstance()
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(creaturePrefabReference.RuntimeKey);
            await handle.Task;
            return handle.Result.GetComponent<Creature>();
        }

        public void DestroyCreature(Creature instance)
        {
            Addressables.ReleaseInstance(instance.gameObject);
        }
    }
}
