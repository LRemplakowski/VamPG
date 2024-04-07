using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Persistence;
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
       

        private void Start()
        {
           
        }


        public MonoBehaviour GetData()
        {
            return this;
        }

        public string GetKey()
        {
            
        }

        public void InjectData(MonoBehaviour data)
        {
            
        }
    }
}
