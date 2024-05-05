namespace SunsetSystems.Inventory.Data
{
    public interface IEquipableItem : IBaseItem
    {
        string TooltipName { get; }
        bool CanBeRemoved { get; }
        bool IsDefaultItem { get; }
    }
}
