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

        private void UpdateHealthDisplay()
        {
            HealthData data = combatManager.CurrentActiveActor.References.GetComponentInChildren<StatsManager>().GetHealthData();
        }
    }
}
