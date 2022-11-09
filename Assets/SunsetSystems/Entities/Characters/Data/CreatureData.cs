using SunsetSystems.Entities.Data;
using System;
using UnityEditor.Animations;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    [Serializable]
    public struct CreatureData
    {
        public string firstName, lastName;
        public string FullName => firstName + " " + lastName;
        public Sprite portrait;
        public Faction faction;
        public BodyType bodyType;
        public CreatureType creatureType;
        public string umaPresetFileName;
        public RuntimeAnimatorController animatorControllerAsset;
        public StatsData stats;
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
            useEquipmentPreset = data.useEquipmentPreset;
        }
    }
}
