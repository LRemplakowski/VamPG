using Apex;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using SunsetSystems.Loading;
using System;
using SunsetSystems.Data;

namespace SunsetSystems.Dialogue
{
    public class PersistentVariableStorage : VariableStorageBehaviour, ISaveRuntimeData, IResetable
    {
        [SerializeField]
        private StringFloatDictionary _floats = new();
        [SerializeField]
        private StringStringDictionary _strings = new();
        [SerializeField]
        private StringBoolDictionary _bools = new();
        [SerializeField]
        private DialogueVariableConfig _variableInjectionConfig;

        private readonly Dictionary<string, object> _variables = new();

        private const string DIALOGUE_DATA_KEY = "DIALOGUE_DATA";

        private void Awake()
        {
            if (_variableInjectionConfig != null)
            {
                DialogueSaveData _injectionData = _variableInjectionConfig.GetVariableInjectionData();
                SetAllVariables(_injectionData._floats, _injectionData._strings, _injectionData._bools);
            }
        }

        public void ResetOnGameStart()
        {
            Clear();
            Awake();
        }

        public void ReinjectPreconfiguredVariables()
        {
            if (_variableInjectionConfig != null)
            {
                DialogueSaveData _injectionData = _variableInjectionConfig.GetVariableInjectionData();
                SetAllVariables(_injectionData._floats, _injectionData._strings, _injectionData._bools, false);
            }
        }

        public override void Clear()
        {
            _floats.Clear();
            _strings.Clear();
            _bools.Clear();
            _variables.Clear();
        }

        public override bool Contains(string variableName)
        {
            return _variables.ContainsKey(variableName);
        }

        public override (Dictionary<string, float>, Dictionary<string, string>, Dictionary<string, bool>) GetAllVariables()
        {
            return (_floats, _strings, _bools);
        }

        public void LoadRuntimeData()
        {
            DialogueSaveData savedData = ES3.Load<DialogueSaveData>(DIALOGUE_DATA_KEY);
            SetAllVariables(savedData._floats, savedData._strings, savedData._bools);
        }

        public void SaveRuntimeData()
        {
            ES3.Save(DIALOGUE_DATA_KEY, new DialogueSaveData(_floats, _strings, _bools));
        }

        public override void SetAllVariables(Dictionary<string, float> floats, Dictionary<string, string> strings, Dictionary<string, bool> bools, bool clear = true)
        {
            if (clear)
            {
                Clear();
            }
            _floats.AddRange(floats);
            _strings.AddRange(strings);
            _bools.AddRange(bools);
            _floats.Apply(kv => _variables.Add(kv.Key, kv.Value));
            _strings.Apply(kv => _variables.Add(kv.Key, kv.Value));
            _bools.Apply(kv => _variables.Add(kv.Key, kv.Value));
        }

        public override void SetValue(string variableName, string stringValue)
        {
            _strings[variableName] = stringValue;
            _variables[variableName] = stringValue;
        }

        public override void SetValue(string variableName, float floatValue)
        {
            _floats[variableName] = floatValue;
            _variables[variableName] = floatValue;
        }

        public override void SetValue(string variableName, bool boolValue)
        {
            _bools[variableName] = boolValue;
            _variables[variableName] = boolValue;
        }

        public override bool TryGetValue<T>(string variableName, out T result)
        {
            result = default;
            if (_variables.TryGetValue(variableName, out object value))
            {
                result = (T)value;
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public struct DialogueSaveData
    {
        public StringFloatDictionary _floats;
        public StringStringDictionary _strings;
        public StringBoolDictionary _bools;

        public DialogueSaveData(StringFloatDictionary _floats, StringStringDictionary _strings, StringBoolDictionary _bools)
        {
            this._floats = _floats;
            this._strings = _strings;
            this._bools = _bools;
        }
    }
}
