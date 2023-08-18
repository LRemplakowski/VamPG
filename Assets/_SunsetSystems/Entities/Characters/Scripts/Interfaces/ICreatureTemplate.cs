using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Entities.Characters.Interfaces
{
    public interface ICreatureTemplate
    {
        string DatabaseID { get; }
        string ReadableID { get; }
        string FullName { get; }

        Faction Faction { get; }
        BodyType BodyType { get; }
        CreatureType CreatureType { get; }

        AssetReferenceSprite PortraitAssetRef { get; }
    }
}
