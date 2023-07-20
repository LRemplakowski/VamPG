using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using UnityEngine;

namespace SunsetSystems.Combat
{
    [RequireComponent(typeof(Collider))]
    public class EncounterTrigger : MonoBehaviour
    {
        [SerializeField]
        private Encounter encounterToTrigger;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ICreature creature) && creature.Faction is Faction.PlayerControlled)
                if (encounterToTrigger)
                    encounterToTrigger.Begin();
                else
                    Debug.LogError("Encounter trigger has no encounter set!");
        }
    }
}
