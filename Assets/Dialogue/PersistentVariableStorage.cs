using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public class PersistentVariableStorage : VariableStorageBehaviour
    {
        public override void Clear()
        {
            throw new System.NotImplementedException();
        }

        public override bool Contains(string variableName)
        {
            throw new System.NotImplementedException();
        }

        public override (Dictionary<string, float>, Dictionary<string, string>, Dictionary<string, bool>) GetAllVariables()
        {
            throw new System.NotImplementedException();
        }

        public override void SetAllVariables(Dictionary<string, float> floats, Dictionary<string, string> strings, Dictionary<string, bool> bools, bool clear = true)
        {
            throw new System.NotImplementedException();
        }

        public override void SetValue(string variableName, string stringValue)
        {
            throw new System.NotImplementedException();
        }

        public override void SetValue(string variableName, float floatValue)
        {
            throw new System.NotImplementedException();
        }

        public override void SetValue(string variableName, bool boolValue)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetValue<T>(string variableName, out T result)
        {
            throw new System.NotImplementedException();
        }
    }
}
