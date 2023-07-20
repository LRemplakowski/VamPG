using UnityEngine;
using SunsetSystems.Entities.Interfaces;
using Sirenix.OdinInspector;

namespace SunsetSystems.Entities
{
    [DisallowMultipleComponent]
    public abstract class Entity : SerializedMonoBehaviour, IEntity, IEntityReferences
    {
        public abstract string ID { get; }
        public virtual string Name => gameObject.name;
        public IEntityReferences References => this;

        public Transform Transform => this.transform;
        public GameObject GameObject => this.gameObject;

        [field: SerializeField, Title("Setup")]
        public Faction Faction { get; private set; }
    }
}
