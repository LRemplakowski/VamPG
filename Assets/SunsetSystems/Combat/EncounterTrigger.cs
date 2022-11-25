using SunsetSystems.Entities.Characters;
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
            if (other.TryGetComponent(out PlayerControlledCharacter _))
                if (encounterToTrigger)
                    encounterToTrigger.Begin();
                else
                    Debug.LogError("Encounter trigger has no encounter set!");
        }
    }
}
