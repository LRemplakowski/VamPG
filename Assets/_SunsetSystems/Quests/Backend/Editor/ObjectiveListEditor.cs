using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Journal.Editor
{
    [CustomEditor(typeof(ObjectiveList))]
    public class ObjectiveListEditor : UnityEditor.Editor
    {
        private ObjectiveList _list;
        private SerializedObject _serializedObject;
        private SerializedProperty _serializedProperty;

        private void OnEnable()
        {

        }
    }
}
