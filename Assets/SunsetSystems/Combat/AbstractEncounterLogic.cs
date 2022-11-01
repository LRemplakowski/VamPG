using UnityEngine;

namespace SunsetSystems.Combat
{
    public abstract class AbstractEncounterLogic : ScriptableObject, IEncounterLogic
    {
        public abstract void Perform();
    }
}
