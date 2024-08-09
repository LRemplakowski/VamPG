namespace SunsetSystems.Core.Database
{
    public interface IDatabase<T>
    {
        bool TryGetEntry(string entryID, out T databaseEntry);
        bool TryGetEntryByReadableID(string readableID, out T databaseEntry);
        bool Register(T databaseEntry);
        void Unregister(T databaseEntry);
    }
}
