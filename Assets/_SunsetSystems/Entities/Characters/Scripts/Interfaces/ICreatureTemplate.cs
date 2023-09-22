using SunsetSystems.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEngine;
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

        EquipmentData EquipmentData { get; }
        StatsData StatsData { get; }
    }
}
