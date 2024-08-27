namespace SunsetSystems.Core.Database
{
    public interface IDatabaseEntry<T>
    {
        string DatabaseID { get; }
        string ReadableID { get; }
    }
}
