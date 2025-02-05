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

        public StatsData(StatsConfig statsAsset)
        {
            this.Trackers = Trackers.DeepCopy(statsAsset.Trackers);
            this.Clan = statsAsset.Clan;
            this.Generation = statsAsset.Generation;
            this.BloodPotency = statsAsset.BloodPotency;
            this.Attributes = Attributes.DeepCopy(statsAsset.Attributes);
            this.Skills = Skills.DeepCopy(statsAsset.Skills);
        }

        public StatsData(StatsData data)
        {
            this.Trackers = Trackers.DeepCopy(data.Trackers);
            this.Clan = data.Clan;
            this.Generation = data.Generation;
            this.BloodPotency = data.BloodPotency;
            this.Attributes = Attributes.DeepCopy(data.Attributes);
            this.Skills = Skills.DeepCopy(data.Skills);
        }
    }
}
