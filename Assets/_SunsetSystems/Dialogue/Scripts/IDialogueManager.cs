using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public interface IDialogueManager
    {
        int DefaultTypewriterSpeed { get; }

        void RegisterView(DialogueViewBase view);

        void UnregisterView(DialogueViewBase view);

        bool StartDialogue(string startNode, YarnProject project = null);

        void CleanupAfterDialogue();

        void SetTypewriterSpeed(float speed);
    }
}
