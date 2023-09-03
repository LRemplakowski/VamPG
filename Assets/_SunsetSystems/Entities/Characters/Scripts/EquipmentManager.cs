using CleverCrow.Fluid.UniqueIds;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    public class EquipmentManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private EquipmentData _equipmentData = new();
        private UniqueId _unique;
        public string DataKey => _unique.Id;

        public static event Action ItemEquipped, ItemUnequipped;
    }
}
