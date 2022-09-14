using SunsetSystems.Inventory.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    [CreateAssetMenu(fileName = "New Equipment Asset", menuName = "Sunset Inventory/Equipment Asset")]
    public class CreatureEquipmentAsset : ScriptableObject
    {
        [SerializeField]
        private List<EquipmentSlot> slots = new();
    }
}
