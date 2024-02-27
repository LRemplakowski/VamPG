using Sirenix.OdinInspector;
using SunsetSystems.Core.AddressableManagement;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Party;
using SunsetSystems.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UI.CharacterPortraits
{
    [RequireComponent(typeof(Tagger))]
    public class PartyPortraitsController : SerializedMonoBehaviour
    {
        [SerializeField]
        private GameObject portraitPrefab;
        [ShowInInspector]
        private readonly List<PortraitController> portraits = new();

        private async void Start()
        {
            var activeParty = PartyManager.Instance.ActiveParty;
            List<Task<Sprite>> partyPortraitsLoadingTasks = new();
            foreach (ICreature partyMember in activeParty)
            {
                partyPortraitsLoadingTasks.Add(AddressableManager.Instance.LoadAssetAsync(partyMember.References.CreatureData.PortraitAssetRef));
            }
            await Task.WhenAll(partyPortraitsLoadingTasks);
            foreach (Task<Sprite> portraitTask in partyPortraitsLoadingTasks)
            {
                AddPortrait(portraitTask.Result);
            }
        }

        public void AddPortrait(Sprite portrait)
        {
            GameObject portraitGO = Instantiate(portraitPrefab, this.transform);
            PortraitController portraitController = portraitGO.GetComponent<PortraitController>();
            if (portraitController == null)
                Debug.LogWarning("jebany null");
            portraitController.InitPotrait(portrait);
            portraits.Add(portraitController);
        }

        public void Clear()
        {
            portraits.ForEach(p => Destroy(p.gameObject));
            portraits.Clear();
        }
    }
}
