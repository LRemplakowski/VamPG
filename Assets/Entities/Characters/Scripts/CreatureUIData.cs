using UnityEngine;

namespace Entities.Characters.Data
{
    public class EntityData
    {

    }

    public class CreatureUIData : EntityData
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

        public class CreatureDataBuilder : EntityDataBuilder<CreatureUIData>
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

            public override CreatureUIData Create()
            {
                return new CreatureUIData(name, portrait, healthData, hunger);
            }
        }
    }

    public class HealthData  : EntityData
    {
        public readonly int maxHealth, superficialDamage, aggravatedDamage;

        private HealthData(int maxHealth, int superficialDamage, int aggravatedDamage)
        {
            this.maxHealth = maxHealth;
            this.superficialDamage = superficialDamage;
            this.aggravatedDamage = aggravatedDamage;
        }

        public class HealthDataBuilder : EntityDataBuilder<HealthData>
        {
            private int maxHealth, superficialDamage = 0, aggravatedDamage = 0;

            public HealthDataBuilder(int maxHealth)
            {
                this.maxHealth = maxHealth;
            }

            public void SetSuperficialDamage(int superficialDamage)
            {
                this.superficialDamage = superficialDamage;
            }

            public void SetAggravatedDamage(int aggravatedDamage)
            {
                this.aggravatedDamage = aggravatedDamage;
            }

            public override HealthData Create()
            {
                return new HealthData(maxHealth, superficialDamage, aggravatedDamage);
            }
        }
    }

    public abstract class EntityDataBuilder<T> where T : EntityData
    {
        public abstract T Create();
    }
}
