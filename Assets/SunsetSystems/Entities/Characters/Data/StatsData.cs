using System;

namespace SunsetSystems.Entities.Data
{
    [Serializable]
    public struct StatsData
    {
        public Trackers Trackers;
        public Clan Clan;
        public int Generation;
        public int BloodPotency;
        public Attributes Attributes;
        public Skills Skills;
        public Disciplines Disciplines;

        public StatsData(StatsConfig statsAsset)
        {
            this.Trackers = statsAsset.Trackers;
            this.Clan = statsAsset.Clan;
            this.Generation = statsAsset.Generation;
            this.BloodPotency = statsAsset.BloodPotency;
            this.Attributes = statsAsset.Attributes;
            this.Skills = statsAsset.Skills;
            this.Disciplines = statsAsset.Disciplines;
        }

        public StatsData(StatsData data)
        {
            this.Trackers = data.Trackers;
            this.Clan = data.Clan;
            this.Generation = data.Generation;
            this.BloodPotency = data.BloodPotency;
            this.Attributes = data.Attributes;
            this.Skills = data.Skills;
            this.Disciplines = data.Disciplines;
        }
    }
}
