using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Spellbook;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Combat.UI
{
    public class ActiveAbilitiesDisplayManager : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField]
        private Transform buttonsParent;

        private List<GameObject> buttonInstances = new();

        public void ShowAbilities(ICombatant currentActor)
        {
            buttonInstances.ForEach(b => Addressables.ReleaseInstance(b));
            buttonInstances.Clear();
            IEnumerable<DisciplinePower> knownPowers = currentActor.MagicUser.KnownPowers;
            foreach (DisciplinePower power in knownPowers)
            {
                Task<GameObject> buttonTask = Addressables.InstantiateAsync(power.PowerGUIButtonAsset, buttonsParent).Task;
                _ = Task.Run(async () =>
                {
                    await buttonTask;
                    buttonInstances.Add(buttonTask.Result);
                });
            }
        }
    }
}
