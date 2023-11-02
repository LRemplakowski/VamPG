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
        public string ReadableID => FullName.ToPascalCase();
        [SerializeField]
        private string _id;
        public string DatabaseID => _id;
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
            _id = template.ReadableID;
            PortraitAssetRef = template.PortraitAssetRef;
            Faction = template.Faction;
            BodyType = template.BodyType;
            CreatureType = template.CreatureType;
            BaseUmaRecipes = template.BaseUmaRecipes;
        }
    }
}
