using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class DialogueHelper
    {
        public static PersistentVariableStorage VariableStorage { get; private set; }

        public const string SPEAKER_ID = "speakerID";
        public const string PC_NAME = "pcName";

        public static void InitializePersistentVariableStorage(PersistentVariableStorage storage)
        {
            if (VariableStorage == null)
                VariableStorage = storage;
        }

        public static void SetSpeakerName(string name)
        {
            VariableStorage.SetValue(SPEAKER_ID, name);
        }

        public static void SetPCName(string name)
        {
            VariableStorage.SetValue(PC_NAME, name);
        }
    }
}
