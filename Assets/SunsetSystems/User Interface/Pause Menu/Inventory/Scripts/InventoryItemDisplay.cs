using SunsetSystems.Inventory.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.UserInterface
{
    public class InventoryItemDisplay : MonoBehaviour
    {
        [SerializeField]
        internal BaseItem item;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _name, _category, _value;

        // Update is called once per frame
        void Update()
        {
            if (item != null)
            {
                _icon.sprite = item.Icon;
                _name.text = item.ItemName;
                _category.text = item.ItemCategory.ToString();
                _value.text = "0 $";
            }
        }
    }
}
