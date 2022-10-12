using SunsetSystems.Entities.Data;
using System;
using UnityEditor.Animations;
using UnityEngine;

namespace Entities.Characters
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
        }

        public void Inject(CreatureData data)
        {
            this = data;
        }
    }
}
