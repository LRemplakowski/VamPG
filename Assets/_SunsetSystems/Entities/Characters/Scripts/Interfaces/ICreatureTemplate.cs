using System.Collections.Generic;
using SunsetSystems.Entities.Data;
using SunsetSystems.Equipment;
using UMA;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Entities.Characters.Interfaces
{
    public interface ICreatureTemplate
    {
        string DatabaseID { get; }
        string ReadableID { get; }
        string FullName { get; }
        string FirstName { get; }
        string LastName { get; }

        Faction Faction { get; }
        BodyType BodyType { get; }
        CreatureType CreatureType { get; }

        AssetReferenceSprite PortraitAssetRef { get; }
        List<UMARecipeBase> BaseUmaRecipes { get; }

        Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlotsData { get; }
        StatsData StatsData { get; }
    }
}
