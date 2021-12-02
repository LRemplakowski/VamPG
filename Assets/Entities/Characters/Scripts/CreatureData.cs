using UnityEngine;

namespace Entities.Characters
{
    public class CreatureData : ExposableMonobehaviour
    {
        [SerializeField]
        private CreatureAsset data;

        private void Reset()
        {
            if (data == null)
                data = Resources.Load<CreatureAsset>("DEBUG/default");
            gameObject.name = data.name;
            CreatureInitializer.InitializeCreature(gameObject, data, Vector3.zero);
        }

        private void Start()
        {
            CreatureInitializer.InitializeCreature(this.gameObject, data, Vector3.zero);
        }

        [ContextMenu("Initialize Creature from Data")]
        private void InitializeInEditor()
        {
            if (data)
                CreatureInitializer.InitializeCreature(gameObject, data, Vector3.zero);
        }
    }
}
