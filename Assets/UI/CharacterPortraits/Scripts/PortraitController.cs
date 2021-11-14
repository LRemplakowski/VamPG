using Entities.Characters.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.CharacterPortraits
{
    public class PortraitController : MonoBehaviour
    {
        [SerializeField]
        private PortraitIcon potraitIcon;
        [SerializeField]
        private HealthTrackerUI healthTracker;
        [SerializeField]
        private HungerTrackerUI hungerTracker;

        private void Start()
        {
            if (potraitIcon == null)
                potraitIcon = GetComponentInChildren<PortraitIcon>();
            if (healthTracker == null)
                healthTracker = GetComponentInChildren<HealthTrackerUI>();
            if (hungerTracker == null)
                hungerTracker = GetComponentInChildren<HungerTrackerUI>();
        }

        internal void InitPotrait(CreatureData data)
        {
            potraitIcon.SetIcon(data.portrait);
            healthTracker.SetHealthData(data.healthData);
            hungerTracker.SetCurrentHunger(data.hunger);
        }    
    }
}
