using System;

namespace SunsetSystems.Entities.Data
{
    [Serializable]
    public struct StatsData
    {
        public Trackers trackers;
        public Clan clan;
        public int generation;
        public Attributes attributes;
        public Skills skills;
        public Disciplines disciplines;

        public StatsData(StatsConfig statsAsset)
        {
            this.trackers = statsAsset.trackers;
            this.clan = statsAsset.clan;
            this.generation = statsAsset.generation;
            this.attributes = statsAsset.attributes;
            this.skills = statsAsset.skills;
            this.disciplines = statsAsset.disciplines;
        }
    }
}
