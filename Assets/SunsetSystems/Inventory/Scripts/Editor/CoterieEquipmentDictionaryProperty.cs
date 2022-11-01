using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SunsetSystems.Inventory;

namespace SunsetSystems.Inventory.Editor
{
    [CustomPropertyDrawer(typeof(SerializableStringEquipmentDataDictionary))]
    public class CoterieEquipmentDictionaryProperty : SerializableDictionaryPropertyDrawer
    {

    }
}
