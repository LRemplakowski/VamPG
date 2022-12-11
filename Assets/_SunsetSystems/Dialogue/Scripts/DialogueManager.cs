using SunsetSystems.Constants;
using SunsetSystems.Data;
using SunsetSystems.Game;
using SunsetSystems.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    [RequireComponent(typeof(Tagger))]
    public class DialogueManager : Singleton<DialogueManager>, IResetable
    {
        [SerializeField]
        private DialogueRunner _dialogueRunner;
        [field: SerializeField]
        public float DefaultTypewriterValue { get; private set; } = 15f;

        protected override void Awake()
        {
            _dialogueRunner ??= GetComponent<DialogueRunner>();
            if (PlayerPrefs.HasKey(SettingsConstants.TYPEWRITER_SPEED_KEY))
                SetTypewriterSpeed(PlayerPrefs.GetInt(SettingsConstants.TYPEWRITER_SPEED_KEY));
            else
                SetTypewriterSpeed((int)DefaultTypewriterValue);
        }

        public void ResetOnGameStart()
        {
            _dialogueRunner.Dialogue.Stop();
            _dialogueRunner.Dialogue.UnloadAll();
            CleanupAfterDialogue();
        }

        private void Start()
        {
            DialogueHelper.InitializePersistentVariableStorage(_dialogueRunner.VariableStorage as PersistentVariableStorage);
        }

        public static void RegisterView(DialogueViewBase view)
        {
            List<DialogueViewBase> views = Instance._dialogueRunner.dialogueViews.ToList();
            views.Add(view);
            Instance._dialogueRunner.SetDialogueViews(views.ToArray());
        }

        public static void UnregisterView(DialogueViewBase view)
        {
            List<DialogueViewBase> views = Instance?._dialogueRunner.dialogueViews.ToList();
            views?.Remove(view);
            Instance?._dialogueRunner.SetDialogueViews(views.ToArray());
        }

        public bool StartDialogue(string startNode, YarnProject project = null)
        {
            if (project != null)
            {
                _dialogueRunner.SetProject(project);
            }
            if (_dialogueRunner.IsDialogueRunning)
                return false;
            foreach (DialogueViewBase view in _dialogueRunner.dialogueViews)
            {
                view.gameObject.SetActive(true);
            }
            _dialogueRunner.StartDialogue(startNode);
            GameManager.CurrentState = GameState.Conversation;
            return true;
        }   

        public void CleanupAfterDialogue()
        {
            GameManager.CurrentState = GameState.Exploration;
            foreach (DialogueViewBase view in _dialogueRunner.dialogueViews)
            {
                view.gameObject.SetActive(false);
                if (view is DialogueWithHistoryView historyView)
                {
                    historyView.Cleanup();
                }
            }    
        }

        public void SetTypewriterSpeed(float speed)
        {
            PlayerPrefs.SetInt(SettingsConstants.TYPEWRITER_SPEED_KEY, Mathf.RoundToInt(speed));
            PlayerPrefs.Save();
            _dialogueRunner.dialogueViews.ToList().ForEach(v => (v as DialogueWithHistoryView)?.SetTypeWriterSpeed(speed));
        }
    }
}
