using NaughtyAttributes;
using SunsetSystems.Entities.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    [CreateAssetMenu(fileName = "new Dialogue Variable Config", menuName = "Sunset Dialogue/Variable Config")]
    public class DialogueVariableConfig : ScriptableObject
    {
        [SerializeField]
        private CreatureConfig _pcConfig;
        [SerializeField]
        private CreatureConfig _beastConfig;
        [SerializeField]
        private CreatureConfig _innerMonologueConfig;
        [SerializeField]
        private CreatureConfig _narratorConfig;
        [SerializeField, Required]
        private CreatureConfig _kieranConfig;
        [SerializeField, Required]
        private CreatureConfig _dominicConfig;

        public const string SPEAKER_ID = "$speakerID";
        public const string PC_NAME = "$pcName";
        public const string BEAST = "$beast";
        public const string INNER_VOICE = "$innerVoice";
        public const string NARRATOR_VOICE = "$narrator";
        public const string PC_ID = "$pcID";
        public const string KIERAN = "$kieran";
        public const string DOMINIC = "$sheriff";

        [SerializeField, HideInInspector]
        private DialogueSaveData _injectionData = new();

        private void OnValidate()
        {
            _injectionData = GenerateInjectionData();
        }

        private DialogueSaveData GenerateInjectionData()
        {
            DialogueSaveData injectionData = new();
            injectionData._floats = new();
            injectionData._strings = new();
            injectionData._bools = new();

            injectionData._strings.Add(PC_NAME, _pcConfig.FullName);
            injectionData._strings.Add(BEAST, _beastConfig.FullName);
            injectionData._strings.Add(INNER_VOICE, _innerMonologueConfig.FullName);
            injectionData._strings.Add(NARRATOR_VOICE, _narratorConfig.FullName);
            injectionData._strings.Add(KIERAN, _kieranConfig.FullName);
            injectionData._strings.Add(DOMINIC, _dominicConfig.FullName);

            return injectionData;
        }

        public DialogueSaveData GetVariableInjectionData()
        {
            return _injectionData;
        }
    }
}
