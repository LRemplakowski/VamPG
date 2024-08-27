using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Core.Database;
using SunsetSystems.Entities.Characters;
using UnityEngine;

namespace SunsetSystems.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue Variable Config", menuName = "Sunset Dialogue/Variable Config")]
    public class DialogueVariableConfig : SerializedScriptableObject
    {
        [OdinSerialize]
        private DialogueSaveData _injectionData = new();

        private void OnValidate()
        {
            _injectionData = GenerateInjectionData();
        }

        private DialogueSaveData GenerateInjectionData()
        {
            DialogueSaveData injectionData = new();
            List<string> configIDs = CreatureDatabase.Instance.ReadableIDs ?? new();
            foreach (string configID in configIDs)
            {
                if (CreatureDatabase.Instance.TryGetEntryByReadableID(configID, out CreatureConfig config))
                {
                    injectionData._strings.Add(configID, config.FullName);
                    //injectionData._strings.Add(config.FullName, configID);
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
