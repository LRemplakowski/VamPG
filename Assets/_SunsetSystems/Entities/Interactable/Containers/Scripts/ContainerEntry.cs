using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SunsetSystems.Inventory.UI
{
    [RequireComponent(typeof(Button))]
    public class ContainerEntry : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _text;
        private InventoryEntry _content;
        private ItemStorage _storage;

        public delegate void ContainerEntryDestroyedHandler(ContainerEntry entry);
        public static event ContainerEntryDestroyedHandler ContainerEntryDestroyed;

        private IInventoryManager _inventoryManager;

        [Inject]
        public void InjectDependencies(IInventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        public void SetEntryContent(InventoryEntry content, ItemStorage storage)
        {
            _content = content;
            _storage = storage;
            _text.text = content._item.ReadableID;
            _icon.sprite = content._item.Icon;
            gameObject.name = content._item.ReadableID;
        }

        public void OnClick()
        {
            Debug.Log("Container Entry clicked!");
            _inventoryManager.AddEntryToPlayerInventory(_content);
            _storage.TryRemoveItem(_content);
            ContainerEntryDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
