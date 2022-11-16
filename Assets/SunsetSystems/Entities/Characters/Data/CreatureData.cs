using SunsetSystems.Entities.Data;
using System;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    [Serializable]
    public struct CreatureData
    {
        public string FirstName, LastName;
        public string FullName => $"{FirstName} {LastName}";
        public string ID => FullName;
        public Sprite Portrait;
        public Faction Faction;
        public BodyType BodyType;
        public CreatureType CreatureType;
        public string UmaPresetFileName;
        public RuntimeAnimatorController AnimatorControllerAsset;
        public StatsData Stats;
        public EquipmentData Equipment;
        public bool UseEquipmentPreset;

        public CreatureData(CreatureConfig config)
        {
            FirstName = config.Name;
            LastName = config.LastName;
            Portrait = config.Portrait;
            Faction = config.CreatureFaction;
            BodyType = config.BodyType;
            CreatureType = config.CreatureType;
            UmaPresetFileName = config.UmaPresetFilename;
            AnimatorControllerAsset = config.AnimatorController;
            Stats = new(config.StatsAsset);
            Equipment = new(config.EquipmentConfig);
            UseEquipmentPreset = config.UseEquipmentPreset;
        }
    }
}
