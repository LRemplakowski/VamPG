using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    public class EquipmentManager : SerializedMonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private EquipmentData _equipmentData = new();

        public static event Action ItemEquipped, ItemUnequipped;
    }
}
