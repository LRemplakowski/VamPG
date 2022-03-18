using Entities.Characters;
using UnityEngine;
using Utils.Resources;

namespace Utils.Scenes
{
    public abstract class AbstractSceneLogic : MonoBehaviour
    {
        [SerializeField]
        protected CreatureData creaturePrefab;
        [SerializeField, ReadOnly]
        protected bool isPartyInitialized;

        private void Reset() => creaturePrefab = ResourceLoader.GetEmptyCreaturePrefab();

        public abstract void StartScene();
    }
}
