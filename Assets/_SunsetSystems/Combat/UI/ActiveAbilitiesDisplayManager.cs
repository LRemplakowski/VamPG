using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Combat.UI
{
    public class ActiveAbilitiesDisplayManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private AssetReference abilityButtonAsset;

        public void ShowAbilities(ICombatant currentActor)
        {

        }
    }
}
