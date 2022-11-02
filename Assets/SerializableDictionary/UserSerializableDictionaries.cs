﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SunsetSystems.Journal;

[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> { }

[Serializable]
public class StringFloatDictionary : SerializableDictionary<string, float> { }

[Serializable]
public class StringBoolDictionary : SerializableDictionary<string, bool> { }

[Serializable]
public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> { }

[Serializable]
public class StringQuestDictionary : SerializableDictionary<string, Quest> { }

[Serializable]
public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> { }

[Serializable]
public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> { }