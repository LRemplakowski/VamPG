using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace SunsetSystems.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        private ItemCategory[] _acceptedItemTypes = new ItemCategory[0];
        [SerializeField]
        private List<BaseItem> _contents = new();
        public IReadOnlyList<BaseItem> Contents => _contents;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnValidate()
        {
            if (_acceptedItemTypes.Count() > 0)
            {
                _contents.FindAll(item => item != null).FindAll(item => !_acceptedItemTypes.Contains(item.ItemCategory)).ForEach(item => _contents.Remove(item));
            }
        }

        public void AddItem(BaseItem item)
        {
            if (IsItemTypeAccepted(item.ItemCategory))
            {
                _contents.Add(item);
            }
        }

        public void RemoveItem(BaseItem item)
        {
            _contents.Remove(item);
        }

        public bool IsItemTypeAccepted(ItemCategory itemType)
        {
            return _acceptedItemTypes.Count() == 0 || _acceptedItemTypes.Contains(itemType);
        }
    }
}
