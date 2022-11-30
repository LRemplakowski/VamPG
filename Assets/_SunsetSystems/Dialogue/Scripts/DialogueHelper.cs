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
    }
}
