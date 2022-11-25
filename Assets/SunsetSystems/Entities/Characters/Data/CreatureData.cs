using SunsetSystems.Entities.Data;
using System;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    [Serializable]
    public struct CreatureData
    {
        public string firstName, lastName;
        public string FullName => firstName + " " + lastName;
        public string ID => FullName;
        public Sprite portrait;
        public Faction faction;
        public BodyType bodyType;
        public CreatureType creatureType;
        public string umaPresetFileName;
        public RuntimeAnimatorController animatorControllerAsset;
        public StatsData stats;
        public EquipmentData equipment;
        public bool useEquipmentPreset;

        public CreatureData(CreatureConfig config)
        {
            firstName = config.Name;
            lastName = config.LastName;
            portrait = config.Portrait;
            faction = config.CreatureFaction;
            bodyType = config.BodyType;
            creatureType = config.CreatureType;
            umaPresetFileName = config.UmaPresetFilename;
            animatorControllerAsset = config.AnimatorController;
            stats = new(config.StatsAsset);
            equipment = new(config.EquipmentConfig);
            useEquipmentPreset = config.UseEquipmentPreset;
        }

        public CreatureData(CreatureData data)
        {
            firstName = data.firstName;
            lastName = data.lastName;
            portrait = data.portrait;
            faction = data.faction;
            bodyType = data.bodyType;
            creatureType = data.creatureType;
            Debug.LogError(data.umaPresetFileName);
            umaPresetFileName = new(data.umaPresetFileName);
            animatorControllerAsset = data.animatorControllerAsset;
            stats = new(data.stats);
            equipment = new EquipmentData(data.equipment);
            useEquipmentPreset = data.useEquipmentPreset;
        }
    }
}
