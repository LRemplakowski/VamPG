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
            _dialogueRunner.dialogueViews.ToList().ForEach(view => view.gameObject.SetActive(true));
            _dialogueRunner.StartDialogue(startNode);
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
