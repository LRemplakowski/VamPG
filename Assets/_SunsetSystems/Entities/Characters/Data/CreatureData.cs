using SunsetSystems.Entities.Data;
using System;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    [Serializable]
    public struct CreatureData
    {
        public string FirstName, LastName;
        public string FullName => $"{FirstName} {LastName}";
        public readonly string ID;
        public Sprite Portrait;
        public Faction Faction;
        public BodyType BodyType;
        public CreatureType CreatureType;
        public TextAsset UmaPreset;
        public RuntimeAnimatorController AnimatorControllerAsset;
        public StatsData Stats;
        public EquipmentData Equipment;
        public bool UseEquipmentPreset;
        public float Money;

        public CreatureData(CreatureConfig config)
        {
            FirstName = config.Name;
            LastName = config.LastName;
            ID = config.ReadableID;
            Portrait = config.Portrait;
            Faction = config.CreatureFaction;
            BodyType = config.BodyType;
            CreatureType = config.CreatureType;
            UmaPreset = config.UmaPreset;
            AnimatorControllerAsset = config.AnimatorController;
            Stats = new(config.StatsAsset);
            Equipment = new(config.EquipmentConfig);
            UseEquipmentPreset = config.UseEquipmentPreset;
            Money = config.EquipmentConfig.Money;
        }
    }
}
