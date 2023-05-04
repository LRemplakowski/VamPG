using NaughtyAttributes;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue Variable Config", menuName = "Sunset Dialogue/Variable Config")]
    public class DialogueVariableConfig : ScriptableObject
    {
        [SerializeField]
        private DialogueSaveData _injectionData = new();

        private void OnValidate()
        {
            _injectionData = GenerateInjectionData();
        }

        private DialogueSaveData GenerateInjectionData()
        {
            DialogueSaveData injectionData = new();
            List<string> configIDs = CreatureDatabase.Instance?.AccessorKeys ?? new();
            foreach (string configID in configIDs)
            {
                if (CreatureDatabase.Instance.TryGetConfig(configID, out CreatureConfig config))
                {
                    injectionData._strings.Add(configID, config.FullName);
                    injectionData._strings.Add(config.FullName, configID);
                }
            }
            return injectionData;
        }

        public DialogueSaveData GetVariableInjectionData()
        {
            return _injectionData;
        }
    }
}
