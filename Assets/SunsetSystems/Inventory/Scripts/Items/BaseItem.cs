using NaughtyAttributes;
using SunsetSystems.Journal;
using SunsetSystems.Resources;
using SunsetSystems.UI.Utils;
using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class BaseItem : ScriptableObject, IRewardable, IGameDataProvider<BaseItem>
    {
        [field: SerializeField]
        public string ItemName { get; protected set; }
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }
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

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (ItemName == "")
            {
                ItemName = name;
                EditorUtility.SetDirty(this);
            }
            if (Icon == null)
            {
                Icon = ResourceLoader.GetFallbackIcon();
                EditorUtility.SetDirty(this);
            }
            if (ID == "")
            {
                ID = GUID.Generate().ToString();
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public void ApplyReward(int amount)
        {
            InventoryManager.PlayerInventory.AddItem(new InventoryEntry(this, amount));
        }
    }
}
