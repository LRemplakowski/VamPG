using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Data;
using SunsetSystems.Resources;
using SunsetSystems.Utils.Extensions;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Entities.Characters
{
    public class CreatureData : MonoBehaviour
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
        public string UmaPresetFileName;
        public TextAsset UmaPreset => ResourceLoader.GetUmaPreset(UmaPresetFileName);
        public string animatorControllerResourceName;
        public RuntimeAnimatorController AnimatorControllerAsset => ResourceLoader.GetAnimatorController(animatorControllerResourceName);

        public void CopyFromConfig(CreatureConfig config)
        {
            FirstName = config.Name;
            LastName = config.LastName;
            _id = config.ReadableID;
            Faction = config.Faction;
            BodyType = config.BodyType;
            CreatureType = config.CreatureType;
            UmaPresetFileName = config.UmaPresetFileName;
            animatorControllerResourceName = config.AnimatorController.name;
        }
    }
}
