using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Core;
using SunsetSystems.Data;
using SunsetSystems.Game;
using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.Events;
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

        public UnityEvent OnDialogueStarted => _dialogueRunner.onDialogueStart;
        public UnityEvent OnDialogueFinished => _dialogueRunner.onDialogueComplete;
        public UnityEvent<string> OnNodeStarted => _dialogueRunner.onNodeStart;
        public UnityEvent<string> OnNodeFinished => _dialogueRunner.onNodeComplete;

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
            _dialogueRunner.onNodeStart.AddListener(MarkNodeVisitedCustom);
        }

        private void OnDestroy()
        {
            _dialogueRunner.onNodeStart.RemoveListener(MarkNodeVisitedCustom);
        }

        private void MarkNodeVisitedCustom(string nodeID)
        {
            Debug.Log($"Visited dialogue node: {nodeID}");
            DialogueHelper.VariableStorage.SetValue($"visited:{nodeID}", true);
        }

        public void RegisterView(DialogueViewBase view)
        {
            List<DialogueViewBase> views = Instance._dialogueRunner.dialogueViews.ToList();
            views.Add(view);
            Instance._dialogueRunner.SetDialogueViews(views.Distinct().ToArray());
        }

        public void UnregisterView(DialogueViewBase view)
        {
            List<DialogueViewBase> views = Instance?._dialogueRunner.dialogueViews.ToList();
            views?.Remove(view);
            Instance?._dialogueRunner.SetDialogueViews(views.ToArray());
        }

        [Button]
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
            GameManager.Instance.CurrentState = GameState.Conversation;
            return true;
        }   

        public void CleanupAfterDialogue()
        {
            GameManager.Instance.CurrentState = GameState.Exploration;
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
