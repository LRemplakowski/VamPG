using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    public struct CreatureUIData
    {
        public readonly string name;
        public readonly Sprite portrait;
        public readonly HealthData healthData;
        public readonly int hunger;
        private CreatureUIData(string name, Sprite portrait, HealthData healthData, int hunger)
        {
            this.name = name;
            this.portrait = portrait;
            this.healthData = healthData;
            this.hunger = hunger;
        }

        public class CreatureDataBuilder
        {
            private string name;
            private Sprite portrait;
            private HealthData healthData;
            private int hunger;

            public CreatureDataBuilder(string name, Sprite portrait, HealthData healthData, int hunger)
            {
                this.name = name;
                this.portrait = portrait;
                this.healthData = healthData;
                this.hunger = hunger;
            }

            public CreatureUIData Create()
            {
                return new CreatureUIData(name, portrait, healthData, hunger);
            }
        }
    }
}
