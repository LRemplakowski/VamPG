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
        [SerializeField]
        private ValueType _valueType;
        private bool StringValue => _valueType == ValueType.StringValue;
        private bool IntValue => _valueType == ValueType.IntValue;
        private bool BoolValue => _valueType == ValueType.BoolValue;
        [SerializeField, ShowIf("StringValue")]
        private string _stringKey;
        [SerializeField, ShowIf("StringValue")]
        private string _stringValue;
        [SerializeField, ShowIf("IntValue")]
        private string _intKey;
        [SerializeField, ShowIf("IntValue")]
        private int _intValue;
        [SerializeField, ShowIf("BoolValue")]
        private string _boolKey;
        [SerializeField, ShowIf("BoolValue")]
        private bool _boolValue;

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
            if (_isActive || CheckCompletion(_objective))
            {
                _objective.Complete();
                _isActive = false;
            }
        }

        public bool CheckCompletion(Objective objective)
        {
            if (DialogueHelper.VariableStorage == null)
                return false;
            switch (_valueType)
            {
                case ValueType.StringValue:
                    if (DialogueHelper.VariableStorage.TryGetValue(_stringKey, out string stringValue))
                        return stringValue.Equals(_stringValue);
                    return false;
                case ValueType.IntValue:
                    if (DialogueHelper.VariableStorage.TryGetValue(_intKey, out int intValue))
                        return intValue == _intValue;
                    return false;
                case ValueType.BoolValue:
                    if (DialogueHelper.VariableStorage.TryGetValue(_boolKey, out bool boolValue))
                        return boolValue == _boolValue;
                    return false;
            }
            return false;
        }

        private enum ValueType
        {
            StringValue, IntValue, BoolValue
        }
    }
}
