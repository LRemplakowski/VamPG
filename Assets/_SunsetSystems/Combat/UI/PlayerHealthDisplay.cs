using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class PlayerHealthDisplay : SerializedMonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI healthText;

        private CombatManager combatManager;

        private void Start()
        {
            combatManager = CombatManager.Instance;
        }

        public void UpdateHealthDisplay()
        {
            HealthData data = combatManager.CurrentActiveActor.References.GetCachedComponentInChildren<StatsManager>().GetHealthData();
            healthText.text = $"HP: {data.maxHealth - data.superficialDamage}/{data.maxHealth}";
        }
    }
}
