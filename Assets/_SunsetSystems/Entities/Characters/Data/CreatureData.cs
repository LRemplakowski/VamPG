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
        public readonly string ID;
        [ES3Serializable]
        public Sprite Portrait;
        public Faction Faction;
        public BodyType BodyType;
        public CreatureType CreatureType;
        [ES3Serializable]
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

        //public CreatureUIData GetCreatureUIData()
        //{
        //    Tracker tracker = Stats.Trackers.GetTracker(TrackerType.Health)
        //    HealthData healthData = new HealthData(. );
        //    CreatureUIData.CreatureDataBuilder builder = new(Data.FullName,
        //        Data.Portrait,
        //        healthData,
        //        0);
        //    return builder.Create();
        //}
    }
}
