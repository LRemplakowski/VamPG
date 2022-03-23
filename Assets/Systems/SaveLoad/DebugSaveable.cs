using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.SaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DebugTest.DebugSaveable;

namespace DebugTest
{
    public class DebugSaveable : MonoBehaviour
    {
        public int someInt = 0;
        public float someFloat = 1.0f;
        public Vector3 someVector = Vector3.one;
        private UniqueId unique;

        private void Start()
        {
            if (unique == null)
                unique = GetComponent<UniqueId>();
        }


        public MonoBehaviour GetData()
        {
            return this;
        }

        public string GetKey()
        {
            return unique.Id;
        }

        public void InjectData(MonoBehaviour data)
        {
            
        }
    }
}
