using SunsetSystems.Constants;
using SunsetSystems.Data;
using SunsetSystems.Game;
using SunsetSystems.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Dialogue
{
    [RequireComponent(typeof(Tagger))]
    public class DialogueManager : Singleton<DialogueManager>, IResetable
    {
        [field: SerializeField]
        public float DefaultTypewriterValue { get; private set; } = 15f;

        protected override void Awake()
        {
            if (PlayerPrefs.HasKey(SettingsConstants.TYPEWRITER_SPEED_KEY))
                SetTypewriterSpeed(PlayerPrefs.GetInt(SettingsConstants.TYPEWRITER_SPEED_KEY));
            else
                SetTypewriterSpeed((int)DefaultTypewriterValue);
        }

        public void ResetOnGameStart()
        {
            CleanupAfterDialogue();
        }

        public static void RegisterView(DialogueWithHistoryView view)
        {

        }

        public static void UnregisterView(DialogueWithHistoryView view)
        {
        }

        public bool StartDialogue(string startNode)
        {
            GameManager.CurrentState = GameState.Conversation;
            return true;
        }   

        public void CleanupAfterDialogue()
        {
            GameManager.CurrentState = GameState.Exploration;
  
        }

        public void SetTypewriterSpeed(float speed)
        {
            PlayerPrefs.SetInt(SettingsConstants.TYPEWRITER_SPEED_KEY, Mathf.RoundToInt(speed));
            PlayerPrefs.Save();

        }
    }
}
