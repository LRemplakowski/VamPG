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

        protected override void Awake()
        {
            _dialogueRunner ??= GetComponent<DialogueRunner>();
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
            return true;
        }

        public static void InterruptCurrentLine()
        {
            Instance._dialogueRunner.OnViewRequestedInterrupt();
        }

        public void DisableViews()
        {
            _dialogueRunner.dialogueViews.ToList().ForEach(view => view.gameObject.SetActive(false));
        }
    }
}
