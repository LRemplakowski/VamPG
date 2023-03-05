using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public interface IDialogueManager
    {
        void RegisterView(DialogueViewBase view);

        void UnregisterView(DialogueViewBase view);

        bool StartDialogue(string startNode, YarnProject project = null);

        void CleanupAfterDialogue();

        void SetTypewriterSpeed(float speed);
    }
}
