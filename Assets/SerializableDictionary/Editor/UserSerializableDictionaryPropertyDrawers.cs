using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
[CustomPropertyDrawer(typeof(StringFloatDictionary))]
[CustomPropertyDrawer(typeof(StringBoolDictionary))]
[CustomPropertyDrawer(typeof(StringQuestDictionary))]
[CustomPropertyDrawer(typeof(StringQuestDataDictionary))]
[CustomPropertyDrawer(typeof(StringEquipmentDataDictionary))]
[CustomPropertyDrawer(typeof(StringEquipmentSlotDictionary))]
[CustomPropertyDrawer(typeof(StringVector3Dictionary))]
[CustomPropertyDrawer(typeof(StringEquipmentSlotDisplayDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer : SerializableDictionaryStoragePropertyDrawer { }
