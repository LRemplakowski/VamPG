using SunsetSystems.Entities.Data;
using SunsetSystems.Resources;
using System;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    [Serializable]
    public class CreatureData
    {
        public string FirstName, LastName;
        public string FullName => $"{FirstName} {LastName}";
        [SerializeField]
        private string _id;
        public string ID => _id;
        public string PortraitFileName;
        public Sprite Portrait => ResourceLoader.GetPortrait(PortraitFileName);
        public Faction Faction;
        public BodyType BodyType;
        public CreatureType CreatureType;
        public string UmaPresetFileName;
        public TextAsset UmaPreset => ResourceLoader.GetUmaPreset(UmaPresetFileName);
        public string animatorControllerResourceName;
        public RuntimeAnimatorController AnimatorControllerAsset => ResourceLoader.GetAnimatorController(animatorControllerResourceName);
        [SerializeField]
        public StatsData Stats;
        [SerializeField]
        public EquipmentData Equipment;
        public bool UseEquipmentPreset;
        public float Money;

        public CreatureData(CreatureConfig config)
        {
            FirstName = config.Name;
            LastName = config.LastName;
            _id = config.ReadableID;
            PortraitFileName = "";
            Faction = config.Faction;
            BodyType = config.BodyType;
            CreatureType = config.CreatureType;
            UmaPresetFileName = config.UmaPresetFileName;
            animatorControllerResourceName = config.AnimatorController.name;
            Stats = new(config.StatsAsset);
            Equipment = new(config.EquipmentConfig);
            UseEquipmentPreset = config.UseEquipmentPreset;
            Money = config.EquipmentConfig.Money;
        }

        public CreatureData()
        {

        }
    }
}
