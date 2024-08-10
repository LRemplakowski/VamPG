using Sirenix.OdinInspector;

namespace SunsetSystems.Core.Database
{
    public abstract class AbstractDatabaseEntry<T> : SerializedScriptableObject, IDatabaseEntry<T>
    {
        public abstract string DatabaseID { get; protected set; }
        public abstract string ReadableID { get; protected set; }

        protected abstract void RegisterToDatabase();
        protected abstract void UnregisterFromDatabase();

#if UNITY_EDITOR
        protected virtual void Awake()
        {
            if (string.IsNullOrWhiteSpace(DatabaseID))
            {
                AssignNewID();
            }
            if (string.IsNullOrEmpty(ReadableID))
                ReadableID = name;
            if (string.IsNullOrWhiteSpace(DatabaseID) == false)
                RegisterToDatabase();
        }

        [Button("Force Validate")]
        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(DatabaseID))
            {
                AssignNewID();
            }
            if (string.IsNullOrWhiteSpace(DatabaseID) == false)
                RegisterToDatabase();
        }

        protected virtual void Reset()
        {
            AssignNewID();
        }

        protected virtual void OnDestroy()
        {
            UnregisterFromDatabase();
        }

        [Button("Randomize ID")]
        private void AssignNewID()
        {
            DatabaseID = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
