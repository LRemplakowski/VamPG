using UMA.CharacterSystem;

namespace SunsetSystems.Inventory.Data
{
    public interface IWearable : IEquipableItem
    {
        UMAWardrobeCollection WearableWardrobe { get; }
    }
}
