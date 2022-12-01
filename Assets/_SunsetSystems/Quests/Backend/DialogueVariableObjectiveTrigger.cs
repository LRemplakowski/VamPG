using NaughtyAttributes;
using SunsetSystems.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public class DialogueVariableObjectiveTrigger : MonoBehaviour, IObjectiveTrigger
    {
        [SerializeField]
        private Objective _objective;
        //[SerializeField]
        //private ValueType _valueType;
        //private bool StringValue => _valueType == ValueType.StringValue;
        //private bool IntValue => _valueType == ValueType.IntValue;
        //private bool BoolValue => _valueType == ValueType.BoolValue;
        //[SerializeField, ShowIf("StringValue")]
        //private string _stringKey;
        //[SerializeField, ShowIf("StringValue")]
        //private string _stringValue;
        //[SerializeField, ShowIf("IntValue")]
        //private string _intKey;
        //[SerializeField, ShowIf("IntValue")]
        //private int _intValue;
        //[SerializeField, ShowIf("BoolValue")]
        //private string _boolKey;
        //[SerializeField, ShowIf("BoolValue")]
        //private bool _boolValue;

        [SerializeField]
        StringStringDictionary _strings;
        [SerializeField]
        StringIntDictionary _ints;
        [SerializeField]
        StringBoolDictionary _bools;

        private bool _isActive;

        private void OnEnable()
        {
            _objective.OnObjectiveActive += StartTracking;    
        }

        private void OnDisable()
        {
            _objective.OnObjectiveActive -= StartTracking;
        }

        private void StartTracking(Objective obj)
        {
            _isActive = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isActive && CheckCompletion(_objective))
            {
                _objective.Complete();
                _isActive = false;
            }
        }

        public bool CheckCompletion(Objective objective)
        {
            foreach (string key in _bools.Keys)
            {
                if (DialogueHelper.VariableStorage.TryGetValue(key, out bool boolValue))
                {
                    if (boolValue == _bools[key])
                        continue;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            foreach (string key in _ints.Keys)
            {
                if (DialogueHelper.VariableStorage.TryGetValue(key, out int intValue))
                {
                    if (intValue == _ints[key])
                        continue;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            foreach (string key in _strings.Keys)
            {
                if (DialogueHelper.VariableStorage.TryGetValue(key, out string stringValue))
                {
                    if (stringValue.Equals(_strings[key]))
                        continue;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private enum ValueType
        {
            StringValue, IntValue, BoolValue
        }
    }
}
