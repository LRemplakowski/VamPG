using Entities.Characters;
using UnityEngine;


namespace Utils.Scenes
{
    public abstract class AbstractSceneLogic : MonoBehaviour
    {
        [SerializeField]
        protected CreatureData creaturePrefab;
        [SerializeField, ReadOnly]
        protected bool isPartyInitialized;

        public abstract void StartScene();
    }
}
