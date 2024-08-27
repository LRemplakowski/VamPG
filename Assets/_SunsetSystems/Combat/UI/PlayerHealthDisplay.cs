using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using TMPro;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class PlayerHealthDisplay : SerializedMonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI healthText;

        public void UpdateHealthDisplay(ICombatant currentActor)
        {
            if (currentActor != null)
            {
                HealthData data = currentActor.References.GetCachedComponentInChildren<StatsManager>().GetHealthData();
                healthText.text = $"HP: {data.maxHealth - data.superficialDamage}/{data.maxHealth}";
            }
        }
    }
}
