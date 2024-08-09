namespace SunsetSystems.Core.Database
{
    public interface IDatabaseEntry
    {
        string DatabaseID { get; }
        string ReadableID { get; }
    }
}
