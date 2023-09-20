using Sirenix.OdinInspector;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public abstract class AbstractEncounterLogic : SerializedMonoBehaviour, IEncounterLogic
    {
        public abstract Task Perform();
    }
}
