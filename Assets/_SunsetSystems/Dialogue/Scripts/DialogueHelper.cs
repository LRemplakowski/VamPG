using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class DialogueHelper
    {
        public static PersistentVariableStorage VariableStorage { get; private set; }

        public static void InitializePersistentVariableStorage(PersistentVariableStorage storage)
        {
            if (VariableStorage == null)
                VariableStorage = storage;
        }

        public static void SetSpeakerID(string name)
        {
            VariableStorage.SetValue(DialogueVariableConfig.SPEAKER_ID, name);
        }

        public static void SetPCName(string name)
        {
            VariableStorage.SetValue(DialogueVariableConfig.PC_NAME, name);
        }
    }
}
