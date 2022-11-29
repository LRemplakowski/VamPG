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
        public string pcName;
        public string beastName;
        public string innerMonologue;
        public string narrator;

        public const string SPEAKER_ID = "speakerID";
        public const string PC_NAME = "pcName";
        public const string BEAST = "beast";
        public const string INNER_VOICE = "innerVoice";
        public const string NARRATOR_VOICE = "narrator";
        public const string PC_ID = "pcID";

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

            injectionData._strings.Add(PC_NAME, pcName);
            injectionData._strings.Add(BEAST, beastName);
            injectionData._strings.Add(INNER_VOICE, innerMonologue);
            injectionData._strings.Add(NARRATOR_VOICE, narrator);

            return injectionData;
        }

        public DialogueSaveData GetVariableInjectionData()
        {
            return _injectionData;
        }
    }
}
