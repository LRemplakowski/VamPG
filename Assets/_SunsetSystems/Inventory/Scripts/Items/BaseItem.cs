using NaughtyAttributes;
using SunsetSystems.Journal;
using SunsetSystems.Resources;
using SunsetSystems.UI.Utils;
using UnityEngine;
using Zenject;

namespace SunsetSystems.Inventory.Data
{
    public abstract class BaseItem : ScriptableObject, IRewardable, IGameDataProvider<BaseItem>
    {
        [field: SerializeField]
        public string ReadableID { get; protected set; }
        [field: SerializeField, ReadOnly]
        public string DatabaseID { get; private set; }
        [field: SerializeField, ReadOnly]
        public ItemCategory ItemCategory { get; protected set; }
        [field: SerializeField, TextArea]
        public string ItemDescription { get; protected set; }
        [field: SerializeField]
        public GameObject Prefab { get; protected set; }
        [field: SerializeField]
        public Sprite Icon { get; protected set; }
        [field: SerializeField]
        public bool Stackable { get; protected set; }

        public BaseItem Data => this;

        // DEPENDENCIES
        private IInventoryManager inventoryManager;

        [Inject]
        public void InjectDependencies(IInventoryManager inventoryManager)
        {
            this.inventoryManager = inventoryManager;
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(ReadableID))
            {
                ReadableID = name;
                UnityEditor.EditorUtility.SetDirty(this);
            }
            if (Icon == null)
            {
                Icon = ResourceLoader.GetFallbackIcon();
                UnityEditor.EditorUtility.SetDirty(this);
            }
            if (string.IsNullOrWhiteSpace(DatabaseID))
                AssignNewID();
#endif
            ItemDatabase.Instance?.Register(this);
        }

        private void Reset()
        {
            ReadableID = name;
            Icon = ResourceLoader.GetFallbackIcon();
            AssignNewID();
        }

        private void OnDestroy()
        {
            ItemDatabase.Instance?.Unregister(this);
        }

#if UNITY_EDITOR
        private void AssignNewID()
        {
            DatabaseID = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        public void ApplyReward(int amount)
        {
            inventoryManager.AddEntryToPlayerInventory(new InventoryEntry(this, amount));
        }
    }
}
