using SunsetSystems.SaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DebugTest.DebugSaveable;

namespace DebugTest
{
    public class DebugSaveable : MonoBehaviour, ISaveable
    {
        public int someInt = 0;
        public float someFloat = 1.0f;
        public Vector3 someVector = Vector3.one;
        [SerializeField, ReadOnly]
        private string uniqueId;

        private void Awake()
        {
            uniqueId = gameObject.GetInstanceID().ToString();
        }


        public SerializedData GetData()
        {
            DebugSerializedData.DataBuilder builder = new DebugSerializedData.DataBuilder(uniqueId)
            {
                serializedInt = someInt,
                serializedFloat = someFloat,
                serializedVector3 = someVector
            };
            return builder.Build();
        }

        public void InjectData(SerializedData data)
        {
            DebugSerializedData debugData = data as DebugSerializedData;
            someInt = debugData.serializedInt;
            someFloat = debugData.serializedFloat;
            someVector = debugData.serializedVector3;
            uniqueId = debugData.Key;
        }

        [System.Serializable]
        public class DebugSerializedData : SerializedData
        {
            [SerializeField]
            private string _key;
            public override string Key
            {
                get => _key;
            }
            [SerializeField]
            internal int serializedInt;
            [SerializeField]
            internal float serializedFloat;
            [SerializeField]
            internal Vector3 serializedVector3;

            internal class DataBuilder
            {
                private readonly string key;
                public int serializedInt;
                public float serializedFloat;
                public Vector3 serializedVector3;
                public DataBuilder(string key)
                {
                    this.key = key;
                }

                public DebugSerializedData Build()
                {
                    DebugSerializedData data = new DebugSerializedData
                    {
                        _key = key
                    };
                    data.serializedInt = serializedInt;
                    data.serializedFloat = serializedFloat;
                    data.serializedVector3 = serializedVector3;
                    return data;
                }
            }
        }
    }
}
