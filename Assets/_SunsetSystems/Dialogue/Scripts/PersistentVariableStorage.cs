using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using SunsetSystems.Persistence;
using System;
using SunsetSystems.Data;
using CleverCrow.Fluid.UniqueIds;
using Sirenix.Utilities;

namespace SunsetSystems.Dialogue
{
    [RequireComponent(typeof(UniqueId))]
    public class PersistentVariableStorage : VariableStorageBehaviour, ISaveable, IResetable
    {
        [SerializeField]
        private Dictionary<string, float> _floats = new();
        [SerializeField]
        private Dictionary<string, string> _strings = new();
        [SerializeField]
        private Dictionary<string, bool> _bools = new();
        [SerializeField]
        private DialogueVariableConfig _variableInjectionConfig;

        private readonly Dictionary<string, object> _variables = new();

        [SerializeField]
        private UniqueId _unique;
        public string DataKey => _unique.Id;

        private void Awake()
        {
            _unique ??= GetComponent<UniqueId>();
            if (_variableInjectionConfig != null)
            {
                DialogueSaveData _injectionData = _variableInjectionConfig.GetVariableInjectionData();
                SetAllVariables(_injectionData._floats, _injectionData._strings, _injectionData._bools);
            }
            ISaveable.RegisterSaveable(this);
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
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

        public void InjectSaveData(object data)
        {
            DialogueSaveData savedData = data as DialogueSaveData;
            SetAllVariables(savedData._floats, savedData._strings, savedData._bools);
        }

        public object GetSaveData()
        {
            return new DialogueSaveData(_floats, _strings, _bools);
        }

        public override void SetAllVariables(Dictionary<string, float> floats, Dictionary<string, string> strings, Dictionary<string, bool> bools, bool clear = true)
        {
            if (clear)
            {
                Clear();
            }
            floats.Keys.ForEach(key => _floats.Add(key, floats[key]));
            strings.Keys.ForEach(key => _strings.Add(key, strings[key]));
            bools.Keys.ForEach(key => _bools.Add(key, bools[key]));
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
    public class DialogueSaveData
    {
        public Dictionary<string, float> _floats = new();
        public Dictionary<string, string> _strings = new();
        public Dictionary<string, bool> _bools = new();

        public DialogueSaveData(Dictionary<string, float> _floats, Dictionary<string, string> _strings, Dictionary<string, bool> _bools)
        {
            this._floats = new(_floats);
            this._strings = new(_strings);
            this._bools = new(_bools);
        }

        public DialogueSaveData()
        {

        }
    }
}
