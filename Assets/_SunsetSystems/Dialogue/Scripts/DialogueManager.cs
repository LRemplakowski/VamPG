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
    public class DialogueManager : Singleton<DialogueManager>
    {
        [SerializeField]
        private DialogueRunner _dialogueRunner;

        private const string UPDATE_SPEAKER_PORTRAIT_TAG = "UPDATE_SPEAKER_PORTRAIT";

        protected override void Awake()
        {
            _dialogueRunner ??= GetComponent<DialogueRunner>();
        }

        private void OnEnable()
        {
            _dialogueRunner.onDialogueComplete.AddListener(ClearCachedVariables);
        }

        private void OnDisable()
        {
            _dialogueRunner.onDialogueComplete.RemoveListener(ClearCachedVariables);
        }

        private void ClearCachedVariables()
        {
            _dialogueRunner.VariableStorage.SetValue(DialogueHelper.SPEAKER_ID, string.Empty);
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

        public static bool StartDialogue(string startNode, YarnProject project = null)
        {
            DialogueRunner cachedRunner = Instance._dialogueRunner;
            if (project != null)
            {
                cachedRunner.SetProject(project);
            }
            if (cachedRunner.IsDialogueRunning)
                return false;
            cachedRunner.dialogueViews.ToList().ForEach(view => view.gameObject.SetActive(true));
            cachedRunner.StartDialogue(startNode);
            if (cachedRunner.VariableStorage.TryGetValue(DialogueHelper.SPEAKER_ID, out string speakerID))
            {
                cachedRunner.dialogueViews.ToList().ForEach(view => (view as IPortraitUpdateReciever).InitializeSpeakerPhoto(speakerID));
            }
            GameManager.CurrentState = GameState.Conversation;
            return true;
        }

        public void InterruptCurrentLine()
        {
            if (_dialogueRunner.IsDialogueRunning)
                _dialogueRunner.OnViewRequestedInterrupt();
        }

        public void CleanupAfterDialogue()
        {
            GameManager.CurrentState = GameState.Exploration;
            _dialogueRunner.dialogueViews.ToList().ForEach(view => view.gameObject.SetActive(false));
        }
    }
}
