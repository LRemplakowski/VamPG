using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Inventory.UI
{
    internal class ContainerEntry : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _text;

        private InventoryEntry _entry;
        public InventoryEntry Entry
        {
            get
            {
                return _entry;
            }
            set
            {
                _entry = value;
                _icon.sprite = value._item.Icon;
                _text.text = value._item.ItemName;
            }
        }
    }
}
