using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Utils.Extensions;
using System;
using System.Collections.Generic;
using UMA;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Entities.Characters
{
    public class CreatureData : SerializedMonoBehaviour
    {
        public string FirstName = "Foo", LastName = "Bar";
        public string FullName => $"{FirstName} {LastName}";
        [field: SerializeField, ReadOnly]
        public string ReadableID { get; private set; }
        [field: SerializeField, ReadOnly]
        public string DatabaseID { get; private set; }
        [field: SerializeField]
        public AssetReferenceSprite PortraitAssetRef { get; private set; }
        [field: SerializeField]
        public Faction Faction { get; private set; }
        [field: SerializeField]
        public BodyType BodyType { get; private set; }
        [field: SerializeField]
        public CreatureType CreatureType { get; private set; }
        [field: SerializeField]
        public List<UMARecipeBase> BaseUmaRecipes { get; private set; }

        public void CopyFromTemplate(ICreatureTemplate template)
        {
            FirstName = template.FirstName;
            LastName = template.LastName;
            DatabaseID = template.DatabaseID;
            ReadableID = template.ReadableID;
            PortraitAssetRef = template.PortraitAssetRef;
            Faction = template.Faction;
            BodyType = template.BodyType;
            CreatureType = template.CreatureType;
            BaseUmaRecipes = template.BaseUmaRecipes;
        }
    }
}
