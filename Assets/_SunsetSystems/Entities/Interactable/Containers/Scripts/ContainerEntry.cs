using SunsetSystems.Core.AddressableManagement;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

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

        public static event Action<ContainerEntry> ContainerEntryDestroyed;

        public void SetEntryContent(InventoryEntry content, ItemStorage storage)
        {
            //if (lastLoadedSprite != null)
            //    AddressableManager.Instance.ReleaseAsset(lastLoadedSprite);
            _content = content;
            _storage = storage;
            _text.text = content.ItemReference.Name;
            _icon.sprite = content.ItemReference.Icon;
            gameObject.name = content.ItemReference.Name;
        }

        public void OnClick()
        {
            Debug.Log("Container Entry clicked!");
            if (_storage.TryRemoveItem(_content))
            {
                if (InventoryManager.Instance.GiveItemToPlayer(_content.ItemReference, _content.StackSize))
                {
                    ContainerEntryDestroyed?.Invoke(this);
                    Destroy(gameObject);
                }
                else
                {
                    _storage.AddItem(_content);
                }
            }
        }
    }
}
