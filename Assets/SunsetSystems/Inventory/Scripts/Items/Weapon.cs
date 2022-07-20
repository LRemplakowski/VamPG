using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Sunset Inventory/Items/Weapon")]
    public class Weapon : EquipableItem
    {
        protected int _damage = 0;
        public int Damage => _damage;
        protected float _range = 0f;
        public float Range => _range;

        private void Awake()
        {
            _itemCategory = ItemCategory.WEAPON;
        }
    }
}
